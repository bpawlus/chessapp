using System.ComponentModel.DataAnnotations.Schema;

namespace ChessWebApp.Models
{
    public class GameDisplay
    {
        public GameDisplay()
        {

        }

        public int Id { get; set; }
        public string? PlayerWinnerName { get; set; }
        public string? PlayerLoserName { get; set; }
        public DateTime? TimeStart { get; set; }
        public DateTime? TimeEnd { get; set; }
        public bool Lost { get; set; }
    }
}
