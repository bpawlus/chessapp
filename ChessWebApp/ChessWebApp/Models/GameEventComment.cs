using System.ComponentModel.DataAnnotations.Schema;

namespace ChessWebApp.Models
{
    public class GameEventComment
    {
        public int Id { get; set; }
        [ForeignKey("GameEventId")]
        public GameEvent? GameEvent { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }
        [ForeignKey("UserId")]
        public string Comment { get; set; }
    }
}
