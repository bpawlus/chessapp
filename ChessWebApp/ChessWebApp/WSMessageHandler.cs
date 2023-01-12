using System.Text;
using System;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis.Differencing;
using ChessApp.game.pieces;
using ChessWebApp.Core;

namespace ChessWebApp
{
    public static class WSMessageHandler
    {
        public static Tuple<bool, string, string> HandleLoginMessage(string message)
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

            string filter = @"LO:(\w*)";
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

        public static string MessageChessboardData(IFigure[,] figures)
        {
            string header = "GBRD:";
            for(int i = 0; i < figures.GetLength(0); i++)
            {
                for (int j = 0; j < figures.GetLength(1), i++)
                {
                    header += ChessPiecesEnumTranslator.TrasnslateFigureToShort(figures[i, j]);
                    header += " ";
                }
            }

            return header;
        }

        public static string MessageGetMovesData(HashSet<Tuple<int, int>> moves)
        {
            string header = "GMOV:";
            foreach (var item in moves)
            {
                header += item.Item1;
                header += ",";
                header += item.Item2;
                header += " ";
            }

            return header;
        }

        public static string MessageChessboardMessage(string message)
        {
            return "GMES:" + message;
        }
    }
}
