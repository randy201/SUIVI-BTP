using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace temp_back.Models
{
    public class Payement : Super_classe
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id_payement { get; set; }
        public Devis Devis { get; set; }
        public DateOnly Date_payement { get; set; }
        public double Montant { get; set; }
        [Range(0, double.MaxValue)]
        [Precision(20, 0)]
        public double Reste_payer { get; set; }
        public string? Ref_payement { get; set; }
    }
    public class PayementDto
    {
        [Required]
        public int? Id_devis { get; set; }
        [Required]
        [Range(0, double.MaxValue)]
        [Precision(20, 0)]
        public double? Montant { get; set; }
        [Required(ErrorMessage = "remplir le champs")]
        public DateOnly? Date_payement { get; set; }

    }
}
