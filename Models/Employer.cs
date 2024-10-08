using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace temp_back.Models
{
    public class Employer : Super_classe
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id_employer { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        [Column(TypeName = "unique")]
        public string Email { get; set; }
        public string Mot_de_passe { get; set; }
        public Post Post { get; set; }
    }
    public class EmployerDto
    {
        public int? Id_employer { get; set; }
        [Required(ErrorMessage = "Le nom est nécessaire")]
        public string? Nom { get; set; }
        [Required(ErrorMessage = "Le prenom est nécessaire")]
        public string? Prenom { get; set; }
        [Required(ErrorMessage = "L'email est nécessaire")]
        [EmailAddress]
        public string? Email { get; set; }
        [Required(ErrorMessage = "Le mot de passe est nécessaire")]
        public string? Mot_de_passe { get; set; }
        [Required(ErrorMessage = "Le post est nécessaire")]
        public int? Id_post { get; set; }
    }
}
