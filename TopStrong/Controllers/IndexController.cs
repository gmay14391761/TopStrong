using Easy4net.DBUtility;
using Entity;
using Model.NewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TopStrong.Controllers
{
    public class IndexController : Controller
    {
        //
        // GET: /Index/

        public ActionResult Index()
        {
            DBHelper db = DBHelper.getInstance();
            string sql = string.Format("select * from T_Adv where Deleted=0");
            List<T_Adv> AdvList = db.FindBySql<T_Adv>(sql);
            ViewBag.AdvList = AdvList;
            string sqltheme = string.Format("select * from T_Theme where Deleted=0 and ThemeType=0 order by ThemeSort");
            List<T_Theme> ThemeList = db.FindBySql<T_Theme>(sqltheme);
            ViewBag.ThemeList = ThemeList;
            List<ThemeNews> tnlist = new List<ThemeNews>();

            string sqlthemenew = string.Format("select * from T_Theme where Deleted=0 and ThemeType=1 order by ThemeSort");
            List<T_Theme> ThemeNewList = db.FindBySql<T_Theme>(sqlthemenew);
            if (ThemeNewList != null && ThemeNewList.Count > 0)
            {
                foreach (var item in ThemeNewList)
                {
                    ThemeNews tn = new ThemeNews();
                    tn.ThemeID = item.ID;
                    tn.ThemeName = item.ThemeName;
                    string sqlnews = string.Format("select top 10 * from T_News where ThemeID='{0}'", item.ID);
                    List<T_News> newslist = db.FindBySql<T_News>(sqlnews);
                    if (newslist != null && newslist.Count > 0)
                    {
                        tn.News = newslist;
                    }
                    tnlist.Add(tn);
                }
            }
            return View(tnlist);
        }

    }
}
