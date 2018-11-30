using Easy4net.DBUtility;
using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TopStrong.Controllers
{
    public class DesignController : Controller
    {
        //
        // GET: /Design/

        public ActionResult Index()
        {
            DBHelper db = DBHelper.getInstance();
            string sql = string.Format("select * from T_Adv where Deleted=0");
            List<T_Adv> AdvList = db.FindBySql<T_Adv>(sql);
            ViewBag.AdvList = AdvList;
            return View();
        }

    }
}
