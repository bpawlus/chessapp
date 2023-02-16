using ChessWebApp.ChessGame;
using ChessWebApp.ChessGame.Pieces;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace ChessWebApp.ChessGame
{
    public class ChessBoardScenario
    {
        public IFigure[,] ChessboardScenario;
        public bool KingBeaten = false;
        public IFigure Issuer { get; }
        public List<IFigure> Moved = new List<IFigure>();
        private string _notation = "";

        public ChessBoardScenario(IFigure[,] baseChessboard, IFigure issuer)
        {
            Issuer = issuer;
            ChessboardScenario = new IFigure[ChessGameController.ChessboardSize, ChessGameController.ChessboardSize];
            for (int i = 0; i < ChessGameController.ChessboardSize; i++)
            {
                for (int j = 0; j < ChessGameController.ChessboardSize; j++)
                {
                    ChessboardScenario[i, j] = baseChessboard[i, j];
                }
            }
        }

        public void SetCustomNotation(string newNotation)
        {
            _notation = newNotation;
        }

        public void AppendToNotation(string extraNotation)
        {
            _notation += extraNotation;
        }

        public string GetNotation()
        {
            return _notation;
        }

        public void MoveScenario(int oldRow, int oldCol, int newRow, int newCol)
        {
            _notation += Issuer.NotationName;
            var pos = Issuer.FindMe(ChessboardScenario);
            _notation += char.ToLower(ChessGameController.BoardColNames[pos.Item2]);
            _notation += char.ToLower(ChessGameController.BoardRowNames[pos.Item1]);
            if (ChessboardScenario[newRow, newCol] != null)
            {
                if (ChessboardScenario[newRow, newCol] is King)
                {
                    KingBeaten = true;
                }
                else
                {
                    _notation += "x";
                }
            }

            _notation += char.ToLower(ChessGameController.BoardColNames[newCol]);
            _notation += char.ToLower(ChessGameController.BoardRowNames[newRow]);

            Moved.Add(ChessboardScenario[oldRow, oldCol]);
            ChessboardScenario[newRow, newCol] = ChessboardScenario[oldRow, oldCol];
            ChessboardScenario[oldRow, oldCol] = null;
        }

        public List<Tuple<int, int, ChessBoardScenario>> GetAllPlayerMoves(bool topPlayer)
        {
            List<Tuple<int, int, ChessBoardScenario>> allMyMoves = new List<Tuple<int, int, ChessBoardScenario>>();

            for (int i = 0; i < ChessGameController.ChessboardSize; i++)
            {
                for (int j = 0; j < ChessGameController.ChessboardSize; j++)
                {
                    if (ChessboardScenario[i, j] != null && ChessboardScenario[i, j].Owner.IsTop == topPlayer)
                    {
                        var myMoves = ChessboardScenario[i, j].GetMovesWithScenarios(ChessboardScenario);
                        allMyMoves.AddRange(myMoves);
                    }
                }
            }

            return allMyMoves;
        }

        public List<Tuple<int, int, ChessBoardScenario>> GetAllPlayerCheckSaveMoves(bool topPlayer)
        {
            List<Tuple<int, int, ChessBoardScenario>> allMyMoves = new List<Tuple<int, int, ChessBoardScenario>>();

            for (int i = 0; i < ChessGameController.ChessboardSize; i++)
            {
                for (int j = 0; j < ChessGameController.ChessboardSize; j++)
                {
                    if (ChessboardScenario[i, j] != null && ChessboardScenario[i, j].Owner.IsTop == topPlayer)
                    {
                        var myMoves = ChessboardScenario[i, j].GetMovesCheckSave(ChessboardScenario);
                        allMyMoves.AddRange(myMoves);
                    }
                }
            }

            return allMyMoves;
        }

        public List<Tuple<int, int, ChessBoardScenario>> GetAllTruePlayerMoves(ChessPlayer player)
        {
            List<Tuple<int, int, ChessBoardScenario>> allMyMoves = GetAllPlayerMoves(player.IsTop);
            List<Tuple<int, int, ChessBoardScenario>> trueMyMoves = new List<Tuple<int, int, ChessBoardScenario>>();

            if (!IsCheckScenario(player))
            {
                allMyMoves.AddRange(GetAllPlayerCheckSaveMoves(player.IsTop));
            }

            foreach (var moveWithScenario in allMyMoves)
            {
                if (!moveWithScenario.Item3.IsCheckScenario(player) && !moveWithScenario.Item3.KingBeaten)
                {
                    trueMyMoves.Add(moveWithScenario);
                }
            }

            return trueMyMoves;
        }

        public bool IsCheckScenario(ChessPlayer player)
        {
            List<Tuple<int, int, ChessBoardScenario>> allEnemyMoves = GetAllPlayerMoves(!player.IsTop);
                
            foreach (var moveWithScenario in allEnemyMoves)
            {
                if (moveWithScenario.Item3.KingBeaten)
                {
                    return true;
                }
            }
 
            return false;
        }

    }
}
