using System.Data.Entity;
using Kondor.Data.DataModel;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Kondor.Data
{
    public class KondorDataContext : IdentityDbContext<ApplicationUser>, IDbContext
    {
        public KondorDataContext() : base("name=MainEntities", throwIfV1Schema: false)
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<KondorDataContext>());
        }

        public DbSet<Mem> Mems { get; set; }
        public DbSet<Card> Cards { get; set; }
        public DbSet<Update> Updates { get; set; }
        public DbSet<Example> Examples { get; set; }
        public DbSet<ExampleView> ExampleViews { get; set; }
        public DbSet<Response> Responses { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<Medium> Media { get; set; }
        public DbSet<Voice> Voices { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<StringResource> StringResources { get; set; }

        public static KondorDataContext Create()
        {
            return new KondorDataContext();
        }
    }
}
