using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using temp_back.Connexion;
using temp_back.Models;

namespace temp_back.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class All_typeController : ControllerBase
    {
        private readonly App_Db_Context _context;
        private readonly IMemoryCache _cache;

        public All_typeController(App_Db_Context context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        // GET: api/All_type
        [HttpGet]
        public async Task<ActionResult<IEnumerable<All_type>>> Getall_types()
        {
            return _context.all_types.ToList();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<All_type>> GetAll_type(int id)
        {
            var all_type = await _context.all_types.FindAsync(id);

            if (all_type == null)
            {
                return NotFound($"All_type est introuvable avec id ={id}");
            }
            return all_type;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAll_type(int id, All_type all_type)
        {
            if (id != all_type.Id_all_type)
            {
                return BadRequest("Valeur id incorrect");
            }

            _context.Entry(all_type).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!All_typeExists(id))
                {
                    return NotFound($"All_type est introuvable avec id ={id}");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }


        [HttpPost]
        public async Task<ActionResult<All_type>> PostAll_type(All_type all_type)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest($"Model state invalide \n {ModelState}");
            }

            _context.all_types.Add(all_type);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAll_type", new { id = all_type.Id_all_type }, all_type);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAll_type(int id)
        {
            var all_type = await _context.all_types.FindAsync(id);
            if (all_type == null)
            {
                return NotFound($"All_type est introuvable avec id ={id}");
            }

            _context.all_types.Remove(all_type);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool All_typeExists(int id)
        {
            return _context.all_types.Any(e => e.Id_all_type == id);
        }
    }
}
