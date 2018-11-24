using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace TopStrong.Areas.Admin.Models
{
    public class AdminLoginAuthorizeAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (!Utils.ExitsAdminLoginSession())
            {
                //前一页面
                string leftUrl = filterContext.HttpContext.Request.Url.ToString();
                if (!string.IsNullOrEmpty(leftUrl))
                {
                    leftUrl = filterContext.HttpContext.Server.UrlEncode(leftUrl);
                }
                //页面跳转到 登录页面
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login", action = "Index", return_url = leftUrl }));
                return;
            }
            //通过验证
            return;
        }
    }
}