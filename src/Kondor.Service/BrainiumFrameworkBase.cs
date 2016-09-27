using Kondor.Service.Handlers;
using Kondor.Service.Managers;

namespace Kondor.Service
{
    public static class BrainiumFrameworkBase
    {
        public static ICacheManager Cache => ObjectManager.GetInstance<ICacheManager>();
        //public static ISettingHandler Settings => ObjectManager.GetInstance<ISettingHandler>();
        //public static ITextManager TextManager => ObjectManager.GetInstance<ITextManager>();
    }
}
