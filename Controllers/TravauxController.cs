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
    [Route("Travaux")]
    public class TravauxController : ControllerBase
    {
        private readonly App_Db_Context _context;
        private readonly IMemoryCache _cache;


        public TravauxController(App_Db_Context context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;

        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Travaux>>> GetTravauxs()
        {
            Console.WriteLine($"List : Travaux");
            return await _context.travaux.Where(t => t.Statut == 0).ToListAsync();
        }

        [HttpGet("pagination")]
        public async Task<ActionResult<IEnumerable<Travaux>>> GetTravauxs_pagination(int page = 1, int element_par_page = 3)
        {
            Console.WriteLine($"List : Travaux");
            int element_skip = element_par_page * (page - 1);
            var list = _context.travaux.Where(t => t.Statut == 0).Include(t => t.Unite).ToList().Skip(element_skip).Take(element_par_page).ToList();
            bool page_suivant = !_context.travaux.Where(t => t.Statut == 0).ToList().Skip(element_par_page * (page)).Take(element_par_page).ToList().IsNullOrEmpty();
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("data", list);
            dict.Add("suivant", page_suivant);
            return Ok(dict);
        }



        [HttpGet("{id}")]
        public async Task<ActionResult<Travaux>> GetTravaux(int id)
        {
            Console.WriteLine($"Detail : Travaux/{id}");
            var travaux = _context.travaux.Where(t => t.Id_travaux == id)
                .Include(t => t.Unite).FirstOrDefault();

            if (travaux == null || travaux.Statut == 1)
            {
                return NotFound($"Travaux est introuvable avec id ={id} ");
            }
            return travaux;
        }

        [HttpPut("{id}"), Authorize(Roles = "Admin")]
        public async Task<ActionResult<Travaux>> PutTravaux(int id, TravauxDto travaux)
        {
            Console.WriteLine($"Modifier : Travaux/{id} || data = {travaux.ToJson()}");
            if (id != travaux.Id_travaux)
            {
                return BadRequest("Valeur id incorrect");
            }
            Travaux travaux_m = _context.travaux.Where(t => t.Id_travaux == id).FirstOrDefault();
            travaux_m.Durree = travaux.Durree.Value;
            travaux_m.Nom = travaux.Nom;
            travaux_m.Prix_unitaire = travaux.Prix_unitaire.Value;
            travaux_m.Code_travaux = travaux.Code_travaux;
            travaux_m.Unite = _context.unites.Where(u => u.Id_unite == travaux.Id_unite).FirstOrDefault();
            _context.travaux.Update(travaux_m);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TravauxExists(id))
                {
                    return NotFound($"Travaux est introuvable avec id ={id}");
                }
                else
                {
                    throw;
                }
            }
            return Ok(travaux);
        }


        [HttpPost, Authorize(Roles = "Admin")]
        public async Task<ActionResult<Travaux>> PostTestClass(TravauxDto travaux)
        {
            Console.WriteLine($"Ajout : Travaux || data = {travaux.ToJson()}");
            if (!ModelState.IsValid)
            {
                return BadRequest($"Model state invalide \n {ModelState}");
            }
            Travaux t = new Travaux()
            {
                Durree = travaux.Durree.Value,
                Nom = travaux.Nom,
                Prix_unitaire = travaux.Prix_unitaire.Value,
                Unite = _context.unites.Where(u => u.Id_unite == travaux.Id_unite).FirstOrDefault(),
                Code_travaux = travaux.Code_travaux

            };
            _context.travaux.Add(t);
            await _context.SaveChangesAsync();
            return Ok(travaux);
        }

        [HttpDelete("{id}"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTestClass(int id)
        {
            Console.WriteLine($"Supprimer : Travaux/{id}");
            var travaux = await _context.travaux.FindAsync(id);
            if (travaux == null)
            {
                return NotFound("Travaux introuvable");
            }
            travaux.Statut = 1;
            await _context.SaveChangesAsync();
            return Ok("supprimé");
        }


        [HttpPost("add_data")]
        public async Task<IActionResult> data(List<object> list)
        { return Ok(list); }

        [HttpPost("importCSV")]
        public async Task<IActionResult> ImportCSV(IFormFile fileFichier)
        { return Ok(fileFichier); }

        private bool TravauxExists(int id)
        {
            return _context.travaux.Any(e => e.Id_travaux == id && e.Statut == 0);
        }

    }
}