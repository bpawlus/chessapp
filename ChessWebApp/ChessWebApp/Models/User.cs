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

        [Display(Name = "King")]
        public short VariantKing { get; set; }
        [Display(Name = "Queen")]
        public short VariantQueen { get; set; }
        [Display(Name = "Left Bishop")]
        public short VariantBishopLeft { get; set; }
        [Display(Name = "Right Bishop")]
        public short VariantBishopRight { get; set; }
        [Display(Name = "Left Knight")]
        public short VariantKnightLeft { get; set; }
        [Display(Name = "Right Knight")]
        public short VariantKnightRight { get; set; }
        [Display(Name = "Left Rook")]
        public short VariantRookLeft { get; set; }
        [Display(Name = "Right Rook")]
        public short VariantRookRight { get; set; }

        [Display(Name = "Pawn 1")]
        public short VariantPawn1 { get; set; }
        [Display(Name = "Pawn 2")]
        public short VariantPawn2 { get; set; }
        [Display(Name = "Pawn 3")]
        public short VariantPawn3 { get; set; }
        [Display(Name = "Pawn 4")]
        public short VariantPawn4 { get; set; }
        [Display(Name = "Pawn 5")]
        public short VariantPawn5 { get; set; }
        [Display(Name = "Pawn 6")]
        public short VariantPawn6 { get; set; }
        [Display(Name = "Pawn 7")]
        public short VariantPawn7 { get; set; }
        [Display(Name = "Pawn 8")]
        public short VariantPawn8 { get; set; }

        public ICollection<Game>? GamesTop { get; set; }
        public ICollection<Game>? GamesBottom { get; set; }
        public ICollection<Game>? GamesWon { get; set; }
        public ICollection<Game>? GamesLost { get; set; }
        public ICollection<GameEvent>? GameEvents { get; set; }
        public ICollection<GameEventComment>? Comments { get; set; }
    }
}
