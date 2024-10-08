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
    [Route("Type_finitions")]
    public class Type_finitionController : ControllerBase
    {
        private readonly App_Db_Context _context;
        private readonly IMemoryCache _cache;

        public Type_finitionController(App_Db_Context context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Type_finition>>> GetType_finitions()
        {
            Console.WriteLine($"List : Type_finitions");
            return await _context.type_finitions.Where(t => t.Statut == 0).ToListAsync();
        }

        [HttpGet("pagination")]
        public async Task<ActionResult<IEnumerable<Type_finition>>> GetType_finitions_pagination(int page = 1, int element_par_page = 3)
        {
            Console.WriteLine($"List : Type_finitions");
            int element_skip = element_par_page * (page - 1);
            var list = _context.type_finitions.ToList().Skip(element_skip).Take(element_par_page).ToList();
            bool page_suivant = !_context.type_finitions.ToList().Skip(element_par_page * (page)).Take(element_par_page).ToList().IsNullOrEmpty();
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("data", list);
            dict.Add("suivant", page_suivant);
            return Ok(dict);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Type_finition>> GetType_finition(int id)
        {
            Console.WriteLine($"Detail : Type_finitions/{id}");
            var type_finition = await _context.type_finitions.FindAsync(id);

            if (type_finition == null || type_finition.Statut == 1)
            {
                return NotFound($"Type_finition est introuvable avec id ={id} ");
            }
            return type_finition;
        }

        [HttpPut("{id}"), Authorize(Roles = "Admin")]
        public async Task<ActionResult<Type_finition>> PutType_finition(int id, Type_finition type_finition)
        {
            Console.WriteLine($"Modifier : Type_finitions/{id} || data = {type_finition.ToJson()}");
            if (id != type_finition.Id_type_finition)
            {
                return BadRequest("Valeur id incorrect");
            }

            _context.type_finitions.Update(type_finition);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Type_finitionExists(id))
                {
                    return NotFound($"Type_finition est introuvable avec id ={id}");
                }
                else
                {
                    throw;
                }
            }
            return Ok(type_finition);
        }


        [HttpPost, Authorize(Roles = "Admin")]
        public async Task<ActionResult<Type_finition>> PostTestClass(Type_finition type_finition)
        {
            Console.WriteLine($"Ajout : Type_finitions || data = {type_finition.ToJson()}");
            if (!ModelState.IsValid)
            {
                return BadRequest($"Model state invalide \n {ModelState}");
            }
            _context.type_finitions.Add(type_finition);
            await _context.SaveChangesAsync();
            return Ok(type_finition);
        }

        [HttpDelete("{id}"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTestClass(int id)
        {
            Console.WriteLine($"Supprimer : Type_finitions/{id}");
            var type_finition = await _context.type_finitions.FindAsync(id);
            if (type_finition == null)
            {
                return NotFound("Type_finitions introuvable");
            }
            type_finition.Statut = 1;
            await _context.SaveChangesAsync();
            return Ok("supprimé");
        }


        [HttpPost("add_data")]
        public async Task<IActionResult> data(List<object> list)
        { return Ok(list); }

        [HttpPost("importCSV")]
        public async Task<IActionResult> ImportCSV(IFormFile fileFichier)
        { return Ok(fileFichier); }

        private bool Type_finitionExists(int id)
        {
            return _context.type_finitions.Any(e => e.Id_type_finition == id);
        }

    }
}