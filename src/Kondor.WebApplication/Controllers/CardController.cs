using System.Web.Mvc;
using Kondor.Service.DataModel;
using Kondor.WebApplication.Models;

namespace Kondor.WebApplication.Controllers
{
    public class CardController : Controller
    {
        // GET: Card
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(CardViewModel model)
        {
            var entityContext = new EntityContext();
            var word = new Word();
            
            return View();
        }
    }
}