using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChessWebApp.Models
{
    public class Game
    {
        public int Id { get; set; }
        public int? PlayerBottomId { get; set; }
        public virtual User? PlayerBottom { get; set; }
        public int? PlayerTopId { get; set; }
        public virtual User? PlayerTop { get; set; }
        public int? PlayerWinnerId { get; set; }
        public virtual User? PlayerWinner { get; set; }
        public int? PlayerLoserId { get; set; }
        public virtual User? PlayerLoser { get; set; }
        public DateTime? TimeStart { get; set; }
        public DateTime? TimeEnd { get; set; }
        public ICollection<GameEvent>? GameEvents { get; set; }
    }
}
