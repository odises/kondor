using System;
using System.Linq;
using System.Web.Mvc;
using Kondor.Domain;
using Kondor.Domain.Models;
using Kondor.WebApplication.Models;
using Microsoft.AspNet.Identity;

namespace Kondor.WebApplication.Controllers
{
    [Authorize]
    public class DeckController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeckController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Decks()
        {
            var decks = _unitOfWork.DeckRepository.Get().Select(p => new DeckViewModel
            {
                Id = p.Id,
                Title = p.Title
            });

            return View(decks);
        }

        public ActionResult CreateDeck()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateDeck(DeckViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (_unitOfWork.DeckRepository.Any(p => p.Title.ToLower() == model.Title.ToLower()))
                {
                    ModelState.AddModelError("Title", "Duplicated");
                    return View(model);
                }

                var deck = new Deck
                {
                    Title = model.Title,
                    CreationDateTime = DateTime.Now,
                    UserId = User.Identity.GetUserId()
                };

                _unitOfWork.DeckRepository.Insert(deck);
                _unitOfWork.Save();

                return RedirectToAction("Decks");
            }
            else
            {
                return View(model);
            }
        }

        public ActionResult EditDeck(int id)
        {
            return View();
        }

        [HttpPost]
        public ActionResult EditDeck(int id, DeckViewModel model)
        {
            return View();
        }

        public ActionResult DeleteDeck(int id)
        {
            return View();
        }

        public ActionResult SubDecks(int id)
        {
            var subDecks = _unitOfWork.SubDeckRepository.Get();
            return View(subDecks);
        }

        public ActionResult CreateSubDeck()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateSubDeck(SubDeck model)
        {
            return View();
        }

        public ActionResult EditSubDeck(int id)
        {
            return View();
        }

        [HttpPost]
        public ActionResult EditSubDeck(int id, SubDeck model)
        {
            return View();
        }

        public ActionResult DeleteSubDeck(int id)
        {
            return View();
        }
    }
}