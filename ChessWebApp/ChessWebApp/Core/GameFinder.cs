using ChessApp.game;
using ChessWebApp.Models;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;

namespace ChessWebApp.Core
{
    public static class GameFinder
    {
        private static Dictionary<User, WebSocket> queuedSockets = new Dictionary<User, WebSocket>();
        private static Dictionary<User, ChessGameController> games = new Dictionary<User, ChessGameController>();
        private static bool hosting = false;

        public static Tuple<ChessGameController, ChessPlayer> findGameOf(User user)
        {
            ChessGameController controller = games[user];
            if(controller != null)
            {
                if(controller.TopPlayer.user == user)
                {
                    return new Tuple<ChessGameController, ChessPlayer>(controller, controller.TopPlayer);
                }
                else if (controller.BottomPlayer.user == user)
                {
                    return new Tuple<ChessGameController, ChessPlayer>(controller, controller.BottomPlayer);
                }
            }
            return null;
        }

        public static void Queue(User user, WebSocket webSocket)
        {
            queuedSockets.Add(user, webSocket);
        }

        public static void Unqueue(User user)
        {
            queuedSockets.Remove(user);
        }

        public static async Task HostGameIfPossible()
        {
            while(true)
            {
                await Task.Delay(5000);

                try
                {
                    if (queuedSockets.Count >= 2)
                    {
                        GameFinder.hosting = true;
                        KeyValuePair<User, WebSocket> playerSocket1 = queuedSockets.First();
                        queuedSockets.Remove(playerSocket1.Key);

                        KeyValuePair<User, WebSocket> playerSocket2 = queuedSockets.First();
                        queuedSockets.Remove(playerSocket2.Key);

                        Random rnd = new Random();
                        ChessPlayer playerTop;
                        ChessPlayer playerBot;

                        int i = rnd.Next(2);
                        if (i == 1)
                        {
                            playerTop = new ChessPlayer(playerSocket1.Key, playerSocket1.Value, true);
                            playerBot = new ChessPlayer(playerSocket2.Key, playerSocket2.Value, false);
                            Console.WriteLine($"WS Info - {playerSocket1.Key.Name} will be playing with {playerSocket2.Key.Name}");
                        }
                        else
                        {
                            playerTop = new ChessPlayer(playerSocket2.Key, playerSocket2.Value, true);
                            playerBot = new ChessPlayer(playerSocket1.Key, playerSocket1.Value, false);
                            Console.WriteLine($"WS Info - {playerSocket2.Key.Name} will be playing with {playerSocket1.Key.Name}");
                        }

                        string messageTo1 = WSMessageHandler.GetGameCustomMessage($"You will be playing vs {playerSocket2.Key.Name}");
                        string messageTo2 = WSMessageHandler.GetGameCustomMessage($"You will be playing vs {playerSocket1.Key.Name}");

                        await WSMessageHandler.SendAsync(playerSocket1.Value, messageTo1);
                        await WSMessageHandler.SendAsync(playerSocket2.Value, messageTo2);

                        ChessGameController newGame = new ChessGameController(playerTop, playerBot);
                        games.Add(playerTop.user, newGame);
                        games.Add(playerBot.user, newGame);
                        GameFinder.hosting = false;
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
