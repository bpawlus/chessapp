using ChessWebApp.ChessGame;
using ChessWebApp.ChessGame.Pieces;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ChessWebApp.ChessGame.Pieces
{
    public class KingFactory : FigureFactory
    {
        public KingFactory() : base("King")
        {
        }

        protected override IFigure GeneraterRawFigure(ChessPlayer owner)
        {
            return new King(owner);
        }
    }

    public class King : BeatableFigure
    {
        public King(ChessPlayer owner) : base(owner)
        {
        }

        public override char NotationName => 'K';

        public override List<Tuple<int, int, ChessBoardScenario>> GetMovesWithScenarios(IFigure[,] board)
        {
            List<Tuple<int, int, ChessBoardScenario>> toRet = new List<Tuple<int, int, ChessBoardScenario>> ();
            Tuple<int, int> ij = FindMe(board);

            int ipos = ij.Item1;
            int jpos = ij.Item2;

            int[] rows = new int[8] { ipos + 1, ipos + 1, ipos - 1, ipos - 1, ipos + 1, ipos - 1, ipos, ipos };
            int[] cols = new int[8] { jpos + 1, jpos - 1, jpos + 1, jpos - 1, jpos, jpos, jpos + 1, jpos - 1 };
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

        public override List<Tuple<int, int, ChessBoardScenario>> GetMovesCheckSave(IFigure[,] board)
        {
            List<Tuple<int, int, ChessBoardScenario>> toRet = base.GetMovesCheckSave(board);

            if (!Moved) {
                ChessBoardScenario checkScenario = new ChessBoardScenario(board, this);
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
                    ChessBoardScenario scenario = new ChessBoardScenario(board, this);
                    scenario.MoveScenario(ipos, jpos, ipos, jpos - 2);
                    scenario.MoveScenario(ipos, 0, ipos, jpos - 1);
                    scenario.SetCustomNotation("O-O-O");

                    toRet.Add(new Tuple<int, int, ChessBoardScenario>(ipos, jpos - 2, scenario));
                }

                if (
                    (board[ipos, 7] != null && board[ipos, 7] is Rook && !board[ipos, 7].Moved) &&
                    empty2
                )
                {
                    ChessBoardScenario scenario = new ChessBoardScenario(board, this);
                    scenario.MoveScenario(ipos, jpos, ipos, jpos + 2);
                    scenario.MoveScenario(ipos, 7, ipos, jpos + 1);
                    scenario.SetCustomNotation("O-O");

                    toRet.Add(new Tuple<int, int, ChessBoardScenario>(ipos, jpos + 2, scenario));
                }
            }
            return toRet;
        }
    }
}
