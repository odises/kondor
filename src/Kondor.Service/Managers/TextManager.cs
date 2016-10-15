using System;
using System.Linq;
using Kondor.Data;
using Kondor.Data.SettingModels;
using Kondor.Domain.Models;
using Kondor.Service.Handlers;

namespace Kondor.Service.Managers
{
    public class TextManager : ITextManager
    {
        private readonly IDbContext _context;
        private readonly ISettingHandler _settingHandler;

        public TextManager(IDbContext context, ISettingHandler settingHandler)
        {
            _context = context;
            _settingHandler = settingHandler;
        }

        public string GetText(string groupCode, string userId = null)
        {
            Guid languageId;
            var defaultLanguageId = _settingHandler.GetSettings<GeneralSettings>().DefaultLanguageId;

            if (string.IsNullOrEmpty(userId))
            {
                // generate text by default langauge
                languageId = defaultLanguageId;
            }
            else
            {
                var user = _context.Set<ApplicationUser>().FirstOrDefault(p => p.Id == userId);
                
                languageId = user?.LanguageId ?? defaultLanguageId;
            }

            return GetText(groupCode, languageId);
        }

        private string GetText(string groupCode, Guid languageId)
        {
            if (_context.Languages.FirstOrDefault(p => p.Id == languageId) == null)
            {
                throw new ArgumentException("There is no language with the passed Id");
            }
            var resource =
                _context.StringResources.FirstOrDefault(p => p.GroupCode == groupCode && p.LanguageId == languageId);
            if (resource == null)
            {
                // generate the text
                var stringResource = new StringResource
                {
                    GroupCode = groupCode,
                    LanguageId = languageId,
                    Text = groupCode
                };
                _context.StringResources.Add(stringResource);
                _context.SaveChanges();

                return stringResource.Text;
            }
            else
            {
                return resource.Text;
            }
        }
    }
}
