using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace temp_back.Models
{
    public class All_type
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id_all_type { get; set; }

        public string? String_value { get; set; } = string.Empty;

        [EmailAddress]
        [Required(ErrorMessage = "Email est obligatoire")]
        public string Email { get; set; } = string.Empty;

        [Precision(30, 2)]
        [Range(0, double.MaxValue, ErrorMessage = "Double_value doit etre positive avec 2 chiffres après la virgule")]
        public double? Double_value { get; set; }

        [Required(ErrorMessage = "Int_value est obligatoire")]
        public int Int_value { get; set; } = 0;

        [Required(ErrorMessage = "TimeOnly_value est obligatoire")]
        public DateOnly DateOnly_value { get; set; }
        [Required(ErrorMessage = "TimeOnly_value est obligatoire")]
        public DateTimeOffset DateTime_value { get; set; }
        [Required(ErrorMessage = "TimeOnly_value est obligatoire")]
        public TimeOnly TimeOnly_value { get; set; }

        public string? File_path { get; set; }
        [NotMapped]
        public IFormFile? File_value { get; set; }

        public static bool valide_numero(string numero)
        {
            if (string.IsNullOrWhiteSpace(numero))
            {
                return false;
            }
            numero = numero.Replace(" ", "");
            if (numero.Length != 10)
            {
                return false;
            }
            Regex regMobile = new Regex("^03[2-478][0-9]{7}$");
            Regex regFixe = new Regex("^0202[0-9]{6}$");
            return regMobile.IsMatch(numero) || regFixe.IsMatch(numero);
        }
        public static string Upload_Image(IFormFile fichier, string url_assets, string dossier)
        {
            var folderName = Path.Combine(url_assets, dossier);
            string nomFichier = fichier.FileName;
            string fullPath = Path.Combine(folderName, nomFichier);
            if (!Directory.Exists(folderName))
            {
                Directory.CreateDirectory(folderName);
                Console.WriteLine($"Dossier creer {folderName}");
            }
            if (System.IO.File.Exists(fullPath))
            {
                throw new Exception("Fichier déjà existant");
            }
            try
            {
                using (var stream = new FileStream(fullPath, FileMode.Create))
                { fichier.CopyToAsync(stream); }
                return (folderName);
            }
            catch (Exception ex)
            {
                throw new Exception("Erreur lors de l'enregistrement du fichier " + ex.Message);
            }
        }
    }
}
