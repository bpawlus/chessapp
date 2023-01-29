using ChessWebApp.ChessGame;
using ChessWebApp.ChessGame.Pieces;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Numerics;

namespace ChessWebApp.ChessGame.Pieces
{
    public abstract class BeatableFigure : IFigure
    {

        protected ChessPlayer _owner;
        public ChessPlayer Owner => _owner;

        protected bool _moved = false;
        public virtual bool Moved { get => _moved; set => _moved = value; }

        protected short _figureId;
        public short FigureId { get => _figureId; set => _figureId = value; }

        public abstract List<Tuple<int, int, ChessBoardScenario>> GetMovesWithScenarios(IFigure[,] board);

        public Tuple<int, int> FindMe(IFigure[,] board)
        {
            for (int i = 0; i < ChessGameController.ChessboardSize; i++)
            {
                for (int j = 0; j < ChessGameController.ChessboardSize; j++)
                {
                    if (board[i, j] == this)
                    {
                        return new Tuple<int, int>(i,j);
                    }
                }
            }
            return null;
        }

        public virtual List<Tuple<int, int, ChessBoardScenario>> GetMovesCheckSave(IFigure[,] board)
        {
            return new List<Tuple<int, int, ChessBoardScenario>>();
        }

        protected BeatableFigure(ChessPlayer owner)
        {
            _owner = owner;
        }
    }
}
