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
            var mem = _context.Mems.FirstOrDefault(p => p.Id == id);
            if (mem == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var images = mem.Media.Where(p => p.ContentType.Contains("image")).Select(p => p.Id);

            return View(images);
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
            if (ttype == TextType.Example)
            {
                var model = new SpeakViewModel();
                var example = _context.Examples.FirstOrDefault(p => p.Id == tid);
                if (example != null)
                {
                    model.Text = example.Sentence;

                    var v = _context.Voices.FirstOrDefault(p => p.Text == example.Sentence);
                    if (v != null)
                    {
                        model.VoiceData = v.VoiceData;
                        model.ContentType = v.ContentType;
                    }
                    else
                    {
                        var voiceData =
                            DownloadFile(
                                string.Format(_settingHandler.GetSettings<GeneralSettings>().TextReaderApiBaseUri,
                                    example.Sentence));

                        _context.Voices.Add(new Voice
                        {
                            ContentType = voiceData.Item1,
                            Text = example.Sentence,
                            VoiceData = voiceData.Item2
                        });
                        _context.SaveChanges();

                        model.VoiceData = voiceData.Item2;
                        model.ContentType = voiceData.Item1;
                    }
                    return View(model);
                }
                else
                {
                    throw new ArgumentNullException();
                }
            }
            else
            {
                throw new Exception();
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