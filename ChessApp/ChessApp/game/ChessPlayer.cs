using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ChessApp.game
{
    public class ChessPlayer
    {
        public Guid ReceivedPlayerUUID { get; set; }
        public Guid ReceivedGameUUID { get; set; }
        public ChessPlayer()
        {

        }
    }
}
