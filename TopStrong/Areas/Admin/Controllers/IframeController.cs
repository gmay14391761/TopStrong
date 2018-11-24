using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TopStrong.Areas.Admin.Models;

namespace TopStrong.Areas.Admin.Controllers
{
    public class IframeController : Controller
    {
        //
        // GET: /Admin/Iframe/

        [AdminLoginAuthorize]
        public ActionResult Index(string url="/Admin/Index")
        {
            ViewBag.url = url;
            return View();
        }

    }
}
