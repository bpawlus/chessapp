using ChessApp.game.pieces;
using ChessWebApp;
using ChessWebApp.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace ChessApp.game
{
    public class ChessGameController
    {
        private readonly char[] boardRowNames = { '1', '2', '3', '4', '5', '6', '7', '8' };
        private readonly char[] boardColNames = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H' };
        public static readonly int chessboardSize = 8;
        public ChessboardScenario currentScenario;
        public ChessPlayer _topPlayer;
        public ChessPlayer _bottomPlayer;
        public bool topPlayerTurn = false;

        public ChessGameController(ChessPlayer topPlayer, ChessPlayer bottomPlayer)
        {
            IFigure[,] chessboard = new IFigure[chessboardSize, chessboardSize];
            for(int i = 0; i < chessboardSize; i++)
            {
                for (int j = 0; j < chessboardSize; j++)
                {
                    chessboard[i,j] = null;
                }
            }

            _topPlayer = topPlayer;
            _bottomPlayer = bottomPlayer;

            for(int i = 0; i < 8; i++)
            {
                chessboard[1, i] = topPlayer.pawnFigures[i];
                chessboard[6, i] = bottomPlayer.pawnFigures[i];
            }

            chessboard[0, 0] = topPlayer.rookFigures[0];
            chessboard[7, 0] = bottomPlayer.rookFigures[0];

            chessboard[0, 7] = topPlayer.rookFigures[1];
            chessboard[7, 7] = bottomPlayer.rookFigures[1];

            chessboard[0, 1] = topPlayer.knightFigures[0];
            chessboard[7, 1] = bottomPlayer.knightFigures[0];

            chessboard[0, 6] = topPlayer.knightFigures[1];
            chessboard[7, 6] = bottomPlayer.knightFigures[1];

            chessboard[0, 2] = topPlayer.bishopFigures[0];
            chessboard[7, 2] = bottomPlayer.bishopFigures[0];

            chessboard[0, 5] = topPlayer.bishopFigures[1];
            chessboard[7, 5] = bottomPlayer.bishopFigures[1];

            chessboard[0, 3] = topPlayer.queenFigure;
            chessboard[7, 3] = bottomPlayer.queenFigure;

            chessboard[0, 4] = topPlayer.kingFigure;
            chessboard[7, 4] = bottomPlayer.kingFigure;

            currentScenario = new ChessboardScenario(new IFigure[chessboardSize, chessboardSize], bottomPlayer);
        }

        public async Task StartGame()
        {
            var topPlayerListener = _topPlayer.ListenForRequests();
            var bottomPlayerListener = _bottomPlayer.ListenForRequests();
            var gameListener = new List<Task<string>> { topPlayerListener, bottomPlayerListener };
            bool end = false;

            while (!end)
            {
                _topPlayer.SendToPlayer(WSMessageHandler.MessageChessboardData(currentScenario.chessboardScenario));
                _bottomPlayer.SendToPlayer(WSMessageHandler.MessageChessboardData(currentScenario.chessboardScenario));

                if (currentScenario.isStalemateScenario(currentScenario.player))
                {
                    if (currentScenario.isCheckScenario(currentScenario.player))
                    {
                        //szach mat
                        end = true;
                        break;
                    }
                    else
                    {
                        //pat
                        end = true;
                        break;
                    }
                }
                else
                {
                    bool nextStage = false;
                    ChessPlayer currentPlayer = topPlayerTurn ? _topPlayer : _bottomPlayer;
                    ChessPlayer notCurrentPlayer = topPlayerTurn ? _bottomPlayer : _topPlayer;
                    while (!nextStage)
                    {
                        Task<string> finishedTask = await Task.WhenAny(gameListener);

                        Tuple<bool, int, int, int, int> gameMoveMessage = WSMessageHandler.HandleGameMoveMessage(finishedTask.Result);
                        Tuple<bool, int, int> gameGetMoveMessage = WSMessageHandler.HandleGameGetMoveMessage(finishedTask.Result);
                        Tuple<bool> gameGiveUp = WSMessageHandler.HandleGameGiveUpMessage(finishedTask.Result);
                        Tuple<bool> gameOpponentDet = WSMessageHandler.HandleGameOppDetMessage(finishedTask.Result);

                        if (gameMoveMessage.Item1)
                        {
                            if ((finishedTask == topPlayerListener && topPlayerTurn) || (finishedTask == bottomPlayerListener && !topPlayerTurn))
                            {
                                int rowo = gameMoveMessage.Item2;
                                int colo = gameMoveMessage.Item3;
                                int rown = gameMoveMessage.Item4;
                                int coln = gameMoveMessage.Item5;
                                var moves = currentScenario.chessboardScenario[rowo, colo].GetAvailableMoves(currentScenario.chessboardScenario, false);
                                Tuple<int, int> move = new Tuple<int, int>(rown, coln);
                                if (moves.ContainsKey(move))
                                {
                                    currentScenario = moves[move];
                                    nextStage = true;
                                }
                            }
                            else
                            {
                                notCurrentPlayer.SendToPlayer(WSMessageHandler.MessageChessboardMessage("This is not your turn!"));
                            }
                                
                            continue;
                        }

                        if (gameGetMoveMessage.Item1)
                        {
                            if ((finishedTask == topPlayerListener && topPlayerTurn) || (finishedTask == bottomPlayerListener && !topPlayerTurn))
                            {
                                int row = gameGetMoveMessage.Item2;
                                int col = gameGetMoveMessage.Item3;
                                var moves = currentScenario.chessboardScenario[row, col].GetMoves(currentScenario.chessboardScenario, false);
                                currentPlayer.SendToPlayer(WSMessageHandler.MessageGetMovesData(moves));
                            }
                            else
                            {
                                notCurrentPlayer.SendToPlayer(WSMessageHandler.MessageChessboardMessage("This is not your turn!"));
                            }

                            continue;
                        }

                        if (gameGiveUp.Item1)
                        {
                            end = true;
                            continue;
                        }

                        if (gameOpponentDet.Item1)
                        {
                            if (finishedTask == topPlayerListener)
                            {
                                _topPlayer.SendToPlayer("User name - " + _bottomPlayer.user.Name + "\nUser description - " + _bottomPlayer.user.Description);
                            }
                            else if (finishedTask == bottomPlayerListener)
                            {
                                _bottomPlayer.SendToPlayer("User name - " + _topPlayer.user.Name + "\nUser description - " + _topPlayer.user.Description);
                            }

                            continue;
                        }

                        topPlayerTurn = !topPlayerTurn;
                    }
                }
            }
            gameListener[0].Dispose();
            gameListener[1].Dispose();
        }



        //public IFigure[] getPiecesThatCanBeOnField(ChessboardField field)
    }
}
