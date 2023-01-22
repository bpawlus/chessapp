using ChessWebApp.Core;
using ChessWebApp.Core.pieces;
using NuGet.Packaging.Signing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessApp.game.pieces
{
    public class Pawn : BeatableFigure
    {
        public Pawn(ChessPlayer owner) : base(owner)
        {
        }

        public override List<Tuple<int, int, ChessboardScenario>> GetMovesWithScenarios(IFigure[,] board)
        {
            List<Tuple<int, int, ChessboardScenario>> toRet = new List<Tuple<int, int, ChessboardScenario>> ();
            Tuple<int, int> ij = FindMe(board);

            int ipos = ij.Item1;
            int jpos = ij.Item2;

            int o = Owner.isTop ? 1 : -1;
            int[] rows = new int[4] { ipos + o * 2, ipos + o, ipos + o, ipos + o, };
            int[] cols = new int[4] { jpos, jpos, jpos - 1, jpos + 1 };

            if (
                !Moved &&
                rows[0] >= 0 && rows[0] < ChessGameController.chessboardSize &&
                cols[0] >= 0 && cols[0] < ChessGameController.chessboardSize
            )
            {
                if (board[rows[0], cols[0]] == null && board[rows[1], cols[1]] == null)
                {
                    ChessboardScenario scenario = new ChessboardScenario(board, this);
                    scenario.MoveScenario(ipos, jpos, rows[0], cols[0]);
                    if (rows[0] == 0 || rows[0] == 7)
                    {
                        IFigure figure = new Queen(Owner);
                        scenario.chessboardScenario[rows[0], cols[0]] = figure;
                    }
                    toRet.Add(new Tuple<int, int, ChessboardScenario>(rows[0], cols[0], scenario));
                }
            }

            if (
                rows[1] >= 0 && rows[1] < ChessGameController.chessboardSize &&
                cols[1] >= 0 && cols[1] < ChessGameController.chessboardSize
            )
            {
                if (board[rows[1], cols[1]] == null)
                {
                    ChessboardScenario scenario = new ChessboardScenario(board, this);
                    scenario.MoveScenario(ipos, jpos, rows[1], cols[1]);
                    if (rows[1] == 0 || rows[1] == 7)
                    {
                        IFigure figure = new Queen(Owner);
                        scenario.chessboardScenario[rows[1], cols[1]] = figure;
                    }
                    toRet.Add(new Tuple<int, int, ChessboardScenario>(rows[1], cols[1], scenario));
                }
            }


            for (int m = 2; m < 4; m++)
            {
                if (
                    rows[m] >= 0 && rows[m] < ChessGameController.chessboardSize &&
                    cols[m] >= 0 && cols[m] < ChessGameController.chessboardSize
                )
                {
                    if (board[rows[m], cols[m]] != null)
                    {
                        IFigure unknownFigure = board[rows[m], cols[m]];

                        if (unknownFigure.Owner != Owner)
                        {
                            ChessboardScenario scenario = new ChessboardScenario(board, this);
                            scenario.MoveScenario(ipos, jpos, rows[m], cols[m]);
                            if (rows[m] == 0 || rows[m] == 7)
                            {
                                IFigure figure = new Queen(Owner);
                                scenario.chessboardScenario[rows[m], cols[m]] = figure;
                            }
                            toRet.Add(new Tuple<int, int, ChessboardScenario>(rows[m], cols[m], scenario));
                        }
                    }
                }
            }

            return toRet;
        }
    }
}
