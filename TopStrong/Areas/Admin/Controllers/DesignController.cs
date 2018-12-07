using Easy4net.Common;
using Easy4net.DBUtility;
using Entity;
using Model.NewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TopStrong.Areas.Admin.Models;

namespace TopStrong.Areas.Admin.Controllers
{
    public class DesignController : Controller
    {
        //
        // GET: /Admin/Design/

        public ActionResult Index(string keyword = "",string type="0", int page = 1, int limit = 10)
        {
            DBHelper db = DBHelper.getInstance();
            string strWhere = "";
            keyword = Utils.SqlTextClear(keyword);
            if (keyword != "")
                strWhere = string.Format(" and (UserName like '%{0}%' or Phone like '%{0}%' or Sele_Item like '%{0}%')", keyword);
            if (type == "0")
                strWhere += string.Format(" and IsReturn=0");
            else
                strWhere += string.Format(" and IsReturn=1");
            string sql = string.Format("select * from T_Design where Deleted=0 {0}", strWhere);
            ParamMap param = ParamMap.newMap();
            param.setPageParamters(page, limit);
            param.setOrderFields("Createdate", true);
            PageResult<DesignDetail> pageResult = db.FindPage<DesignDetail>(sql, param);
            if (pageResult != null)
            {
                if (pageResult.Total > 0)
                {
                    pageResult.Total = pageResult.Total <= limit ? 1 : (pageResult.Total % limit == 0 ? pageResult.Total / limit : pageResult.Total / limit + 1);
                }
            }
            ViewBag.key = keyword;
            ViewBag.type = type;
            return View(pageResult);
        }

        public ActionResult DelDesign(string id)
        {
            DBHelper db = DBHelper.getInstance();
            try
            {
                string sql = string.Format("Update T_Design set Deleted=1 where ID='{0}'", id);
                db.ExcuteSQL(sql);
                return Content("1");
            }
            catch
            {
                return Content("0");
            }
        }

        public ActionResult ReturnDesign(string id)
        {
            DBHelper db = DBHelper.getInstance();
            try
            {
                string sql = string.Format("Update T_Design set IsReturn=1 where ID='{0}'", id);
                db.ExcuteSQL(sql);
                return Content("1");
            }
            catch
            {
                return Content("0");
            }
        }


        public ActionResult DelMoreDesign(string ids)
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
                        string sql = string.Format("Update T_Design set Deleted=1 where ID in ({0})", tmpids.Substring(0, tmpids.Length - 1));
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
