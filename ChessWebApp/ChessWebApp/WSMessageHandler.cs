using System.Text;
using System;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis.Differencing;
using ChessWebApp.ChessGame.Pieces;
using ChessWebApp.ChessGame;
using System.Net.WebSockets;

namespace ChessWebApp
{
    public static class WSMessageHandler
    {
        public static Tuple<bool, string, string> HandleUserLoginMessage(string message)
        {
            bool correct = false;
            string name = "";
            string password = "";

            string filter = @"L:(\w*) P:(\w*)";
            Regex rg = new Regex(filter);
            MatchCollection mc = Regex.Matches(message, filter);
            if (mc.Count == 1)
            {
                foreach (Match match in mc)
                {
                    correct = true;
                    name = match.Groups[1].Value;
                    password = match.Groups[2].Value;
                }
            }
            return new Tuple<bool, string, string>(correct, name, password);
        }

        public static Tuple<bool, string> HandleUserLogoutMessage(string message)
        {
            bool correct = false;
            string reason = "";

            string filter = @"LO:(.*)";
            Regex rg = new Regex(filter);
            MatchCollection mc = Regex.Matches(message, filter);
            if (mc.Count == 1)
            {
                foreach (Match match in mc)
                {
                    correct = true;
                    reason = match.Groups[1].Value;
                }
            }
            return new Tuple<bool, string>(correct, reason);
        }

        public static Tuple<bool> HandleUserFindGameMessage(string message)
        {
            bool correct = false;

            string filter = @"FG";
            Regex rg = new Regex(filter);
            MatchCollection mc = Regex.Matches(message, filter);
            if (mc.Count == 1)
            {
                correct = true;
            }
            return new Tuple<bool>(correct);
        }

        public static Tuple<bool, int, int, int, int> HandleGameMoveMessage(string message)
        {
            bool correct = false;
            int rowo = -1;
            int colo = -1;
            int rown = -1;
            int coln = -1;

            string filter = @"GM RO:(\d*) CO:(\d*) RN:(\d*) CN:(\d*)";
            Regex rg = new Regex(filter);
            MatchCollection mc = Regex.Matches(message, filter);
            if (mc.Count == 1)
            {
                foreach (Match match in mc)
                {
                    correct = true;
                    rowo = Int16.Parse(match.Groups[1].Value);
                    colo = Int16.Parse(match.Groups[2].Value);
                    rown = Int16.Parse(match.Groups[3].Value);
                    coln = Int16.Parse(match.Groups[4].Value);
                }
            }
            return new Tuple<bool, int, int, int, int>(correct, rowo, colo, rown, coln);
        }

        public static Tuple<bool, int, int> HandleGameGetMoveMessage(string message)
        {
            bool correct = false;
            string reason = "";
            int row = -1;
            int col = -1;

            string filter = @"GM R:(\d*) C:(\d*)";
            Regex rg = new Regex(filter);
            MatchCollection mc = Regex.Matches(message, filter);
            if (mc.Count == 1)
            {
                foreach (Match match in mc)
                {
                    correct = true;
                    row = Int16.Parse(match.Groups[1].Value);
                    col = Int16.Parse(match.Groups[2].Value);
                }
            }

            return new Tuple<bool, int, int>(correct, row, col);
        }

        public static Tuple<bool> HandleGameGiveUpMessage(string message)
        {
            bool correct = false;
            string reason = "";

            string filter = @"GM GO";
            Regex rg = new Regex(filter);
            MatchCollection mc = Regex.Matches(message, filter);
            if (mc.Count == 1)
            {
                correct = true;

            }
            return new Tuple<bool>(correct);
        }

        public static Tuple<bool> HandleGameOppDetMessage(string message)
        {
            bool correct = false;
            string reason = "";

            string filter = @"GM ED";
            Regex rg = new Regex(filter);
            MatchCollection mc = Regex.Matches(message, filter);
            if (mc.Count == 1)
            {
                correct = true;
            }
            return new Tuple<bool>(correct);
        }

        public static Tuple<bool, string> HandleExit(string message)
        {
            bool correct = false;
            string reason = "";

            string filter = @"EXIT:(.*)";
            Regex rg = new Regex(filter);
            MatchCollection mc = Regex.Matches(message, filter);
            if (mc.Count == 1)
            {
                foreach (Match match in mc)
                {
                    correct = true;
                    reason = match.Groups[1].Value;
                }
            }
            return new Tuple<bool, string>(correct, reason);
        }


        public static string GetGameChessboardData(IFigure[,] figures)
        {
            string header = "GBRD: ";
            for(int i = 0; i < figures.GetLength(0); i++)
            {
                for (int j = 0; j < figures.GetLength(1); j++)
                {
                    if (figures[i,j] != null)
                    {
                        short number = figures[i, j].FigureId;
                        header += $"{number},{i},{j} ";
                    }
                }
            }

            return header;
        }

        public static string GetGameTurn(bool yourmove)
        {
            string movelabel = yourmove ? "T" : "F";
            string header = "GTRN: " + movelabel;

            return header;
        }

        public static string GetGameMovesData(HashSet<Tuple<int, int>> moves)
        {
            string header = "GMOV: ";
            foreach (var item in moves)
            {
                header += $"{item.Item1},{item.Item2} ";
            }

            return header;
        }

        public static string GetGameCustomMessage(string message)
        {
            return "GMES:" + message;
        }

        public static string GetGameStatusMessage(string message, bool start)
        {
            string startorend = start ? "ST" : "GO";
            return "GS: " + startorend + " " + message;
        }

        public static string GetGameStartPosition(bool istop)
        {
            string movelabel = istop ? "T" : "F";
            string header = "GPOSTOP: " + movelabel;

            return header;
        }

        public static void SendServiceLoginOk(WebSocket ws)
        {
            string header = "L OK";
            SendAsync(ws, header);
        }

        public async static Task SendAsync(WebSocket ws, string message)
        {
            await ws.SendAsync(Encoding.ASCII.GetBytes(message), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        public async static Task<string> ReceiveAsync(WebSocket ws)
        {
            var buffer = new byte[1024];

            try
            {
                var receiveResult = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (receiveResult.MessageType == WebSocketMessageType.Close)
                {
                    return $"EXIT:{receiveResult.CloseStatusDescription}";
                }
                else
                    return Encoding.ASCII.GetString(buffer, 0, receiveResult.Count);
            }
            catch (Exception e)
            {
                Console.WriteLine("WS Connection error: " + e.ToString());
                return "";
            }
        }
    }
}
