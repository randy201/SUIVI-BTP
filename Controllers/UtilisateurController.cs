using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using NuGet.Protocol;
using System.ComponentModel.DataAnnotations;
using temp_back.Connexion;
using temp_back.Models;
namespace temp_back.Controllers
{
    [ApiController]
    [Route("Utilisateurs")]
    public class UtilisateurController : ControllerBase
    {
        private readonly App_Db_Context _context;
        private readonly IMemoryCache _cache;


        public UtilisateurController(App_Db_Context context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Utilisateur>>> GetUtilisateurs()
        {
            Console.WriteLine($"List : Utilisateurs");
            return await _context.utilisateurs.Where(t => t.Statut == 0).ToListAsync();
        }



        [HttpGet("pagination")]
        public async Task<ActionResult<IEnumerable<Utilisateur>>> GetUtilisateurs_pagination(int page = 1, int element_par_page = 3)
        {
            Console.WriteLine($"List : Utilisateurs");
            int element_skip = element_par_page * (page - 1);
            var list = _context.utilisateurs.ToList().Skip(element_skip).Take(element_par_page).ToList();
            bool page_suivant = !_context.utilisateurs.ToList().Skip(element_par_page * (page)).Take(element_par_page).ToList().IsNullOrEmpty();
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("data", list);
            dict.Add("suivant", page_suivant);
            return Ok(dict);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Utilisateur>> GetUtilisateur(int id)
        {
            Console.WriteLine($"Detail : Utilisateurs/{id}");
            var utilisateur = await _context.utilisateurs.FindAsync(id);

            if (utilisateur == null || utilisateur.Statut == 1)
            {
                return NotFound($"Utilisateur est introuvable avec id ={id} ");
            }
            return utilisateur;
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Utilisateur>> PutUtilisateur(int id, UtilisateurDto utilisateur)
        {
            Console.WriteLine($"Modifier : Utilisateurs/{id} || data = {utilisateur.ToJson()}");
            if (id != utilisateur.Id_utilisateur)
            {
                return BadRequest("Valeur id incorrect");
            }
            Utilisateur u = _context.utilisateurs
                .Where(u => u.Id_utilisateur == id).Include(u => u.Profil).FirstOrDefault();
            u.Nom = utilisateur.Nom;
            u.Prenom = utilisateur.Prenom;
            u.Email = utilisateur.Email;
            u.Mot_de_passe = utilisateur.Mot_de_passe;
            _context.utilisateurs.Update(u);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UtilisateurExists(id))
                {
                    return NotFound($"Utilisateur est introuvable avec id ={id}");
                }
                else
                {
                    throw;
                }
            }
            return Ok(utilisateur);
        }

        [HttpPost]
        public async Task<ActionResult<Utilisateur>> PostTestClass(UtilisateurDto utilisateur)
        {
            Console.WriteLine($"Ajout : Utilisateurs || data = {utilisateur.ToJson()}");
            if (!ModelState.IsValid)
            {
                return BadRequest($"Model state invalide \n {ModelState}");
            }
            Utilisateur u = new Utilisateur()
            {
                Profil = _context.profils.Where(p => p.Nom == "Client").FirstOrDefault(),
                Nom = utilisateur.Nom,
                Email = utilisateur.Email,
                Prenom = utilisateur.Prenom,
                Mot_de_passe = utilisateur.Mot_de_passe,
                Telephone = utilisateur.Telephone
            };
            _context.utilisateurs.Add(u);
            await _context.SaveChangesAsync();
            return Ok(u);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTestClass(int id)
        {
            Console.WriteLine($"Supprimer : Utilisateurs/{id}");
            var utilisateur = await _context.utilisateurs.FindAsync(id);
            if (utilisateur == null)
            {
                return NotFound("Utilisateurs introuvable");
            }
            utilisateur.Statut = 1;
            await _context.SaveChangesAsync();
            return Ok("supprimé");
        }


        [HttpPost("add_data")]
        public async Task<IActionResult> data(List<object> list)
        { return Ok(list); }

        [HttpPost("importCSV")]
        public async Task<IActionResult> ImportCSV(IFormFile fileFichier)
        { return Ok(fileFichier); }

        private bool UtilisateurExists(int id)
        {
            return _context.utilisateurs.Any(e => e.Id_utilisateur == id);
        }

    }
    public class UtilisateurLogin
    {
        [Required]
        public string Telephone { get; set; }
    }
}