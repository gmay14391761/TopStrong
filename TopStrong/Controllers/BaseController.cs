using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TopStrong.Controllers
{
    public class BaseController : Controller
    {
        //
        // GET: /Base/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Top(string title)
        {
            ViewBag.Title = title;
            return PartialView();
        }
    }
}
