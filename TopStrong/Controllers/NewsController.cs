using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TopStrong.Controllers
{
    public class NewsController : Controller
    {
        //
        // GET: /Theme/

        public ActionResult Index(string ThemeID)
        {
            return View();
        }

        public ActionResult NewsDetail(string ID)
        {
            return View();
        }
    }
}
