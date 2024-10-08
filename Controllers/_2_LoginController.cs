using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NuGet.Protocol;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using temp_back.Connexion;
using temp_back.Models;

namespace temp_back.Controllers
{
    [ApiController]
    [Route("2_Login")]
    public class _2_LoginController : ControllerBase
    {
        private IConfiguration _config;
        private readonly App_Db_Context _context;
        public _2_LoginController(IConfiguration config, App_Db_Context context)
        {
            _config = config;
            _context = context;
        }
        [HttpPost]
        public IActionResult Post(LoginDto login)
        {
            Dictionary<string, object> rep = new Dictionary<string, object>();
            Utilisateur utilisateur = _context.utilisateurs
                .Where(u => u.Email == login.Email)
                .Include(u => u.Profil)
                .FirstOrDefault();
            if (utilisateur == null) { return BadRequest("Utilisateur introuvable"); }
            if (utilisateur.Mot_de_passe != login.Mot_de_passe)
            {
                rep.Add("Mot_de_passe", "Mot de passe incorrecte");
                return BadRequest(rep);
            }
            string tokken = generationTokken(login.Email, login.Mot_de_passe, utilisateur.Profil.Nom);
            rep.Add("tokken", tokken);
            rep.Add("personne", utilisateur);
            return Ok(rep);
        }
        [HttpPost("utilisateur")]
        public async Task<ActionResult> GetLogin(string telephone)
        {
            Dictionary<string, object> rep = new Dictionary<string, object>();

            if (All_type.valide_numero(telephone))
            {
                Utilisateur utilisateur = _context.utilisateurs.Where(u => u.Telephone == telephone).Include(u => u.Profil).FirstOrDefault();
                if (utilisateur == null)
                {
                    utilisateur = new Utilisateur() { Telephone = telephone, Profil = _context.profils.Where(c => c.Nom == "Client").FirstOrDefault() };
                    try
                    {
                        _context.utilisateurs.Add(utilisateur);
                        _context.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("erreur " + ex.Message + "\n");
                        return BadRequest("erreur " + ex.Message);
                    }
                }
                string tokken = generationTokken(telephone, "randy", utilisateur.Profil.Nom);
                rep.Add("tokken", tokken);
                rep.Add("personne", utilisateur);
                return Ok(rep);
            }
            return BadRequest("Valeur invalide");
        }

        [Authorize]
        [HttpGet("TestAuthentification"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> TestAuthentification(string token)
        {
            return Ok(token);
        }
        private string generationTokken(string nom, string mot_de_passe, string role)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, nom),
                new Claim(ClaimTypes.Role, role),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config.GetSection("Jwt:Key").Value!)
            );
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddDays(10),
                signingCredentials: creds
            );
            Console.WriteLine($"Tokken := {token.ToJson()}");
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }
    }
    public class LoginDto
    {
        public string Email { get; set; }
        public string Mot_de_passe { get; set; }
    }
}
