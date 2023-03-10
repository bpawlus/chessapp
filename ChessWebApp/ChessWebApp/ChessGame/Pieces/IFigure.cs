using ChessWebApp.ChessGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessWebApp.ChessGame.Pieces
{
    public interface IFigure
    {
        public char NotationName
        {
            get;
        }

        public short FigureId
        {
            get; set;
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

        public List<Tuple<int, int, ChessBoardScenario>> GetMovesWithScenarios(IFigure[,] board);
        public Tuple<int, int> FindMe(IFigure[,] board);
        public List<Tuple<int, int, ChessBoardScenario>> GetMovesCheckSave(IFigure[,] board);
    }
}
