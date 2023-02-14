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
using ChessWebApp.ChessGame;
using ChessWebApp.ChessGame;
using System.Numerics;
using Microsoft.AspNetCore.Mvc.Rendering;
using static Azure.Core.HttpHeader;
using NuGet.Packaging.Signing;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.Design;

namespace ChessWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MvcGameContext _context;
        public const string SessionUserId = "_UserId";
        public const string SessionUserName = "_UserName";

        private string[] keys = new string[]{
                        "VariantKing", "VariantQueen",
                        "VariantBishopLeft", "VariantBishopRight",
                        "VariantKnightLeft", "VariantKnightRight",
                        "VariantRookLeft", "VariantRookRight",
                        "VariantPawn1", "VariantPawn2", "VariantPawn3", "VariantPawn4",
                        "VariantPawn5", "VariantPawn6", "VariantPawn7", "VariantPawn8"
            };

        public HomeController(MvcGameContext context, ILogger<HomeController> logger)
        {
            _logger = logger;
            _context = context;
        }

        private void LoadPicturesToViewBag()
        {
            var factoryIds = ChessPiecesFactories.GetFactoryIDs();
            var pictures = new Dictionary<short, string>();

            foreach (short id in factoryIds)
            {
                pictures.Add(id, ChessPiecesFactories.GetFactoryFigureNames(id).Item2);
                pictures.Add((short)-id, ChessPiecesFactories.GetFactoryFigureNames(id).Item3);
            }

            ViewData["pictures"] = pictures;
        }
        private void LoadSelectListItemsToViewBag(User user)
        {
            short[] selected = new short[]{
                        user.VariantKing, user.VariantQueen,
                        user.VariantBishopLeft, user.VariantBishopRight,
                        user.VariantKnightLeft, user.VariantKnightRight,
                        user.VariantRookLeft, user.VariantRookRight,
                        user.VariantPawn1, user.VariantPawn2, user.VariantPawn3, user.VariantPawn4,
                        user.VariantPawn5, user.VariantPawn6, user.VariantPawn7, user.VariantPawn8
            };

            var factoryIds = ChessPiecesFactories.GetFactoryIDs();
            short kingFactory = 6;

            factoryIds.Remove(kingFactory);

            var kingItems = new List<SelectListItem>();
            var kingNames = ChessPiecesFactories.GetFactoryFigureNames(kingFactory);
            if (kingFactory != selected[0])
            {
                kingItems.Add(new SelectListItem { Text = kingNames.Item1, Value = kingFactory.ToString() });
            }
            else
            {
                kingItems.Add(new SelectListItem { Text = kingNames.Item1, Value = kingFactory.ToString(), Selected = true });
            }
            ViewData[keys[0]] = kingItems;

            for (int i = 1; i < selected.Length; i++)
            {
                var items = new List<SelectListItem>();

                foreach (short id in factoryIds)
                {
                    var names = ChessPiecesFactories.GetFactoryFigureNames(id);
                    if (id != selected[i])
                    {
                        items.Add(new SelectListItem { Text = names.Item1, Value = id.ToString() });
                    }
                    else
                    {
                        items.Add(new SelectListItem { Text = names.Item1, Value = id.ToString(), Selected = true });
                    }
                }
                ViewData[keys[i]] = items;
            }

            ViewData["keys"] = keys;
        }
        public IActionResult Index()
        {
            var id = HttpContext.Session.GetInt32(SessionUserId);
            var name = HttpContext.Session.GetString(SessionUserName);
            if (id != null && name != null)
            {
                var ans = _context.User.Where(dbuser => dbuser.Id == id);
                var ele = ans.ToArray().ElementAt(0);

                LoadPicturesToViewBag();
                LoadSelectListItemsToViewBag(ele);
                ViewData["timesWon"] = UserVictories(ele.Id);
                ViewData["timesLost"] = UserLoses(ele.Id);

                return View(ele);
            }

            return Unauthorized();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditBoard([Bind(
                        "VariantKing", "VariantQueen",
                        "VariantBishopLeft", "VariantBishopRight",
                        "VariantKnightLeft", "VariantKnightRight",
                        "VariantRookLeft", "VariantRookRight",
                        "VariantPawn1", "VariantPawn2", "VariantPawn3", "VariantPawn4",
                        "VariantPawn5", "VariantPawn6", "VariantPawn7", "VariantPawn8",
                        "Description"
            )] User user)
        {
            int id = (int)HttpContext.Session.GetInt32(SessionUserId);

            if (id != null)
            {
                user.Id = id;
                try
                {
                    _context.User.Attach(user);
                    _context.Entry(user).Property(x => x.VariantPawn1).IsModified = true;
                    _context.Entry(user).Property(x => x.VariantPawn2).IsModified = true;
                    _context.Entry(user).Property(x => x.VariantPawn3).IsModified = true;
                    _context.Entry(user).Property(x => x.VariantPawn4).IsModified = true;
                    _context.Entry(user).Property(x => x.VariantPawn5).IsModified = true;
                    _context.Entry(user).Property(x => x.VariantPawn6).IsModified = true;
                    _context.Entry(user).Property(x => x.VariantPawn7).IsModified = true;
                    _context.Entry(user).Property(x => x.VariantPawn8).IsModified = true;

                    _context.Entry(user).Property(x => x.VariantKing).IsModified = true;
                    _context.Entry(user).Property(x => x.VariantQueen).IsModified = true;
                    _context.Entry(user).Property(x => x.VariantBishopLeft).IsModified = true;
                    _context.Entry(user).Property(x => x.VariantBishopRight).IsModified = true;
                    _context.Entry(user).Property(x => x.VariantRookLeft).IsModified = true;
                    _context.Entry(user).Property(x => x.VariantRookRight).IsModified = true;
                    _context.Entry(user).Property(x => x.VariantKnightLeft).IsModified = true;
                    _context.Entry(user).Property(x => x.VariantKnightRight).IsModified = true;
                    _context.Entry(user).Property(x => x.Description).IsModified = true;
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
                    {
                        return BadRequest();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return Unauthorized();
        }

        public IActionResult Login()
        {
            int? id = HttpContext.Session.GetInt32(SessionUserId);

            if (id != null)
            {
                return RedirectToAction(nameof(Index));
            }
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

        public async Task<IActionResult> UserGames(int? userid)
        {
            var id = HttpContext.Session.GetInt32(SessionUserId);
            var name = HttpContext.Session.GetString(SessionUserName);
            if (id != null && name != null)
            {
                if (userid == null)
                {
                    return NotFound();
                }

                if(userid != id)
                {
                    return Unauthorized();
                }

                var games = UserGamesDto(userid);

                if (games == null)
                {
                    return NotFound();
                }

                var gamesList = await games.ToListAsync();

                return View(gamesList);
            }

            return Unauthorized();
        }

        public async Task<IActionResult> GameSummary(int? gameid)
        {
            var id = HttpContext.Session.GetInt32(SessionUserId);
            var name = HttpContext.Session.GetString(SessionUserName);
            if (id != null && name != null)
            {
                if (gameid == null)
                {
                    return NotFound();
                }
                var game = FindGameIfPlayerPlayed(gameid, id);
                if (game == null)
                {
                    return NotFound();
                }

                var gameEvents = UserGameEventsDto(gameid, id);

                if (gameEvents == null)
                {
                    return NotFound();
                }

                var gameEventsList = await gameEvents.ToListAsync();
                LoadPicturesToViewBag();
                ViewData["boardRowNames"] = ChessGameController.BoardRowNames;
                ViewData["boardColNames"] = ChessGameController.BoardColNames;
                ViewData["chessboardSize"] = ChessGameController.ChessboardSize;
                return View(gameEventsList);
            }

            return Unauthorized();
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

        private bool UserExists(int? id)
        {
            return _context.User.Any(e => e.Id == id);
        }

        private int UserVictories(int? id)
        {
            return _context.Game.Where(e => e.PlayerWinner.Id == id).Count();
        }

        private int UserLoses(int? id)
        {
            return _context.Game.Where(e => e.PlayerLoser.Id == id).Count();
        }

        private IQueryable<GameDto> UserGamesDto(int? id)
        {
            var result = (from game in _context.Game
                          where (((game.PlayerWinner != null) && (game.PlayerLoser != null)) && ((game.PlayerWinner.Id == id) || (game.PlayerLoser.Id == id)))
                          select new GameDto()
                          {
                                Id = game.Id,
                                PlayerLoserName = game.PlayerLoser.Name,
                                PlayerWinnerName = game.PlayerWinner.Name,
                                TimeStart = game.TimeStart,
                                TimeEnd = game.TimeEnd,
                                Lost = (game.PlayerLoser.Id == id ? true : false),
                          });;
            return result;
        }

        private Game FindGameIfPlayerPlayed(int? gameid, int? userid)
        {
            var ans = _context.Game.Where(e => e.Id == gameid && (e.PlayerBottom.Id == userid || e.PlayerWinner.Id == userid));
            if (ans.Count() > 0)
            {
                return ans.ToArray().ElementAt(0);
            }
            return null;
        }

        private GameEventComment FindComment(int? eventid, int? userid)
        {
            var ans = _context.GameEventComment.Where(e => e.GameEvent != null && e.GameEvent.Id == eventid && e.User.Id == userid);
            if (ans.Count() > 0)
            {
                return ans.ToArray().ElementAt(0);
            }
            return null;
        }

        private IQueryable<GameEventDto> UserGameEventsDto(int? gameid, int? userid)
        {
            var ans = _context.Game.Where(e => e.Id == gameid).Select(e => new Tuple<int,int>(e.PlayerTop.Id, e.PlayerBottom.Id));
            if (ans.Count() > 0)
            {
                var ele = ans.ToArray().ElementAt(0);
                var topAndBot = ans.ToArray().ElementAt(0);

                var result = (from gameevent in _context.GameEvent
                              where (gameevent.Game.Id == gameid)
                              select new GameEventDto()
                              {
                                  Id = gameevent.Id,
                                  Status = gameevent.Status,
                                  Notation = gameevent.Notation,
                                  Time = gameevent.Time,
                                  GameEventComment = null,
                                  IsTop = gameevent.User.Id == ele.Item1 ? true : false
                              });

                foreach(var res in result)
                {
                    res.GameEventComment = FindComment(res.Id, userid);
                }
                return result;
            }
            return null;
        }

        private async Task ServeClient(WebSocket webSocket, User user)
        {
            var buffer = new byte[1024];
            while (true)
            {
                try
                {
                    var receiveResult = await WSMessageHandler.ReceiveAsync(webSocket);

                    var receivedMessage = WSMessageHandler.HandleUserFindGameMessage(receiveResult);
                    if (receivedMessage.Item1)
                    {
                        var game = GameFinder.FindGameOf(user);
                        if (game != null)
                        {
                            Console.WriteLine($"WS Info - User {user.Name} somehow reconnected?");
                        }
                        else
                        {
                            Console.WriteLine($"WS Info - User {user.Name} wants to find a game!");
                            GameFinder.Queue(user, webSocket);
                        }
                        continue;
                    }

                    var receivedMessage2 = WSMessageHandler.HandleUserLogoutMessage(receiveResult);
                    if (receivedMessage2.Item1)
                    {

                        var game = GameFinder.FindGameOf(user);
                        if (game != null)
                        {
                            game.Item1.HandleGameGiveUp(game.Item2);
                        }
                        else
                        {
                            GameFinder.Unqueue(user);
                        }

                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, $"Logged out! - Reason {receivedMessage2.Item2}", CancellationToken.None);
                        Console.WriteLine($"WS Info - User {user.Name} logged out! ({receivedMessage2.Item2})");
                        return;
                    }

                    var gameMoveMessage = WSMessageHandler.HandleGameMoveMessage(receiveResult);
                    var gameGetMoveMessage = WSMessageHandler.HandleGameGetMoveMessage(receiveResult);
                    var gameGiveUp = WSMessageHandler.HandleGameGiveUpMessage(receiveResult);
                    var gameOpponentDet = WSMessageHandler.HandleGameOppDetMessage(receiveResult);

                    if (gameMoveMessage.Item1 || gameGetMoveMessage.Item1 || gameGiveUp.Item1 || gameOpponentDet.Item1)
                    {
                        var game = GameFinder.FindGameOf(user);
                        if (game != null)
                        {
                            Console.WriteLine($"WS Game Info - Handling {receiveResult} from {user.Name}");
                            if (gameMoveMessage.Item1)
                            {
                                game.Item1.HandleMessageGameMove(game.Item2, gameMoveMessage);
                                continue;
                            }
                            else if (gameGetMoveMessage.Item1)
                            {
                                game.Item1.HandleMessageGameGetMove(game.Item2, gameGetMoveMessage);
                                continue;
                            }
                            else if (gameGiveUp.Item1)
                            {
                                game.Item1.HandleGameGiveUp(game.Item2);
                                continue;
                            }
                            else if (gameOpponentDet.Item1)
                            {
                                game.Item1.HandleGameOppDet(game.Item2);
                                continue;
                            }
                        }
                        else
                        {
                            Console.WriteLine($"WS Info - User {user.Id} sent game move while not in game!");
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