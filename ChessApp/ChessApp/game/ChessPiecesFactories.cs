using ChessApp.game;
using ChessApp.game.pieces;
using ChessApp.Game;
using System;
using System.Collections.Generic;
using System.Windows.Documents;

namespace ChessWebApp.Core
{
    public static class ChessPiecesFactories
    {
        private static Dictionary<short, FigureFactory> factoriesDict = new Dictionary<short, FigureFactory>() {
            { 1, new PawnFactory() },
            { 2, new RookFactory() },
            { 3, new KnightFactory() },
            { 4, new BishopFactory() },
            { 5, new QueenFactory() },
            { 6, new KingFactory() }
        };

        public static HashSet<short> GetFactoryIDs()
        {
            var toRet = new HashSet<short>();
            foreach (short key in factoriesDict.Keys)
            {
                toRet.Add(key);
            }
            return toRet;
        }

        public static Tuple<string, string, string> GetFactoryFigureNames(short e)
        {
            if (factoriesDict.ContainsKey(e))
            {
                return new Tuple<string, string, string>(factoriesDict[e].DisplayFigureName, factoriesDict[e].TopPlayerFigureImageSource, factoriesDict[e].BottomPlayerFigureImageSource);
            }
            return null;
        }
    }
}
