using System.ComponentModel.DataAnnotations.Schema;

namespace ChessWebApp.Models
{
    public class AllowedPiece
    {
        public int Id { get; set; }
        public string VariantKey { get; set; }
        public short AllowedFactoryId { get; set; }
        public string? ConditionName { get; set; }
    }
}
