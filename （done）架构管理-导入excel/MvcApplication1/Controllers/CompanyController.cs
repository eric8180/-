using MVCDemo.Models;
using MVCDemo.ViewModel;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace MVCDemo.Controllers
{
    public class CompanyController : Controller
    {
        private JsonData jsondata = new JsonData();
        private MVCDemoContext db = new MVCDemoContext();
        // GET: Company
       [OutputCache(Duration = 3600, VaryByParam = "seq")]
        public ActionResult Index(int? id = 1)
        {
            int num = 10;
            var count = db.ZC.Count();
            var list = from l in db.ZC
                       from h in db.Hn_Area
                       where l.branch_no == h.branch
                       select new Workplace 
                       {
                           seq = l.seq,
                           name = h.name,
                           branch_no=l.branch_no,
                           zyh_name = l.zyh_name,
                           zcname = l.zcname,
                           zc_no = l.zc_no,
                           sys=l.sys,
                       };
            list = list.OrderBy(m => m.seq)
                                         .Skip(num * Convert.ToInt32(id - 1))
                                         .Take(num);
            ViewBag.Current = id;
            ViewBag.Count = (count % num == 0) ?
                (count / num) : count / num + 1;
            SetPageNoCache();
            return View(list);
        }

       public ActionResult Search(string name,int? id=1)
       {

           if (name == null)//空查询，返回Index
           {
               name = this.TempData["name"].ToString();
           }
           this.TempData["name"] = name;
           int num = 10;
           var list = from l in db.ZC
                      from h in db.Hn_Area
                      where l.branch_no == h.branch
                      && (l.zcname.Contains(name) || l.zyh_name.Contains(name))
                      select new Workplace
                      {
                          seq = l.seq,
                          name = h.name,
                          branch_no = l.branch_no,
                          zyh_name = l.zyh_name,
                          zcname = l.zcname,
                          zc_no = l.zc_no,
                          sys=l.sys,
                      };
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
           SetPageNoCache();
           return View();
       }
        
       [HttpPost]
       [ValidateAntiForgeryToken]
       public ActionResult Add(ZC model)
       {
           try
           {
               if (ModelState.IsValid)
               {
                   db.ZC.Add(model);
                   db.SaveChanges();
                   this.TempData["name"] = model.zcname;
                   return RedirectToAction("Search");
               }
           }
           catch (Exception ex)
           {
               jsondata.Success = false;
               ViewBag.msg = ex.Message + model.zcname + "该职场已经存在，不允许重复添加";
       //        Response.Write("<script languge='javascript'>alert('失败\\n"+jsondata.Data+"');</script>");
           }
           return View("Error");
          // return Json(jsondata, JsonRequestBehavior.AllowGet);
       }
       #endregion






       #region 删除
       public ActionResult Delete(int? id)
       {
           if (id == null)
           {
               return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
           }
           ZC zc = db.ZC.Find(id);
           if (zc == null)
           {
               return HttpNotFound();
           }
           return View(zc);
       }
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

           ZC zc = db.ZC.Find(id);
           ViewBag.cname = zc.zyh_name;
           if (zc == null)
           {
               return HttpNotFound();
           }
           return View(zc);
       }
       [HttpPost]
       [ValidateAntiForgeryToken]
       public ActionResult Edit([Bind(Include = "seq,branch_no,zyh_name,zcname,zc_no,sys")] ZC zc)
       {
           try
           {
               if (ModelState.IsValid)
               {
                   db.Entry(zc).State = EntityState.Modified;
                   db.SaveChanges();
                   this.TempData["name"] = zc.zcname;
                   return RedirectToAction("Search");
               }
           }
           catch (Exception ex)
           {
               jsondata.Success = false;
               ViewBag.msg = ex.Message + zc.zcname + "该职场已经存在，不允许重复添加";
           }
           return View("Error");
       }
       #endregion
       // POST: 一个安全漏洞， 不用使用delete 来删除某东西
       [HttpPost, ActionName("Delete")]
       [ValidateAntiForgeryToken]
       public ActionResult DeleteConfirmed(int id)
       {
           ZC zc = db.ZC.Find(id);
           db.ZC.Remove(zc);
           db.SaveChanges();
           return RedirectToAction("Index");
       }
       #endregion


       #region 地区和支公司下拉框联动
       public JsonResult GetBZ(string branch_no) //职场添加页面的支公司从人员表中获取
       {
           try
           {
               var branch = from b in db.Hn_Area
                            from l in db.RY
                            where l.area == b.name && b.branch == branch_no
                            select new
                            {
                                bname = l.c_name
                            };
               jsondata.Success = true;
               jsondata.Data = branch.Distinct().ToArray();
           }
           catch (Exception ex)
           {
               jsondata.Success = false;
               jsondata.Data = ex.Message;
           }
           return Json(jsondata, JsonRequestBehavior.AllowGet);

       }
       #endregion
       #region 地区和职场
       public JsonResult GetZC(string branch_no) //根据地区编号获取职场
       {
           try
           {
               var zc_no = (from z in db.ZC
                            where z.branch_no == branch_no
                            select new
                            {
                                zcno = z.zc_no
                            }).First().ToString();
               //zc_no=zc_no.ToString();
               zc_no = zc_no.Substring(9, 4);//取4位地市标志
               var zcn = from zc in db.ZClist
                         where zc.bid.Substring(0, 4) == zc_no//取4位地市标志
                         select new
                         {
                             zname = zc.zcname,
                             zcno = zc.zcno
                         };

               jsondata.Success = true;
               jsondata.Data = zcn.ToArray();
           }
           catch (Exception ex)
           {
               jsondata.Success = false;
               jsondata.Data = ex.Message;
           }
           return Json(jsondata, JsonRequestBehavior.AllowGet);

       }
       #endregion
       #region 选择职场确定职场编号
       public JsonResult GetBH(string zcname) //GetBZ对应View的GetBZ，orgID也是通过View可以获取参数值
       {
           try
           {
               var zc_no = (from z in db.ZClist
                            where z.zcname == zcname
                            select new
                            {
                                zcno = z.zcno,
                                sys=z.sys
                            });
               jsondata.Success = true;
               jsondata.Data = zc_no.Distinct().ToArray();
           }
           catch (Exception ex)
           {
               jsondata.Success = false;
               jsondata.Data = ex.Message;
           }
           return Json(jsondata, JsonRequestBehavior.AllowGet);

       }
       #endregion

       #region 支公司和职场下拉框联动
       //public JsonResult GetZC(string zyh_name) //GetBZ对应View的GetBZ，orgID也是通过View可以获取参数值
       //{
       //    try
       //    {
       //        var zc_no = (from z in db.ZC
       //                     where z.zyh_name == zyh_name
       //                     select new
       //                     {
       //                         zcno = z.zc_no
       //                     }).First().ToString();
       //        //zc_no=zc_no.ToString();
       //        zc_no=zc_no.Substring(9,6);
       //        var zcn= from zc in db.ZClist
       //                where zc.bid==zc_no
       //                select new
       //                {
       //                    zname=zc.zcname,
       //                    zcno=zc.zcno
       //                };

       //        jsondata.Success = true;
       //        jsondata.Data = zcn.ToArray();
       //    }
       //    catch (Exception ex)
       //    {
       //        jsondata.Success = false;
       //        jsondata.Data = ex.Message;
       //    }
       //    return Json(jsondata, JsonRequestBehavior.AllowGet);

       //}
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