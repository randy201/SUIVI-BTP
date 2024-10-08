using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace temp_back.Models
{
    public class Travaux : Super_classe
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id_travaux { get; set; }
        public string Nom { get; set; }

        [Column(TypeName = "decimal(20,2)")]
        [Range(0, double.MaxValue)]

        public double Prix_unitaire { get; set; }
        public Unite Unite { get; set; }
        [Range(0, int.MaxValue)]

        public int Durree { get; set; }
        public string? Code_travaux { get; set; }
    }

    public class TravauxDto
    {
        public int? Id_travaux { get; set; }
        [Required]
        public string? Nom { get; set; }
        [Required]
        [Precision(20, 2)]
        [Range(0, double.MaxValue)]
        public double? Prix_unitaire { get; set; }
        [Required]
        public int? Id_unite { get; set; }
        [Required]
        [Range(0, double.MaxValue)]
        public int? Durree { get; set; } = 0;
        [Required]
        public string? Code_travaux { get; set; }
    }
}
