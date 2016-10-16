using System;
using Kondor.Data.SettingModels;
using Kondor.Domain;
using Kondor.Domain.Models;
using Newtonsoft.Json;

namespace Kondor.Service.Handlers
{
    public class SettingHandler : ISettingHandler
    {
        private readonly IUnitOfWork _unitOfWork;
        public SettingHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public T GetSettings<T>() where T : class
        {
            var typeName = typeof (T).Name;
            

            var result = ObjectManager.GetInstance<ICacheManager>().FromCache<T>($"cacheKey:{typeName}", () =>
            {
                var setting = _unitOfWork.SettingRepository.GetSettingByType(typeName);
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

            var setting = _unitOfWork.SettingRepository.GetSettingByType(typeName);

            if (setting != null)
            {
                setting.SettingData = JsonConvert.SerializeObject(settings);
            }
            else
            {
                var newSetting = new Setting
                {
                    CreationDate = DateTime.Now,
                    SettingType = settings.GetType().Name,
                    SettingData = JsonConvert.SerializeObject(settings)
                };

                _unitOfWork.SettingRepository.Insert(newSetting);
            }

            _unitOfWork.Save();
        }
    }
}
