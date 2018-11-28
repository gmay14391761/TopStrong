using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using TopStrong.Areas.Admin.Models;

namespace TopStrong.Areas.Admin.Controllers
{
    [AdminLoginAuthorize]
    public class FileUploadController : Controller
    {
        //
        // GET: /Admin/FileUpload/

        public ActionResult Index()
        {
            try
            {
                HttpPostedFileBase file = Request.Files["file"];
                //string userid = Utils.GetCookie("C_ID", false);
                //if (string.IsNullOrWhiteSpace(userid))
                //{
                //    return Content("你不满足使用该功能的权限");
                //}
                if (file.ContentLength > 0 && file.ContentLength / 1024 < 5000)
                {
                    string savePath = Server.MapPath("~/Upload");
                    string backname = "";
                    if (file.FileName.Contains(".jpg") || file.FileName.Contains(".jpeg"))
                    {
                        backname = ".jpg";
                    }
                    else if (file.FileName.Contains(".png"))
                    {
                        backname = ".png";
                    }
                    else if (file.FileName.Contains(".gif"))
                    {
                        backname = ".gif";
                    }
                    else if (file.FileName.Contains(".bmp"))
                    {
                        backname = ".bmp";
                    }
                    else
                    {
                        return Content("您选择的图片格式有误,请检查");
                    }

                    string time = DateTime.Now.ToString("yyyyMMddHHmmssfff") + backname;//获取当前系统时间
                    string failPath = savePath + "\\" + time;
                    WebImage thumbImage = new WebImage(file.InputStream);
                    Utils.Picresize(thumbImage);
                    thumbImage.Save(failPath);
                    return Content(time);
                }
                else
                    return Content("文件大小不符合限制");
            }
            catch (Exception)
            {
                return Content("超时");
            }
        }

    }
}
