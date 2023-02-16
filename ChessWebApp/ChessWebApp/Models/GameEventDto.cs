using System.ComponentModel.DataAnnotations.Schema;

namespace ChessWebApp.Models
{
    public class GameEventDto
    {
        public int Id { get; set; }
        public int GameId { get; set; }
        public string Status { get; set; }
        public string Notation { get; set; }
        public DateTime? Time { get; set; }
        public string? Comment { get; set; }
        public bool IsTop { get; set; }
    }
}
