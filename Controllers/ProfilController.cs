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
    [Route("Profils")]
    public class ProfilController : ControllerBase
    {
        private readonly App_Db_Context _context;
        private readonly IMemoryCache _cache;

        public ProfilController(App_Db_Context context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Profil>>> GetProfils()
        {
            Console.WriteLine($"List : Profils");
            return await _context.profils.Where(t => t.Statut == 0).ToListAsync();
        }

        [HttpGet("pagination")]
        public async Task<ActionResult<IEnumerable<Profil>>> GetProfils_pagination(int page = 1, int element_par_page = 3)
        {
            Console.WriteLine($"List : Profils");
            int element_skip = element_par_page * (page - 1);
            var list = _context.profils.ToList().Skip(element_skip).Take(element_par_page).ToList();
            bool page_suivant = !_context.profils.ToList().Skip(element_par_page * (page)).Take(element_par_page).ToList().IsNullOrEmpty();
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("data", list);
            dict.Add("suivant", page_suivant);
            return Ok(dict);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Profil>> GetProfil(int id)
        {
            Console.WriteLine($"Detail : Profils/{id}");
            var profil = await _context.profils.FindAsync(id);

            if (profil == null || profil.Statut == 1)
            {
                return NotFound($"Profil est introuvable avec id ={id} ");
            }
            return profil;
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Profil>> PutProfil(int id, Profil profil)
        {
            Console.WriteLine($"Modifier : Profils/{id} || data = {profil.ToJson()}");
            if (id != profil.Id_profil)
            {
                return BadRequest("Valeur id incorrect");
            }
            _context.profils.Update(profil);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProfilExists(id))
                {
                    return NotFound($"Profil est introuvable avec id ={id}");
                }
                else
                {
                    throw;
                }
            }
            return Ok(profil);
        }


        [HttpPost]
        public async Task<ActionResult<Profil>> PostTestClass(Profil profil)
        {
            Console.WriteLine($"Ajout : Profils || data = {profil.ToJson()}");
            if (!ModelState.IsValid)
            {
                return BadRequest($"Model state invalide \n {ModelState}");
            }
            _context.profils.Add(profil);
            await _context.SaveChangesAsync();
            return Ok(profil);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTestClass(int id)
        {
            Console.WriteLine($"Supprimer : Profils/{id}");
            var profil = await _context.profils.FindAsync(id);
            if (profil == null)
            {
                return NotFound("Profils introuvable");
            }
            profil.Statut = 1;
            await _context.SaveChangesAsync();
            return Ok("supprimé");
        }
        [HttpPost("add_data")]
        public async Task<IActionResult> data(List<object> list)
        { return Ok(list); }

        [HttpPost("importCSV")]
        public async Task<IActionResult> ImportCSV(IFormFile fileFichier)
        { return Ok(fileFichier); }

        private bool ProfilExists(int id)
        {
            return _context.profils.Any(e => e.Id_profil == id);
        }

    }
}