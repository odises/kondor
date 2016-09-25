using System;
using System.Linq;
using Kondor.Data;
using Kondor.Data.DataModel;
using Kondor.Data.SettingModels;
using Newtonsoft.Json;

namespace Kondor.Service.Handlers
{
    public class SettingHandler : ISettingHandler
    {
        private readonly IDbContext _context;

        public SettingHandler(IDbContext context)
        {
            _context = context;
        }

        public T GetSettings<T>() where T : class
        {
            var typeName = typeof (T).Name;
            

            var result = ObjectManager.GetInstance<ICacheManager>().FromCache<T>($"cacheKey:{typeName}", () =>
            {
                var setting = _context.Settings.FirstOrDefault(p => p.SettingType == typeName);
                if (setting != null)
                {
                    var data = JsonConvert.DeserializeObject<T>(setting.SettingData);
                    return data;
                }
                else
                {
                    return Activator.CreateInstance<T>();
                }

            });

            return result;
        }

        public void SaveSettings(ISettings settings)
        {
            var typeName = settings.GetType().Name;

            var setting = _context.Settings.FirstOrDefault(p => p.SettingType == typeName);
            if (setting != null)
            {
                setting.SettingData = JsonConvert.SerializeObject(settings);
            }
            else
            {
                _context.Settings.Add(new Setting
                {
                    CreationDate = DateTime.Now,
                    SettingType = settings.GetType().Name,
                    SettingData = JsonConvert.SerializeObject(settings)
                });
            }

            _context.SaveChanges();
        }
    }
}
