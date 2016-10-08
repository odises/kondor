using Autofac;
using Kondor.Data;
using Kondor.Service.Handlers;
using Kondor.Service.Leitner;
using Kondor.Service.Managers;
using Kondor.Service.Parsers;
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
            var builder = GetContainerBuilder();

            return builder.Build();
        }

        public static ContainerBuilder GetContainerBuilder()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<LeitnerService>().As<ILeitnerService>();

            builder.RegisterType<TelegramApiManager>().As<ITelegramApiManager>();

            builder.RegisterType<NotificationHandler>().As<INotificationHandler>();

            builder.RegisterType<UserApi>().As<IUserApi>();

            builder.RegisterType<TelegramMessageHandler>().As<ITelegramMessageHandler>();

            builder.RegisterType<TaskManager>().As<ITaskManager>();

            builder.RegisterType<Application>().As<IApplication>();

            builder.RegisterType<KondorDataContext>().As<IDbContext>();

            builder.RegisterType<QueryProcessor>().As<IQueryProcessor>();

            builder.RegisterType<SettingHandler>().As<ISettingHandler>();

            builder.RegisterType<TextManager>().As<ITextManager>();

            builder.RegisterType<RichSideParser>().As<IParser>();

            builder.RegisterType<CacheManager>().As<ICacheManager>().SingleInstance();
            

            return builder;
        }

    }
}
