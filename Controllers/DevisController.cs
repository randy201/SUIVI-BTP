using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using NuGet.Protocol;
using temp_back.Connexion;
using temp_back.Models;
namespace temp_back.Controllers
{
    [ApiController]
    [Route("Devis")]
    public class DevisController : ControllerBase
    {
        private readonly App_Db_Context _context;
        private readonly IMemoryCache _cache;

        public DevisController(App_Db_Context context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Devis>>> GetDeviss()
        {
            Console.WriteLine($"List : Devis");
            return await _context.devis.Where(t => t.Statut == 0).Include(d => d.Maison).Include(d => d.Utilisateur).ToListAsync();
        }

        [HttpGet("utilisateur/{id}"), Authorize(Roles = "Client")]

        public async Task<ActionResult<IEnumerable<Devis>>> Devis_user(int id)
        {
            return await _context.devis
                .Where(t => t.Statut == 0 && t.Utilisateur.Id_utilisateur == id)
                .Include(d => d.Maison)
                .Include(d => d.Utilisateur).ToListAsync();
        }
        [HttpGet("total_devis"), Authorize(Roles = "Admin")]
        public async Task<ActionResult> montant_total()
        {
            double montant = 0;
            montant = _context.devis.Where(t => t.Statut == 0).Sum(p => p.Prix_total);
            return Ok(montant);
        }

        [HttpGet("mois_anne"), Authorize(Roles = "Admin")]
        public async Task<ActionResult> devis_mois_anne(int anne = 2024)
        {

            List<int> mois = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            List<double> rep = new List<double>();
            List<Devis> devis_annee = _context.devis.Where(d => d.Date_devis.Year == anne && d.Statut == 0).ToList();
            foreach (int m in mois)
            {
                List<Devis> d = devis_annee.Where(de => de.Date_devis.Month == m).ToList();
                double prix = d.Sum(d => d.Prix_total);
                rep.Add(prix);
            }
            return Ok(rep);
        }
        [HttpGet("en_cours"), Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<object>>> devis_encours()
        {
            var list = await _context.devis
                .Where(t => t.Statut == 0)
                .Include(d => d.Maison)
                .Include(d => d.Utilisateur)
                .ToListAsync();
            List<Object> list_en_cour = new List<Object>();
            foreach (Devis devis in list)
            {
                double payer = _context.payements
                    .Where(ps => ps.Devis == devis && ps.Statut == 0)
                    .Sum(pt => pt.Montant);


                var obj = new
                {
                    Rest = payer,
                    Devis = devis,
                };

                list_en_cour.Add(obj);
            }
            return list_en_cour;
        }

        [HttpGet("pagination")]
        public async Task<ActionResult<IEnumerable<Devis>>> GetDeviss_pagination(int page = 1, int element_par_page = 3)
        {
            Console.WriteLine($"List : Devis");
            int element_skip = element_par_page * (page - 1);
            var list = _context.devis.Where(t => t.Statut == 0).Include(d => d.Maison).Include(d => d.Utilisateur).ToList().Skip(element_skip).Take(element_par_page).ToList();
            bool page_suivant = !_context.devis.Where(t => t.Statut == 0).Include(d => d.Maison).Include(d => d.Utilisateur).ToList().Skip(element_par_page * (page)).Take(element_par_page).ToList().IsNullOrEmpty();
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("data", list);
            dict.Add("suivant", page_suivant);
            return Ok(dict);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Devis>> GetDevis(int id)
        {
            Console.WriteLine($"Detail : Devis/{id}");
            Devis devis = _context.devis
                .Include(d => d.Maison)
                .Include(d => d.Utilisateur)
                .Where(d => d.Id_devis == id && d.Statut == 0).FirstOrDefault();


            if (devis == null || devis.Statut == 1)
            {
                return NotFound($"Devis est introuvable avec id ={id} ");
            }

            return devis;
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Devis>> PutDevis(int id, Devis devis)
        {
            Console.WriteLine($"Modifier : Devis/{id} || data = {devis.ToJson()}");
            if (id != devis.Id_devis)
            {
                return BadRequest("Valeur id incorrect");
            }

            _context.devis.Update(devis);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DevisExists(id))
                {
                    return NotFound($"Devis est introuvable avec id ={id}");
                }
                else
                {
                    throw;
                }
            }
            return Ok(devis);
        }


        [HttpPost]
        public async Task<ActionResult<Devis>> PostTestClass(DevitDto d)
        {
            Console.WriteLine($"Ajout : Devis || data = {d.ToJson()}");
            if (!ModelState.IsValid)
            {
                return BadRequest($"Model state invalide \n {ModelState}");
            }

            double jour = 0;
            double prix_total = 0;
            Devis devis = new Devis()
            {
                Date_debut = d.Date_debut.Value,
                Heurre_debut = d.Heurre_debut.Value,
                Utilisateur = _context.utilisateurs.Where(u => u.Id_utilisateur == d.Id_utlisateur).FirstOrDefault(),
                Maison = _context.maisons.Where(m => m.Id_maison == d.Id_maison).FirstOrDefault(),
                Date_devis = DateOnly.FromDateTime(DateTime.Now),
                Type_finition = _context.type_finitions.Where(t => t.Id_type_finition == d.Id_type_finition).FirstOrDefault(),
                Lieu = d.Lieu
            };

            List<Detail_maison> list_travaux = _context.detail_maisons
                .Where(t => t.Statut == 0)
                .Include(t => t.Travaux)
                .Include(t => t.Maison)
                .Where(t => t.Maison == devis.Maison).ToList();

            devis.Augmentation = devis.Type_finition.Augmentation;
            devis.Prix_total = (devis.Maison.Prix_total) + ((devis.Maison.Prix_total * devis.Type_finition.Augmentation) / 100);
            devis.Date_fin = devis.Date_debut.AddDays((int)devis.Maison.Durrer_totale);
            devis.Heurre_fin = devis.Heurre_debut;
            _context.devis.Add(devis);

            await _context.SaveChangesAsync();
            List<Devis_detail> list_devis = new List<Devis_detail>();
            foreach (Detail_maison t in list_travaux)
            {
                Devis_detail detail = new Devis_detail()
                {
                    Devis = devis,
                    Travaux = t.Travaux,
                    Maison = t.Maison,
                    Quantite = t.Quantite,
                    Prix = t.Travaux.Prix_unitaire,
                };
                list_devis.Add(detail);
            }
            foreach (var item in list_devis)
            {
                _context.devis_details.Add(item);
                await _context.SaveChangesAsync();

            }
            return Ok(devis);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTestClass(int id)
        {
            Console.WriteLine($"Supprimer : Devis/{id}");
            var devis = await _context.devis.FindAsync(id);
            if (devis == null)
            {
                return NotFound("Devis introuvable");
            }
            devis.Statut = 1;
            await _context.SaveChangesAsync();
            return Ok("supprimé");
        }

        [HttpPost("add_data")]
        public async Task<IActionResult> data(List<object> list)
        { return Ok(list); }

        [HttpPost("importCSV")]
        public async Task<IActionResult> ImportCSV(IFormFile fileFichier)
        { return Ok(fileFichier); }

        private bool DevisExists(int id)
        {
            return _context.devis.Any(e => e.Id_devis == id);
        }


    }
}