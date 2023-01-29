using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChessWebApp.Models
{
    public class Game
    {
        public int Id { get; set; }
        [ForeignKey("PlayerBottomId")]
        public User? PlayerBottom { get; set; }
        [ForeignKey("PlayerTopId")]
        public User? PlayerTop { get; set; }
        [ForeignKey("PlayerWinnerId")]
        public User? PlayerWinner { get; set; }
        [ForeignKey("PlayerLoserId")]
        public User? PlayerLoser { get; set; }
        public ICollection<GameEvent>? GameEvents { get; set; }
    }
}
