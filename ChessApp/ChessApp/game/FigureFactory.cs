using ChessApp.game.pieces;
using System.Windows.Documents;

namespace ChessApp.Game
{
    public class FigureFactory
    {
        protected string _topPlayerFigureImageSource;
        public string TopPlayerFigureImageSource
        {
            get => _topPlayerFigureImageSource;
        }

        protected string _bottomPlayerFigureImageSource;
        public string BottomPlayerFigureImageSource
        {
            get => _bottomPlayerFigureImageSource;
        }

        protected string _displayFigureName;
        public string DisplayFigureName
        {
            get => _displayFigureName;
        }

        protected FigureFactory(string name)
        {
            _topPlayerFigureImageSource = "black_" + name.ToLower() + ".png";
            _bottomPlayerFigureImageSource = "white_" + name.ToLower() + ".png";
            _displayFigureName = name;
        }
    }
}
