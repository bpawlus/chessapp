using ChessApp.game;
using ChessApp.game.pieces;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Numerics;

namespace ChessWebApp.Core.pieces
{
    public abstract class BeatableFigure : IFigure
    {

        protected ChessPlayer _owner;
        public ChessPlayer Owner => _owner;

        protected bool _moved = false;
        public virtual bool Moved { get => _moved; set => _moved = value; }

        public abstract List<Tuple<int, int, ChessboardScenario>> GetMovesWithScenarios(IFigure[,] board);

        public Tuple<int, int> FindMe(IFigure[,] board)
        {
            for (int i = 0; i < ChessGameController.chessboardSize; i++)
            {
                for (int j = 0; j < ChessGameController.chessboardSize; j++)
                {
                    if (board[i, j] == this)
                    {
                        return new Tuple<int, int>(i,j);
                    }
                }
            }
            return null;
        }

        public virtual List<Tuple<int, int, ChessboardScenario>> GetMovesCheckSave(IFigure[,] board)
        {
            return new List<Tuple<int, int, ChessboardScenario>>();
        }

        public BeatableFigure(ChessPlayer owner)
        {
            _owner = owner;
        }
    }
}
