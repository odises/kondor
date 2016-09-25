using System;
using System.Web.Mvc;
using Kondor.Data.SettingModels;
using Kondor.Service;

namespace Kondor.WebApplication.Controllers
{
    public class SettingController : Controller
    {
        // GET: Setting
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult General()
        {
            try
            {
                using (BrainiumFrameworkBase.Cache.Ignore())
                {
                    var model = BrainiumFrameworkBase.Settings.GetSettings<GeneralSettings>();
                    return View(model);
                }
            }
            catch (ArgumentNullException)
            {
                return View(new GeneralSettings());
            }
        }

        [HttpPost]
        public ActionResult General(GeneralSettings model)
        {
            BrainiumFrameworkBase.Settings.SaveSettings(model);
            return View();
        }
    }
}