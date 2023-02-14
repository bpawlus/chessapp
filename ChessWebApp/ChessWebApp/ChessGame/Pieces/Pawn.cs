using ChessWebApp.ChessGame;
using ChessWebApp.ChessGame.Pieces;
using NuGet.Packaging.Signing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessWebApp.ChessGame.Pieces
{
    public class PawnFactory : FigureFactory
    {
        public PawnFactory() : base("Pawn")
        {
        }

        protected override IFigure GeneraterRawFigure(ChessPlayer owner)
        {
            return new Pawn(owner);
        }
    }

    public class Pawn : BeatableFigure
    {
        public Pawn(ChessPlayer owner) : base(owner)
        {
        }

        public override char NotationName => 'P';

        public override List<Tuple<int, int, ChessBoardScenario>> GetMovesWithScenarios(IFigure[,] board)
        {
            List<Tuple<int, int, ChessBoardScenario>> toRet = new List<Tuple<int, int, ChessBoardScenario>> ();
            Tuple<int, int> ij = FindMe(board);

            int ipos = ij.Item1;
            int jpos = ij.Item2;

            int o = Owner.IsTop ? 1 : -1;
            int[] rows = new int[4] { ipos + o * 2, ipos + o, ipos + o, ipos + o, };
            int[] cols = new int[4] { jpos, jpos, jpos - 1, jpos + 1 };

            if (
                !Moved &&
                rows[0] >= 0 && rows[0] < ChessGameController.ChessboardSize &&
                cols[0] >= 0 && cols[0] < ChessGameController.ChessboardSize
            )
            {
                if (board[rows[0], cols[0]] == null && board[rows[1], cols[1]] == null)
                {
                    ChessBoardScenario scenario = new ChessBoardScenario(board, this);
                    scenario.MoveScenario(ipos, jpos, rows[0], cols[0]);
                    if (rows[0] == 0 || rows[0] == 7)
                    {
                        IFigure figure = new Queen(Owner);
                        scenario.ChessboardScenario[rows[0], cols[0]] = figure;
                    }
                    toRet.Add(new Tuple<int, int, ChessBoardScenario>(rows[0], cols[0], scenario));
                }
            }

            if (
                rows[1] >= 0 && rows[1] < ChessGameController.ChessboardSize &&
                cols[1] >= 0 && cols[1] < ChessGameController.ChessboardSize
            )
            {
                if (board[rows[1], cols[1]] == null)
                {
                    ChessBoardScenario scenario = new ChessBoardScenario(board, this);
                    scenario.MoveScenario(ipos, jpos, rows[1], cols[1]);
                    if (rows[1] == 0 || rows[1] == 7)
                    {
                        IFigure figure = new Queen(Owner);
                        scenario.ChessboardScenario[rows[1], cols[1]] = figure;
                    }
                    toRet.Add(new Tuple<int, int, ChessBoardScenario>(rows[1], cols[1], scenario));
                }
            }


            for (int m = 2; m < 4; m++)
            {
                if (
                    rows[m] >= 0 && rows[m] < ChessGameController.ChessboardSize &&
                    cols[m] >= 0 && cols[m] < ChessGameController.ChessboardSize
                )
                {
                    if (board[rows[m], cols[m]] != null)
                    {
                        IFigure unknownFigure = board[rows[m], cols[m]];

                        if (unknownFigure.Owner != Owner)
                        {
                            ChessBoardScenario scenario = new ChessBoardScenario(board, this);
                            scenario.MoveScenario(ipos, jpos, rows[m], cols[m]);
                            if (rows[m] == 0 || rows[m] == 7)
                            {
                                IFigure figure = new Queen(Owner);
                                scenario.ChessboardScenario[rows[m], cols[m]] = figure;
                                scenario.AppendToNotation("=H");
                            }
                            toRet.Add(new Tuple<int, int, ChessBoardScenario>(rows[m], cols[m], scenario));
                        }
                    }
                }
            }

            return toRet;
        }
    }
}
