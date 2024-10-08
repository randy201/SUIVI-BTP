using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace temp_back.Models
{
    public class Post : Super_classe
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id_post { get; set; }
        public string Nom { get; set; }
        [Column(TypeName = "decimal(20,2)")]
        [Precision(20, 2)]
        public double Salaire { get; set; }
    }
}
