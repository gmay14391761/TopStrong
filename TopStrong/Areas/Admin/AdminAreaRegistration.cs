using System.Web.Mvc;

namespace TopStrong.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Admin_default",
                "Admin/{controller}/{action}/{id}",
                new { controller = "Iframe", action = "Index", id = UrlParameter.Optional },
                new string[] { "TopStrong.Areas.Admin.Controllers" }
            );
        }
    }
}
