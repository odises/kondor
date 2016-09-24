using Kondor.Data.SettingModels;

namespace Kondor.Service.Handlers
{
    public interface ISettingHandler
    {
        T GetSettings<T>() where T : class;
        void SaveSettings(ISettings settings);
    }
}
