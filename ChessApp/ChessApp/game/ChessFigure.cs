using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessApp.Game
{
    public class ChessFigure
    {
        public string ImageSource { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }

        public ChessFigure(string imageSource, int row, int column)
        {
            this.ImageSource = imageSource;
            this.Row = row;
            this.Column = column;
        }
    }
}
