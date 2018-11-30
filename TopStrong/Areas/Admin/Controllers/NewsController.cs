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
    public class NewsController : Controller
    {
        //
        // GET: /Admin/News/

        public ActionResult Index(string keyword = "", string selNewstype = "", int page = 1, int limit = 10)
        {
            DBHelper db = DBHelper.getInstance();
            string strWhere = "";
            keyword = Utils.SqlTextClear(keyword);
            if (keyword != "")
                strWhere = string.Format(" and (NewsTitle like '%{0}%')", keyword);
            if (selNewstype != "")
                strWhere = string.Format(" and (NewsType={0})", selNewstype);
            string sql = string.Format("select * from T_News where Deleted=0 {0}", strWhere);
            ParamMap param = ParamMap.newMap();
            param.setPageParamters(page, limit);
            param.setOrderFields("CreateDate", true);
            PageResult<T_News> pageResult = db.FindPage<T_News>(sql, param);
            if (pageResult != null)
            {
                if (pageResult.Total > 0)
                {
                    pageResult.Total = pageResult.Total <= limit ? 1 : (pageResult.Total % limit == 0 ? pageResult.Total / limit : pageResult.Total / limit + 1);
                }
            }
            ViewBag.key = keyword;
            ViewBag.selNewstype = selNewstype;
            return View(pageResult);
        }


        public ActionResult EditNews(string id)
        {
            DBHelper db = DBHelper.getInstance();
            T_News na = new T_News();
            if (!string.IsNullOrWhiteSpace(id))
                na = db.FindOneByID<T_News>("T_News", id);
            string sql = string.Format("select * from T_Theme where Deleted=0");
            List<T_Theme> themeList = db.FindBySql<T_Theme>(sql);
            ViewBag.themeList = themeList;
            return View(na);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit()
        {
            string msg = "";
            DBHelper db = DBHelper.getInstance();
            string ID = Utils.SqlTextClear(Request["ID"].ToString());
            string NewsTitle = Utils.SqlTextClear(Request.Form["NewsTitle"].ToString());
            string NewsImg = Utils.SqlTextClear(Request.Form["NewsImg"].ToString());
            string NewsContent = Utils.SqlTextClear(Request.Form["txtcontent"].ToString());
            string Sort = Utils.SqlTextClear(Request.Form["Sort"].ToString());
            T_News News = new T_News();
            if (!string.IsNullOrWhiteSpace(ID))
            {
                News = db.FindOneByID<T_News>("T_News", ID);
            }
            else
            {
                News.ID = Guid.NewGuid().ToString();
                News.CREATEDATE = DateTime.Now;
            }
            News.NewsTitle = NewsTitle;
            News.NewsImg = NewsImg;
            News.SORT = Convert.ToInt16(Sort == "" ? "0" : Sort);
            News.NewsDetail = NewsContent;
            try
            {
                if (!string.IsNullOrWhiteSpace(ID))
                {
                    db.Update<T_News>(News);
                }
                else
                {
                    db.Save<T_News>(News);
                }
                msg = "success";
            }
            catch
            {
                msg = "error";
            }
            return RedirectToAction("index", "News", new { res = msg });
        }

        public ActionResult DelNews(string id)
        {
            DBHelper db = DBHelper.getInstance();
            try
            {
                string sql = string.Format("Update T_News set Deleted=1 where ID='{0}'", id);
                db.ExcuteSQL(sql);
                return Content("1");
            }
            catch
            {
                return Content("0");
            }
        }

        public ActionResult DelMoreNews(string ids)
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
                        string sql = string.Format("Update T_News set Deleted=1 where ID in ({0})", tmpids.Substring(0, tmpids.Length - 1));
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
