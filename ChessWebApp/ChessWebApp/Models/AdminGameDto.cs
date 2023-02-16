namespace ChessWebApp.Models
{
    public class AdminGameDto
    {
        public int Id { get; set; }
        public string? PlayerWinnerName { get; set; }
        public string? PlayerLoserName { get; set; }
        public string? PlayerTopName { get; set; }
        public string? PlayerBottomName { get; set; }
        public DateTime? TimeStart { get; set; }
        public DateTime? TimeEnd { get; set; }
        public bool MarkForDel { get; set; }
    }
}
