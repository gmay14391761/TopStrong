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
    public class ThemeController : Controller
    {
        //
        // GET: /Theme/

        public ActionResult Index()
        {
            string ThemeID = Request["ThemeID"] == null ? "" : Utils.SqlTextClear(Request["ThemeID"]);
            if (ThemeID == "")
                return Content("<script>alert('参数错误!');location.href='/Index'</script>");
            string ThemeName = Request["ThemeName"] == null ? "" : Utils.SqlTextClear(Request["ThemeName"]);
            DBHelper db = DBHelper.getInstance();
            string sql = string.Format("select * from T_Adv where Deleted=0");
            List<T_Adv> AdvList = db.FindBySql<T_Adv>(sql);
            ViewBag.AdvList = AdvList;

            sql = string.Format("select * from T_News where ThemeID='{0}'", ThemeID);
            List<T_News> ThemeList = db.FindBySql<T_News>(sql);
            ViewBag.ThemeName = ThemeName;
            return View(ThemeList);
        }

        public string GetThemeNews()
        {
            string page = Utils.SqlTextClear(Request["page"]);
            string tid = Utils.SqlTextClear(Request["tid"]);
            string type = Utils.SqlTextClear(Request["type"]);
            if (string.IsNullOrWhiteSpace(page))
                page = "1";
            JsonList<T_News> json = new JsonList<T_News>();
            string[] pages = Utils.GetPagingVal(page);
            DBHelper db = DBHelper.getInstance();
            string orderstr = string.Format(" order by CreateDate desc");
            if (type == "1")
            {
                orderstr = string.Format(" order by NewsClickNum desc");
            }
            json.page = Convert.ToInt32(page) + 1;
            int toppage = 10 * (Convert.ToInt32(page) - 1);
            string notinstr = "";
            if (toppage > 0)
                notinstr = string.Format(" and ID not in (select top {1} ID from T_News where ThemeID='{0}' {1})", tid, toppage, orderstr);
            string sql = string.Format("select * from (select top 10 * from T_News where ThemeID='{0}' {1} {2}) a ", tid, notinstr, orderstr);
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
    }
}
