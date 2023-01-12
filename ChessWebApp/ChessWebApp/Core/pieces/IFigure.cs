using ChessWebApp.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessApp.game.pieces
{
    public interface IFigure
    {
        public bool Beatable
        {
            get;
        }

        public ChessPlayer Owner
        {
            get;
        }

        public bool Moved
        {
            get;
            set;
        }

        public Dictionary<Tuple<int, int>, ChessboardScenario> GetAvailableMoves(IFigure[,] board, bool ignoreCheck);
        public Dictionary<Tuple<int, int>, ChessboardScenario> GetAvailableMoves(Dictionary<Tuple<int, int>, ChessboardScenario> prepared, IFigure[,] board, bool ignoreCheck);
        public ChessboardScenario MakeMove(IFigure[,] board, Tuple<int, int> id);
        public HashSet<Tuple<int, int>> GetMoves(IFigure[,] board, bool ignoreCheck);
        public Tuple<int, int> FindMe(IFigure[,] board);
    }
}
