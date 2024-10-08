using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using temp_back.Connexion;

namespace temp_back.Controllers
{
    [Route("3_Delete")]
    [ApiController]
    public class _3_DeleteController : ControllerBase
    {
        private readonly App_Db_Context _context;
        public _3_DeleteController(App_Db_Context context)
        {
            _context = context;
        }
        [HttpGet("back_up")]
        public async Task<ActionResult> back_up(string script)
        {
            try
            {
                int rowsAffected = _context.Database.ExecuteSqlRaw(script);
                if (rowsAffected > 0)
                {
                    return Ok(new { Message = "Query executed successfully", RowsAffected = rowsAffected });
                }
                else
                {
                    return Ok(new { Message = "Query executed without affecting any rows" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<ActionResult> Clean_database()
        {
            try
            {
                _context.utilisateurs.RemoveRange(_context.utilisateurs.Where(u => u.Nom != "root"));
                _context.devis.RemoveRange(_context.devis);
                _context.maisons.RemoveRange(_context.maisons);
                _context.detail_maisons.RemoveRange(_context.detail_maisons);
                _context.type_finitions.RemoveRange(_context.type_finitions);
                _context.unites.RemoveRange(_context.unites);
                _context.devis_details.RemoveRange(_context.devis_details);
                _context.payements.RemoveRange(_context.payements);
                _context.travaux.RemoveRange(_context.travaux);
                _context.SaveChanges();
                _context.Database.ExecuteSqlInterpolated($"ALTER SEQUENCE \"utilisateurs_Id_utilisateur_seq\" RESTART WITH 2");
                _context.Database.ExecuteSqlInterpolated($"ALTER SEQUENCE \"devis_Id_devis_seq\" RESTART WITH 1");
                _context.Database.ExecuteSqlInterpolated($"ALTER SEQUENCE \"maisons_Id_maison_seq\" RESTART WITH 1");
                _context.Database.ExecuteSqlInterpolated($"ALTER SEQUENCE \"detail_maisons_Id_detail_maison_seq\" RESTART WITH 1");
                _context.Database.ExecuteSqlInterpolated($"ALTER SEQUENCE \"type_finitions_Id_type_finition_seq\" RESTART WITH 1");
                _context.Database.ExecuteSqlInterpolated($"ALTER SEQUENCE \"unites_Id_unite_seq\" RESTART WITH 1");
                _context.Database.ExecuteSqlInterpolated($"ALTER SEQUENCE \"devis_details_Id_devis_detail_seq\" RESTART WITH 1");
                _context.Database.ExecuteSqlInterpolated($"ALTER SEQUENCE \"payements_Id_payement_seq\" RESTART WITH 1");
                _context.Database.ExecuteSqlInterpolated($"ALTER SEQUENCE \"travaux_Id_travaux_seq\" RESTART WITH 1");
                return Ok("you are safe");
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }
    }
}
