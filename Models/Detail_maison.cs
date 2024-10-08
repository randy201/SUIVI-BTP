using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace temp_back.Models
{
    public class Detail_maison : Super_classe
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id_detail_maison { get; set; }
        public Travaux Travaux { get; set; }
        public Maison Maison { get; set; }
        public double Quantite { get; set; }
    }
    public class Detail_maisonDto
    {
        public int Id_travaux { get; set; }
        public int Id_maison { get; set; }
        public double Quantite { get; set; }
    }
}
