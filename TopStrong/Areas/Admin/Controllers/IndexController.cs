using Easy4net.DBUtility;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TopStrong.Areas.Admin.Models;

namespace TopStrong.Areas.Admin.Controllers
{
    public class IndexController : Controller
    {
        //
        // GET: /Admin/Index/

        public ActionResult Index()
        {
            try
            {
                string uid = Utils.GetAdminData("ZSS_UserLoginUserID");
                if (!string.IsNullOrWhiteSpace(uid))
                {
                    string sql = string.Format("select * from T_User where deleted=0 and status=0 and Id='{0}'", uid);
                    DBHelper db = DBHelper.getInstance();
                    T_User admin = db.FindOne<T_User>(sql);
                    if (admin != null)
                    {
                        admin.LastLoginDate = DateTime.Now;
                        admin.LastLoginIp = Utils.GetIP();
                        ViewBag.lastlogindate = admin.LastLoginDate;
                        ViewBag.LoginCount = admin.LoginCount;
                        db.Update<T_User>(admin);
                        admin = new T_User();//操作完成清除用户数据,以防被盗取
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
            return View();
        }

    }
}
