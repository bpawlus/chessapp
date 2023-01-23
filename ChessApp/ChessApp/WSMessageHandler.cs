using ChessApp.Game;
using ChessWebApp.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Xml.Linq;

namespace ChessApp
{
    public static class WSMessageHandler
    {
        public static void SendUserLoginMessage(WebSocket ws, string login, string password)
        {
            string header = $"L:{login} P:{password}";
            SendAsync(ws, header);
        }

        public static void SendUserLogoutMessage(WebSocket ws, string reason)
        {
            string header = $"LO:{reason}";
            SendAsync(ws, header);
        }

        public static void SendUserFindGameMessage(WebSocket ws)
        {
            SendAsync(ws, "FG");
        }

        public static void SendGameMoveMessage(WebSocket ws, int ro, int co, int rn, int cn)
        {
            string header = $"GM RO:{ro} CO:{co} RN:{rn} CN:{cn}";
            SendAsync(ws, header);
        }

        public static void SendGameGetMoveMessage(WebSocket ws, int ro, int co)
        {
            string header = $"GM R:{ro} C:{co}";
            SendAsync(ws, header);
        }

        public static void SendGameGiveUpMessage(WebSocket ws)
        {
            SendAsync(ws, "GM GO");
        }

        public static void SendGameOppDetMessage(WebSocket ws)
        {
            SendAsync(ws, "GM ED");
        }



        public static Tuple<bool> HandleServiceLoginOk(string message)
        {
            bool correct = false;

            string filter = @"L OK";
            Regex rg = new Regex(filter);
            MatchCollection mc = Regex.Matches(message, filter);
            if (mc.Count == 1)
            {
                correct = true;
            }
            return new Tuple<bool>(correct);
        }

        public static Tuple<bool, bool> HandleGameTurn(string message)
        {
            bool correct = false;
            bool yourturn = false;

            string filter = @"GTRN: (T|F)";
            Regex rg = new Regex(filter);
            MatchCollection mc = Regex.Matches(message, filter);
            if (mc.Count == 1)
            {
                foreach (Match match in mc)
                {
                    correct = true;
                    yourturn = match.Groups[1].Value == "T" ? true : false;
                }
            }
            return new Tuple<bool, bool>(correct, yourturn);
        }

        public static Tuple<bool, bool, string> HandleGameStatusMessage(string message)
        {
            bool correct = false;
            bool start = false;
            string info = "";
            HashSet<ChessFigure> figures = new HashSet<ChessFigure>();

            string filter1 = @"GS: ST (.*)";
            string filter2 = @"GS: GO (.*)";
            Regex rg = new Regex(filter1);
            MatchCollection mc = Regex.Matches(message, filter1);

            if (mc.Count == 1)
            {
                foreach (Match match in mc)
                {
                    correct = true;
                    start = true;
                    info = match.Groups[1].Value;
                }
            }

            rg = new Regex(filter2);
            mc = Regex.Matches(message, filter2);

            if (mc.Count == 1)
            {
                foreach (Match match in mc)
                {
                    correct = true;
                    start = false;
                    info = match.Groups[1].Value;
                }
            }

            return new Tuple<bool, bool, string>(correct, start, info);
        }

        public static Tuple<bool, HashSet<ChessFigure>> HandleGameChessboardData(string message)
        {
            bool correct = false;
            HashSet<ChessFigure> figures = new HashSet<ChessFigure>();

            string filter = @"GBRD: ((-?\d*),(\d*),(\d*) )*";
            Regex rg = new Regex(filter);
            MatchCollection mc = Regex.Matches(message, filter);

            if (mc.Count == 1)
            {
                correct = true;
            }

            if (!correct) return new Tuple<bool, HashSet<ChessFigure>>(correct, figures);

            filter = @"(-?\d*),(\d*),(\d*) ";
            rg = new Regex(filter);
            mc = Regex.Matches(message, filter);
            var factoryIds = ChessPiecesFactories.GetFactoryIDs();

            if (mc.Count >= 1)
            {
                foreach (Match match in mc)
                {
                    int fig = Int16.Parse(match.Groups[1].Value);
                    int ro = Int16.Parse(match.Groups[2].Value);
                    int co = Int16.Parse(match.Groups[3].Value);
                    string figname = "unknown.png";
                    foreach (var factoryId in factoryIds) {
                        if(factoryId == Math.Abs((short)fig))
                        {
                            var names = ChessPiecesFactories.GetFactoryFigureNames(factoryId);
                            figname = fig < 0 ? names.Item2 : names.Item3;
                            break;
                        }
                    }

                    figures.Add(new ChessFigure(figname, ro, co));
                }
            }

            return new Tuple<bool, HashSet<ChessFigure>>(correct, figures);
        }

        public static Tuple<bool, HashSet<Tuple<int,int>>> HandleGameMovesData(string message)
        {
            bool correct = false;
            HashSet<Tuple<int, int>> moves = new HashSet<Tuple<int, int>>();

            string filter = @"GMOV: ((\d*),(\d*) )*";
            Regex rg = new Regex(filter);
            MatchCollection mc = Regex.Matches(message, filter);

            if (mc.Count == 1)
            {
                correct = true;
            }

            if (!correct) return new Tuple<bool, HashSet<Tuple<int, int>>>(correct, moves) ;

            filter = @"(\d*),(\d*)";
            rg = new Regex(filter);
            mc = Regex.Matches(message, filter);

            if (mc.Count >= 1)
            {
                foreach (Match match in mc)
                {
                    int ro = Int16.Parse(match.Groups[1].Value);
                    int co = Int16.Parse(match.Groups[2].Value);
                    moves.Add(new Tuple<int, int>(ro, co));
                }
            }

            return new Tuple<bool, HashSet<Tuple<int, int>>>(correct, moves);
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

        public static Tuple<bool, string> HandleGameCustomMessage(string message)
        {
            bool correct = false;
            string servmessage = "";

            string filter = @"GMES:(.*)";
            Regex rg = new Regex(filter);
            MatchCollection mc = Regex.Matches(message, filter);
            if (mc.Count == 1)
            {
                foreach (Match match in mc)
                {
                    correct = true;
                    servmessage = match.Groups[1].Value;
                }
            }

            return new Tuple<bool, string>(correct, servmessage);
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
