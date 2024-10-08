using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace temp_back.Models
{
    public class Utilisateur : Super_classe
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id_utilisateur { get; set; }
        public string? Nom { get; set; }
        public string? Prenom { get; set; }
        [Column(TypeName = "text unique")]
        public string? Email { get; set; }
        public string? Mot_de_passe { get; set; }
        public Profil Profil { get; set; }
        [Column(TypeName = "text unique")]
        public string Telephone { get; set; }
    }
    public class UtilisateurDto
    {
        public int? Id_utilisateur { get; set; }
        [Required(ErrorMessage = "Le nom est nécessaire")]
        public string? Nom { get; set; }
        [Required(ErrorMessage = "Le prenom est nécessaire")]
        public string? Prenom { get; set; }
        [Required(ErrorMessage = "L'email est nécessaire")]
        [EmailAddress(ErrorMessage = "Entrer une email valide")]
        public string? Email { get; set; }
        [Required(ErrorMessage = "Le mot de passe est nécessaire")]
        public string? Mot_de_passe { get; set; }
        public int? Id_profil { get; set; }
        public string? Telephone { get; set; }
    }


}
