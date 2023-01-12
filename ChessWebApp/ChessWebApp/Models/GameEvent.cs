using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChessWebApp.Models
{
    public class GameEvent
    {
        public int Id { get; set; }
        public short ActionId { get; set; }
        [ForeignKey("GameId")]
        public Game Game { get; set; }
        public string ActionValue { get; set; }
        [Timestamp]
        public Byte[] TimeStamp { get; set; }
    }
}
