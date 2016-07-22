using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVCDemo.Models;
using System.Data.OleDb;
using System.Text.RegularExpressions;
using System.Data.SqlClient;

namespace MvcApplication1.Controllers
{
    public class AddClientController : Controller
    {
        private MVCDemoContext db = new MVCDemoContext();

        //
        // GET: /AddClient/

        public ActionResult Index()
        {
            return View(db.Client.ToList());
        }

        //
        // GET: /AddClient/Details/5

        public ActionResult Details(string id = null)
        {
            Client client = db.Client.Find(id);
            if (client == null)
            {
                return HttpNotFound();
            }
            return View(client);
        }

        //
        // GET: /AddClient/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /AddClient/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Client client)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Client.Add(client);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch(Exception)
            {
                return View("KeyError");
            }


            return View(client);
        }

        //
        // GET: /AddClient/Edit/5

        public ActionResult Edit(string id = null)
        {
            Client client = db.Client.Find(id);
            if (client == null)
            {
                return HttpNotFound();
            }
            return View(client);
        }

        //
        // POST: /AddClient/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Client client)
        {
            if (ModelState.IsValid)
            {
                db.Entry(client).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(client);
        }

        //
        // GET: /AddClient/Delete/5

        public ActionResult Delete(string id = null)
        {
            Client client = db.Client.Find(id);
            if (client == null)
            {
                return HttpNotFound();
            }
            return View(client);
        }

        //
        // POST: /AddClient/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Client client = db.Client.Find(id);
            db.Client.Remove(client);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        #region 导入excel
        public ActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Upload(int i = 0)
        {
            HttpPostedFileBase fb = Request.Files[0];
            string fileName = fb.FileName;
            string path = "~/App_Data/" + fileName;
            fb.SaveAs(Server.MapPath(path));                 //将上传获得的文件fb保存为程序本地文件/App_Data/Clent.xlsx
            DataSet dataset = new DataSet();
            dataset = GetDataSet(path);
            if (dataset == null)
            {
                ViewBag.msg = "excel文件打开失败";
                return View("~/Views/Shared/Error.cshtml");
            }

            JsonData result = UploadtoDB(dataset.Tables["client"]);
            ViewBag.counter = result.Data;
            if (result.Success == true)
                return View("result");
            else 
                return View(result.type);
                 
        }
        private class WrongClient : Exception
        {
        }

        private JsonData UploadtoDB(DataTable dt)
        {
            int count = 0;
            string temp = null;
            JsonData result = new JsonData();
            Regex rex = new Regex("^ $");
            foreach (DataRow dr in dt.Rows)
            {
                Client client = new Client();
                bool IsValid = true;

                temp = dr["客户姓名"].ToString();     
                if (temp!="")                              //姓名非空即可
                    client.client_name = temp;
                else
                    IsValid = false;

                rex = new Regex("^([0-9]{15}|[0-9]{18}|[0-9]{17}[0-9Xx])$");
                temp = dr["身份证号码"].ToString();
                if (rex.IsMatch(temp))
                    client.identify_number = temp;
                else
                    IsValid = false;

                rex = new Regex("^[0-9]{11,11}$");
                temp = dr["手机号码"].ToString();
                if (rex.IsMatch(temp))
                    client.phone_number = temp;
                else
                    IsValid = false;

                rex = new Regex("^[0-9]{14}$");
                temp = dr["客户经理工号"].ToString();
                if (rex.IsMatch(temp))
                    client.client_manager = temp;
                else
                    IsValid = false;

                temp = dr["参会时间"].ToString();
                DateTime temp_time = Convert.ToDateTime(temp);
                client.join_time = temp_time;

                try
                {
                    if (IsValid)
                    {
                        db.Client.Add(client);
                        db.SaveChanges();
                        count++;
                    }
                    else
                        throw new WrongClient();
                }
                catch (WrongClient)
                {
                    result.Success = false;
                    result.Data = ++count;
                    result.type = "WrongResult";
                    return result;
                }
                catch (Exception)
                {
                    result.Success = false;
                    result.Data = ++count;
                    result.type = "KeyConflict";
                    return result;
                }
            }
                result.Success = true;
                result.Data = count;
                return result;
        }
        /*************************************/
        // 函数功能 将excel数据保存到内存数据库dataset
        private DataSet  GetDataSet(string filePath) 
        {
            JsonData result = new JsonData();
            string strConn = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Server.MapPath(filePath) + ";Extended Properties='Excel 12.0;HDR=Yes;IMEX=1';";
            OleDbConnection Oleconn = new OleDbConnection(strConn);
            OleDbDataAdapter excelCommand = null;
            DataSet excel_ds = new DataSet();
            string strSql = "select * from [sheet1$]";
            try
            {
                Oleconn.Open();
                excelCommand = new OleDbDataAdapter(strSql, Oleconn);
                excelCommand.Fill(excel_ds, "client");      //将数据保存到表"my_client"中,假如dataset中该表不存在则新建一个
            }
            catch (System.Exception)
            {
                return null;
            }
            finally
            {
                Oleconn.Close();
                Oleconn.Dispose();
            }
            return excel_ds;
        }
        #endregion

    }
}

