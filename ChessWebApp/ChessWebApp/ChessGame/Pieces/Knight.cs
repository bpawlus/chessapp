using ChessWebApp.ChessGame;
using ChessWebApp.ChessGame.Pieces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessWebApp.ChessGame.Pieces
{
    public class KnightFactory : FigureFactory
    {
        public KnightFactory() : base("Knight")
        {
        }

        protected override IFigure GeneraterRawFigure(ChessPlayer owner)
        {
            return new Knight(owner);
        }
    }

    public class Knight : BeatableFigure
    {
        public Knight(ChessPlayer owner) : base(owner)
        {
        }

        public override char NotationName => 'N';

        public override List<Tuple<int, int, ChessBoardScenario>> GetMovesWithScenarios(IFigure[,] board)
        {
            List<Tuple<int, int, ChessBoardScenario>> toRet = new List<Tuple<int, int, ChessBoardScenario>> ();
            Tuple<int, int> ij = FindMe(board);

            int ipos = ij.Item1;
            int jpos = ij.Item2;

            int[] rows = new int[8] { ipos + 2, ipos + 2, ipos - 2, ipos - 2, ipos + 1, ipos + 1, ipos - 1, ipos - 1 };
            int[] cols = new int[8] { jpos + 1, jpos - 1, jpos + 1, jpos - 1, jpos + 2, jpos - 2, jpos + 2, jpos - 2 };
            for (int m = 0; m < rows.Length; m++)
            {
                if (
                    rows[m] >= 0 && rows[m] < ChessGameController.ChessboardSize &&
                    cols[m] >= 0 && cols[m] < ChessGameController.ChessboardSize
                )
                {
                    if (board[rows[m], cols[m]] != null)
                    {
                        IFigure unknownFigure = board[rows[m], cols[m]];

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

            return toRet;

        }
    }
}
