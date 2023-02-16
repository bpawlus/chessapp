using ChessWebApp.ChessGame.Pieces;
using ChessWebApp.Data;
using ChessWebApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Reflection;
using System.Text;

namespace ChessWebApp.ChessGame
{
    public static class GameFinder
    {
        public static Dictionary<ChessGameController, HashSet<WebSocket>> GamesSpectate = new Dictionary<ChessGameController, HashSet<WebSocket>>();
        private static Dictionary<int, WebSocket> _queuedSockets = new Dictionary<int, WebSocket>();
        private static Dictionary<int, ChessGameController> _games = new Dictionary<int, ChessGameController>();
        private static DbContextOptions<MvcGameContext> _connectionSettings;
        public static Dictionary<string, Func<User, bool>> Conditions = new Dictionary<string, Func<User, bool>>()
        {
            { "Wins5", Wins5 },
            { "Wins10", Wins10 }
        };

        private static bool Wins5(User user)
        {
            return WinCount(user, 5);
        }

        private static bool Wins10(User user)
        {
            return WinCount(user, 10);
        }

        private static bool WinCount(User user, int count)
        {
            using (var context = new MvcGameContext(_connectionSettings))
            {
                var ans1 = context.Game.Where(e => e.PlayerWinner != null && e.PlayerWinner.Id == user.Id).Count();
                if (ans1 >= count)
                {
                    return true;
                }
                return false;
            }
        }





        public static Game AddGame(ChessPlayer topPlayer, ChessPlayer bottomPlayer)
        {
            using (var context = new MvcGameContext(_connectionSettings))
            {
                var ans1 = context.User.Where(dbuser => dbuser.Id == topPlayer.User.Id);
                var ele1 = ans1.ToArray().ElementAt(0);

                var ans2 = context.User.Where(dbuser => dbuser.Id == bottomPlayer.User.Id);
                var ele2 = ans2.ToArray().ElementAt(0);

                var game = new Game();
                game.PlayerTop = ele1;
                game.PlayerBottom = ele2;
                game.TimeStart = DateTime.Now;

                context.Game.Add(game);
                context.SaveChanges();

                return game;
            }
        }

        public static GameEvent AddBoardStatus(ChessGameController cgc, string status, User user, string notation)
        {
            using (var context = new MvcGameContext(_connectionSettings))
            {
                var ans1 = context.Game.Where(dbgame => dbgame.Id == cgc.Game.Id);
                var ele1 = ans1.ToArray().ElementAt(0);

                var ans2 = context.User.Where(dbuser => dbuser.Id == user.Id);
                var ele2 = ans2.ToArray().ElementAt(0);

                var gameEvent = new GameEvent();
                gameEvent.Game = ele1;
                gameEvent.Status = status;
                gameEvent.User = ele2;
                gameEvent.Time = DateTime.Now;
                gameEvent.Notation = notation;

                context.GameEvent.Add(gameEvent);
                context.SaveChanges();

                int usertop = user == cgc.TopPlayer.User ? -1 : 1;
                foreach (var entry in GamesSpectate[cgc])
                {
                    WSMessageHandler.SendAsync(entry, "ISTOP:" + usertop + " STATUS:" + status + " NOTATION:" + notation);
                }

                return gameEvent;
            }
        }

        public static void InitializeContext(IServiceProvider serviceProvider)
        {
            _connectionSettings = serviceProvider.GetRequiredService<DbContextOptions<MvcGameContext>>();
        }

        public static Tuple<ChessGameController, ChessPlayer> FindGameOf(User user)
        {
            if(_games.ContainsKey(user.Id))
            {
                var controller = _games[user.Id];
                if (controller.TopPlayer.User.Id == user.Id)
                {
                    return new Tuple<ChessGameController, ChessPlayer>(controller, controller.TopPlayer);
                }
                else if (controller.BottomPlayer.User.Id == user.Id)
                {
                    return new Tuple<ChessGameController, ChessPlayer>(controller, controller.BottomPlayer);
                }
            }
            return null;
        }

        public static void ConcludeGame(ChessGameController controller, ChessPlayer winner, ChessPlayer loser)
        {
            using (var context = new MvcGameContext(_connectionSettings))
            {
                _games.Remove(controller.TopPlayer.User.Id);
                _games.Remove(controller.BottomPlayer.User.Id);
                GamesSpectate.Remove(controller);

                var ans = context.Game.Where(dbgame => dbgame.Id == controller.Game.Id);
                var game = ans.ToArray().ElementAt(0);

                var answinner = context.User.Where(dbuser => dbuser.Id == winner.User.Id);
                var elewinner = answinner.ToArray().ElementAt(0);

                var ansloser = context.User.Where(dbuser => dbuser.Id == loser.User.Id);
                var eleloser = ansloser.ToArray().ElementAt(0);

                game.PlayerWinner = elewinner;
                game.PlayerLoser = eleloser;
                game.TimeEnd = DateTime.Now;

                try
                {
                    context.Game.Attach(game);
                    context.Entry(game).Reference(x => x.PlayerWinner).IsModified = true;
                    context.Entry(game).Reference(x => x.PlayerLoser).IsModified = true;
                    context.Entry(game).Property(x => x.TimeEnd).IsModified = true;
                    context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
            }
        }

        public static void Queue(User user, WebSocket webSocket)
        {
            _queuedSockets.Add(user.Id, webSocket);
        }

        public static void Unqueue(User user)
        {
            if (_queuedSockets.ContainsKey(user.Id))
            {
                _queuedSockets.Remove(user.Id);
            }
        }

        public static void AddSpectator(ChessGameController cgc, WebSocket webSocket)
        {
            GamesSpectate[cgc]?.Add(webSocket);
        }

        public static void RemoveSpectator(ChessGameController cgc, WebSocket webSocket)
        {
            GamesSpectate[cgc]?.Remove(webSocket);
        }

        public static async Task HostGameIfPossible()
        {
            while(true)
            {
                await Task.Delay(2000);

                try
                {
                    
                    if (_queuedSockets.Count >= 2)
                    {
                        using (var context = new MvcGameContext(_connectionSettings))
                        {
                            var playerSocket1 = _queuedSockets.First();
                            _queuedSockets.Remove(playerSocket1.Key);
                            var ans1 = context.User.Where(dbuser => dbuser.Id == playerSocket1.Key);
                            var ele1 = ans1.ToArray().ElementAt(0);

                            var playerSocket2 = _queuedSockets.First();
                            _queuedSockets.Remove(playerSocket2.Key);
                            var ans2 = context.User.Where(dbuser => dbuser.Id == playerSocket2.Key);
                            var ele2 = ans2.ToArray().ElementAt(0);

                            var rnd = new Random();
                            ChessPlayer playerTop;
                            ChessPlayer playerBot;

                            int i = rnd.Next(2);
                            if (i == 1)
                            {
                                playerTop = new ChessPlayer(ele1, playerSocket1.Value, true);
                                playerBot = new ChessPlayer(ele2, playerSocket2.Value, false);
                                Console.WriteLine($"WS Info - {ele1.Name} will be playing with {ele2.Name}");
                            }
                            else
                            {
                                playerTop = new ChessPlayer(ele2, playerSocket2.Value, true);
                                playerBot = new ChessPlayer(ele1, playerSocket1.Value, false);
                                Console.WriteLine($"WS Info - {ele2.Name} will be playing with {ele1.Name}");
                            }

                            string messageTo1 = WSMessageHandler.GetGameStatusMessage($"You will be playing vs {ele2.Name}", true);
                            string messageTo2 = WSMessageHandler.GetGameStatusMessage($"You will be playing vs {ele1.Name}", true);

                            await WSMessageHandler.SendAsync(playerSocket1.Value, messageTo1);
                            await WSMessageHandler.SendAsync(playerSocket2.Value, messageTo2);

                            var newGame = new ChessGameController(playerTop, playerBot);
                            _games.Add(playerTop.User.Id, newGame);
                            _games.Add(playerBot.User.Id, newGame);
                            GamesSpectate.Add(newGame, new HashSet<WebSocket>());
                            newGame.Start();
                        }
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine($"Hosting Exception: {e.ToString()}");
                }
            }
        }
    }
}
