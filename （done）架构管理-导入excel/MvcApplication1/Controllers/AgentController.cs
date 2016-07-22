using MVCDemo.Models;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
namespace MVCDemo.Controllers
{
    public class AgentController : Controller
    {
        private JsonData jsondata = new JsonData();
        private MVCDemoContext db = new MVCDemoContext();
        // GET: Agent

        [OutputCache(Duration = 3600, VaryByParam = "seq")]
        public ActionResult Index(int? id = 1)
        {
            int num = 10;
            var count = db.RY.Count();
            var list = db.RY.OrderBy(m => m.seq)
                                        .Skip(num * Convert.ToInt32(id - 1))
                                        .Take(num);
            ViewBag.Current = id;
            ViewBag.Count = (count % num == 0) ?
                (count / num) : count / num + 1;
            SetPageNoCache();
            return View(list);
        }
        public ActionResult Search(string name, int? id = 1)
        {
            if (name == null)//空查询，返回Index
            {
                name = this.TempData["name"].ToString();
            }
            this.TempData["name"] = name;
            int num = 10;
            var list = from r in db.RY
                       where r.c_person.Contains(name)
                       || r.c_name.Contains(name)
                       select r;
            var count = list.Count();
            list = list.OrderBy(m => m.seq)
                                         .Skip(num * Convert.ToInt32(id - 1))
                                         .Take(num);
            ViewBag.Current = id;
            ViewBag.Count = (count % num == 0) ?
                (count / num) : count / num + 1;
            return View("Index", list);
        }
        #region 添加
        public ActionResult Add()
        {
            var area = from r in db.Hn_Area
                       select r;
            ViewBag.hnarae = area;

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(RY model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.RY.Add(model);
                    db.SaveChanges();
                    this.TempData["name"] = model.c_person;
                    return RedirectToAction("Search");
                }
            }
            catch (Exception ex)
            {
                jsondata.Success = false;
                ViewBag.msg = ex.Message + model.c_name + "该职场负责人已经存在，不允许重复添加";
                //Response.Write("<script languge='javascript'>alert('失败" + jsondata.Data + "');</script>");
            }
            return View("Error");
        }
        #endregion
        #region 编辑人员信息
        public ActionResult Edit(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var area = from r in db.Hn_Area //地区下拉列表
                       select r;
            ViewBag.hnarae = area;

            RY ry = db.RY.Find(id);
            if (ry == null)
            {
                return HttpNotFound();
            }
            ViewBag.cname = ry.c_name;
            return View(ry);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "seq,area,c_name,c_person,phone,sys,type")] RY ry)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(ry).State = EntityState.Modified;
                    db.SaveChanges();
                    this.TempData["name"] = ry.c_person;
                    return RedirectToAction("Search");
                }
            }
            catch (Exception ex)
            {
                jsondata.Success = false;
                ViewBag.msg = ex.Message + ry.c_name + "该支公司联系人已经存在，不允重复添加为该支公司联系人";
            }
            return View("Error");
        }
        #endregion
        #region 删除
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RY ry = db.RY.Find(id);
            if (ry == null)
            {
                return HttpNotFound();
            }
            return View(ry);
        }

        // POST: 一个安全漏洞， 不用使用delete 来删除某东西
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            RY ry = db.RY.Find(id);
            db.RY.Remove(ry);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        #endregion
        
        #region 下拉框联动
        public JsonResult GetBZ(string area) //获取zc表的支公司名称
        {
            try
            {
                var branch = from z in db.ZC
                             from b in db.Hn_Area
                             from l in db.RY
                             where l.area == area && l.area == b.name && z.branch_no == b.branch
                             select new
                             {
                                 bname = l.c_name
                             };
                jsondata.Success = true;
                jsondata.Data = branch.Distinct().ToArray();
            }
            catch (Exception ex) {
                jsondata.Success= false;
                jsondata.Data = ex.Message;
            }
            return Json(jsondata, JsonRequestBehavior.AllowGet);

        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        //禁止使用浏览器cache
        private void SetPageNoCache()
        {

            Response.Buffer = true;

            Response.ExpiresAbsolute = System.DateTime.Now.AddSeconds(-1);

            Response.Expires = 0;

            Response.CacheControl = "no-cache";

            Response.AppendHeader("Pragma", "No-Cache");

        }
    }
}