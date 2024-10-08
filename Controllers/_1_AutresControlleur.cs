using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using ExcelDataReader;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Globalization;
using temp_back.Models;

namespace temp_back.Controllers
{
    [Route("1_AutresControlleur")]
    [ApiController]
    public class _1_AutresControlleur : ControllerBase
    {

        private readonly IConfiguration _configuration;
        private string _cheminFront { get; set; }
        public _1_AutresControlleur(IConfiguration configuration)
        {
            _configuration = configuration;
            _cheminFront = _configuration.GetValue<string>("ApplicationSettings:FrontEndPath");
        }

        [DisableRequestSizeLimit]
        [RequestFormLimits(MultipartBodyLengthLimit = 234217728)]
        [HttpPost("importCSV")]
        public async Task<IActionResult> ImportCSV(IFormFile fileFichier)
        {
            var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ",",
                HasHeaderRecord = true, // Indique si la première ligne est l'en-tête
                HeaderValidated = null, //  Indique si le nombre de colle = nombre attribut
                MissingFieldFound = null
            };
            csvConfig.IgnoreBlankLines = true; // Ignorer les lignes vides
            csvConfig.IgnoreReferences = true; // Ignorer les références circulaires
            //Console.WriteLine($"import du fichier {fileFichier.fichier.FileName}");
            try
            {
                using (var reader = new StreamReader(fileFichier.OpenReadStream()))
                using (var csv = new CsvReader(reader, csvConfig))
                {
                    var records = csv.GetRecords<Article>().ToList();
                    return Ok(records);
                }
                return Ok("Lecture du fichier terminée !");
            }
            catch (Exception ex)
            {
                return BadRequest($"Erreur lors de la lecture du fichier : {ex.Message}");
            }

        }
        [HttpPost("importCSV_controller")]
        public async Task<IActionResult> import_controller(IFormFile fileFichier)
        {
            var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ",",
                HasHeaderRecord = true,
                HeaderValidated = null,
                MissingFieldFound = null
            };
            csvConfig.IgnoreBlankLines = true;
            csvConfig.IgnoreReferences = true;
            try
            {
                using (var reader = new StreamReader(fileFichier.OpenReadStream()))
                using (var csv = new CsvReader(reader, csvConfig))
                {
                    csv.Read();
                    csv.ReadHeader();
                    var records = new List<Article>();
                    while (csv.Read())
                    {
                        var personne = new Article();
                        personne.Nom = csv.GetField("col1").Trim();
                        records.Add(personne);
                    }
                    return Ok(records);
                }

            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPost("importExcel")]
        public async Task<IActionResult> ImportExcel(IFormFile fileFichier)
        {
            List<Article> users = new List<Article>();
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            using (var stream = new MemoryStream())
            {
                fileFichier.CopyTo(stream);
                stream.Position = 0;

                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    // Configuration pour utiliser la première ligne comme en-tête
                    var excelConfig = new ExcelDataSetConfiguration
                    {
                        ConfigureDataTable = (_) => new ExcelDataTableConfiguration
                        {
                            UseHeaderRow = true
                        }
                    };

                    // Utilisation de la configuration lors de la création du DataSet
                    var dataSet = reader.AsDataSet(excelConfig);

                    // Traitement des données (vous pouvez maintenant accéder aux colonnes par leur nom)
                    foreach (DataRow row in dataSet.Tables[0].Rows)
                    {
                        int? valeurId = int.TryParse(row["Id"]?.ToString(), out int valeur_id) ? valeur_id : null;
                        double? valeurNumero = double.TryParse(row["Numero"]?.ToString(), out double valeur_numero) ? valeur_numero : null;
                        DateTime? valeurDateTime = DateTime.TryParse(row["Date_heure_debut"]?.ToString(), out DateTime valeur_datetime) ? valeur_datetime : null;
                        DateOnly? valeurnaissance = DateOnly.TryParse(row["Date_naissance"]?.ToString(), out DateOnly valeur_naissance) ? valeur_naissance : null;

                        users.Add(new Article
                        {
                            Id = valeurId,
                            Nom = null,
                            Prenom = row["Prenom"]?.ToString(),
                            Email = row["Email"]?.ToString(),
                            Numero = valeurNumero,
                            Date_heure_debut = valeurDateTime,
                            Date_naissance = valeurnaissance
                        });
                    }
                }
            }

            return Ok(users);
        }
        [HttpPost("uploadImage"), DisableRequestSizeLimit]
        public async Task<IActionResult> Upload_Image(IFormFile fichier)
        {
            var folderName = @"D:\00ITU\DOSSIER S6\00-prep\projet-test-4\temp-front\src\assets";
            try
            {
                string rep = All_type.Upload_Image(fichier, folderName, "image");
                return Ok(rep);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
    public class Fichier
    {
        [AllowedExtensions(new[] { ".csv" })]
        public IFormFile? fichier { get; set; }
    }

    public class FichierExcel
    {
        [AllowedExtensions(new[] { ".xls", "xlsx", ".odf" })]
        public IFormFile? fichier { get; set; }
    }

    public class Article
    {
        [Key]
        public int? Id { get; set; }
        [Required(ErrorMessage = "Nom est obligatoire")]
        public string Nom { get; set; }
        public string? Prenom { get; set; }
        [EmailAddress(ErrorMessage = "Le champs doit etre valide")]
        public string? Email { get; set; }

        [Default(0)]
        public double? Numero { get; set; } = 0;
        public DateTime? Date_heure_debut { get; set; }
        public DateOnly? Date_naissance { get; set; }
    }
}
