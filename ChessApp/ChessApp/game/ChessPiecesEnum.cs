using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media.Media3D;

namespace ChessApp.Game
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
        public static string TrasnslateShortToImage(short e)
        {
            string prefix = e < 0 ? "black_" : "white_";
            string sufix = ".png";
            short figure = Math.Abs(e);

            switch (figure)
            {
                case (short)ChessPiecesEnum.ClassicPawn:
                    return prefix + "pawn" + sufix;
                case (short)ChessPiecesEnum.ClassicRook:
                    return prefix + "rook" + sufix;
                case (short)ChessPiecesEnum.ClassicKnight:
                    return prefix + "knight" + sufix;
                case (short)ChessPiecesEnum.ClassicBishop:
                    return prefix + "bishop" + sufix;
                case (short)ChessPiecesEnum.ClassicQueen:
                    return prefix + "queen" + sufix;
                case (short)ChessPiecesEnum.ClassicKing:
                    return prefix + "king" + sufix;
                default:
                    return "empty";
            }
        }
    }
}
