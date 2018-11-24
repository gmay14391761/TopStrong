using Easy4net.Common;
using Easy4net.DBUtility;
using Entity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TopStrong.Areas.Admin.Models;

namespace TopStrong.Areas.Admin.Controllers
{
    [AdminLoginAuthorize]
    public class AdvController : Controller
    {
        //
        // GET: /Admin/Adv/

        public ActionResult Index(string keyword = "", string seladvtype="", int page = 1, int limit = 10)
        {

            DBHelper db = DBHelper.getInstance();
            string strWhere = "";
            keyword = Utils.SqlTextClear(keyword);
            if (keyword != "")
                strWhere = string.Format(" and (AdvTitle like '%{0}%')", keyword);
            if (seladvtype != "")
                strWhere = string.Format(" and (AdvType={0})", seladvtype);
            string sql = string.Format("select * from T_Adv where Deleted=0 {0}", strWhere);
            ParamMap param = ParamMap.newMap();
            param.setPageParamters(page, limit);
            param.setOrderFields("CreateDate", true);
            PageResult<T_Adv> pageResult = db.FindPage<T_Adv>(sql, param);
            if (pageResult != null)
            {
                if (pageResult.Total > 0)
                {
                    pageResult.Total = pageResult.Total <= limit ? 1 : (pageResult.Total % limit == 0 ? pageResult.Total / limit : pageResult.Total / limit + 1);
                }
            }
            ViewBag.key = keyword;
            ViewBag.seladvtype = seladvtype;
            return View(pageResult);
        }


        public ActionResult EditAdv(string id)
        {
            DBHelper db = DBHelper.getInstance();
            T_Adv na = new T_Adv();
            if (!string.IsNullOrWhiteSpace(id))
                na = db.FindOneByID<T_Adv>("T_Adv", id);
            return View(na);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit()
        {
            string msg = "";
            DBHelper db = DBHelper.getInstance();
            string ID = Utils.SqlTextClear(Request["ID"].ToString());
            string AdvTitle = Utils.SqlTextClear(Request.Form["AdvTitle"].ToString());
            string AdvImg = Utils.SqlTextClear(Request.Form["AdvImg"].ToString());
            string AdvType = Utils.SqlTextClear(Request.Form["AdvType"].ToString());
            string AdvContent = Utils.SqlTextClear(Request.Form["txtcontent"].ToString());
            string Sort = Utils.SqlTextClear(Request.Form["Sort"].ToString());
            T_Adv adv = new T_Adv();
            if (!string.IsNullOrWhiteSpace(ID))
            {
                adv = db.FindOneByID<T_Adv>("T_Adv", ID);
            }
            else
            {
                adv.CREATEDATE = DateTime.Now;
            }
            adv.AdvTitle = AdvTitle;
            adv.AdvImg = AdvImg;
            adv.AdvType = Convert.ToInt32(AdvType);
            adv.SORT = Convert.ToInt16(Sort == "" ? "0" : Sort);
            adv.AdvDetail = AdvContent;
            try
            {
                if (!string.IsNullOrWhiteSpace(ID))
                {
                    db.Update<T_Adv>(adv);
                }
                else
                {
                    db.Save<T_Adv>(adv);
                }
                msg = "success";
            }
            catch
            {
                msg = "error";
            }
            return RedirectToAction("index", "Adv", new { res = msg });
        }

        public ActionResult DelAdv(string id)
        {
            DBHelper db = DBHelper.getInstance();
            try
            {
                string sql = string.Format("Update T_Adv set Deleted=1 where ID='{0}'", id);
                db.ExcuteSQL(sql);
                return Content("1");
            }
            catch
            {
                return Content("0");
            }
        }

        public ActionResult DelMoreAdv(string ids)
        {
            string _ids = ids;
            string[] idslist = ids.Split(',');
            if (idslist.Length > 0)
            {
                string tmpids = "";
                foreach (string item in idslist)
                {
                    if(item!="")
                        tmpids += "'" + item + "'"+",";
                }
                if (tmpids != "")
                {
                    DBHelper db = DBHelper.getInstance();
                    try
                    {
                        string sql = string.Format("Update T_Adv set Deleted=1 where ID in ({0})", tmpids.Substring(0,tmpids.Length-1));
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

        /// <summary>
        /// banner选择单个广告图
        /// </summary>
        /// <returns></returns>
        public ActionResult FindOneAdv(string where = "", int page = 1)
        {
            int pageStart = page;
            pageStart = (pageStart - 1) * 12;
            int pageEnd = pageStart + 12;

            where = string.IsNullOrEmpty(where) ? "1=1" : string.Format("(AdvTitle like '%{0}%')", where);

            DBHelper dbHelper = DBHelper.getInstance();
            List<T_Adv> list = new List<T_Adv>();
            string sql = string.Format(@"select * from 
(select row_number()over(order by CREATEDATE desc)rownumber,* from T_Adv where {2})a
where rownumber>{0} and rownumber<={1}", pageStart, pageEnd, where);

            list = dbHelper.FindBySql<T_Adv>(sql);
            return View(list);
        }

        [HttpGet]
        public ActionResult FindOne(string id)
        {
            DBHelper dbHelper = DBHelper.getInstance();
            T_Adv pro = new T_Adv();
            string sql = string.Format("select top 1 * from T_Adv where id='{0}'", id);
            pro = dbHelper.FindOne<T_Adv>(sql);
            return Content(JsonConvert.SerializeObject(pro));
        }

        /// <summary>
        /// tag多选广告窗口的分页
        /// </summary>
        /// <returns></returns>
        public ActionResult GetMorePageNumber(string where)
        {
            where = string.IsNullOrEmpty(where) ? "1=1" : string.Format("(AdvTitle like '%{0}%')", where);
            DBHelper dbHelper = DBHelper.getInstance();
            int dataNum = 0, pageNum = 0;
            string sql = string.Format("select count(AID) from T_Adv where {0}", where);
            dataNum = dbHelper.Count(sql);
            Dictionary<string, int> pages = new Dictionary<string, int>();
            double temA = 0.0;
            temA = dataNum / 12.0;
            int temB = Convert.ToInt32(temA);
            pageNum = temA > temB ? temB + 1 : temB;
            pages.Add("dataNum", dataNum);
            pages.Add("pageNum", pageNum);
            return Content(JsonConvert.SerializeObject(pages));
        }
    }
}
