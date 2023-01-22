using ChessApp.game;
using ChessApp.game.pieces;
using ChessWebApp.Core.pieces;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace ChessWebApp.Core
{
    public class ChessboardScenario
    {
        public IFigure[,] chessboardScenario;
        public bool kingBeaten = false;
        public IFigure Issuer { get; }
        public List<IFigure> moved = new List<IFigure>();

        public ChessboardScenario(IFigure[,] baseChessboard, IFigure issuer)
        {
            Issuer = issuer;
            chessboardScenario = new IFigure[ChessGameController.chessboardSize, ChessGameController.chessboardSize];
            for (int i = 0; i < ChessGameController.chessboardSize; i++)
            {
                for (int j = 0; j < ChessGameController.chessboardSize; j++)
                {
                    chessboardScenario[i, j] = baseChessboard[i, j];
                }
            }
        }

        public void MoveScenario(int oldRow, int oldCol, int newRow, int newCol)
        {
            if(chessboardScenario[newRow, newCol] != null)
            {
                if(chessboardScenario[newRow, newCol] is King)
                {
                    kingBeaten = true;
                }
            }

            moved.Add(chessboardScenario[oldRow, oldCol]);
            chessboardScenario[newRow, newCol] = chessboardScenario[oldRow, oldCol];
            chessboardScenario[oldRow, oldCol] = null;
        }

        public List<Tuple<int, int, ChessboardScenario>> GetAllPlayerMoves(bool topPlayer)
        {
            List<Tuple<int, int, ChessboardScenario>> allMyMoves = new List<Tuple<int, int, ChessboardScenario>>();

            for (int i = 0; i < ChessGameController.chessboardSize; i++)
            {
                for (int j = 0; j < ChessGameController.chessboardSize; j++)
                {
                    if (chessboardScenario[i, j] != null && chessboardScenario[i, j].Owner.isTop == topPlayer)
                    {
                        var myMoves = chessboardScenario[i, j].GetMovesWithScenarios(chessboardScenario);
                        allMyMoves.AddRange(myMoves);
                    }
                }
            }

            return allMyMoves;
        }

        public List<Tuple<int, int, ChessboardScenario>> GetAllPlayerCheckSaveMoves(bool topPlayer)
        {
            List<Tuple<int, int, ChessboardScenario>> allMyMoves = new List<Tuple<int, int, ChessboardScenario>>();

            for (int i = 0; i < ChessGameController.chessboardSize; i++)
            {
                for (int j = 0; j < ChessGameController.chessboardSize; j++)
                {
                    if (chessboardScenario[i, j] != null && chessboardScenario[i, j].Owner.isTop == topPlayer)
                    {
                        var myMoves = chessboardScenario[i, j].GetMovesCheckSave(chessboardScenario);
                        allMyMoves.AddRange(myMoves);
                    }
                }
            }

            return allMyMoves;
        }

        public List<Tuple<int, int, ChessboardScenario>> GetAllTruePlayerMoves(ChessPlayer player)
        {
            List<Tuple<int, int, ChessboardScenario>> allMyMoves = GetAllPlayerMoves(player.isTop);
            List<Tuple<int, int, ChessboardScenario>> trueMyMoves = new List<Tuple<int, int, ChessboardScenario>>();

            if (!IsCheckScenario(player))
            {
                allMyMoves.AddRange(GetAllPlayerCheckSaveMoves(player.isTop));
            }

            foreach (var moveWithScenario in allMyMoves)
            {
                if (!moveWithScenario.Item3.IsCheckScenario(player) && !moveWithScenario.Item3.kingBeaten)
                {
                    trueMyMoves.Add(moveWithScenario);
                }
            }

            return trueMyMoves;
        }

        public bool IsCheckScenario(ChessPlayer player)
        {
            List<Tuple<int, int, ChessboardScenario>> allEnemyMoves = GetAllPlayerMoves(!player.isTop);
                
            foreach (var moveWithScenario in allEnemyMoves)
            {
                if (moveWithScenario.Item3.kingBeaten)
                {
                    return true;
                }
            }
 
            return false;
        }

    }
}
