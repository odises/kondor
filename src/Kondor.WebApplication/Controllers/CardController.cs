using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Kondor.Data;
using Kondor.Data.DataModel;
using Kondor.Data.Enums;
using Kondor.Data.LeitnerDataModels;
using Kondor.Service;
using Kondor.Service.Parsers;
using Kondor.WebApplication.Models;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;

namespace Kondor.WebApplication.Controllers
{
    public class CardController : Controller
    {
        private readonly IDbContext _context;
        public CardController(IDbContext context)
        {
            _context = context;
        }

        // GET: Card
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult CreateRichCard()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult CreateRichCard(RichCardViewModel model)
        {
            if (ModelState.IsValid)
            {
                var parser = ObjectManager.GetInstance<IParser>();

                var richCard = new RichCard
                {
                    Front = parser.ParseSimpleSide(model.FrontSide),
                    Back = parser.ParseRichSide(model.BackSide)
                };

                var backSide = richCard.Back as RichSide;
                if (backSide == null || backSide.PartsOfSpeech.Count == 0)
                {
                    ModelState.AddModelError("BackSide", "Input text is not valid.");
                    return View(model);
                }
                else
                {
                    var serializedCard = JsonConvert.SerializeObject(richCard);

                    _context.Cards.Add(new Card
                    {
                        CardStatus = CardStatus.Draft,
                        CardType = CardType.RichCard,
                        UserId = User.Identity.GetUserId(),
                        CardData = serializedCard
                    });

                    var examples = backSide.PartsOfSpeech.SelectMany(p => p.Definitions).SelectMany(x => x.Examples);

                    foreach (var example in examples)
                    {
                        _context.Examples.Add(new Data.DataModel.Example
                        {
                            Sentence = example.Value,
                            ExampleUniqueId = example.Id
                        });
                    }

                    _context.SaveChanges();

                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                return View(model);
            }
        }

        [Authorize]
        public ActionResult CreateSimpleCard()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult CreateSimpleCard(SimpleCardViewModel model)
        {
            if (ModelState.IsValid)
            {
                var parser = ObjectManager.GetInstance<IParser>();

                var simpleCard = new SimpleCard
                {
                    Front = parser.ParseSimpleSide(model.FrontSide),
                    Back = parser.ParseSimpleSide(model.BackSide)
                };

                var serializedCard = JsonConvert.SerializeObject(simpleCard);

                _context.Cards.Add(new Card
                {
                    CardStatus = CardStatus.Draft,
                    CardType = CardType.SimpleCard,
                    UserId = User.Identity.GetUserId(),
                    CardData = serializedCard
                });

                _context.SaveChanges();

                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View(model);
            }
        }

        [Authorize]
        [ChildActionOnly]
        public ActionResult SearchOnCards(string id)
        {
            // todo not now
            throw new NotImplementedException();
            //if (!string.IsNullOrEmpty(id))
            //{

            //    var result = _context
            //        .Mems
            //        .Where(p => p.MemBody.ToLower().Contains(id.ToLower().Trim()) && p.UserId == HttpContext.User.Identity.GetUserId())
            //        .ToList();
            //    return Json(result.Select(p => new { memId = p.Id, content = p.MemBody }), JsonRequestBehavior.AllowGet);
            //}
            //else
            //{
            //    return Json(new { }, JsonRequestBehavior.AllowGet);
            //}
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