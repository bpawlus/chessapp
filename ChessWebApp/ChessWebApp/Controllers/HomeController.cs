using ChessWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;
using ChessWebApp.Data;
using Microsoft.AspNetCore.Http;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Net.WebSockets;
using System.Net;
using System.Text;
using ChessWebApp.Core;
using ChessApp.game;
using System.Numerics;

namespace ChessWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MvcGameContext _context;
        public const string SessionUserId = "_UserId";
        public const string SessionUserName = "_UserName";

        public HomeController(MvcGameContext context, ILogger<HomeController> logger)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var id = HttpContext.Session.GetInt32(SessionUserId);
            var user = HttpContext.Session.GetString(SessionUserName);
            if (id != null && user != null)
            {
                var ans = _context.User.Where(dbuser => dbuser.Id == id);
                var ele = ans.ToArray().ElementAt(0);
                return View(ele);
            }
 
            return RedirectToAction(nameof(Login));
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login([Bind("Name,Password")] User user)
        {
            var users = from m in _context.User select m;
            var ans = _context.User.Where(dbuser => string.Equals(dbuser.Name, user.Name) && string.Equals(dbuser.Password, user.Password));

            if (ans.Count() == 1)
            {
                var ele = ans.ToArray().ElementAt(0);
                HttpContext.Session.SetInt32(SessionUserId, ele.Id);
                HttpContext.Session.SetString(SessionUserName, ele.Name);
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        public IActionResult Register()
        {
            ViewData["UserExists"] = false;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("Name,Password")] User user)
        {
            var ans = _context.User.Where(dbuser => dbuser.Name == user.Name);

            if (ans.Count() != 0)
            {
                ViewData["UserExists"] = true;
                return View();
            }
            if (ModelState.IsValid)
            {
                ViewData["UserExists"] = false;
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Login));
            }
            else
            {
                ViewData["UserExists"] = false;
                return View();
            }
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction(nameof(Login));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Route("/ws")]
        public async Task Get()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                var buffer = new byte[1024];
                try
                {
                    var receiveResult = await WSMessageHandler.ReceiveAsync(webSocket);
                    Tuple<bool, string, string> receivedMessage = WSMessageHandler.HandleUserLoginMessage(receiveResult);

                    if (receivedMessage.Item1)
                    {
                        var users = from m in _context.User select m;
                        var ans = _context.User.Where(dbuser => string.Equals(dbuser.Name, receivedMessage.Item2) && string.Equals(dbuser.Password, receivedMessage.Item3));
                        if (ans.Count() == 1)
                        {
                            var ele = ans.ToArray().ElementAt(0);
                            Console.WriteLine($"WS Info - User {receivedMessage.Item2} logged in!");
                            WSMessageHandler.SendServiceLoginOk(webSocket);
                            await ServeClient(webSocket, ele);
                            return;
                        }
                        else
                        {
                            Console.WriteLine($"WS Info - Failed login attempt: {receivedMessage.Item2} {receivedMessage.Item3}");
                            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "ERROR: Invalid login creditialents!", CancellationToken.None);
                            return;
                        }
                    }
                    else
                    {
                        Console.WriteLine($"WS Info - INVALID LOGIN REQUEST: {receiveResult}");
                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "ERROR: Invalid login request!", CancellationToken.None);
                        return;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("WS Connection error: " + e.ToString());
                    return;
                }
            }
            else
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
        }

        private async Task ServeClient(WebSocket webSocket, User user)
        {
            var buffer = new byte[1024];
            while (true)
            {
                try
                {
                    var receiveResult = await WSMessageHandler.ReceiveAsync(webSocket);

                    Tuple<bool> receivedMessage = WSMessageHandler.HandleUserFindGameMessage(receiveResult);
                    if (receivedMessage.Item1)
                    {
                        Console.WriteLine($"WS Info - User {user.Name} wants to find a game!");
                        GameFinder.Queue(user, webSocket);
                        continue;
                    }

                    Tuple<bool, string> receivedMessage2 = WSMessageHandler.HandleUserLogoutMessage(receiveResult);
                    if (receivedMessage2.Item1)
                    {
                        GameFinder.Unqueue(user);
                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, $"Logged out! - Reason {receivedMessage2.Item2}", CancellationToken.None);
                        Console.WriteLine($"WS Info - User {user.Name} logged out! ({receivedMessage2.Item2})");
                        return;
                    }

                    Tuple<bool, int, int, int, int> gameMoveMessage = WSMessageHandler.HandleGameMoveMessage(receiveResult);
                    Tuple<bool, int, int> gameGetMoveMessage = WSMessageHandler.HandleGameGetMoveMessage(receiveResult);
                    Tuple<bool> gameGiveUp = WSMessageHandler.HandleGameGiveUpMessage(receiveResult);
                    Tuple<bool> gameOpponentDet = WSMessageHandler.HandleGameOppDetMessage(receiveResult);

                    if (gameMoveMessage.Item1 || gameGetMoveMessage.Item1 || gameGiveUp.Item1 || gameOpponentDet.Item1)
                    {
                        Tuple<ChessGameController, ChessPlayer> game = GameFinder.findGameOf(user);
                        if (game != null)
                        {
                            Console.WriteLine($"WS Game Info - Handling {receiveResult} from {user.Name}");
                            if (gameMoveMessage.Item1)
                            {
                                game.Item1.HandleMessageGameMove(game.Item2, gameMoveMessage);
                                continue;
                            }
                            else if(gameGetMoveMessage.Item1)
                            {
                                game.Item1.HandleMessageGameGetMove(game.Item2, gameGetMoveMessage);
                                continue;
                            }
                            else if (gameGiveUp.Item1)
                            {
                                game.Item1.HandleGameGiveUp(game.Item2, gameGiveUp);
                                continue;
                            }
                            else if (gameOpponentDet.Item1)
                            {
                                game.Item1.HandleGameOppDet(game.Item2, gameOpponentDet);
                                continue;
                            }
                        }
                        else
                        {
                            Console.WriteLine($"WS Info - User {user.Id} logged out!");
                            continue;
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("WS Connection error: " + e.ToString());
                    return;
                }
            }
        }
    }
}