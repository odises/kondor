using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Kondor.Data;
using Kondor.Data.DataModel;
using Kondor.Data.Enums;
using Kondor.Data.SettingModels;
using Kondor.Service.Handlers;
using Kondor.WebApplication.Models;

namespace Kondor.WebApplication.Controllers
{
    public class MemController : Controller
    {
        private readonly IDbContext _context;
        private readonly ISettingHandler _settingHandler;

        public MemController(IDbContext context, ISettingHandler settingHandler)
        {
            _context = context;
            _settingHandler = settingHandler;
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

        public async Task<ActionResult> RenderImage(int id)
        {
            var medium = await _context.Media.FirstOrDefaultAsync(p => p.Id == id);
            if (medium == null)
            {
                throw new ArgumentNullException();
            }

            return File(medium.MediumContent, medium.ContentType);
        }

        [Route("learning/mem/speak/tid/{tid}/ttype/{ttype}")]
        public ActionResult Speak(int tid, TextType ttype)
        {
            switch (ttype)
            {
                case TextType.Example:
                    {
                        var model = new SpeakViewModel();
                        var example = _context.Examples.FirstOrDefault(p => p.Id == tid);
                        if (example != null)
                        {
                            model.Text = example.Sentence;
                            return View(model);
                        }

                        throw new ArgumentNullException();
                    }
                case TextType.CardFrontSideText:
                    throw new NotImplementedException();
                default:
                    throw new ArgumentOutOfRangeException(nameof(ttype), ttype, null);
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