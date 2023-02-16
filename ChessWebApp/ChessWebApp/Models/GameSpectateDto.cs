namespace ChessWebApp.Models
{
    public class GameSpectateDto
    {
        public int Id { get; set; }
        public string? PlayerTopName { get; set; }
        public string? PlayerBottomName { get; set; }
        public DateTime? TimeStart { get; set; }
    }
}
