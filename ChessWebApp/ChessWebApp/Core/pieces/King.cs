using ChessWebApp.Core;
using ChessWebApp.Core.pieces;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ChessApp.game.pieces
{
    public class King : BeatableFigure
    {
        public King(ChessPlayer owner) : base(owner)
        {
        }

        public override List<Tuple<int, int, ChessboardScenario>> GetMovesWithScenarios(IFigure[,] board)
        {
            List<Tuple<int, int, ChessboardScenario>> toRet = new List<Tuple<int, int, ChessboardScenario>> ();
            Tuple<int, int> ij = FindMe(board);

            int ipos = ij.Item1;
            int jpos = ij.Item2;

            int[] rows = new int[8] { ipos + 1, ipos + 1, ipos - 1, ipos - 1, ipos + 1, ipos - 1, ipos, ipos };
            int[] cols = new int[8] { jpos + 1, jpos - 1, jpos + 1, jpos - 1, jpos, jpos, jpos + 1, jpos - 1 };
            for (int m = 0; m < rows.Length; m++)
            {
                if (
                    rows[m] >= 0 && rows[m] < ChessGameController.chessboardSize &&
                    cols[m] >= 0 && cols[m] < ChessGameController.chessboardSize
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

                    ChessboardScenario scenario = new ChessboardScenario(board, this);
                    scenario.MoveScenario(ipos, jpos, rows[m], cols[m]);
                    toRet.Add(new Tuple<int, int, ChessboardScenario>(rows[m], cols[m], scenario));
                }
            }

            return toRet;
        }

        public override List<Tuple<int, int, ChessboardScenario>> GetMovesCheckSave(IFigure[,] board)
        {
            List<Tuple<int, int, ChessboardScenario>> toRet = base.GetMovesCheckSave(board);

            if (!Moved) {
                ChessboardScenario checkScenario = new ChessboardScenario(board, this);
                Tuple<int, int> ij = FindMe(board);

                int ipos = ij.Item1;
                int jpos = ij.Item2;

                bool empty1 = true;
                for(int i = 1; i < jpos; i++)
                {
                    if(board[ipos, i] != null)
                    {
                        empty1 = false;
                        break;
                    }
                }

                bool empty2 = true;
                for (int i = jpos + 1; i < 7; i++)
                {
                    if (board[ipos, i] != null)
                    {
                        empty2 = false;
                        break;
                    }
                }

                if (
                    (board[ipos, 0] != null && board[ipos, 0] is Rook && !board[ipos, 0].Moved) &&
                    empty1
                )
                {
                    ChessboardScenario scenario = new ChessboardScenario(board, this);
                    scenario.MoveScenario(ipos, jpos, ipos, jpos - 2);
                    scenario.MoveScenario(ipos, 0, ipos, jpos - 1);

                    toRet.Add(new Tuple<int, int, ChessboardScenario>(ipos, jpos - 2, scenario));
                }

                if (
                    (board[ipos, 7] != null && board[ipos, 7] is Rook && !board[ipos, 7].Moved) &&
                    empty2
                )
                {
                    ChessboardScenario scenario = new ChessboardScenario(board, this);
                    scenario.MoveScenario(ipos, jpos, ipos, jpos + 2);
                    scenario.MoveScenario(ipos, 7, ipos, jpos + 1);

                    toRet.Add(new Tuple<int, int, ChessboardScenario>(ipos, jpos + 2, scenario));
                }
            }
            return toRet;
        }
    }
}
