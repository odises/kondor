using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Kondor.Data;
using Kondor.Data.LeitnerDataModels;
using Kondor.Domain.Enums;
using Kondor.Domain.Models;
using Kondor.Service;
using Kondor.Service.Parsers;
using Kondor.WebApplication.Models;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;

namespace Kondor.WebApplication.Controllers
{
    [Authorize]
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
            var model = new List<CardViewModel>();
            var userId = User.Identity.GetUserId();
            var cards = _context.Cards.Where(p => p.UserId == userId);

            foreach (var card in cards)
            {
                var item = new CardViewModel
                {
                    Id = card.Id,
                    CardType = card.CardType
                };

                switch (card.CardType)
                {
                    case CardType.SimpleCard:
                        {
                            var cardData = JsonConvert.DeserializeObject<SimpleCard>(card.CardData);

                            item.Back = cardData.Back;
                            item.Front = cardData.Front;

                            break;
                        }
                    case CardType.RichCard:
                        {
                            var cardData = JsonConvert.DeserializeObject<RichCard>(card.CardData);

                            item.Back = cardData.Back;
                            item.Front = cardData.Front;

                            break;
                        }
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                model.Add(item);
            }


            return View(model);
        }

        public ActionResult CreateRichCard()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateRichCard(RawCardViewModel model)
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

                    var card = new Card
                    {
                        CardStatus = CardStatus.Draft,
                        CardType = CardType.RichCard,
                        UserId = User.Identity.GetUserId(),
                        CardData = serializedCard
                    };

                    _context.Cards.Add(card);

                    var examples = backSide.PartsOfSpeech.SelectMany(p => p.Definitions).SelectMany(x => x.Examples);

                    foreach (var example in examples)
                    {
                        card.Examples.Add(new Domain.Models.Example
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

        public ActionResult CreateSimpleCard()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateSimpleCard(RawCardViewModel model)
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

        public ActionResult Edit(int id)
        {
            var card = _context.Cards.FirstOrDefault(p => p.Id == id);
            if (card == null)
            {
                throw new NullReferenceException();
            }
            else
            {
                var model = new RawCardViewModel();

                if (card.CardType == CardType.SimpleCard)
                {
                    var cardData = JsonConvert.DeserializeObject<SimpleCard>(card.CardData);

                    model.FrontSide = cardData.Front.Raw();
                    model.BackSide = cardData.Back.Raw();

                    return View(model);
                }
                else if (card.CardType == CardType.RichCard)
                {
                    var cardData = JsonConvert.DeserializeObject<RichCard>(card.CardData);

                    model.FrontSide = cardData.Front.Raw();
                    model.BackSide = cardData.Back.Raw();

                    return View(model);
                }
                else
                {
                    throw new IndexOutOfRangeException();
                }
            }
        }

        [HttpPost]
        public ActionResult Edit(int id, RawCardViewModel model)
        {
            if (ModelState.IsValid)
            {
                var parser = ObjectManager.GetInstance<IParser>();

                var card = _context.Cards.FirstOrDefault(p => p.Id == id);
                if (card == null)
                {
                    throw new NullReferenceException();
                }
                else
                {
                    if (card.CardType == CardType.SimpleCard)
                    {
                        var simpleCard = new SimpleCard
                        {
                            Front = parser.ParseSimpleSide(model.FrontSide),
                            Back = parser.ParseSimpleSide(model.BackSide)
                        };
                        var serialized = JsonConvert.SerializeObject(simpleCard);
                        card.CardData = serialized;

                        _context.SaveChanges();

                        return RedirectToAction("Index");
                    }
                    else if (card.CardType == CardType.RichCard)
                    {
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
                            var serialized = JsonConvert.SerializeObject(richCard);

                            card.CardData = serialized;

                            var beforeEditExamples = _context.Examples.Where(p => p.CardId == card.Id);
                            var exampleViews =
                                _context.ExampleViews.Where(p => beforeEditExamples.Any(c => c.Id == p.ExampleId));

                            foreach (var beforeEditExample in beforeEditExamples)
                            {
                                beforeEditExample.RowStatus = RowStatus.Removed;
                            }
                            foreach (var exampleView in exampleViews)
                            {
                                exampleView.RowStatus = RowStatus.Removed;
                            }

                            var examples = backSide.PartsOfSpeech.SelectMany(p => p.Definitions).SelectMany(x => x.Examples);

                            foreach (var example in examples)
                            {
                                card.Examples.Add(new Domain.Models.Example
                                {
                                    Sentence = example.Value,
                                    ExampleUniqueId = example.Id
                                });
                            }

                            _context.SaveChanges();

                            if (card.CardStates.Any())
                            {
                                foreach (var example in card.Examples)
                                {
                                    if (!_context.ExampleViews.Any(p => p.ExampleId == example.Id && p.UserId == example.Card.UserId))
                                    {
                                        _context.ExampleViews.Add(new ExampleView
                                        {
                                            ExampleId = example.Id,
                                            UserId = example.Card.UserId,
                                            Views = 0
                                        });
                                    }
                                }
                            }

                            _context.SaveChanges();


                            return RedirectToAction("Index");
                        }
                    }
                    else
                    {
                        throw new IndexOutOfRangeException();
                    }
                }
            }
            else
            {
                return View(model);
            }
        }

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