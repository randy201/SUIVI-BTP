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
    [Route("Unites")]
    public class UniteController : ControllerBase
    {
        private readonly App_Db_Context _context;
        private readonly IMemoryCache _cache;


        public UniteController(App_Db_Context context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Unite>>> GetUnites()
        {
            Console.WriteLine($"List : Unites");
            return await _context.unites.Where(t => t.Statut == 0).ToListAsync();
        }

        [HttpGet("pagination")]
        public async Task<ActionResult<IEnumerable<Unite>>> GetUnites_pagination(int page = 1, int element_par_page = 3)
        {
            Console.WriteLine($"List : Unites");
            int element_skip = element_par_page * (page - 1);
            var list = _context.unites.ToList().Skip(element_skip).Take(element_par_page).ToList();
            bool page_suivant = !_context.unites.ToList().Skip(element_par_page * (page)).Take(element_par_page).ToList().IsNullOrEmpty();
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("data", list);
            dict.Add("suivant", page_suivant);
            return Ok(dict);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Unite>> GetUnite(int id)
        {
            Console.WriteLine($"Detail : Unites/{id}");
            var unite = await _context.unites.FindAsync(id);

            if (unite == null || unite.Statut == 1)
            {
                return NotFound($"Unite est introuvable avec id ={id} ");
            }
            return unite;
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Unite>> PutUnite(int id, Unite unite)
        {
            Console.WriteLine($"Modifier : Unites/{id} || data = {unite.ToJson()}");
            if (id != unite.Id_unite)
            {
                return BadRequest("Valeur id incorrect");
            }

            _context.unites.Update(unite);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UniteExists(id))
                {
                    return NotFound($"Unite est introuvable avec id ={id}");
                }
                else
                {
                    throw;
                }
            }
            return Ok(unite);
        }


        [HttpPost]
        public async Task<ActionResult<Unite>> PostTestClass(Unite unite)
        {
            Console.WriteLine($"Ajout : Unites || data = {unite.ToJson()}");
            if (!ModelState.IsValid)
            {
                return BadRequest($"Model state invalide \n {ModelState}");
            }
            _context.unites.Add(unite);
            await _context.SaveChangesAsync();
            return Ok(unite);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTestClass(int id)
        {
            Console.WriteLine($"Supprimer : Unites/{id}");
            var unite = await _context.unites.FindAsync(id);
            if (unite == null)
            {
                return NotFound("Unites introuvable");
            }
            unite.Statut = 1;
            await _context.SaveChangesAsync();
            return Ok("supprimé");
        }

        [HttpPost("add_data")]
        public async Task<IActionResult> data(List<object> list)
        { return Ok(list); }

        [HttpPost("importCSV")]
        public async Task<IActionResult> ImportCSV(IFormFile fileFichier)
        { return Ok(fileFichier); }

        private bool UniteExists(int id)
        {
            return _context.unites.Any(e => e.Id_unite == id);
        }

    }
}