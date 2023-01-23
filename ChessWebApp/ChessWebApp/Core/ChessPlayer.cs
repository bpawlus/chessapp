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
            short[] ids = new short[]{
                        u.VariantKing, u.VariantQueen,
                        u.VariantBishopLeft, u.VariantBishopRight,
                        u.VariantKnightLeft, u.VariantKnightRight,
                        u.VariantRookLeft, u.VariantRookRight,
                        u.VariantPawn1, u.VariantPawn2, u.VariantPawn3, u.VariantPawn4,
                        u.VariantPawn5, u.VariantPawn6, u.VariantPawn7, u.VariantPawn8
                    };

            _ws = ws;
            isTop = top;
            kingFigure = ChessPiecesFactories.CreateFigure(this, ids[0]);
            queenFigure = ChessPiecesFactories.CreateFigure(this, ids[1]);
            bishopFigures[0] = ChessPiecesFactories.CreateFigure(this, ids[2]);
            bishopFigures[1] = ChessPiecesFactories.CreateFigure(this, ids[3]);
            knightFigures[0] = ChessPiecesFactories.CreateFigure(this, ids[4]);
            knightFigures[1] = ChessPiecesFactories.CreateFigure(this, ids[5]);
            rookFigures[0] = ChessPiecesFactories.CreateFigure(this, ids[6]);
            rookFigures[1] = ChessPiecesFactories.CreateFigure(this, ids[7]);

            for(int i = 0; i < 8; i++)
            {
                pawnFigures[i] = ChessPiecesFactories.CreateFigure(this, ids[i + 8]);
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
