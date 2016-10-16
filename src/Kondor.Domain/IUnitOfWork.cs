using System;
using Kondor.Domain.Models;

namespace Kondor.Domain
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Notification> NotificationRepository { get; }
        IRepository<Language> LanguageRepository { get; }
        IRepository<Medium> MediumRepository { get; }
        IRepository<CardState> CardStateRepository { get; }
        ICardRepository CardRepository { get; }
        IExampleRepository ExampleRepository { get; }
        IExampleViewRepository ExampleViewRepository { get; }
        IResponseRepository ResponseRepository { get; }
        ISettingRepository SettingRepository { get; }
        IUpdateRepository UpdateRepository { get; }
        IUserRepository UserRepository { get; }
        void Save();
    }
}
