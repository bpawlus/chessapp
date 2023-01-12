using ChessApp.game.pieces;

namespace ChessWebApp.Core.pieces
{
    public abstract class KingFigure : BeatableFigure
    {
        public override bool Beatable => false;

        public KingFigure(ChessPlayer owner) : base(owner)
        {
        }
    }
}
