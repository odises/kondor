using System.Data.Entity;
using Kondor.Data.DataModel;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Kondor.Data
{
    public class EntityContext : IdentityDbContext<ApplicationUser>
    {
        public EntityContext() : base("name=MainEntities", throwIfV1Schema: false)
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<EntityContext>());
        }

        public DbSet<Mem> Mems { get; set; }
        public DbSet<Card> Cards { get; set; }
        public DbSet<Update> Updates { get; set; }
        public DbSet<Example> Examples { get; set; }
        public DbSet<ExampleView> ExampleViews { get; set; }
        public DbSet<Response> Responses { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        public static EntityContext Create()
        {
            return new EntityContext();
        }
    }
}
