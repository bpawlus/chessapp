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

                if (queuedSockets.Count >= 2)
                {
                    GameFinder.hosting = true;
                    KeyValuePair<User, WebSocket> playerSocket1 = queuedSockets.First();
                    queuedSockets.Remove(playerSocket1.Key);

                    KeyValuePair<User, WebSocket> playerSocket2 = queuedSockets.First();
                    queuedSockets.Remove(playerSocket2.Key);

                    byte[] data1 = Encoding.ASCII.GetBytes($"You will be playing vs {playerSocket2.Key.Id}");
                    await playerSocket1.Value.SendAsync(data1, WebSocketMessageType.Text, true, CancellationToken.None);

                    byte[] data2 = Encoding.ASCII.GetBytes($"You will be playing vs {playerSocket1.Key.Id}");
                    await playerSocket2.Value.SendAsync(data2, WebSocketMessageType.Text, true, CancellationToken.None);

                    Random rnd = new Random();
                    ChessPlayer playerTop;
                    ChessPlayer playerBot;

                    int i = rnd.Next(2);
                    if (i == 1)
                    {
                        playerTop = new ChessPlayer(playerSocket1.Key, playerSocket1.Value, true);
                        playerBot = new ChessPlayer(playerSocket2.Key, playerSocket2.Value, false);
                        Console.WriteLine($"WS Info - {playerSocket1.Key} will be playing with {playerSocket2.Key}");
                    }
                    else
                    {
                        playerTop = new ChessPlayer(playerSocket2.Key, playerSocket2.Value, true);
                        playerBot = new ChessPlayer(playerSocket1.Key, playerSocket1.Value, false);
                        Console.WriteLine($"WS Info - {playerSocket2.Key} will be playing with {playerSocket1.Key}");
                    }
                    
                    ChessGameController newGame = new ChessGameController(playerTop, playerBot);
                    newGame.StartGame();

                    GameFinder.hosting = false;
                }
            }
        }
    }
}
