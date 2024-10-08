using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace temp_back.Models
{
    public class Unite : Super_classe
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id_unite { get; set; }
        public string Nom { get; set; }
    }
}
