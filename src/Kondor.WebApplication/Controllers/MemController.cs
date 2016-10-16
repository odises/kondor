using System;
using System.IO;
using System.Net;
using System.Web.Mvc;
using Kondor.Data.SettingModels;
using Kondor.Domain;
using Kondor.Service;
using Kondor.Service.Handlers;

namespace Kondor.WebApplication.Controllers
{
    public class MemController : Controller
    {
        private readonly ISettingHandler _settingHandler;
        private readonly IUnitOfWork _unitOfWork;

        public MemController(ISettingHandler settingHandler, IUnitOfWork unitOfWork)
        {
            _settingHandler = settingHandler;
            _unitOfWork = unitOfWork;
        }

        [Route("learning/mem/{id}/images")]
        public ActionResult Images(int id)
        {
            //var mem = _context.Mems.FirstOrDefault(p => p.Id == id);
            //if (mem == null)
            //{
            //    return RedirectToAction("Index", "Home");
            //}

            //var images = mem.Media.Where(p => p.ContentType.Contains("image")).Select(p => p.Id);

            //return View(images);

            // todo not now

            throw new NotImplementedException();
        }

        public ActionResult RenderImage(int id)
        {
            var medium = _unitOfWork.MediumRepository.GetById(id);

            if (medium == null)
            {
                throw new ArgumentNullException();
            }

            return File(medium.MediumContent, medium.ContentType);
        }

        public ActionResult Speak(int id)
        {
            var example = _unitOfWork.ExampleRepository.GetById(id);

            using (BrainiumFrameworkBase.Cache.Ignore())
            {
                var url = $"{_settingHandler.GetSettings<GeneralSettings>().GoogleTranslateUrl}{example.Sentence}";
                return Redirect(url);
            }
        }

        protected virtual Tuple<string, byte[]> DownloadFile(string url)
        {
            var request = HttpWebRequest.Create(url) as HttpWebRequest;
            var response = request?.GetResponse() as HttpWebResponse;

            if (response != null)
            {
                var contentType = response.ContentType;
                var responseStream = response.GetResponseStream();

                if (responseStream != null)
                {
                    var bytes = GetBytes(responseStream);
                    return new Tuple<string, byte[]>(contentType, bytes);
                }
            }
            throw new Exception();
        }

        protected virtual byte[] GetBytes(Stream inputStream)
        {
            using (var memory = new MemoryStream())
            {
                inputStream.CopyTo(memory);
                var result = memory.ToArray();
                return result;
            }
        }
    }
}