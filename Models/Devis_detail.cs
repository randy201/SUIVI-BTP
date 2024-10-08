using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace temp_back.Models
{
    public class Devis_detail : Super_classe
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id_devis_detail { get; set; }
        public Devis Devis { get; set; }
        public int Id_detail_maison { get; set; }
        public Travaux Travaux { get; set; }
        public Maison Maison { get; set; }
        [Precision(20, 2)]
        [Range(0, double.MaxValue)]
        public double Quantite { get; set; }
        [Precision(20, 2)]
        [Range(0, double.MaxValue)]
        public double Prix { get; set; }
    }
}
