using CsvHelper.Configuration.Attributes;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace temp_back.Models
{
    public class Maison : Super_classe
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id_maison { get; set; }
        public string Nom { get; set; }
        public double Prix_total { get; set; } = 0;
        //en jour
        public double Durrer_totale { get; set; } = 0;
        public string Description { get; set; }
        [Precision(20, 2)]
        [Column(TypeName = "decimal(20,2)")]
        [Default(0)]
        [Range(0, double.MaxValue)]
        public double Surface { get; set; }
    }

}
