using Easy4net.DBUtility;
using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TopStrong.Areas.Admin.Models;
using TopStrong.Models;

namespace TopStrong.Controllers
{
    public class NewsController : Controller
    {
        //
        // GET: /News/

        public ActionResult Index(string ThemeID)
        {
            return View();
        }

        public ActionResult NewsSearch()
        {
            string ThemeID = Request["ThemeID"] == null ? "" : Utils.SqlTextClear(Request["ThemeID"]);
            if (ThemeID == "")
                return Content("<script>alert('参数错误!');history.go(-1);</script>");
            string ThemeName = Request["ThemeName"] == null ? "" : Request["ThemeName"];
            ViewBag.ThemeName = ThemeName;
            ViewBag.ThemeID = ThemeID;
            return View();
        }

        public string GetThemeNews()
        {
            string page = Utils.SqlTextClear(Request["page"]);
            string tid = Utils.SqlTextClear(Request["tid"]);
            string keyword = Utils.SqlTextClear(Request["keyword"]);

            if (string.IsNullOrWhiteSpace(page))
                page = "1";
            JsonList<T_News> json = new JsonList<T_News>();
            string[] pages = Utils.GetPagingVal(page);
            DBHelper db = DBHelper.getInstance();
            if (keyword.Trim() != "")
            {
                keyword = string.Format(" and NewsTitle like '%{0}%'", keyword);
            }
            json.page = Convert.ToInt32(page) + 1;
            int toppage = 10 * (Convert.ToInt32(page) - 1);
            string notinstr = "";
            if (toppage > 0)
                notinstr = string.Format(" and ID not in (select top {1} ID from T_News where ThemeID='{0}' {1})", tid, toppage, keyword);
            string sql = string.Format("select * from (select top 10 * from T_News where ThemeID='{0}' {1} {2}) a ", tid, notinstr, keyword);
            List<T_News> list = db.FindBySql<T_News>(sql);
            if (list != null && list.Count > 0)
            {
                json.status = 1000;
                json.msg = "数据加载成功";
                json.list = list;
                return Utils.ConvertJson(json);
            }
            else
            {
                json.status = 1002;
                json.msg = "没有更多数据了";
                return Utils.ConvertJson(json);
            }
        }

        public ActionResult NewsDetail()
        {
            string id = Request["ID"] == null ? "" : Utils.SqlTextClear(Request["ID"]);
            if (id == "")
                return Content("<script>alert('参数错误!');history.go(-1);</script>");
            string ThemeName = Request["ThemeName"] == null ? "" : Utils.SqlTextClear(Request["ThemeName"]);
            DBHelper db = DBHelper.getInstance();

            string sqlup = string.Format("update T_News set NewsClickNum=NewsClickNum+1 where ID='{0}'", id);
            db.ExcuteSQL(sqlup);

            string sql = string.Format("select * from T_News where Deleted=0 and ID='{0}'", id);
            T_News news = db.FindOne<T_News>(sql);
            if (news == null)
                news = new T_News();
            ViewBag.ThemeName = ThemeName;
            return View(news);
        }

        public ActionResult NewsList()
        {
            string ThemeID = Request["ThemeID"] == null ? "" : Utils.SqlTextClear(Request["ThemeID"]);
            if (ThemeID == "")
                return Content("<script>alert('参数错误!');history.go(-1);</script>");
            string ThemeName = Request["ThemeName"] == null ? "" : Request["ThemeName"];
            ViewBag.ThemeName = ThemeName;
            ViewBag.ThemeID = ThemeID;
            return View();
        }
    }
}
