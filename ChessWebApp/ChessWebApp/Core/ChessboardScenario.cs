using ChessApp.game;
using ChessApp.game.pieces;
using ChessWebApp.Core.pieces;

namespace ChessWebApp.Core
{
    public class ChessboardScenario
    {
        public IFigure[,] chessboardScenario;
        private IFigure[,] beatenField;
        public ChessPlayer player;

        public ChessboardScenario(IFigure[,] baseChessboard, ChessPlayer player)
        {
            chessboardScenario = new IFigure[ChessGameController.chessboardSize, ChessGameController.chessboardSize];
            for (int i = 0; i < ChessGameController.chessboardSize; i++)
            {
                for (int j = 0; j < ChessGameController.chessboardSize; j++)
                {
                    chessboardScenario[i, j] = baseChessboard[i, j];
                    beatenField[i, j] = null;
                }
            }
        }

        public void moveScenario(int oldRow, int oldCol, int newRow, int newCol)
        {
            if(chessboardScenario[newRow, newCol] != null)
            {
                beatenField[newRow, newCol] = chessboardScenario[newRow, newCol];
            }
            chessboardScenario[newRow, newCol] = chessboardScenario[oldRow, oldCol];
            chessboardScenario[oldRow, oldCol] = null;
        }

        public void swapScenario(int oldRow, int oldCol, int newRow, int newCol)
        {
            IFigure temp = chessboardScenario[newRow, newCol];
            chessboardScenario[newRow, newCol] = chessboardScenario[oldRow, oldCol];
            chessboardScenario[oldRow, oldCol] = temp;
        }

        public bool isCheckScenario(ChessPlayer player)
        {
            Tuple<int, int> playerKingPos = null;
            HashSet<Tuple<int, int>> allEnemyMoves = new HashSet<Tuple<int, int>>();

            for (int i = 0; i < ChessGameController.chessboardSize; i++)
            {
                for (int j = 0; j < ChessGameController.chessboardSize; j++)
                {
                    if (chessboardScenario[i,j] is KingFigure && chessboardScenario[i, j].Owner == player)
                    {
                        playerKingPos = new Tuple<int, int>(i, j);
                    }
                    else if(chessboardScenario[i, j] != null && chessboardScenario[i, j].Owner != player)
                    {
                        HashSet<Tuple<int, int>> enemyMoves = chessboardScenario[i, j].GetMoves(chessboardScenario, true);
                        allEnemyMoves.UnionWith(enemyMoves);
                    }
                }
            }

            if(playerKingPos == null)
            {
                throw new Exception("King not found exception");
            }
            else if(allEnemyMoves.Contains(playerKingPos))
            {
                return true;
            }

            return false;
        }

        public bool isStalemateScenario(ChessPlayer player)
        {
            for (int i = 0; i < ChessGameController.chessboardSize; i++)
            {
                for (int j = 0; j < ChessGameController.chessboardSize; j++)
                {
                    if (chessboardScenario[i, j] != null && chessboardScenario[i, j].Owner == player)
                    {
                        var myMoves = chessboardScenario[i, j].GetAvailableMoves(chessboardScenario, false);
                        foreach(var entry in myMoves)
                        {
                            if(!entry.Value.isCheckScenario(player))
                            {
                                return false;
                            }
                        }
                    }
                }
            }

            return true;
        }
    }
}
