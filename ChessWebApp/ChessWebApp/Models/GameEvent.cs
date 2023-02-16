using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChessWebApp.Models
{
    public class GameEvent
    {
        public int Id { get; set; }
        [ForeignKey("GameId")]
        public Game? Game { get; set; }
        public string Status { get; set; }
        public string Notation { get; set; }
        public DateTime? Time { get; set; }
        public ICollection<GameEventComment>? GameEventComments { get; set; }
        public int? UserId { get; set; }
        public User? User { get; set; }
    }
}
