using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChessWebApp.Models
{
    public class GameEvent
    {
        public int Id { get; set; }
        [ForeignKey("GameId")]
        public Game? Game { get; set; }
        public string ActionDescription { get; set; }
        public DateTime? Time { get; set; }
    }
}
