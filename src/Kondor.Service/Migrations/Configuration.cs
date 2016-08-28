using System.Data.Entity.Migrations;
using Kondor.Service.DataModel;

namespace Kondor.Service.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<EntityContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
            ContextKey = "YourDictionary.Application.EntityContext";
        }

        protected override void Seed(EntityContext context)
        {
            context.Words.Add(new Word { Vocabulary = "Hello 01", Definition = "Test 01" });
            context.Words.Add(new Word { Vocabulary = "Hello 02", Definition = "Test 02" });
            context.Words.Add(new Word { Vocabulary = "Hello 03", Definition = "Test 03" });
            context.Words.Add(new Word { Vocabulary = "Hello 04", Definition = "Test 04" });
            context.Words.Add(new Word { Vocabulary = "Hello 05", Definition = "Test 05" });
            context.Words.Add(new Word { Vocabulary = "Hello 06", Definition = "Test 06" });

            context.Users.Add(new User() {TelegramUserId = 42274705, TelegramUsername = "odises"});
        }
    }
}
