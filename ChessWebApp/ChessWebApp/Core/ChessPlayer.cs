using ChessApp.game.pieces;
using ChessWebApp.Models;
using System;
using System.Net.WebSockets;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace ChessWebApp.Core
{
    public class ChessPlayer
    {
        public static readonly bool DISABLE_WS_FOR_DEBUG = false;

        public IFigure kingFigure;
        public IFigure queenFigure;
        public IFigure[] bishopFigures = new IFigure[2];
        public IFigure[] knightFigures = new IFigure[2];
        public IFigure[] rookFigures = new IFigure[2];
        public IFigure[] pawnFigures = new IFigure[8];
        public bool isTop;
        private WebSocket _ws;
        public User user;
        public ChessPlayer(User u, WebSocket ws, bool top)
        {
            user = u;
            ChessPiecesEnum[] enumIds = ChessPiecesEnumTranslator.Translate(new short[]{
                        u.VariantKing, u.VariantQueen,
                        u.VariantBishopLeft, u.VariantBishopRight,
                        u.VariantKnightLeft, u.VariantKnightRight,
                        u.VariantRookLeft, u.VariantRookRight,
                        u.VariantPawn1, u.VariantPawn2, u.VariantPawn3, u.VariantPawn4,
                        u.VariantPawn5, u.VariantPawn6, u.VariantPawn7, u.VariantPawn8
                    });

            _ws = ws;
            isTop = top;
            kingFigure = ChessPiecesEnumTranslator.CreateFigure(this, enumIds[0]);
            queenFigure = ChessPiecesEnumTranslator.CreateFigure(this, enumIds[1]);
            bishopFigures[0] = ChessPiecesEnumTranslator.CreateFigure(this, enumIds[2]);
            bishopFigures[1] = ChessPiecesEnumTranslator.CreateFigure(this, enumIds[3]);
            knightFigures[0] = ChessPiecesEnumTranslator.CreateFigure(this, enumIds[4]);
            knightFigures[1] = ChessPiecesEnumTranslator.CreateFigure(this, enumIds[5]);
            rookFigures[0] = ChessPiecesEnumTranslator.CreateFigure(this, enumIds[6]);
            rookFigures[1] = ChessPiecesEnumTranslator.CreateFigure(this, enumIds[7]);

            for(int i = 0; i < 8; i++)
            {
                pawnFigures[i] = ChessPiecesEnumTranslator.CreateFigure(this, enumIds[i + 8]);
            }
        }

        public async Task SendToPlayer(string message)
        {
            if(!DISABLE_WS_FOR_DEBUG) {
                await WSMessageHandler.SendAsync(_ws, message);
            }
            Console.WriteLine($"WS Game Info - Sent {message} to {user.Name}");
        }
    }
}
