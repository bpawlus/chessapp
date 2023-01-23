using ChessWebApp.Core;
using ChessWebApp.Core.pieces;
using ChessWebApp.Core.piecesFactories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessApp.game.pieces
{
    public class QueenFactory : FigureFactory
    {
        public QueenFactory() : base("Queen")
        {
        }

        protected override IFigure GeneraterRawFigure(ChessPlayer owner)
        {
            return new Queen(owner);
        }
    }

    public class Queen : BeatableFigure
    {
        public Queen(ChessPlayer owner) : base(owner)
        {
        }

        public override List<Tuple<int, int, ChessBoardScenario>> GetMovesWithScenarios(IFigure[,] board)
        {
            List<Tuple<int, int, ChessBoardScenario>> toRet = new List<Tuple<int, int, ChessBoardScenario>> ();
            Tuple<int, int> ij = FindMe(board);

            int ipos = ij.Item1;
            int jpos = ij.Item2;

            bool[] hadBeateable = new bool[8] { false, false, false, false, false, false, false, false };
            for (int n = 1; n < ChessGameController.chessboardSize; n++)
            {
                int[] rows = new int[8] { ipos + n, ipos + n, ipos - n, ipos - n, ipos + n, ipos - n, ipos, ipos };
                int[] cols = new int[8] { jpos + n, jpos - n, jpos + n, jpos - n, jpos, jpos, jpos + n, jpos - n };
                for (int m = 0; m < rows.Length; m++)
                {
                    if (
                        !hadBeateable[m] &&
                        rows[m] >= 0 && rows[m] < ChessGameController.chessboardSize &&
                        cols[m] >= 0 && cols[m] < ChessGameController.chessboardSize
                    )
                    {
                        if (board[rows[m], cols[m]] != null)
                        {
                            IFigure unknownFigure = board[rows[m], cols[m]];
                            hadBeateable[m] = true;

                            if (unknownFigure.Owner == Owner)
                            {
                                continue;
                            }
                        }

                        ChessBoardScenario scenario = new ChessBoardScenario(board, this);
                        scenario.MoveScenario(ipos, jpos, rows[m], cols[m]);
                        toRet.Add(new Tuple<int, int, ChessBoardScenario>(rows[m], cols[m], scenario));
                    }
                }
            }
            return toRet;
        }
    }
}
