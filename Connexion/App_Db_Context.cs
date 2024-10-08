using Microsoft.EntityFrameworkCore;
using temp_back.Models;

namespace temp_back.Connexion
{
    public class App_Db_Context : DbContext
    {
        public App_Db_Context(DbContextOptions<App_Db_Context> options) : base(options) { }
        public DbSet<All_type> all_types { get; set; }
        public DbSet<Profil> profils { get; set; }
        public DbSet<Utilisateur> utilisateurs { get; set; }
        public DbSet<Devis> devis { get; set; }
        public DbSet<Travaux> travaux { get; set; }
        public DbSet<Maison> maisons { get; set; }
        public DbSet<Detail_maison> detail_maisons { get; set; }
        public DbSet<Type_finition> type_finitions { get; set; }
        public DbSet<Unite> unites { get; set; }
        public DbSet<Devis_detail> devis_details { get; set; }
        public DbSet<Payement> payements { get; set; }
    }
}
