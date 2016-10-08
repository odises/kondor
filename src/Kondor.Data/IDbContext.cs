using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using Kondor.Data.DataModel;

namespace Kondor.Data
{
    public interface IDbContext
    {
        DbEntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
        DbEntityEntry Entry(object entity);
        DbSet<TEntity> Set<TEntity>() where TEntity : class;
        DbSet Set(Type entityType);

        int SaveChanges();

        DbSet<CardState> CardStates { get; set; }
        DbSet<Example> Examples { get; set; }
        DbSet<ExampleView> ExampleViews { get; set; }
        DbSet<Card> Cards { get; set; }
        DbSet<Notification> Notifications { get; set; }
        DbSet<Response> Responses { get; set; }
        DbSet<Update> Updates { get; set; }
        DbSet<Setting> Settings { get; set; }
        DbSet<Medium> Media { get; set; }
        DbSet<Language> Languages { get; set; }
        DbSet<StringResource> StringResources { get; set; }
    }
}