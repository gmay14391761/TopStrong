using Easy4net.Common;
using Easy4net.DBUtility;
using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TopStrong.Areas.Admin.Models;

namespace TopStrong.Areas.Admin.Controllers
{
    public class ThemeController : Controller
    {
        //
        // GET: /Admin/Theme/

        public ActionResult Index(string keyword = "", string seladvtype = "", int page = 1, int limit = 10)
        {
            DBHelper db = DBHelper.getInstance();
            string strWhere = "";
            keyword = Utils.SqlTextClear(keyword);
            if (keyword != "")
                strWhere = string.Format(" and (ThemeName like '%{0}%')", keyword);
            string sql = string.Format("select * from T_Theme where Deleted=0 {0}", strWhere);
            ParamMap param = ParamMap.newMap();
            param.setPageParamters(page, limit);
            param.setOrderFields("CreateDate", true);
            PageResult<T_Theme> pageResult = db.FindPage<T_Theme>(sql, param);
            if (pageResult != null)
            {
                if (pageResult.Total > 0)
                {
                    pageResult.Total = pageResult.Total <= limit ? 1 : (pageResult.Total % limit == 0 ? pageResult.Total / limit : pageResult.Total / limit + 1);
                }
            }
            ViewBag.key = keyword;
            return View(pageResult);
        }


        public ActionResult EditTheme(string id)
        {
            DBHelper db = DBHelper.getInstance();
            T_Theme na = new T_Theme();
            if (!string.IsNullOrWhiteSpace(id))
                na = db.FindOneByID<T_Theme>("T_Theme", id);
            return View(na);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit()
        {
            string msg = "";
            DBHelper db = DBHelper.getInstance();
            string ID = Utils.SqlTextClear(Request["ID"].ToString());
            string ThemeName = Utils.SqlTextClear(Request.Form["ThemeName"].ToString());
            string ThemeImg = Utils.SqlTextClear(Request.Form["ThemeImg"].ToString());
            string ThemeType = Utils.SqlTextClear(Request.Form["ThemeType"].ToString());
            string ThemeSort = Utils.SqlTextClear(Request.Form["ThemeSort"].ToString());
            T_Theme theme = new T_Theme();
            if (!string.IsNullOrWhiteSpace(ID))
            {
                theme = db.FindOneByID<T_Theme>("T_Adv", ID);
            }
            else
            {
                theme.CREATEDATE = DateTime.Now;
            }
            theme.ThemeName = ThemeName;
            theme.ThemeImg = ThemeImg;
            theme.ThemeType = Convert.ToInt32(ThemeType);
            theme.ThemeSort = Convert.ToInt16(ThemeSort == "" ? "0" : ThemeSort);
            try
            {
                if (!string.IsNullOrWhiteSpace(ID))
                {
                    db.Update<T_Theme>(theme);
                }
                else
                {
                    db.Save<T_Theme>(theme);
                }
                msg = "success";
            }
            catch
            {
                msg = "error";
            }
            return RedirectToAction("index", "Theme", new { res = msg });
        }

        public ActionResult DelTheme(string id)
        {
            DBHelper db = DBHelper.getInstance();
            try
            {
                string sql = string.Format("Update T_Theme set Deleted=1 where ID='{0}'", id);
                db.ExcuteSQL(sql);
                return Content("1");
            }
            catch
            {
                return Content("0");
            }
        }

        public ActionResult DelMoreTheme(string ids)
        {
            string _ids = ids;
            string[] idslist = ids.Split(',');
            if (idslist.Length > 0)
            {
                string tmpids = "";
                foreach (string item in idslist)
                {
                    if (item != "")
                        tmpids += "'" + item + "'" + ",";
                }
                if (tmpids != "")
                {
                    DBHelper db = DBHelper.getInstance();
                    try
                    {
                        string sql = string.Format("Update T_Theme set Deleted=1 where ID in ({0})", tmpids.Substring(0, tmpids.Length - 1));
                        db.ExcuteSQL(sql);
                        return Content("1");
                    }
                    catch
                    {
                        return Content("0");
                    }
                }
            }
            return Content("0");
        }
    }
}
