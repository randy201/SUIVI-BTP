using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NuGet.Protocol;
using temp_back.Connexion;
using temp_back.Models;
namespace temp_back.Controllers
{
    [ApiController]
    [Route("Devis_details")]
    public class Devis_detailController : ControllerBase
    {
        private readonly App_Db_Context _context;

        public Devis_detailController(App_Db_Context context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Devis_detail>>> GetDevis_details()
        {
            Console.WriteLine($"List : Devis_details");
            return await _context.devis_details.Where(t => t.Statut == 0).ToListAsync();
        }

        [HttpGet("pagination")]
        public async Task<ActionResult<IEnumerable<Devis_detail>>> GetDevis_details_pagination(int page = 1, int element_par_page = 3)
        {
            Console.WriteLine($"List : Devis_details");
            int element_skip = element_par_page * (page - 1);
            var list = _context.devis_details.ToList().Skip(element_skip).Take(element_par_page).ToList();
            bool page_suivant = !_context.devis_details.ToList().Skip(element_par_page * (page)).Take(element_par_page).ToList().IsNullOrEmpty();
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("data", list);
            dict.Add("suivant", page_suivant);
            return Ok(dict);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Devis_detail>> GetDevis_detail(int id)
        {
            Console.WriteLine($"Detail : Devis_details/{id}");
            var devis_detail = await _context.devis_details.FindAsync(id);

            if (devis_detail == null || devis_detail.Statut == 1)
            {
                return NotFound($"Devis_detail est introuvable avec id ={id} ");
            }
            return devis_detail;
        }

        [HttpGet("devis/{id}")]
        public async Task<ActionResult<IEnumerable<Devis_detail>>> devetail_par_devis(int id)
        {
            Devis d = _context.devis.Where(devis => devis.Id_devis == id).FirstOrDefault();
            if (d == null)
            {
                return BadRequest("Aucun devis correspondant");
            }
            List<Devis_detail> list = _context.devis_details
                .Include(t => t.Travaux)
                .Include(t => t.Travaux.Unite)
                .Include(t => t.Maison)
                .Where(devis => devis.Devis == d).ToList();
            return list;
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Devis_detail>> PutDevis_detail(int id, Devis_detail devis_detail)
        {
            Console.WriteLine($"Modifier : Devis_details/{id} || data = {devis_detail.ToJson()}");
            if (id != devis_detail.Id_devis_detail)
            {
                return BadRequest("Valeur id incorrect");
            }

            _context.devis_details.Update(devis_detail);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Devis_detailExists(id))
                {
                    return NotFound($"Devis_detail est introuvable avec id ={id}");
                }
                else
                {
                    throw;
                }
            }
            return Ok(devis_detail);
        }


        [HttpPost]
        public async Task<ActionResult<Devis_detail>> PostTestClass(Devis_detail devis_detail)
        {
            Console.WriteLine($"Ajout : Devis_details || data = {devis_detail.ToJson()}");
            if (!ModelState.IsValid)
            {
                return BadRequest($"Model state invalide \n {ModelState}");
            }
            _context.devis_details.Add(devis_detail);
            await _context.SaveChangesAsync();
            return Ok(devis_detail);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTestClass(int id)
        {
            Console.WriteLine($"Supprimer : Devis_details/{id}");
            var devis_detail = await _context.devis_details.FindAsync(id);
            if (devis_detail == null)
            {
                return NotFound("Devis_details introuvable");
            }
            devis_detail.Statut = 1;
            await _context.SaveChangesAsync();
            return Ok("supprimé");
        }
        [HttpPost("add_data")]
        public async Task<IActionResult> data(List<object> list)
        { return Ok(list); }

        [HttpPost("importCSV")]
        public async Task<IActionResult> ImportCSV(IFormFile fileFichier)
        { return Ok(fileFichier); }

        private bool Devis_detailExists(int id)
        {
            return _context.devis_details.Any(e => e.Id_devis_detail == id);
        }

    }
}