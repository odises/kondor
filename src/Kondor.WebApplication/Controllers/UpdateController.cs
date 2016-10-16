using System.IO;
using System.Web.Mvc;
using Kondor.Data.TelegramTypes;
using Kondor.Service.Handlers;
using Newtonsoft.Json;

namespace Kondor.WebApplication.Controllers
{
    public class UpdateController : Controller
    {
        private readonly ITelegramMessageHandler _telegramMessageHandler;

        public UpdateController(ITelegramMessageHandler telegramMessageHandler)
        {
            _telegramMessageHandler = telegramMessageHandler;
        }

        // GET: Update
        [Route("webhook/6f4f8c855085b4d677d093edcc186400/update/push")]
        [HttpPost]
        public void Update()
        {
            using (var reader =new StreamReader(Request.InputStream))
            {
                reader.BaseStream.Seek(0, SeekOrigin.Begin);
                var data = reader.ReadToEnd();
                var update = JsonConvert.DeserializeObject<TelegramUpdate>(data);

                _telegramMessageHandler.SaveUpdate(update);
            }
        }
    }
}