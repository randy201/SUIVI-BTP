using CsvHelper;
using CsvHelper.Configuration;
using ExcelDataReader;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using NuGet.Protocol;
using System.Data;
using System.Globalization;
using temp_back.Connexion;
using temp_back.Models;
namespace temp_back.Controllers
{
    [ApiController]
    [Route("Maisons")]
    public class MaisonController : ControllerBase
    {
        private readonly App_Db_Context _context;
        private readonly IMemoryCache _cache;

        public MaisonController(App_Db_Context context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Maison>>> GetMaisons()
        {
            Console.WriteLine($"List : Maisons");
            return await _context.maisons.Where(t => t.Statut == 0).ToListAsync();
        }

        [HttpGet("pagination")]
        public async Task<ActionResult<IEnumerable<Maison>>> GetMaisons_pagination(int page = 1, int element_par_page = 3)
        {
            Console.WriteLine($"List : Maisons");
            int element_skip = element_par_page * (page - 1);
            var list = _context.maisons.Where(t => t.Statut == 0).ToList().Skip(element_skip).Take(element_par_page).ToList();
            bool page_suivant = !_context.maisons.Where(t => t.Statut == 0).ToList().Skip(element_par_page * (page)).Take(element_par_page).ToList().IsNullOrEmpty();
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("data", list);
            dict.Add("suivant", page_suivant);
            return Ok(dict);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Maison>> GetMaison(int id)
        {
            Console.WriteLine($"Detail : Maisons/{id}");
            var maison = await _context.maisons.FindAsync(id);

            if (maison == null || maison.Statut == 1)
            {
                return NotFound($"Maison est introuvable avec id ={id} ");
            }
            return maison;
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Maison>> PutMaison(int id, Maison maison)
        {
            Console.WriteLine($"Modifier : Maisons/{id} || data = {maison.ToJson()}");
            if (id != maison.Id_maison)
            {
                return BadRequest("Valeur id incorrect");
            }

            _context.maisons.Update(maison);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MaisonExists(id))
                {
                    return NotFound($"Maison est introuvable avec id ={id}");
                }
                else
                {
                    throw;
                }
            }
            return Ok(maison);
        }


        [HttpPost]
        public async Task<ActionResult<Maison>> PostTestClass(Maison maison)
        {
            Console.WriteLine($"Ajout : Maisons || data = {maison.ToJson()}");
            if (!ModelState.IsValid)
            {
                return BadRequest($"Model state invalide \n {ModelState}");
            }
            _context.maisons.Add(maison);
            await _context.SaveChangesAsync();
            return Ok(maison);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTestClass(int id)
        {
            Console.WriteLine($"Supprimer : Maisons/{id}");
            var maison = await _context.maisons.FindAsync(id);
            if (maison == null)
            {
                return NotFound("Maisons introuvable");
            }
            maison.Statut = 1;
            await _context.SaveChangesAsync();
            return Ok("supprimé");
        }


        [HttpPost("add_data")]
        public async Task<IActionResult> data(Import_data_csv data)
        {
            var tx = _context.Database.BeginTransaction();
            List<MaisonTravauxExcel> list_maison = data.MaisonTravauxExcel;
            List<DevisExcel> list_devi = data.DevisExcel;

            foreach (MaisonTravauxExcel item in list_maison)
            {
                if (!item.Message.IsNullOrEmpty()) { tx.Rollback(); return BadRequest($"verifier les données {item.ToJson()}"); }
                Maison m = _context.maisons.Where(maison => maison.Nom.ToLower() == item.type_maison.ToLower()).FirstOrDefault();
                if (m == null)
                {
                    m = new Maison()
                    {
                        Nom = item.type_maison,
                        Description = item.description,
                        Surface = double.Parse(item.surface),
                        Durrer_totale = double.Parse(item.durre_travaux),
                    };
                    _context.maisons.Add(m);
                    try { _context.SaveChanges(); } catch (Exception ex) { tx.Rollback(); return BadRequest(ex.Message); }
                }
                Unite unite = _context.unites.Where(u => u.Nom.ToLower() == item.unite.ToLower()).FirstOrDefault();
                if (unite == null)
                {
                    unite = new Unite() { Nom = item.unite };
                    _context.unites.Add(unite);
                    try { _context.SaveChanges(); } catch (Exception ex) { tx.Rollback(); return BadRequest(ex.Message); }
                }

                Travaux travaux = _context.travaux.Where(t => t.Nom.ToLower() == item.type_travaux.ToLower() && t.Code_travaux.ToLower() == item.code_travaux.ToLower()).FirstOrDefault();
                if (travaux == null)
                {
                    travaux = new Travaux()
                    {
                        Nom = item.type_travaux,
                        Unite = unite,
                        Prix_unitaire = double.Parse(item.prix_unitaire),
                        Code_travaux = item.code_travaux
                    };
                    _context.travaux.Add(travaux);
                    try { _context.SaveChanges(); } catch (Exception ex) { tx.Rollback(); return BadRequest(ex.Message); }
                }

                Detail_maison detail = _context.detail_maisons.Where(dm => dm.Maison == m && dm.Travaux == travaux).FirstOrDefault();
                if (detail == null)
                {
                    detail = new Detail_maison()
                    {
                        Travaux = travaux,
                        Maison = m,
                        Quantite = double.Parse(item.quantite.Replace(".", ",")),

                    };
                    m.Prix_total = m.Prix_total + (detail.Quantite * detail.Travaux.Prix_unitaire);
                    _context.maisons.Update(m);
                    _context.detail_maisons.Add(detail);
                    try { _context.SaveChanges(); } catch (Exception ex) { tx.Rollback(); return BadRequest(ex.Message); }
                }
            }



            foreach (DevisExcel item in list_devi)
            {
                if (!item.Message.IsNullOrEmpty()) { tx.Rollback(); return BadRequest($"verifier les données {item.ToJson()}"); }
                Utilisateur client = _context.utilisateurs.Where(u => u.Telephone == item.client).FirstOrDefault();
                if (client == null)
                {
                    client = new Utilisateur() { Telephone = item.client, Profil = _context.profils.Where(p => p.Nom == "Client").FirstOrDefault() };
                    _context.utilisateurs.Add(client);
                    try { _context.SaveChanges(); } catch (Exception ex) { tx.Rollback(); return BadRequest(ex.Message); }
                }

                Type_finition type_finition = _context.type_finitions.Where(t => t.Nom.ToLower() == item.finition.ToLower()).FirstOrDefault();
                if (type_finition == null)
                {
                    type_finition = new Type_finition() { Nom = item.finition, Augmentation = double.Parse(item.taux_finition) };
                    _context.type_finitions.Add(type_finition);
                    try { _context.SaveChanges(); } catch (Exception ex) { tx.Rollback(); return BadRequest(ex.Message); }
                }

                Maison maison = _context.maisons.Where(m => m.Nom.ToLower() == item.type_maison.ToLower())
                    .FirstOrDefault();
                if (maison == null) { return BadRequest($" impossible d'insérer {item.ToJson()}"); }
                Devis devis = new Devis()
                {
                    Date_devis = DateOnly.Parse(item.date_devis),
                    Utilisateur = client,
                    Ref_devis = item.ref_devis,
                    Date_debut = DateOnly.Parse(item.date_debut),
                    Heurre_debut = new TimeOnly(0, 0),
                    Maison = maison,
                    Type_finition = type_finition,
                    Heurre_fin = new TimeOnly(23, 59),
                    Lieu = item.lieu,
                    Augmentation = type_finition.Augmentation,
                    Prix_total = maison.Prix_total + ((type_finition.Augmentation * maison.Prix_total) / 100),
                };
                devis.Date_fin = devis.Date_debut.AddDays((int)maison.Durrer_totale);
                _context.devis.Add(devis);
                try
                {
                    _context.SaveChanges();
                }
                catch (Exception ex) { tx.Rollback(); return BadRequest(ex.Message); }

                List<Devis_detail> devis_Details = new List<Devis_detail>();

                List<Detail_maison> detail = _context.detail_maisons.Where(d => d.Maison == maison)
                    .Include(de => de.Maison)
                    .Include(de => de.Travaux).ToList();
                foreach (Detail_maison detail_maison in detail)
                {
                    Devis_detail d_d = new Devis_detail()
                    {
                        Devis = devis,
                        Travaux = detail_maison.Travaux,
                        Maison = detail_maison.Maison,
                        Quantite = detail_maison.Quantite,
                        Prix = detail_maison.Travaux.Prix_unitaire,
                    };
                    devis_Details.Add(d_d);
                }
                _context.devis_details.AddRange(devis_Details);
                try { _context.SaveChanges(); } catch (Exception ex) { tx.Rollback(); return BadRequest(ex.Message); }
            }
            try
            {
                _context.SaveChanges();
                tx.Commit();
            }
            catch (Exception)
            {
                tx.Rollback();
                return BadRequest("ROLLBACK DATA");
            }

            return Ok("succès");
        }

        [HttpPost("importCSV")]
        public async Task<IActionResult> ImportCSV(IFormFile file_devis, IFormFile file_maison)
        {
            List<MaisonTravauxExcel> list_maison = new List<MaisonTravauxExcel>();
            List<DevisExcel> list_devis = new List<DevisExcel>();

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
                using (var reader = new StreamReader(file_maison.OpenReadStream()))
                using (var csv = new CsvReader(reader, csvConfig))
                {
                    csv.Read();
                    csv.ReadHeader();

                    while (csv.Read())
                    {
                        string message = "";
                        MaisonTravauxExcel m_t = new MaisonTravauxExcel();
                        string? type_maison = csv.GetField("type_maison").ToString().Trim();
                        if (type_maison.IsNullOrEmpty()) { message += " Type_maison est vide"; }

                        string? description = csv.GetField("description").ToString().Trim();
                        if (description.IsNullOrEmpty()) { message += " description est vide"; }

                        string? surface = csv.GetField("surface").Replace(".", ",").ToString().Trim();
                        if (surface.IsNullOrEmpty()) { message += " surface est vide"; }
                        if (!double.TryParse(surface, NumberStyles.Any, CultureInfo.InvariantCulture, out _)) { message += $" Conversion de {surface} impossible "; }
                        if (double.TryParse(surface, NumberStyles.Any, CultureInfo.InvariantCulture, out double sur)) { if (sur < 0) { message += $" Conversion valeur négalive de surface {surface} "; } }

                        string? code_travaux = csv.GetField("code_travaux").ToString().Trim();
                        if (code_travaux.IsNullOrEmpty()) { message += " code_travaux est vide"; }

                        string? type_travaux = csv.GetField("type_travaux").ToString().Trim();
                        if (type_travaux.IsNullOrEmpty()) { message += " type_travaux est vide"; }

                        string? unite = csv.GetField("unité").ToString().Trim();
                        if (unite.IsNullOrEmpty()) { message += " unite est vide"; }

                        string? prix_unitaire = csv.GetField("prix_unitaire").Replace(".", ",").ToString().Trim();
                        if (prix_unitaire.IsNullOrEmpty()) { message += " prix_unitaire est vide"; }
                        if (!double.TryParse(prix_unitaire, NumberStyles.Any, CultureInfo.InvariantCulture, out _)) { message += $" Conversion de {prix_unitaire} impossible "; }
                        if (double.TryParse(prix_unitaire, NumberStyles.Any, CultureInfo.InvariantCulture, out double p_u)) { if (p_u < 0) { message += $" Conversion valeur négalive de prix unitaire {prix_unitaire} "; } }

                        string? quantite = csv.GetField("quantite").Replace(".", ",").ToString().Trim();
                        if (quantite.IsNullOrEmpty()) { message += " quantite est vide"; }
                        if (!double.TryParse(quantite, NumberStyles.Any, CultureInfo.InvariantCulture, out _)) { message += $" Conversion de {quantite} impossible "; }
                        if (double.TryParse(quantite, NumberStyles.Any, CultureInfo.InvariantCulture, out double qtt)) { if (qtt < 0) { message += $" Conversion valeur négalive de quantite {quantite} "; } }

                        string? duree_travaux = csv.GetField("duree_travaux").Replace(".", ",").ToString().Trim();
                        if (duree_travaux.IsNullOrEmpty()) { message += " duree_travaux est vide"; }
                        if (!double.TryParse(duree_travaux, NumberStyles.Any, CultureInfo.InvariantCulture, out _)) { message += $" Conversion de {duree_travaux} impossible "; }
                        if (double.TryParse(duree_travaux, NumberStyles.Any, CultureInfo.InvariantCulture, out double d_t)) { if (d_t < 0) { message += $" Conversion valeur durée travaux  {duree_travaux} "; } }

                        m_t = new MaisonTravauxExcel()
                        {
                            type_maison = type_maison,
                            description = description,
                            surface = surface,
                            code_travaux = code_travaux,
                            type_travaux = type_travaux,
                            unite = unite,
                            prix_unitaire = prix_unitaire,
                            quantite = quantite,
                            durre_travaux = duree_travaux,
                            Message = message
                        };
                        list_maison.Add(m_t);
                    }
                }

                using (var reader = new StreamReader(file_devis.OpenReadStream()))
                using (var csv = new CsvReader(reader, csvConfig))
                {
                    csv.Read();
                    csv.ReadHeader();
                    while (csv.Read())
                    {
                        string message = "";

                        DevisExcel m_t = new DevisExcel();
                        string? client = csv.GetField("client").ToString().Trim();
                        if (client.IsNullOrEmpty()) { message += " client est vide"; }

                        string? ref_devis = csv.GetField("ref_devis").ToString().Trim();
                        if (ref_devis.IsNullOrEmpty()) { message += " ref_devis est vide"; }

                        string? type_maison = csv.GetField("type_maison").ToString().Trim();
                        if (type_maison.IsNullOrEmpty()) { message += " type_maison est vide"; }

                        string? finition = csv.GetField("finition")?.ToString().Trim();
                        if (finition.IsNullOrEmpty()) { message += " finition est vide"; }

                        string? taux_finition = csv.GetField("taux_finition").Replace(".", ",").ToString().Replace("%", "").Trim();
                        if (taux_finition.IsNullOrEmpty()) { message += " taux_finition est vide"; }
                        if (!double.TryParse(taux_finition, NumberStyles.Any, CultureInfo.InvariantCulture, out _)) { message += $" Conversion de {taux_finition} impossible "; }
                        if (double.TryParse(taux_finition, NumberStyles.Any, CultureInfo.InvariantCulture, out double t_f)) { if (t_f < 0) { message += $" Valeur négative durée travaux  {taux_finition} "; } }


                        string? date_devis = csv.GetField("date_devis").ToString().Trim();
                        if (date_devis.IsNullOrEmpty()) { message += " date_devis est vide"; }
                        //if (!DateOnly.TryParse(date_devis, CultureInfo.InvariantCulture, out _)) { message += "Conversion de " + date_devis + " impossible"; }

                        string? date_debut = csv.GetField("date_debut").ToString().Trim();
                        if (date_debut.IsNullOrEmpty()) { message += " date_debut est vide"; }
                        //if (!DateOnly.TryParse(date_debut, CultureInfo.InvariantCulture, out _)) { message += "Conversion de " + date_debut + " impossible"; }


                        string? lieu = csv.GetField("lieu").ToString().Trim();
                        if (lieu.IsNullOrEmpty()) { message += " lieu est vide"; }

                        m_t = new DevisExcel()
                        {
                            client = client,
                            ref_devis = ref_devis,
                            type_maison = type_maison,
                            finition = finition,
                            taux_finition = taux_finition,
                            date_devis = date_devis,
                            date_debut = date_debut,
                            lieu = lieu,
                            Message = message
                        };
                        list_devis.Add(m_t);
                    }

                }

            }
            catch (Exception ex) { return BadRequest(ex.Message); }
            Dictionary<string, object> rep = new Dictionary<string, object>();
            rep.Add("file_devis", list_devis);
            rep.Add("list_maison", list_maison);
            return Ok(rep);
        }

        [HttpPost("importExcel")]
        public async Task<IActionResult> importExcel(IFormFile file_devis, IFormFile file_maison)
        {
            try
            {
                List<MaisonTravauxExcel> list_maison = new List<MaisonTravauxExcel>();
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                using (var stream = new MemoryStream())
                {
                    file_maison.CopyTo(stream);
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
                            string message = "";

                            MaisonTravauxExcel m_t = new MaisonTravauxExcel();
                            string? type_maison = row["type_maison"].ToString().Trim();
                            if (type_maison.IsNullOrEmpty()) { message += " Type_maison est vide"; }

                            string? description = row["description"].ToString().Trim();
                            if (description.IsNullOrEmpty()) { message += " description est vide"; }

                            string? surface = row["surface"].ToString().Trim();
                            if (surface.IsNullOrEmpty()) { message += " surface est vide"; }

                            string? code_travaux = row["code_travaux"].ToString().Trim();
                            if (code_travaux.IsNullOrEmpty()) { message += " code_travaux est vide"; }

                            string? type_travaux = row["type_travaux"].ToString().Trim();
                            if (type_travaux.IsNullOrEmpty()) { message += " type_travaux est vide"; }

                            string? unite = row["unite"].ToString().Trim();
                            if (unite.IsNullOrEmpty()) { message += " unite est vide"; }

                            string? prix_unitaire = row["prix_unitaire"].ToString().Trim();
                            if (prix_unitaire.IsNullOrEmpty()) { message += " prix_unitaire est vide"; }

                            string? quantite = row["quantite"].ToString().Trim();
                            if (quantite.IsNullOrEmpty()) { message += " quantite est vide"; }

                            string? duree_travaux = row["duree_travaux"].ToString().Trim();
                            if (duree_travaux.IsNullOrEmpty()) { message += " duree_travaux est vide"; }

                            m_t = new MaisonTravauxExcel()
                            {
                                type_maison = type_maison,
                                description = description,
                                surface = surface,
                                code_travaux = code_travaux,
                                type_travaux = type_travaux,
                                unite = unite,
                                prix_unitaire = prix_unitaire,
                                quantite = quantite,
                                durre_travaux = duree_travaux,
                                Message = message
                            };


                            list_maison.Add(m_t);
                        }
                    }
                }

                List<DevisExcel> list_devis = new List<DevisExcel>();
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                using (var stream = new MemoryStream())
                {
                    file_devis.CopyTo(stream);
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
                            string message = "";

                            DevisExcel m_t = new DevisExcel();
                            string? client = row["client"].ToString().Trim();
                            if (client.IsNullOrEmpty()) { message += " client est vide"; }

                            string? ref_devis = row["ref_devis"].ToString().Trim();
                            if (ref_devis.IsNullOrEmpty()) { message += " ref_devis est vide"; }

                            string? type_maison = row["type_maison"].ToString().Trim();
                            if (type_maison.IsNullOrEmpty()) { message += " type_maison est vide"; }

                            string? finition = row["finition"].ToString().Trim();
                            if (finition.IsNullOrEmpty()) { message += " finition est vide"; }

                            string? taux_finition = row["taux_finition"].ToString().Trim();
                            if (taux_finition.IsNullOrEmpty()) { message += " taux_finition est vide"; }

                            string? date_devis = row["date_devis"].ToString().Trim();
                            if (date_devis.IsNullOrEmpty()) { message += " date_devis est vide"; }

                            string? date_debut = row["date_debut"].ToString().Trim();
                            if (date_debut.IsNullOrEmpty()) { message += " date_debut est vide"; }

                            string? lieu = row["lieu"].ToString().Trim();
                            if (lieu.IsNullOrEmpty()) { message += " lieu est vide"; }

                            m_t = new DevisExcel()
                            {
                                client = client,
                                ref_devis = ref_devis,
                                type_maison = type_maison,
                                finition = finition,
                                taux_finition = taux_finition,
                                date_devis = date_devis,
                                date_debut = date_debut,
                                lieu = lieu,
                                Message = message
                            };
                            list_devis.Add(m_t);
                        }
                    }
                }

                Dictionary<string, object> rep = new Dictionary<string, object>();
                rep.Add("file_devis", list_devis);
                rep.Add("file_maison", list_maison);
                return Ok(rep);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
            return Ok();
        }
        private bool MaisonExists(int id)
        {
            return _context.maisons.Any(e => e.Id_maison == id);
        }

    }
    public class Import_data_csv
    {
        public List<MaisonTravauxExcel>? MaisonTravauxExcel { get; set; }
        public List<DevisExcel>? DevisExcel { get; set; }

    }
}