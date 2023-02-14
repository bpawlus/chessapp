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
        private static Dictionary<int, WebSocket> queuedSockets = new Dictionary<int, WebSocket>();
        private static Dictionary<int, ChessGameController> games = new Dictionary<int, ChessGameController>();
        private static DbContextOptions<MvcGameContext> _connectionSettings;

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

        public static GameEvent AddBoardStatus(Game game, string status, User user, string notation)
        {
            using (var context = new MvcGameContext(_connectionSettings))
            {
                var ans1 = context.Game.Where(dbgame => dbgame.Id == game.Id);
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

                return gameEvent;
            }
        }

        public static void InitializeContext(IServiceProvider serviceProvider)
        {
            _connectionSettings = serviceProvider.GetRequiredService<DbContextOptions<MvcGameContext>>();
        }

        public static Tuple<ChessGameController, ChessPlayer> FindGameOf(User user)
        {
            if(games.ContainsKey(user.Id))
            {
                var controller = games[user.Id];
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
                games.Remove(controller.TopPlayer.User.Id);
                games.Remove(controller.BottomPlayer.User.Id);

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
            queuedSockets.Add(user.Id, webSocket);
        }

        public static void Unqueue(User user)
        {
            if (queuedSockets.ContainsKey(user.Id))
            {
                queuedSockets.Remove(user.Id);
            }
        }

        public static async Task HostGameIfPossible()
        {
            while(true)
            {
                await Task.Delay(2000);

                try
                {
                    
                    if (queuedSockets.Count >= 2)
                    {
                        using (var context = new MvcGameContext(_connectionSettings))
                        {
                            var playerSocket1 = queuedSockets.First();
                            queuedSockets.Remove(playerSocket1.Key);
                            var ans1 = context.User.Where(dbuser => dbuser.Id == playerSocket1.Key);
                            var ele1 = ans1.ToArray().ElementAt(0);

                            var playerSocket2 = queuedSockets.First();
                            queuedSockets.Remove(playerSocket2.Key);
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
                            games.Add(playerTop.User.Id, newGame);
                            games.Add(playerBot.User.Id, newGame);
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
