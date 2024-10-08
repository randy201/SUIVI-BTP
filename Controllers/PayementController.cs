using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using NuGet.Protocol;
using System.Globalization;
using temp_back.Connexion;
using temp_back.Models;
namespace temp_back.Controllers
{
    [ApiController]
    [Route("Payements")]
    public class PayementController : ControllerBase
    {
        private readonly App_Db_Context _context;
        private readonly IMemoryCache _cache;

        public PayementController(App_Db_Context context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Payement>>> GetPayements()
        {
            Console.WriteLine($"List : Payements");
            return await _context.payements.Where(t => t.Statut == 0).ToListAsync();
        }
        [HttpGet("total_payement"), Authorize(Roles = "Admin")]
        public async Task<ActionResult> Pay()
        {
            double total = _context.payements.Where(pa => pa.Statut == 0).ToList().Sum(p => p.Montant);
            return Ok(total);
        }
        [HttpGet("trie_payer")]
        public async Task<ActionResult> trie_payer()
        {
            List<Devis> devis = _context.devis.ToList();
            List<Payement> list_pay = new List<Payement>();
            foreach (Devis item in devis)
            {
                Payement payement = _context.payements
                .Where(p => p.Devis == item)
                .OrderByDescending(p => p.Date_payement).FirstOrDefault();
                if (payement != null)
                {
                    list_pay.Add(payement);

                }

            }

            list_pay = list_pay.OrderBy(l => l.Reste_payer).ToList();
            return Ok(list_pay);
        }

        [HttpGet("devis/{id}")]
        public async Task<ActionResult> Get_payementpardevis(int id)
        {
            List<Payement> list_payement = _context.payements.Where(p => p.Devis.Id_devis == id).ToList();
            return Ok(list_payement);
        }

        [HttpGet("pagination")]
        public async Task<ActionResult<IEnumerable<Payement>>> GetPayements_pagination(int page = 1, int element_par_page = 3)
        {
            Console.WriteLine($"List : Payements");
            int element_skip = element_par_page * (page - 1);
            var list = _context.payements.ToList().Skip(element_skip).Take(element_par_page).ToList();
            bool page_suivant = !_context.payements.ToList().Skip(element_par_page * (page)).Take(element_par_page).ToList().IsNullOrEmpty();
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("data", list);
            dict.Add("suivant", page_suivant);
            return Ok(dict);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Payement>> GetPayement(int id)
        {
            Console.WriteLine($"Detail : Payements/{id}");
            var payement = await _context.payements.FindAsync(id);

            if (payement == null || payement.Statut == 1)
            {
                return NotFound($"Payement est introuvable avec id ={id} ");
            }
            return payement;
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Payement>> PutPayement(int id, Payement payement)
        {
            Console.WriteLine($"Modifier : Payements/{id} || data = {payement.ToJson()}");
            if (id != payement.Id_payement)
            {
                return BadRequest("Valeur id incorrect");
            }

            _context.payements.Update(payement);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PayementExists(id))
                {
                    return NotFound($"Payement est introuvable avec id ={id}");
                }
                else
                {
                    throw;
                }
            }
            return Ok(payement);
        }


        [HttpPost]
        public async Task<ActionResult<Payement>> PostTestClass(PayementDto payement)
        {
            Console.WriteLine($"Ajout : Payements || data = {payement.ToJson()}");
            if (!ModelState.IsValid)
            {
                return BadRequest($"Model state invalide \n {ModelState}");
            }


            double totalPayer = _context.devis
                    .Where(p => p.Id_devis == payement.Id_devis)
                    .Select(p => p.Prix_total)
                    .FirstOrDefault();

            double payer = _context.payements
                    .Where(ps => ps.Devis.Id_devis == payement.Id_devis)
                    .Sum(pt => pt.Montant);


            double total_a_payer = totalPayer;

            Payement pp = new Payement()
            {
                Montant = payement.Montant.Value,
                Devis = _context.devis.Where(pay => pay.Id_devis == payement.Id_devis).FirstOrDefault(),
                Date_payement = payement.Date_payement.Value,
                Reste_payer = totalPayer - (payement.Montant.Value + payer),
            };
            if (pp.Reste_payer < 0) { return BadRequest("Le montant payer dépasse du prix à payer"); }
            _context.payements.Add(pp);
            await _context.SaveChangesAsync();
            return Ok(pp);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTestClass(int id)
        {
            Console.WriteLine($"Supprimer : Payements/{id}");
            var payement = await _context.payements.FindAsync(id);
            if (payement == null)
            {
                return NotFound("Payements introuvable");
            }
            payement.Statut = 1;
            await _context.SaveChangesAsync();
            return Ok("supprimé");
        }
        [HttpPost("add_data")]
        public async Task<IActionResult> data(List<PayementExcel> payementExcels)
        {
            string message = "";
            foreach (PayementExcel item in payementExcels)
            {

                Devis devis = _context.devis.Where(d => d.Ref_devis == item.ref_devis).FirstOrDefault();
                if (devis == null) { return BadRequest($"Le devis n'existe pas ref = {item.ref_devis}"); }
                double payer = _context.payements
                        .Where(ps => ps.Devis == devis)
                        .Sum(pt => pt.Montant);
                Payement paye = _context.payements.Where(p => p.Ref_payement == item.ref_paiement).FirstOrDefault();
                if (paye == null)
                {
                    Payement payement = new Payement()
                    {
                        Devis = devis,
                        Ref_payement = item.ref_paiement,
                        Date_payement = DateOnly.Parse(item.date_paiement),
                        Montant = double.Parse(item.montant)

                    };
                    payement.Reste_payer = devis.Prix_total - (payement.Montant + payer);
                    _context.payements.Add(payement);
                    try
                    {
                        _context.SaveChanges();
                        message = message + $" insertion de {payement.Ref_payement}";

                    }
                    catch (Exception ex) { return BadRequest(ex.Message); }
                }
                else
                {
                    message = message + $" le payement existe deja ref={paye.Ref_payement} \n";
                    Console.WriteLine($" LE PAYEMENT EXISTE DEJA {paye.ToJson()} ");
                }

            }
            return Ok(message);
        }

        [HttpPost("importCSV")]
        public async Task<IActionResult> ImportCSV(IFormFile fileFichier)
        {
            List<PayementExcel> payementExcels = new List<PayementExcel>();
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
                using (var reader = new StreamReader(fileFichier.OpenReadStream()))
                using (var csv = new CsvReader(reader, csvConfig))
                {
                    csv.Read();
                    csv.ReadHeader();

                    while (csv.Read())
                    {
                        string message = "";
                        PayementExcel payement = new PayementExcel();
                        string? ref_devis = csv.GetField("ref_devis").ToString().Trim();
                        if (ref_devis.IsNullOrEmpty()) { message += " ref_devis est vide"; }

                        string? ref_paiement = csv.GetField("ref_paiement").ToString().Trim();
                        if (ref_paiement.IsNullOrEmpty()) { message += " ref_paiement est vide"; }

                        string? date_paiement = csv.GetField("date_paiement").ToString().Trim();
                        if (date_paiement.IsNullOrEmpty()) { message += " date_paiement est vide"; }

                        string? montant = csv.GetField("montant").ToString().Trim();
                        if (montant.IsNullOrEmpty()) { message += " montant est vide"; }
                        payement = new PayementExcel()
                        {
                            ref_devis = ref_devis,
                            ref_paiement = ref_paiement,
                            date_paiement = date_paiement,
                            montant = montant,
                            message = message

                        };
                        payementExcels.Add(payement);
                    }
                }
            }
            catch (Exception e) { BadRequest(e.Message); }
            return Ok(payementExcels);
        }

        private bool PayementExists(int id)
        {
            return _context.payements.Any(e => e.Id_payement == id);
        }

    }
}