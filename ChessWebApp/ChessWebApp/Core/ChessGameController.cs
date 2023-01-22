using ChessApp.game.pieces;
using ChessWebApp;
using ChessWebApp.Core;
using ChessWebApp.Models;
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
        public ChessPlayer TopPlayer { get; }
        public ChessPlayer BottomPlayer { get; }
        public ChessPlayer CurrentPlayer { get; set; }
        public ChessPlayer NotCurrentPlayer { get; set; }
        bool end;
        private List<Tuple<int, int, ChessboardScenario>> scenarios;

        public ChessGameController(ChessPlayer topPlayer, ChessPlayer bottomPlayer)
        {
            IFigure[,] chessboard = new IFigure[chessboardSize, chessboardSize];
            for (int i = 0; i < chessboardSize; i++)
            {
                for (int j = 0; j < chessboardSize; j++)
                {
                    chessboard[i, j] = null;
                }
            }

            for (int i = 0; i < 8; i++)
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

            TopPlayer = CurrentPlayer = topPlayer;
            BottomPlayer = NotCurrentPlayer = bottomPlayer;

            currentScenario = new ChessboardScenario(chessboard, null);
            UpdateGameState();
        }

        private void conclude(string info, ChessPlayer winner)
        {
            Console.WriteLine($"WS Game Info - ({TopPlayer.user.Name} vs {BottomPlayer.user.Name}) - {info} - {winner.user.Name} won!");
            end = true;
        }

        private void UpdateGameState()
        {
            if (!end)
            {
                string board = WSMessageHandler.GetGameChessboardData(currentScenario.chessboardScenario);
                TopPlayer.SendToPlayer(board);
                BottomPlayer.SendToPlayer(board);

                CurrentPlayer = CurrentPlayer == BottomPlayer ? TopPlayer : BottomPlayer;
                NotCurrentPlayer = NotCurrentPlayer == BottomPlayer ? TopPlayer : BottomPlayer;
                scenarios = currentScenario.GetAllTruePlayerMoves(CurrentPlayer);

                if (scenarios.Count() == 0)
                {
                    if (currentScenario.IsCheckScenario(CurrentPlayer))
                    {
                        conclude("Checkmate", NotCurrentPlayer);
                    }
                    else
                    {
                        conclude("Stalemate", NotCurrentPlayer);
                    }
                }
            }
        }

        public void HandleMessageGameMove(ChessPlayer player, Tuple<bool, int, int, int, int> gameMoveMessage)
        {
            if (CurrentPlayer == player)
            {
                int rowo = gameMoveMessage.Item2;
                int colo = gameMoveMessage.Item3;
                int rown = gameMoveMessage.Item4;
                int coln = gameMoveMessage.Item5;
                IFigure chosenFigure = currentScenario.chessboardScenario[rowo, colo];

                if (chosenFigure != null)
                {
                    foreach (var moveWithScenario in scenarios)
                    {
                        if (
                            moveWithScenario.Item3.Issuer == chosenFigure &&
                            moveWithScenario.Item1 == rown &&
                            moveWithScenario.Item2 == coln
                        )
                        {
                            currentScenario = moveWithScenario.Item3;
                            foreach (var figure in moveWithScenario.Item3.moved)
                            {
                                figure.Moved = true;
                            }
                            UpdateGameState();
                            return;
                        }
                    }
                }

                player.SendToPlayer(WSMessageHandler.GetGameCustomMessage("This move is unavailable!"));
            }
            else
            {
                player.SendToPlayer(WSMessageHandler.GetGameCustomMessage("This is not your turn!"));
            }
        }

        public void HandleMessageGameGetMove(ChessPlayer player, Tuple<bool, int, int> gameGetMoveMessage)
        {
            if (CurrentPlayer == player)
            {
                int row = gameGetMoveMessage.Item2;
                int col = gameGetMoveMessage.Item3;
                IFigure chosenFigure = currentScenario.chessboardScenario[row, col];

                if (chosenFigure != null)
                {
                    var moves = new HashSet<Tuple<int, int>>();
                    foreach (var moveWithScenario in scenarios)
                    {
                        if (moveWithScenario.Item3.Issuer == chosenFigure)
                        {
                            moves.Add(new Tuple<int, int>(moveWithScenario.Item1, moveWithScenario.Item2));
                        }
                    }
                    player.SendToPlayer(WSMessageHandler.GetGameMovesData(moves));
                    return;
                }

                player.SendToPlayer(WSMessageHandler.GetGameCustomMessage("This move is unavailable!"));
            }
            else
            {
                player.SendToPlayer(WSMessageHandler.GetGameCustomMessage("This is not your turn!"));
            }
        }

        public void HandleGameGiveUp(ChessPlayer player, Tuple<bool> gameGiveUp)
        {
            if (player == TopPlayer)
            {
                conclude($"Surrendered by {TopPlayer.user.Name}", BottomPlayer);
            }
            else if (player == BottomPlayer)
            {
                conclude($"Surrendered by {BottomPlayer.user.Name}", TopPlayer);
            }
        }

        public void HandleGameOppDet(ChessPlayer player, Tuple<bool> gameOpponentDet)
        {
            if (player == TopPlayer)
            {
                player.SendToPlayer("User name - " + BottomPlayer.user.Name + "\nUser description - " + BottomPlayer.user.Description);
            }
            else if (player == BottomPlayer)
            {
                player.SendToPlayer("User name - " + TopPlayer.user.Name + "\nUser description - " + TopPlayer.user.Description);
            }
        }
    }
}
