using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Kondor.Data;
using Kondor.Data.DataModel;
using Kondor.WebApplication.Models;
using Microsoft.AspNet.Identity;

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
        public ActionResult Create()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult Create(CardViewModel model)
        {
            var mimeTypeWhitelist = new List<string>
            {
                "image/jpeg",
                "audio/mpeg3",
                "image/gif",
                "image/png"
            };

            if (ModelState.IsValid)
            {
                var examples = new List<string>();
                var files = new List<Tuple<string, byte[]>>();

                if (!string.IsNullOrEmpty(model.Examples))
                {
                    examples = model.Examples.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                }
                if (!string.IsNullOrEmpty(model.MediumUrls))
                {
                    var urls = model.MediumUrls.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var url in urls)
                    {
                        try
                        {
                            var result = DownloadFile(url);
                            files.Add(result);
                        }
                        catch (Exception)
                        {
                        }
                    }
                }



                var mem = new Mem
                {
                    MemBody = model.FrontSide,
                    Definition = model.BackSide,
                    UserId = HttpContext.User.Identity.GetUserId()
                };

                foreach (var example in examples)
                {
                    mem.Examples.Add(new Example
                    {
                        Sentence = example
                    });
                }

                foreach (var file in files)
                {
                    if (mimeTypeWhitelist.Contains(file.Item1))
                    {
                        mem.Media.Add(new Medium
                        {
                            ContentType = file.Item1,
                            MediumContent = file.Item2
                        });
                    }
                }

                _context.Mems.Add(mem);
                _context.SaveChanges();


                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
        }

        [Authorize]
        [ChildActionOnly]
        public ActionResult SearchOnCards(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {

                var result = _context
                    .Mems
                    .Where(p => p.MemBody.ToLower().Contains(id.ToLower().Trim()) && p.UserId == HttpContext.User.Identity.GetUserId())
                    .ToList();
                return Json(result.Select(p => new { memId = p.Id, content = p.MemBody }), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { }, JsonRequestBehavior.AllowGet);
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