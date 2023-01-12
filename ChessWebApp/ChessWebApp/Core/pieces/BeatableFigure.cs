using ChessApp.game;
using ChessApp.game.pieces;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Numerics;

namespace ChessWebApp.Core.pieces
{
    public abstract class BeatableFigure : IFigure
    {
        public virtual bool Beatable => true;

        protected ChessPlayer _owner;
        public ChessPlayer Owner => _owner;

        protected bool _moved = false;
        public virtual bool Moved { get => _moved; set => _moved = value; }

        protected abstract Dictionary<Tuple<int, int>, ChessboardScenario> GetRealMoves(IFigure[,] board);

        public virtual Dictionary<Tuple<int, int>, ChessboardScenario> GetAvailableMoves(IFigure[,] board, bool ignoreCheck)
        {
            Dictionary<Tuple<int, int>, ChessboardScenario> empty = new Dictionary<Tuple<int, int>, ChessboardScenario>();
            return GetAvailableMoves(empty, board, ignoreCheck);
        }

        public Dictionary<Tuple<int, int>, ChessboardScenario> GetAvailableMoves(Dictionary<Tuple<int, int>, ChessboardScenario> prepared, IFigure[,] board, bool ignoreCheck)
        {
            Dictionary<Tuple<int, int>, ChessboardScenario> allMoves = GetRealMoves(board);
            foreach (Tuple<int, int> ij in allMoves.Keys)
            {
                int j = ij.Item1;
                int i = ij.Item2;

                if (ignoreCheck || (!allMoves[ij].isCheckScenario(_owner) && !board[i, j].Beatable))
                {
                    prepared.Add(ij, allMoves[ij]);
                }
            }
            return prepared;
        }

        public ChessboardScenario MakeMove(IFigure[,] board, Tuple<int, int> id)
        {
            Dictionary<Tuple<int, int>, ChessboardScenario> moves = GetAvailableMoves(board, false);
            if(moves.ContainsKey(id))
            {
                this.Moved = true;
                return moves[id];
            }
            else
            {
                return null;
            }
        }

        public HashSet<Tuple<int, int>> GetMoves(IFigure[,] board, bool ignoreCheck)
        {
            HashSet<Tuple<int, int>> toRet = new HashSet<Tuple<int, int>>();
            Dictionary<Tuple<int, int>, ChessboardScenario> moves = GetAvailableMoves(board, ignoreCheck);
            foreach(Tuple<int, int> key in moves.Keys)
            {
                toRet.Add(key);
            }
            return toRet;
        }

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

        public BeatableFigure(ChessPlayer owner)
        {
            _owner = owner;
        }
    }
}
