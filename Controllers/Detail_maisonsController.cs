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
    [Route("Detail_maisons")]
    public class Detail_maisonController : ControllerBase
    {
        private readonly App_Db_Context _context;
        private readonly IMemoryCache _cache;


        public Detail_maisonController(App_Db_Context context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;

        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Detail_maison>>> GetDetail_maisons()
        {
            Console.WriteLine($"List : Detail_maisons");
            return await _context.detail_maisons.Where(t => t.Statut == 0).ToListAsync();
        }

        [HttpGet("pagination")]
        public async Task<ActionResult<IEnumerable<Detail_maison>>> GetDetail_maisons_pagination(int page = 1, int element_par_page = 3)
        {
            Console.WriteLine($"List : Detail_maisons");
            int element_skip = element_par_page * (page - 1);
            var list = _context.detail_maisons.Where(t => t.Statut == 0).ToList().Skip(element_skip).Take(element_par_page).ToList();
            bool page_suivant = !_context.detail_maisons.Where(t => t.Statut == 0).ToList().Skip(element_par_page * (page)).Take(element_par_page).ToList().IsNullOrEmpty();
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("data", list);
            dict.Add("suivant", page_suivant);
            return Ok(dict);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Detail_maison>> GetDetail_maison(int id)
        {
            Console.WriteLine($"Detail : Detail_maisons/{id}");
            var detail_maison = await _context.detail_maisons.FindAsync(id);

            if (detail_maison == null || detail_maison.Statut == 1)
            {
                return NotFound($"Detail_maison est introuvable avec id ={id} ");
            }
            return detail_maison;
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Detail_maison>> PutDetail_maison(int id, Detail_maison detail_maison)
        {
            Console.WriteLine($"Modifier : Detail_maisons/{id} || data = {detail_maison.ToJson()}");
            if (id != detail_maison.Id_detail_maison)
            {
                return BadRequest("Valeur id incorrect");
            }

            _context.detail_maisons.Update(detail_maison);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Detail_maisonExists(id))
                {
                    return NotFound($"Detail_maison est introuvable avec id ={id}");
                }
                else
                {
                    throw;
                }
            }
            return Ok(detail_maison);
        }


        [HttpPost]
        public async Task<ActionResult<Detail_maison>> PostTestClass(Detail_maisonDto detail_maison)
        {
            Console.WriteLine($"Ajout : Detail_maisons || data = {detail_maison.ToJson()}");
            if (!ModelState.IsValid)
            {
                return BadRequest($"Model state invalide \n {ModelState}");
            }
            Detail_maison detail = new Detail_maison()
            {
                Travaux = _context.travaux.Where(t => t.Id_travaux == detail_maison.Id_travaux).Where(t => t.Statut == 0).FirstOrDefault(),
                Maison = _context.maisons.Where(d => d.Id_maison == detail_maison.Id_maison).Where(t => t.Statut == 0).FirstOrDefault(),
                Quantite = detail_maison.Quantite,
            };
            _context.detail_maisons.Add(detail);



            try
            {
                await _context.SaveChangesAsync();
                //manao anleh calcul denormalisation
                Maison maison = _context.maisons.Where(m => m.Id_maison == detail.Maison.Id_maison && m.Statut == 0).FirstOrDefault();
                maison.Prix_total = maison.Prix_total + detail.Travaux.Prix_unitaire * detail.Quantite;
                maison.Durrer_totale = maison.Durrer_totale + detail.Travaux.Durree;
                _context.maisons.Update(maison);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            return Ok(detail_maison);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTestClass(int id)
        {
            Console.WriteLine($"Supprimer : Detail_maisons/{id}");
            var detail_maison = await _context.detail_maisons.FindAsync(id);
            if (detail_maison == null)
            {
                return NotFound("Detail_maisons introuvable");
            }
            detail_maison.Statut = 1;
            await _context.SaveChangesAsync();
            return Ok("supprimé");
        }


        [HttpPost("add_data")]
        public async Task<IActionResult> data(List<object> list)
        { return Ok(list); }

        [HttpPost("importCSV")]
        public async Task<IActionResult> ImportCSV(IFormFile fileFichier)
        { return Ok(fileFichier); }

        private bool Detail_maisonExists(int id)
        {
            return _context.detail_maisons.Any(e => e.Id_detail_maison == id);
        }

    }
}