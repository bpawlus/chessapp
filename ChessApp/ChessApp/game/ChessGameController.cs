using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessApp.game
{
    public class ChessGameController
    {
        public readonly char[] boardRowNames = { '1', '2', '3', '4', '5', '6', '7', '8' };
        public readonly char[] boardColNames = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H' };
        public readonly int chessboardSize = 8;
        public ChessPlayer Player { get; set; }

        public ChessGameController()
        {
            Player = new ChessPlayer();
        }
    }
}
