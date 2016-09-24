using System.Collections.Generic;
using Autofac;
using Autofac.Core;
using Kondor.Data;
using Kondor.Data.Enums;
using Kondor.Service.Handlers;
using Kondor.Service.Leitner;
using Kondor.Service.Managers;
using Kondor.Service.Processors;

namespace Kondor.Service
{
    public static class ObjectManager
    {
        private static IContainer _container;

        public static void Initialize()
        {
            _container = BuildContainer();
        }

        public static T GetInstance<T>()
        {
            return _container.Resolve<T>();
        }

        private static IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<LeitnerService>().As<ILeitnerService>();

            builder.RegisterType<TelegramApiManager>().As<ITelegramApiManager>();

            builder.RegisterType<NotificationHandler>().As<INotificationHandler>();

            builder.RegisterType<UserApi>().As<IUserApi>();

            builder.RegisterType<TelegramMessageHandler>().As<ITelegramMessageHandler>().WithParameters(new List<Parameter>
            {
                new NamedParameter("cipherKey", "testkey"),
                new NamedParameter("registrationBaseUri", "http://www.brainium.ir/account/newuser")
            });

            builder.RegisterType<TaskManager>().As<ITaskManager>();

            builder.RegisterType<Application>().As<IApplication>();

            builder.RegisterType<KondorDataContext>().As<IDbContext>();

            builder.RegisterType<QueryProcessor>().As<IQueryProcessor>();

            builder.RegisterType<SettingHandler>().As<ISettingHandler>();

            return builder.Build();
        }

    }
}
