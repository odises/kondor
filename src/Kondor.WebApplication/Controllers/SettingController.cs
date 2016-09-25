using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kondor.Data.SettingModels;
using Kondor.Service.Handlers;

namespace Kondor.WebApplication.Controllers
{
    public class SettingController : Controller
    {
        private readonly ISettingHandler _settingHandler;

        public SettingController(ISettingHandler settingHandler)
        {
            _settingHandler = settingHandler;
        }

        // GET: Setting
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult General()
        {
            try
            {
                var model = _settingHandler.GetSettings<GeneralSettings>();
                return View(model);
            }
            catch (ArgumentNullException)
            {
                return View(new GeneralSettings());
            }
        }

        [HttpPost]
        public ActionResult General(GeneralSettings model)
        {
            _settingHandler.SaveSettings(model);
            return View();
        }
    }
}