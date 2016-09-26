using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Kondor.Data;

namespace Kondor.WebApplication.Controllers
{
    public class MemController : Controller
    {
        private readonly IDbContext _context;

        public MemController(IDbContext context)
        {
            _context = context;
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
    }
}