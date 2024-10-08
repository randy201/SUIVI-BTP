using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace temp_back.Models
{
    public class Type_finition : Super_classe
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id_type_finition { get; set; }
        public string Nom { get; set; }
        [Column(TypeName = "decimal(20,2)")]
        [Precision(20, 2)]
        [Range(0, double.MaxValue)]
        public double Augmentation { get; set; }
    }
}
