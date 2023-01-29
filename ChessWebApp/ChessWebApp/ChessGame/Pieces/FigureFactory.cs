using ChessWebApp.ChessGame.Pieces;
using ChessWebApp.ChessGame;

namespace ChessWebApp.ChessGame.Pieces
{
    public abstract class FigureFactory
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

        protected abstract IFigure GeneraterRawFigure(ChessPlayer owner);

        public IFigure GenerateFigure(ChessPlayer owner)
        {
            var figure = GeneraterRawFigure(owner);
            var id = owner.IsTop ? -1 : 1;

            figure.FigureId = (short)id;
            return figure;
        }
    }
}
