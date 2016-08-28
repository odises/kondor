using System.Data.Entity;

namespace Kondor.Service.DataModel
{
    public class EntityContext : DbContext
    {
        public EntityContext() : base("name=MainEntities")
        {
            Database.SetInitializer<EntityContext>(new DropCreateDatabaseIfModelChanges<EntityContext>());
        }

        public DbSet<Word> Words { get; set; }
        public DbSet<Card> Cards { get; set; }
        public DbSet<User> Users { get; set; } 
        public DbSet<Message> Messages { get; set; } 
    }
}
