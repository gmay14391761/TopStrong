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
    public class LoginController : Controller
    {
        //
        // GET: /Admin/Login/

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignIn()
        {
            string res = "失败";
            if (ModelState.IsValid)
            {
                //这里执行登录操作
                string name = Utils.SqlTextClear(Request["username"]);
                string pass = Utils.SqlTextClear(Request["password"]);
                string online = Utils.SqlTextClear(Request["online"]);//是否永久登录

                if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(pass))
                    res = "请输入登录名和密码";
                else
                {
                    string sql = string.Format("select * from T_User where UserName='{0}' and Pwd='{1}'", name, Utils.SHA1Encrypt(pass));
                    DBHelper db = DBHelper.getInstance();
                    T_User admin = db.FindOne<T_User>(sql);
                    if (admin == null)
                        res = "登录失败,请检查登录名和密码";
                    else
                    {
                        if (admin.Deleted == 1)
                            res = "该账号已被系统删除,如有疑问请咨询管理员";
                        else if (admin.Status == 1)
                            res = "该账号已被系统禁止登录,如有疑问请咨询管理员";
                        else
                        {
                            if (online == null)
                            {
                                Utils.SetSession("ZSS_UserLoginUserID", Utils.Encryption(admin.ID));
                                Utils.SetSession("ZSS_UserLoginName", admin.UserName);
                            }
                            else
                            {
                                Utils.SetCookie("ZSS_UserLoginUserID", Utils.Encryption(admin.ID));
                                Utils.SetCookie("ZSS_UserLoginName", admin.UserName);
                            }

                            admin.LoginCount = admin.LoginCount + 1;
                            db.Update<T_User>(admin);
                            return Content("<script>alert('欢迎回来,尊敬的" + admin.UserName + "');location.href='/Admin/Iframe';</script>");
                        }
                    }
                }
            }
            else
            {
                res = "非法请求";
            }
            return Content("<script>alert('" + res + "');location.href='/Admin/Login'</script>");

        }


        public ActionResult Logout()
        {
            Utils.ClearAdminLoginSession();
            return RedirectToAction("/Admin");
        }
    }
}
