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
using Microsoft.AspNetCore.Mvc.Rendering;

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

        private Dictionary<string, List<SelectListItem>> GetSelectListItems(User user)
        {
            var toRet = new Dictionary<string, List<SelectListItem>>();

            short[] selected = new short[]{
                        user.VariantKing, user.VariantQueen,
                        user.VariantBishopLeft, user.VariantBishopRight,
                        user.VariantKnightLeft, user.VariantKnightRight,
                        user.VariantRookLeft, user.VariantRookRight,
                        user.VariantPawn1, user.VariantPawn2, user.VariantPawn3, user.VariantPawn4,
                        user.VariantPawn5, user.VariantPawn6, user.VariantPawn7, user.VariantPawn8 
            };
          

            string[] keys = new string[]{
                        "VariantKing", "VariantQueen",
                        "VariantBishopLeft", "VariantBishopRight",
                        "VariantKnightLeft", "VariantKnightRight",
                        "VariantRookLeft", "VariantRookRight",
                        "VariantPawn1", "VariantPawn2", "VariantPawn3", "VariantPawn4",
                        "VariantPawn5", "VariantPawn6", "VariantPawn7", "VariantPawn8"
            };

            for (int i = 0; i < keys.Length; i++)
            {
                List<SelectListItem> items = new List<SelectListItem>();
                foreach (ChessPiecesEnum cp in (ChessPiecesEnum[])Enum.GetValues(typeof(ChessPiecesEnum)))
                {
                    if (cp != ChessPiecesEnum.Null)
                    {
                        short val = (short)cp;
                        if (val != selected[i])
                        {
                            items.Add(new SelectListItem { Text = cp.ToString(), Value = val.ToString() });
                        }
                        else
                        {
                            items.Add(new SelectListItem { Text = cp.ToString(), Value = val.ToString(), Selected = true });
                        }
                    }
                }
                toRet.Add(keys[i], items);
            }

            return toRet;
        }
        public IActionResult Index()
        {
            var id = HttpContext.Session.GetInt32(SessionUserId);
            var name = HttpContext.Session.GetString(SessionUserName);
            if (id != null && name != null)
            {
                var ans = _context.User.Where(dbuser => dbuser.Id == id);
                var ele = ans.ToArray().ElementAt(0);

                var elements = GetSelectListItems(ele);
                ViewBag.VariantPawn1 = elements["VariantPawn1"];
                ViewBag.VariantPawn2 = elements["VariantPawn2"];
                ViewBag.VariantPawn3 = elements["VariantPawn3"];
                ViewBag.VariantPawn4 = elements["VariantPawn4"];
                ViewBag.VariantPawn5 = elements["VariantPawn5"];
                ViewBag.VariantPawn6 = elements["VariantPawn6"];
                ViewBag.VariantPawn7 = elements["VariantPawn7"];
                ViewBag.VariantPawn8 = elements["VariantPawn8"];

                ViewBag.VariantKing = elements["VariantKing"];
                ViewBag.VariantQueen = elements["VariantQueen"];
                ViewBag.VariantBishopLeft = elements["VariantBishopLeft"];
                ViewBag.VariantBishopRight = elements["VariantBishopRight"];
                ViewBag.VariantKnightLeft = elements["VariantKnightLeft"];
                ViewBag.VariantKnightRight = elements["VariantKnightRight"];
                ViewBag.VariantRookLeft = elements["VariantRookLeft"];
                ViewBag.VariantRookRight = elements["VariantRookRight"];

                return View(ele);
            }
 
            return RedirectToAction(nameof(Login));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditBoard([Bind(
                        "VariantKing", "VariantQueen",
                        "VariantBishopLeft", "VariantBishopRight",
                        "VariantKnightLeft", "VariantKnightRight",
                        "VariantRookLeft", "VariantRookRight",
                        "VariantPawn1", "VariantPawn2", "VariantPawn3", "VariantPawn4",
                        "VariantPawn5", "VariantPawn6", "VariantPawn7", "VariantPawn8"
            )] User user)
        {
            int id = (int)HttpContext.Session.GetInt32(SessionUserId);

            if (id != null)
            {
                user.Id = id;
                try
                {
                    using (var db = _context)
                    {
                        db.User.Attach(user);
                        db.Entry(user).Property(x => x.VariantPawn1).IsModified = true;
                        db.Entry(user).Property(x => x.VariantPawn2).IsModified = true;
                        db.Entry(user).Property(x => x.VariantPawn3).IsModified = true;
                        db.Entry(user).Property(x => x.VariantPawn4).IsModified = true;
                        db.Entry(user).Property(x => x.VariantPawn5).IsModified = true;
                        db.Entry(user).Property(x => x.VariantPawn6).IsModified = true;
                        db.Entry(user).Property(x => x.VariantPawn7).IsModified = true;
                        db.Entry(user).Property(x => x.VariantPawn8).IsModified = true;

                        db.Entry(user).Property(x => x.VariantKing).IsModified = true;
                        db.Entry(user).Property(x => x.VariantQueen).IsModified = true;
                        db.Entry(user).Property(x => x.VariantBishopLeft).IsModified = true;
                        db.Entry(user).Property(x => x.VariantBishopRight).IsModified = true;
                        db.Entry(user).Property(x => x.VariantRookLeft).IsModified = true;
                        db.Entry(user).Property(x => x.VariantRookRight).IsModified = true;
                        db.Entry(user).Property(x => x.VariantKnightLeft).IsModified = true;
                        db.Entry(user).Property(x => x.VariantKnightRight).IsModified = true;
                        db.SaveChanges();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.Id == id);
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