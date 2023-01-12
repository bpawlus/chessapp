using ChessApp.game;
using ChessApp.game.pieces;
using System;

namespace ChessWebApp.Core
{
    public enum ChessPiecesEnum : short
    {
        ClassicPawn = 1,
        ClassicRook = 2,
        ClassicKnight = 3,
        ClassicBishop = 4,
        ClassicQueen = 5,
        ClassicKing = 6,
        Null = 0,
    }

    public static class ChessPiecesEnumTranslator
    {
        public static IFigure CreateFigure(ChessPlayer owner, ChessPiecesEnum e)
        {
            switch(e) 
            {
                case ChessPiecesEnum.ClassicPawn: return new Pawn(owner);
                case ChessPiecesEnum.ClassicRook: return new Rook(owner);
                case ChessPiecesEnum.ClassicKnight: return new Knight(owner);
                case ChessPiecesEnum.ClassicBishop: return new Bishop(owner);
                case ChessPiecesEnum.ClassicQueen: return new Queen(owner);
                case ChessPiecesEnum.ClassicKing: return new King(owner);
                default: return null;
            }
        }

        public static short TrasnslateFigureToShort(IFigure e)
        {
            short sign = 1;
            if (e == null)
                return (short)ChessPiecesEnum.Null;
            else if(e.Owner.isTop)
            {
                sign = -1;
            }
                
            if (e is Pawn)
                return (short)((short)ChessPiecesEnum.ClassicPawn * sign);
            else if(e is Rook)
                return (short)((short)ChessPiecesEnum.ClassicRook * sign);
            else if (e is Knight)
                return (short)((short)ChessPiecesEnum.ClassicKnight * sign);
            else if (e is Bishop)
                return (short)((short)ChessPiecesEnum.ClassicBishop * sign);
            else if (e is Queen)
                return (short)((short)ChessPiecesEnum.ClassicQueen * sign);
            else if (e is King)
                return (short)((short)ChessPiecesEnum.ClassicKing * sign);
            else
                return (short)ChessPiecesEnum.Null;
        }

        public static ChessPiecesEnum[] Translate(short[] toTranslate)
        {
            ChessPiecesEnum[] toRet = new ChessPiecesEnum[toTranslate.Length];
            for (int i = 0; i < toTranslate.Length; i++)
            {
                if (Enum.IsDefined(typeof(ChessPiecesEnum), toTranslate[i]))
                {
                    toRet[i] = (ChessPiecesEnum)toTranslate[i];
                }
                else
                {
                    toRet[i] = ChessPiecesEnum.Null;
                }
            }
            return toRet;
        }
    }
}
