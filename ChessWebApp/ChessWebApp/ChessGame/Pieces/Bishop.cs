using ChessWebApp.ChessGame;
using ChessWebApp.ChessGame.Pieces;
using ChessWebApp.ChessGame.Pieces;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessWebApp.ChessGame.Pieces
{
    public class BishopFactory : FigureFactory
    {
        public BishopFactory() : base("Bishop")
        {
        }

        protected override IFigure GeneraterRawFigure(ChessPlayer owner)
        {
            return new Bishop(owner);
        }
    }

    public class Bishop : BeatableFigure
    {
        public Bishop(ChessPlayer owner) : base(owner)
        {
        }

        public override char NotationName => 'B';

        public override List<Tuple<int, int, ChessBoardScenario>> GetMovesWithScenarios(IFigure[,] board)
        {
            List<Tuple<int, int, ChessBoardScenario>> toRet = new List<Tuple<int, int, ChessBoardScenario>> ();
            Tuple<int, int> ij = FindMe(board);
            
            int ipos = ij.Item1;
            int jpos = ij.Item2;

            bool[] hadBeateable = new bool[4] { false, false, false, false };
            for (int n = 1; n < ChessGameController.ChessboardSize; n++)
            {
                int[] rows = new int[4] { ipos + n, ipos + n, ipos - n, ipos - n };
                int[] cols = new int[4] { jpos + n, jpos - n, jpos + n, jpos - n };
                for (int m = 0; m < rows.Length; m++)
                {
                    if (
                        !hadBeateable[m] &&
                        rows[m] >= 0 && rows[m] < ChessGameController.ChessboardSize &&
                        cols[m] >= 0 && cols[m] < ChessGameController.ChessboardSize
                    )
                    {
                        if (board[rows[m],cols[m]] != null)
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
