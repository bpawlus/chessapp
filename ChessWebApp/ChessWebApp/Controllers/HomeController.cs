using ChessWebApp.ChessGame;
using ChessWebApp.ChessGame.Pieces;
using ChessWebApp.Data;
using ChessWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.WebSockets;
using System.Text;

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
                pictures.Add(id, ChessPiecesFactories.GetFactoryFigureNames(id).Item3);
                pictures.Add((short)-id, ChessPiecesFactories.GetFactoryFigureNames(id).Item2);
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

            for (int i = 0; i < selected.Length; i++)
            {
                var items = new List<SelectListItem>();
                var ans = _context.AllowedPiece.Where(e => e.VariantKey == keys[i]);
                foreach (var allowedPiece in ans)
                {
                    if (allowedPiece.ConditionName != null && GameFinder.Conditions.ContainsKey(allowedPiece.ConditionName))
                    {
                        if (!GameFinder.Conditions[allowedPiece.ConditionName](user))
                        {
                            continue;
                        }
                    }

                    var id = allowedPiece.AllowedFactoryId;
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
            var id = HttpContext.Session.GetInt32(SessionUserId);

            if (id != null)
            {
                user.Id = (int)id;
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
                gamesList.Sort((x, y) => x.TimeStart <= y.TimeStart ? 1 : -1);

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
                if (game == null || game.PlayerBottomId == null || game.PlayerTopId == null)
                {
                    return NotFound();
                }

                var gameEvents = GameEventsDto(gameid);

                if (gameEvents == null)
                {
                    return NotFound();
                }

                var gameEventsList = await gameEvents.ToListAsync();
                for (int i = 0; i < gameEventsList.Count(); i++)
                {
                    var gec = FindComment(gameEventsList[i].Id, id);
                    gameEventsList[i].Comment = gec != null ? gec.Comment : "";
                }

                LoadPicturesToViewBag();
                ViewData["boardRowNames"] = ChessGameController.BoardRowNames;
                ViewData["boardColNames"] = ChessGameController.BoardColNames;
                ViewData["chessboardSize"] = ChessGameController.ChessboardSize;
                ViewData["gameid"] = gameid;
                return View(gameEventsList);
            }

            return Unauthorized();
        }

        [HttpPost]
        public async Task<IActionResult> SaveComments(int gameid, List<GameEventDto> gameEventDtos)
        {
            var userid = HttpContext.Session.GetInt32(SessionUserId);
            var name = HttpContext.Session.GetString(SessionUserName);
            if (userid != null && name != null)
            {
                for (int i = 0; i < gameEventDtos.Count(); i++)
                {
                    var gec = FindComment(gameEventDtos[i].Id, userid);
                    if (gec != null)
                    {
                        if (gameEventDtos[i].Comment == null || gameEventDtos[i].Comment == "")
                        {
                            try
                            {
                                _context.GameEventComment.Remove(gec);
                                _context.SaveChanges();
                            }
                            catch (DbUpdateConcurrencyException)
                            {

                            }
                        }
                        else
                        {
                            try
                            {
                                gec.Comment = gameEventDtos[i].Comment;
                                _context.GameEventComment.Attach(gec);
                                _context.Entry(gec).Property(x => x.Comment).IsModified = true;
                                _context.SaveChanges();
                            }
                            catch (DbUpdateConcurrencyException)
                            {

                            }
                        }
                        continue;
                    }
                    else if(gameEventDtos[i].Comment != null && gameEventDtos[i].Comment != "")
                    {
                        gec = new GameEventComment();

                        var ansuser = _context.User.Where(dbuser => dbuser.Id == userid);
                        var eleuser = ansuser.ToArray().ElementAt(0);

                        var ansgameevent = _context.GameEvent.Where(dbgevent => dbgevent.Id == gameEventDtos[i].Id);
                        var elegameevent = ansgameevent.ToArray().ElementAt(0);

                        gec.GameEvent = elegameevent;
                        gec.User = eleuser;
                        gec.Comment = gameEventDtos[i].Comment;
                        _context.Add(gec);
                        _context.SaveChanges();
                    }
                }
                return RedirectToAction(nameof(GameSummary), new { gameid });
            }
            return Unauthorized();
        }

        public async Task<IActionResult> Spectate()
        {
            var games = new List<GameSpectateDto>();
            foreach(var gc in GameFinder.GamesSpectate)
            {
                var game = UserGameSpectateDto(gc.Key.Game.Id);
                games.Add(game);
            }
            return View(games);
        }

        public async Task<IActionResult> SpectateSummary(int? gameid)
        {
            if (gameid == null)
            {
                return NotFound();
            }
            var game = FindGame(gameid);
            if (game == null)
            {
                return NotFound();
            }

            var gameEvents = GameEventsDto(gameid);

            if (gameEvents == null)
            {
                return NotFound();
            }

            var gameEventsList = await gameEvents.ToListAsync();
            LoadPicturesToViewBag();
            ViewData["boardRowNames"] = ChessGameController.BoardRowNames;
            ViewData["boardColNames"] = ChessGameController.BoardColNames;
            ViewData["chessboardSize"] = ChessGameController.ChessboardSize;
            ViewData["gameid"] = gameid;
            return View(gameEventsList);
        }

        public async Task<IActionResult> AdminUsers()
        {
            var userid = HttpContext.Session.GetInt32(SessionUserId);
            var name = HttpContext.Session.GetString(SessionUserName);
            if (userid != null && name != null)
            {
                var ans = _context.User.Where(dbuser => dbuser.Id == userid);

                if (ans.Count() == 1)
                {
                    var ele = ans.ToArray().ElementAt(0);
                    if(ele.Admin)
                    {
                        return(View(AdminUsersDto().ToList()));
                    }
                }
            }
            return Unauthorized();
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(List<AdminUserDto> usersDto)
        {
            var userid = HttpContext.Session.GetInt32(SessionUserId);
            var name = HttpContext.Session.GetString(SessionUserName);
            if (userid != null && name != null)
            {
                var ans = _context.User.Where(dbuser => dbuser.Id == userid && dbuser.Admin == true);

                if (ans.Count() == 1)
                {
                    foreach (var user in usersDto) {
                        User dbuser = new User()
                        {
                            Id = user.Id,
                            Description = user.Description,
                            Name = user.Name,
                            Admin = user.Admin
                        };

                        try
                        {
                            if (!user.MarkForDel)
                            {
                                _context.User.Attach(dbuser);
                                _context.Entry(dbuser).Property(x => x.Admin).IsModified = true;
                                _context.Entry(dbuser).Property(x => x.Description).IsModified = true;
                                _context.SaveChanges();
                            }
                            else
                            {
                                var games = _context.Game.Where(dbgame => dbgame.PlayerWinnerId == dbuser.Id);
                                foreach(var game in games)
                                {
                                    _context.Game.Attach(game);

                                    game.PlayerWinner = null;
                                    game.PlayerWinnerId = null;
                                    _context.Entry(game).Reference(x => x.PlayerWinner).IsModified = true;
                                    _context.Entry(game).Property(x => x.PlayerWinnerId).IsModified = true;

                                    _context.SaveChanges();
                                }

                                games = _context.Game.Where(dbgame => dbgame.PlayerLoserId == dbuser.Id);
                                foreach (var game in games)
                                {
                                    _context.Game.Attach(game);

                                    game.PlayerLoser = null;
                                    game.PlayerLoserId = null;
                                    _context.Entry(game).Reference(x => x.PlayerLoser).IsModified = true;
                                    _context.Entry(game).Property(x => x.PlayerLoserId).IsModified = true;

                                    _context.SaveChanges();
                                }

                                games = _context.Game.Where(dbgame => dbgame.PlayerBottomId == dbuser.Id);
                                foreach (var game in games)
                                {
                                    _context.Game.Attach(game);

                                    game.PlayerBottom = null;
                                    game.PlayerBottomId = null;
                                    _context.Entry(game).Reference(x => x.PlayerBottom).IsModified = true;
                                    _context.Entry(game).Property(x => x.PlayerBottomId).IsModified = true;

                                    _context.SaveChanges();
                                }

                                games = _context.Game.Where(dbgame => dbgame.PlayerTopId == dbuser.Id);
                                foreach (var game in games)
                                {
                                    _context.Game.Attach(game);

                                    game.PlayerTop = null;
                                    game.PlayerTopId = null;
                                    _context.Entry(game).Reference(x => x.PlayerTop).IsModified = true;
                                    _context.Entry(game).Property(x => x.PlayerTopId).IsModified = true;

                                    _context.SaveChanges();
                                }

                                var gameevents = _context.GameEvent.Where(dbgameevent => dbgameevent.UserId == dbuser.Id);
                                foreach (var gameEvent in gameevents)
                                {
                                    _context.GameEvent.Attach(gameEvent);

                                    gameEvent.User = null;
                                    gameEvent.UserId = null;
                                    _context.Entry(gameEvent).Reference(x => x.User).IsModified = true;
                                    _context.Entry(gameEvent).Property(x => x.UserId).IsModified = true;

                                    _context.SaveChanges();
                                }

                                _context.User.Remove(dbuser);
                                _context.SaveChanges();
                            }
                        }
                        catch (DbUpdateConcurrencyException)
                        {
                            throw;
                        }
                    }

                    return RedirectToAction(nameof(AdminUsers));
                }
            }
            return Unauthorized();
        }

        public async Task<IActionResult> AdminGames()
        {
            var userid = HttpContext.Session.GetInt32(SessionUserId);
            var name = HttpContext.Session.GetString(SessionUserName);
            if (userid != null && name != null)
            {
                var ans = _context.User.Where(dbuser => dbuser.Id == userid);

                if (ans.Count() == 1)
                {
                    var ele = ans.ToArray().ElementAt(0);
                    if (ele.Admin)
                    {
                        return (View(AdminGamesDto().ToList()));
                    }
                }
            }
            return Unauthorized();
        }

        [HttpPost]
        public async Task<IActionResult> EditGame(List<AdminGameDto> gamesDto)
        {
            var userid = HttpContext.Session.GetInt32(SessionUserId);
            var name = HttpContext.Session.GetString(SessionUserName);
            if (userid != null && name != null)
            {
                var ans = _context.User.Where(dbuser => dbuser.Id == userid && dbuser.Admin == true);

                if (ans.Count() == 1)
                {
                    foreach (var game in gamesDto)
                    {
                        Game dbgame = new Game()
                        {
                            Id = game.Id
                        };

                        try
                        {
                            if (game.MarkForDel)
                            {
                                _context.Game.Remove(dbgame);
                                _context.SaveChanges();
                            }
                        }
                        catch (DbUpdateConcurrencyException)
                        {
                            throw;
                        }
                    }

                    return RedirectToAction(nameof(AdminGames));
                }
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

        [Route("/wsspectate")]
        public async Task GetSpectate()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                var buffer = new byte[1024];
                WSMessageHandler.SendAsync(webSocket, "HEY");
                while (true)
                {
                    try
                    {
                        var message = await WSMessageHandler.ReceiveAsync(webSocket);

                        var receivedMessage = WSMessageHandler.HandleExit(message);
                        if (receivedMessage.Item1)
                        {
                            foreach (var gc in GameFinder.GamesSpectate)
                            {
                                GameFinder.RemoveSpectator(gc.Key, webSocket);
                            }
                            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                            return;
                        }

                        var receiveGame = WSMessageHandler.HandleSpectateMessage(message);
                        if (receiveGame.Item1)
                        {
                            foreach (var gc in GameFinder.GamesSpectate)
                            {
                                if(gc.Key.Game.Id == receiveGame.Item2)
                                {
                                    GameFinder.AddSpectator(gc.Key, webSocket);
                                    break;
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("WS Spectate Connection error: " + e.ToString());
                        return;
                    }
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
            return _context.Game.Where(e => e.PlayerWinner != null && e.PlayerWinner.Id == id).Count();
        }

        private int UserLoses(int? id)
        {
            return _context.Game.Where(e => e.PlayerLoser != null && e.PlayerLoser.Id == id).Count();
        }

        private IQueryable<GameDto> UserGamesDto(int? id)
        {
            var result = (from game in _context.Game
                          where (((game.PlayerWinner != null) && (game.PlayerLoser != null)) && ((game.PlayerWinner.Id == id) || (game.PlayerLoser.Id == id)))
                          select new GameDto()
                          {
                              Id = game.Id,
                              PlayerLoserName = game.PlayerLoser != null ? game.PlayerLoser.Name : "",
                              PlayerWinnerName = game.PlayerWinner != null ? game.PlayerWinner.Name : "",
                              TimeStart = game.TimeStart,
                              TimeEnd = game.TimeEnd,
                              Lost = (game.PlayerLoser.Id == id ? true : false),
                          });
            return result;
        }

        private IQueryable<AdminUserDto> AdminUsersDto()
        {
            var result = (from user in _context.User
                          select new AdminUserDto()
                          {
                              Id = user.Id,
                              Name = user.Name,
                              Description = user.Description != null ? user.Description : "",
                              Admin = user.Admin,
                              MarkForDel = false
                          });
            return result;
        }

        private IQueryable<AdminGameDto> AdminGamesDto()
        {
            var result = (from game in _context.Game
                          select new AdminGameDto()
                          {
                              Id = game.Id,
                              PlayerTopName = game.PlayerTop != null ? game.PlayerTop.Name : "",
                              PlayerBottomName = game.PlayerBottom != null ? game.PlayerBottom.Name : "",
                              PlayerLoserName = game.PlayerLoser != null ? game.PlayerLoser.Name : "",
                              PlayerWinnerName = game.PlayerWinner != null ? game.PlayerWinner.Name : "",
                              TimeStart = game.TimeStart,
                              TimeEnd = game.TimeEnd,
                              MarkForDel = false
                          });
            return result;
        }

        private Game FindGame(int? gameid)
        {
            var ans = _context.Game.Where(e => e.Id == gameid);
            if (ans.Count() > 0)
            {
                var ele = ans.ToArray().ElementAt(0);
                if (ele.TimeEnd != null) {
                    var delta = DateTime.Now - ele.TimeEnd;
                    if(delta.Value.Minutes > 0)
                    {
                        return null;
                    }
                }

                return ele;
            }
            return null;
        }

        private Game FindGameIfPlayerPlayed(int? gameid, int? userid)
        {
            var ans = _context.Game.Where(e => e.Id == gameid && (e.PlayerWinner.Id == userid || e.PlayerLoser.Id == userid));
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

        private IQueryable<GameEventDto> GameEventsDto(int? gameid)
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
                                  GameId = (int)gameid,
                                  Status = gameevent.Status,
                                  Notation = gameevent.Notation,
                                  Time = gameevent.Time,
                                  Comment = "",
                                  IsTop = gameevent.User.Id == ele.Item1 ? true : false
                              });
                return result;
            }
            return null;
        }

        private GameSpectateDto UserGameSpectateDto(int? gameid)
        {
            var result = (from game in _context.Game
                          where (game.Id == gameid)
                          select new GameSpectateDto()
                          {
                                Id = game.Id,
                                PlayerTopName = game.PlayerTop.Name,
                                PlayerBottomName = game.PlayerBottom.Name,
                                TimeStart = game.TimeStart
                          });
            if(result.Count() > 0)
            {
                return result.ToArray().ElementAt(0);
            }    
            return null;
        }



       
    }
}