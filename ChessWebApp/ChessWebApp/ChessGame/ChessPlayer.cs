using ChessWebApp.ChessGame.Pieces;
using ChessWebApp.Models;
using System;
using System.Net.WebSockets;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace ChessWebApp.ChessGame
{
    public class ChessPlayer
    {
        public static readonly bool DISABLE_WS_FOR_DEBUG = false;

        public IFigure KingFigure;
        public IFigure QueenFigure;
        public IFigure[] BishopFigures = new IFigure[2];
        public IFigure[] KnightFigures = new IFigure[2];
        public IFigure[] RookFigures = new IFigure[2];
        public IFigure[] PawnFigures = new IFigure[8];
        public bool IsTop;
        private WebSocket _ws;
        public User User;

        public ChessPlayer(User u, WebSocket ws, bool top)
        {
            User = u;
            short[] ids = new short[]{
                        u.VariantKing, u.VariantQueen,
                        u.VariantBishopLeft, u.VariantBishopRight,
                        u.VariantKnightLeft, u.VariantKnightRight,
                        u.VariantRookLeft, u.VariantRookRight,
                        u.VariantPawn1, u.VariantPawn2, u.VariantPawn3, u.VariantPawn4,
                        u.VariantPawn5, u.VariantPawn6, u.VariantPawn7, u.VariantPawn8
                    };

            _ws = ws;
            IsTop = top;
            KingFigure = ChessPiecesFactories.CreateFigure(this, ids[0]);
            QueenFigure = ChessPiecesFactories.CreateFigure(this, ids[1]);
            BishopFigures[0] = ChessPiecesFactories.CreateFigure(this, ids[2]);
            BishopFigures[1] = ChessPiecesFactories.CreateFigure(this, ids[3]);
            KnightFigures[0] = ChessPiecesFactories.CreateFigure(this, ids[4]);
            KnightFigures[1] = ChessPiecesFactories.CreateFigure(this, ids[5]);
            RookFigures[0] = ChessPiecesFactories.CreateFigure(this, ids[6]);
            RookFigures[1] = ChessPiecesFactories.CreateFigure(this, ids[7]);

            for(int i = 0; i < 8; i++)
            {
                PawnFigures[i] = ChessPiecesFactories.CreateFigure(this, ids[i + 8]);
            }
        }

        public async Task SendToPlayer(string message)
        {
            if(!DISABLE_WS_FOR_DEBUG) {
                await WSMessageHandler.SendAsync(_ws, message);
            }
            Console.WriteLine($"WS Game Info - Sent {message} to {User.Name}");
        }
    }
}
