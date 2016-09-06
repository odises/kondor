using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Kondor.Data.DataModel;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Kondor.Data
{
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class EntityContext : IdentityDbContext<ApplicationUser>
    {
        public EntityContext() : base("name=MainEntities", throwIfV1Schema: false)
        {
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<EntityContext>());
        }

        public DbSet<Word> Words { get; set; }
        public DbSet<Card> Cards { get; set; }
        public DbSet<CustomUser> CustomUsers { get; set; } 
        public DbSet<Message> Messages { get; set; }

        public static EntityContext Create()
        {
            return new EntityContext();
        }
    }
}
