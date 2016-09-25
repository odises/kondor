using Kondor.Service.Handlers;

namespace Kondor.Service
{
    public static class BrainiumFrameworkBase
    {
        public static ICacheManager Cache => ObjectManager.GetInstance<ICacheManager>();
        public static ISettingHandler Settings => ObjectManager.GetInstance<ISettingHandler>();
    }
}
