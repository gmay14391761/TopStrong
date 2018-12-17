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
    public class ClassifyController : Controller
    {
        //
        // GET: /Admin/Classify/

        public ActionResult Index()
        {
            DBHelper db = DBHelper.getInstance();
            string sql = string.Format("select * from T_Classify where Deleted=0");
            List<T_Classify> list = db.FindBySql<T_Classify>(sql);
            return View(list);
        }

        public ActionResult EditClassify(string id)
        {
            DBHelper db = DBHelper.getInstance();
            T_Classify na = new T_Classify();
            if (!string.IsNullOrWhiteSpace(id))
                na = db.FindOneByID<T_Classify>("T_Classify", id);
            string sql = string.Format("select * from T_Classify where Deleted=0 and Pid=0 order by Sort desc");
            List<T_Classify> classifylist = db.FindBySql<T_Classify>(sql);
            ViewBag.classifylist = classifylist;
            return View(na);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit()
        {
            string msg = "";
            DBHelper db = DBHelper.getInstance();
            string ID = Utils.SqlTextClear(Request["ID"].ToString());
            string ClassifyName = Utils.SqlTextClear(Request.Form["ClassifyName"].ToString());
            string Pid = Utils.SqlTextClear(Request.Form["Pid"].ToString());
            string Sort = Utils.SqlTextClear(Request.Form["Sort"].ToString());
            T_Classify Classify = new T_Classify();
            if (!string.IsNullOrWhiteSpace(ID))
            {
                Classify = db.FindOneByID<T_Classify>("T_Classify", ID);
            }
            else
            {
                Classify.ID = Guid.NewGuid().ToString();
                Classify.CREATEDATE = DateTime.Now;
            }
            Classify.ClassifyName = ClassifyName;
            Classify.Pid = Convert.ToInt32(Pid);
            Classify.SORT = Convert.ToInt16(Sort == "" ? "0" : Sort);
            //try
            //{
            //    db.BeginTransaction();
            //    if (!string.IsNullOrWhiteSpace(ID))
            //    {
            //        db.Update<T_Classify>(Classify);
            //    }
            //    else
            //    {
            //            int aid = db.Save<T_Classify>(Classify);
            //            string sqlsel = string.Format("select * from T_Classify where AID='{0}'", Pid);
            //            T_Classify t_c = db.FindOne<T_Classify>(sqlsel);
            //            string sqlup = string.Format("update T_Classify set PidItem={0}+{1}_ where AID='{1}'", t_c.PidItem, aid);
            //            db.ExcuteSQL(sqlup);
            //    }
            //    msg = "success";
            //    db.CommitTransaction();
            //}
            //catch
            //{
            //    db.RollbackTransaction();
            //    msg = "error";
            //}
            try
            {
                if (!string.IsNullOrWhiteSpace(ID))
                {
                    db.Update<T_Classify>(Classify);
                }
                else
                {
                    db.Save<T_Classify>(Classify);
                }
                msg = "success";
            }
            catch
            {
                msg = "error";
            }
            return Content("<script>parent.location.reload();layer_close();</script>");
        }

        public ActionResult DelClassify(string id)
        {
            DBHelper db = DBHelper.getInstance();
            try
            {
                string sqlpid = string.Format("select * from T_Classify where Deleted=0 and pid={0}", id);
                T_Classify item = db.FindOne<T_Classify>(sqlpid);
                if (item != null)
                    return Content("存在子类,不能删除!");
                string sql = string.Format("Update T_Classify set Deleted=1 where ID='{0}'", id);
                db.ExcuteSQL(sql);
                return Content("success");
            }
            catch
            {
                return Content("删除失败");
            }
        }

        public string GetClassify()
        {
            DBHelper db = DBHelper.getInstance();
            string sql = string.Format("select * from T_Classify where Deleted=0");
            List<T_Classify> list = db.FindBySql<T_Classify>(sql);
            List<ClassifyModel> listgroup = new List<ClassifyModel>();
            var resultlist = list.Where(p => p.Pid == 0).ToList();
            if (resultlist.Count > 0)
            {
                foreach (var item in resultlist)
                {
                    ClassifyModel group = new ClassifyModel();
                    group.ClassifyName = item.ClassifyName.ToString();
                    var result = from itemlist in list where itemlist.Pid.ToString() == item.AID.ToString() select itemlist;
                    List<T_Classify> newlist = result.ToList();
                    group.list = newlist;
                    listgroup.Add(group);
                }
            }
            return Utils.ConvertJson(listgroup);
        }
    }
}
