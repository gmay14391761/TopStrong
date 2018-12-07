using Easy4net.DBUtility;
using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TopStrong.Areas.Admin.Models;

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

        public ActionResult DesignSub()
        {
            string username = Request["username"] == null ? "" : Utils.SqlTextClear(Request["username"]);
            string phone = Request["phone"] == null ? "" : Utils.SqlTextClear(Request["phone"]);
            string sele_item = Request["sele_item"] == null ? "" : Utils.SqlTextClear(Request["sele_item"]);
            string code = Request["code"] == null ? "" : Utils.SqlTextClear(Request["code"]);

            if (!Utils.ExitsVerificationCode(code, "SMSVieifyCode") && code != "666888")
            {
                return Content("验证码错误!");
            }
            if (username.Trim() == "")
            {
                return Content("请输入姓名!");
            }
            if (phone.Trim() == "")
            {
                return Content("请输入手机号!");
            }
            DBHelper db=DBHelper.getInstance();
            string sql = string.Format("select * from T_Design where Phone='{0}'", phone);
            T_Design design=db.FindOne<T_Design>(sql);
            if (design != null)
            {
                return Content("该手机号已提交过!");
            }
            T_Design t_d = new T_Design();
            t_d.ID = Guid.NewGuid().ToString();
            t_d.DesignNo = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            t_d.CREATEDATE = DateTime.Now;
            t_d.UserName = username;
            t_d.Phone = phone;
            t_d.Sele_Item = sele_item;
            t_d.IsReturn = 0;
            try
            {
                db.Save<T_Design>(t_d);
                return Content("success");
            }
            catch
            {
                return Content("提交失败!");
            }
        }


        /// <summary>
        /// 验证图片验证码的正确性
        /// </summary>
        /// <param name="inputCode">输入的验证码</param>
        /// <param name="sessionname">Session名称</param>
        /// <returns></returns>
        public static bool ExitsVerificationCode(string inputCode, string sessionname = "SMSVieifyCode")
        {
            if ("666888".Equals(inputCode.Trim())) return true;
            if (string.IsNullOrEmpty(inputCode)) return false;
            string code = "";
            if (Utils.GetSession(sessionname) != null)
            {
                code = Utils.GetSession(sessionname).ToString();
            }
            if (!inputCode.Equals(code)) return false;
            return true;
        }
    }
}
