using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChessWebApp.Models
{
    public class User
    {
        public int Id { get; set; }
        [StringLength(32, MinimumLength = 3)]
        [Required]
        public string Name { get; set; }
        [StringLength(64, MinimumLength = 3)]
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [StringLength(256)]
        public string? Description { get; set; }
        public bool Admin { get; set; }

        public short VariantKing { get; set; }
        public short VariantQueen { get; set; }
        public short VariantBishopLeft { get; set; }
        public short VariantBishopRight { get; set; }
        public short VariantKnightLeft { get; set; }
        public short VariantKnightRight { get; set; }
        public short VariantRookLeft { get; set; }
        public short VariantRookRight { get; set; }

        public short VariantPawn1 { get; set; }
        public short VariantPawn2 { get; set; }
        public short VariantPawn3 { get; set; }
        public short VariantPawn4 { get; set; }
        public short VariantPawn5 { get; set; }
        public short VariantPawn6 { get; set; }
        public short VariantPawn7 { get; set; }
        public short VariantPawn8 { get; set; }

        public ICollection<Game>? GamesTop { get; set; }
        public ICollection<Game>? GamesBottom { get; set; }
    }
}
