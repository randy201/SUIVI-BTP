using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace temp_back.Models
{
    public class Devis : Super_classe
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id_devis { get; set; }
        public Utilisateur Utilisateur { get; set; }
        public Maison Maison { get; set; }
        public DateOnly Date_devis { get; set; }
        public DateOnly Date_debut { get; set; }
        public TimeOnly Heurre_debut { get; set; }
        public DateOnly Date_fin { get; set; }
        public TimeOnly Heurre_fin { get; set; }
        public Type_finition Type_finition { get; set; }
        [Column(TypeName = "decimal(20,2)")]
        [Precision(20, 2)]
        public double Prix_total { get; set; }

        public double Augmentation { get; set; }
        public string? Ref_devis { get; set; }
        public string? Lieu { get; set; }

    }
    public class DevitDto
    {
        public int? Id_devis { get; set; }
        [Required]
        public int? Id_utlisateur { get; set; }
        [Required]
        public DateOnly? Date_debut { get; set; }
        [Required]
        public TimeOnly? Heurre_debut { get; set; }
        [Precision(20, 2)]
        [Range(0, double.MaxValue)]
        public double Prix_total { get; set; }
        [Required]
        public int? Id_maison { get; set; }
        [Required]
        public int? Id_type_finition { get; set; }
        [Required]
        public string? Lieu { get; set; }
    }
}
