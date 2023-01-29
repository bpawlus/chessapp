using ChessWebApp.ChessGame.Pieces;
using ChessWebApp;
using ChessWebApp.ChessGame;
using ChessWebApp.Data;
using ChessWebApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.WebSockets;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace ChessWebApp.ChessGame
{
    public class ChessGameController
    {
        private readonly char[] _boardRowNames = { '1', '2', '3', '4', '5', '6', '7', '8' };
        private readonly char[] _boardColNames = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H' };
        public static readonly int ChessboardSize = 8;
        public ChessBoardScenario CurrentScenario;
        public ChessPlayer TopPlayer { get; }
        public ChessPlayer BottomPlayer { get; }
        public ChessPlayer CurrentPlayer { get; set; }
        public ChessPlayer NotCurrentPlayer { get; set; }
        private List<Tuple<int, int, ChessBoardScenario>> _scenarios;
        public Game Game;

        public ChessGameController(ChessPlayer topPlayer, ChessPlayer bottomPlayer)
        {
            Game = GameFinder.AddGame(topPlayer, bottomPlayer);

            IFigure[,] chessboard = new IFigure[ChessboardSize, ChessboardSize];
            for (int i = 0; i < ChessboardSize; i++)
            {
                for (int j = 0; j < ChessboardSize; j++)
                {
                    chessboard[i, j] = null;
                }
            }

            for (int i = 0; i < 8; i++)
            {
                chessboard[1, i] = topPlayer.PawnFigures[i];
                chessboard[6, i] = bottomPlayer.PawnFigures[i];
            }

            chessboard[0, 0] = topPlayer.RookFigures[0];
            chessboard[7, 0] = bottomPlayer.RookFigures[0];

            chessboard[0, 7] = topPlayer.RookFigures[1];
            chessboard[7, 7] = bottomPlayer.RookFigures[1];

            chessboard[0, 1] = topPlayer.KnightFigures[0];
            chessboard[7, 1] = bottomPlayer.KnightFigures[0];

            chessboard[0, 6] = topPlayer.KnightFigures[1];
            chessboard[7, 6] = bottomPlayer.KnightFigures[1];

            chessboard[0, 2] = topPlayer.BishopFigures[0];
            chessboard[7, 2] = bottomPlayer.BishopFigures[0];

            chessboard[0, 5] = topPlayer.BishopFigures[1];
            chessboard[7, 5] = bottomPlayer.BishopFigures[1];

            chessboard[0, 3] = topPlayer.QueenFigure;
            chessboard[7, 3] = bottomPlayer.QueenFigure;

            chessboard[0, 4] = topPlayer.KingFigure;
            chessboard[7, 4] = bottomPlayer.KingFigure;

            TopPlayer = CurrentPlayer = topPlayer;
            BottomPlayer = NotCurrentPlayer = bottomPlayer;

            CurrentScenario = new ChessBoardScenario(chessboard, null);
            UpdateGameState();
        }

        private void Conclude(string info, ChessPlayer winner, ChessPlayer loser)
        {
            Console.WriteLine($"WS Game Info - ({TopPlayer.User.Name} vs {BottomPlayer.User.Name}) - {info} - {winner.User.Name} won!");
            winner.SendToPlayer(WSMessageHandler.GetGameStatusMessage($"GAME OVER! You won!\nReason: {info}", false));
            loser.SendToPlayer(WSMessageHandler.GetGameStatusMessage($"GAME OVER! You lost!\nReason: {info}", false));
            GameFinder.ConcludeGame(this, winner, loser);
        }

        private void UpdateGameState()
        {
            CurrentPlayer = CurrentPlayer == BottomPlayer ? TopPlayer : BottomPlayer;
            NotCurrentPlayer = NotCurrentPlayer == BottomPlayer ? TopPlayer : BottomPlayer;

            var data = WSMessageHandler.GetGameChessboardData(CurrentScenario.ChessboardScenario);
            HandleMessageGameBoard(TopPlayer, data);
            HandleMessageGameBoard(BottomPlayer, data);
            GameFinder.AddBoardStatus(Game, NotCurrentPlayer.User.Id+" "+data[6..]);

            _scenarios = CurrentScenario.GetAllTruePlayerMoves(CurrentPlayer);

            if (_scenarios.Count() == 0)
            {
                if (CurrentScenario.IsCheckScenario(CurrentPlayer))
                {
                    Conclude("Checkmate", NotCurrentPlayer, CurrentPlayer);
                }
                else
                {
                    Conclude("Stalemate", NotCurrentPlayer, CurrentPlayer);
                }
            }
        }

        public void HandleMessageGameBoard(ChessPlayer player, string data)
        {
            player.SendToPlayer(data);

            string hismove = WSMessageHandler.GetGameTurn(player == CurrentPlayer);
            player.SendToPlayer(hismove);
        }

        public void HandleMessageGameMove(ChessPlayer player, Tuple<bool, int, int, int, int> gameMoveMessage)
        {
            if (CurrentPlayer == player)
            {
                int rowo = gameMoveMessage.Item2;
                int colo = gameMoveMessage.Item3;
                int rown = gameMoveMessage.Item4;
                int coln = gameMoveMessage.Item5;
                IFigure chosenFigure = CurrentScenario.ChessboardScenario[rowo, colo];

                if (chosenFigure != null)
                {
                    foreach (var moveWithScenario in _scenarios)
                    {
                        if (
                            moveWithScenario.Item3.Issuer == chosenFigure &&
                            moveWithScenario.Item1 == rown &&
                            moveWithScenario.Item2 == coln
                        )
                        {
                            CurrentScenario = moveWithScenario.Item3;
                            foreach (var figure in moveWithScenario.Item3.Moved)
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
                IFigure chosenFigure = CurrentScenario.ChessboardScenario[row, col];

                if (chosenFigure != null)
                {
                    var moves = new HashSet<Tuple<int, int>>();
                    foreach (var moveWithScenario in _scenarios)
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

        public void HandleGameGiveUp(ChessPlayer player)
        {
            if (player == TopPlayer)
            {
                Conclude($"Surrendered by {TopPlayer.User.Name}", BottomPlayer, TopPlayer);
            }
            else if (player == BottomPlayer)
            {
                Conclude($"Surrendered by {BottomPlayer.User.Name}", TopPlayer, BottomPlayer);
            }
        }

        public void HandleGameOppDet(ChessPlayer player)
        {
            if (player == TopPlayer)
            {
                player.SendToPlayer("User name - " + BottomPlayer.User.Name + "\nUser description - " + BottomPlayer.User.Description);
            }
            else if (player == BottomPlayer)
            {
                player.SendToPlayer("User name - " + TopPlayer.User.Name + "\nUser description - " + TopPlayer.User.Description);
            }
        }
    }
}
