using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebAPI_DohomeApplication.Models;
using WebAPI_DohomeApplication.Models.Message;

namespace WebAPI_DohomeApplication.Controllers
{
    public class Message_TagController : Controller
    {
        // GET: Message_Tag
        string conn;
        DBMasterDataContext dm = new DBMasterDataContext();
        DBTransDataContext db = new DBTransDataContext();
        GetconfigConnection getconfig = new GetconfigConnection();

        [Route("SaveToTableBracodeTag")]
        [HttpPost]
        public JsonResult SaveToTableBracodeTag(List<List<Model_Save_TBTrans_PRE_Print_Message>> list_hd_dt)
        {
            using (db)
            {                
                string ssssite = GetSitebybranch(list_hd_dt[0][0].BRANCHCODE);     

                conn = getconfig.Check_Entities(ssssite); // check connection String โดยส่ง Site เข้าไปหา
                db.Connection.ConnectionString = conn; // Set connection String โดยส่ง Site เข้าไปหา
                string ddoc = RunningNewDocNo(ssssite);

                int i = 0;
                TBTrans_PRE_Print_Message hd = new TBTrans_PRE_Print_Message();
                TBTrans_PRE_Print_MessageSub dt;
                List<TBTrans_PRE_Print_MessageSub> list_dt = new List<TBTrans_PRE_Print_MessageSub>();


                if (list_hd_dt != null)
                {
                    //Save                       

                    hd.DOCNO = ddoc;  //item.DOCNO;
                    hd.DOCDATE = DateTime.Now.Date;//list_hd_dt[0][0].DOCDATE;
                    hd.BRANCHCODE = list_hd_dt[0][0].BRANCHCODE;//list_hd_dt[0][0].BRANCHCODE;
                    hd.CREATEDATE = DateTime.Now;
                    hd.CREATEUSER = list_hd_dt[0][0].REQUEST_USER;
                    hd.MAT_SELLER = list_hd_dt[0][0].MAT_SELLER;


                    foreach (var item2 in list_hd_dt[0])
                    {
                        dt = new TBTrans_PRE_Print_MessageSub();
                        dt.DOCNO = ddoc; //item2.DOCNO;
                        dt.ROWORDER = i;//item2.ROWORDER;
                        dt.BRANCHCODE = item2.BRANCHCODE;
                        dt.DOCDATE = DateTime.Now.Date;
                        dt.MESSAGE = item2.MESSAGE;
                        dt.QUANTITY = item2.QTYREQUEST;
                        dt.PRINTDATE = item2.PRINTDATE;
                        dt.PRINTUSER = item2.PRINTUSER;
                        list_dt.Add(dt);
                        i++;
                    }
                }

                else
                {
                    return Json(new { message = "Value list is null" }, JsonRequestBehavior.AllowGet);
                }

                try
                {
                    db.Connection.Open();
                    db.Transaction = db.Connection.BeginTransaction();
                    db.TBTrans_PRE_Print_Messages.InsertOnSubmit(hd);
                    db.TBTrans_PRE_Print_MessageSubs.InsertAllOnSubmit(list_dt);
                    db.SubmitChanges();
                    db.Transaction.Commit();
                    db.Connection.Close();
                    db.Connection.Dispose();
                    return Json(new { message = "success", data = ddoc }, JsonRequestBehavior.AllowGet);

                }
                catch (Exception ex)
                {
                    var err = ex.Message;
                    db.Transaction.Rollback();
                    db.Connection.Close();
                    db.Connection.Dispose();
                    return Json(new { message = "error", data = err }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        [HttpPost]
        public JsonResult GETHDDT(string branch, string requester, string docno, string docdate)
        {            
            string ssssite = GetSitebybranch(branch);
            conn = getconfig.Check_Entities(ssssite); // check connection String โดยส่ง Site เข้าไปหา                                                      
            db.Connection.ConnectionString = conn;  //db.Database.Connection.ConnectionString = conn; // Set connection String โดยส่ง Site เข้าไปหา

            if (docdate != "")
            {
                DateTime oDate2 = DateTime.Parse(docdate);
                string sDate = oDate2.ToString("yyyy-MM-dd", new CultureInfo("en-US"));

                var linq = (from hd in db.TBTrans_PRE_Print_Messages
                            join dt in db.TBTrans_PRE_Print_MessageSubs on hd.DOCNO equals dt.DOCNO
                            where hd.BRANCHCODE == branch
                               && hd.DOCDATE == Convert.ToDateTime(sDate)
                               && hd.DOCNO.Contains(docno)
                               && hd.CREATEUSER.Contains(requester)
                            select new { hd, dt }).ToList();

                Model_GETHDDT model_GETHDDT;
                List<Model_GETHDDT> list = new List<Model_GETHDDT>();
                foreach (var item in linq)
                {
                    model_GETHDDT = new Model_GETHDDT();
                    model_GETHDDT._DOCNO = item.hd.DOCNO;
                    model_GETHDDT._DOCDATE = item.hd.DOCDATE;
                    model_GETHDDT._BRANCHCODE = item.hd.BRANCHCODE;
                    model_GETHDDT._CREATEDATE = item.hd.CREATEDATE;
                    model_GETHDDT._CREATEUSERCODE = item.hd.CREATEUSER;
                    model_GETHDDT._CREATEUSERNAME = getUsername(item.hd.CREATEUSER);
                    model_GETHDDT._MAT_SELLER = item.hd.MAT_SELLER;
                    model_GETHDDT._ROWORDER = item.dt.ROWORDER;
                    model_GETHDDT._MESSAGE = item.dt.MESSAGE;
                    model_GETHDDT._QUANTITY = item.dt.QUANTITY;
                    model_GETHDDT._PRINTDATE = item.dt.PRINTDATE;
                    model_GETHDDT._PRINTUSER = item.dt.PRINTUSER;
                    list.Add(model_GETHDDT);
                }

                return Json(new { GETHDDT = list }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var linq = (from hd in db.TBTrans_PRE_Print_Messages
                            join dt in db.TBTrans_PRE_Print_MessageSubs on hd.DOCNO equals dt.DOCNO
                            where hd.BRANCHCODE == branch
                               && hd.DOCNO.Contains(docno)
                               && hd.CREATEUSER.Contains(requester)
                            select new { hd, dt }).ToList();

                Model_GETHDDT model_GETHDDT;
                List<Model_GETHDDT> list = new List<Model_GETHDDT>();
                foreach (var item in linq)
                {
                    model_GETHDDT = new Model_GETHDDT();
                    model_GETHDDT._DOCNO = item.hd.DOCNO;
                    model_GETHDDT._DOCDATE = item.hd.DOCDATE;
                    model_GETHDDT._BRANCHCODE = item.hd.BRANCHCODE;
                    model_GETHDDT._CREATEDATE = item.hd.CREATEDATE;
                    model_GETHDDT._CREATEUSERCODE = item.hd.CREATEUSER;
                    model_GETHDDT._CREATEUSERNAME = getUsername(item.hd.CREATEUSER);
                    model_GETHDDT._MAT_SELLER = item.hd.MAT_SELLER;
                    model_GETHDDT._ROWORDER = item.dt.ROWORDER;
                    model_GETHDDT._MESSAGE = item.dt.MESSAGE;
                    model_GETHDDT._QUANTITY = item.dt.QUANTITY;
                    model_GETHDDT._PRINTDATE = item.dt.PRINTDATE;
                    model_GETHDDT._PRINTUSER = item.dt.PRINTUSER;
                    list.Add(model_GETHDDT);
                }

                return Json(new { GETHDDT = list }, JsonRequestBehavior.AllowGet);
            }           
        }

        public string getUsername(string usercode)
        {

            var getuser = (from user in dm.TBMaster_Users
                           where user.CODE == usercode
                           select new { user }).ToList();


            return getuser[0].user.MYNAME.ToString(); ;
        }
        private string RunningNewDocNo(string site)  //UBBR-ERP-20190508-0001
        {
            dm.Connection.ConnectionString = conn; // Set connection String โดยส่ง Site เข้าไปหา

            string sResult = "", sShortName = "";
            int docn;
            string substr = site.ToUpper().Substring(0, 2); // UB11 ---> UB            
            string sdate = DateTime.Now.ToString("yyyyMMdd", new CultureInfo("en-US"));

            if (substr == "M0")
            {
                if (site == "M001")
                {
                    sShortName = "JR";
                }
                if (site == "M002")
                {
                    sShortName = "ST";
                }
                if (site == "M003")
                {
                    sShortName = "BP";
                }
            }
            else
            {
                sShortName = substr;
            }
            string sformat = sShortName + "MS-ERP-" + sdate + "-";

            var match = db.TBTrans_PRE_Print_Messages
                        .Where(x => x.DOCNO.Contains(sformat))
                        //.Where(x => x.DOCNO.Contains("CMBR-20180318-"))
                        .Select(s => new { DOCNO = s.DOCNO })
                        .OrderByDescending(v => v.DOCNO)
                        .FirstOrDefault();

            if (match != null)
            {
                int i = Int32.Parse(match.DOCNO.Substring(18, 4));  // ใส่ ตัด UBN-ERP-20190401-0001 ให้ได้ 0001
                docn = i + 1;
                string sRepleace = docn.ToString().PadLeft(4, '0'); // ใส่ 0 ไป 4 ตัว
                sResult = sformat + sRepleace;
            }
            else
            {
                docn = 1;
                string sRepleace = docn.ToString().PadLeft(4, '0');
                sResult = sformat + sRepleace;
            }
            return sResult;
        }
        public string GetSitebybranch(string branch)
        {
            var linq = (from bs in dm.TBMaster_Branch_Sites
                        where bs.BRANCHCODE == branch
                        select new { site = bs.SITECODE }).FirstOrDefault();
            return linq.site;
        }
    }
}