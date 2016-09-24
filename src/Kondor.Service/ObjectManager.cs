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

            builder.RegisterType<LeitnerService>().As<ILeitnerService>().WithParameters(new List<Parameter>
            {
                new NamedParameter("overStoppingTolerance", 20),
                new NamedParameter("maximumCardInFirstPosition", 15),
                new NamedParameter("timeUnit", TimeUnit.Minute)
            });

            builder.RegisterType<TelegramApiManager>().As<ITelegramApiManager>().WithParameter("apiKey", "bot264301717:AAHxLu9FcPWahQni6L8ahQvu74sHf-TlX_E");

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

            return builder.Build();
        }

    }
}
