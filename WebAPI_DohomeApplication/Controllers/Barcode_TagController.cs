// Decompiled with JetBrains decompiler
// Type: WebAPI_DohomeApplication.Controllers.Barcode_TagController
// Assembly: WebAPI_DohomeApplication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 9FD02E5D-69DD-405E-BE01-E2CBADC5A772
// Assembly location: C:\Users\withawat.bun\Music\QAS\WebAPI_DohomeApplication.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Description;
using WebAPI_DohomeApplication.Models;
using WebAPI_DohomeApplication.Models.Barcode;

namespace WebAPI_DohomeApplication.Controllers
{
    public class Barcode_TagController : ApiController
    {
        private string conn;
        private DBMasterDataContext dm = new DBMasterDataContext();
        private DBTransDataContext db = new DBTransDataContext();
        private GetconfigConnection getconfig = new GetconfigConnection();

        /// <summary>
        /// SaveToTableBracodeTag บันทึกข้อมูล
        /// </summary>
        /// <param name="list_hd_dt">ลิสที่ save</param>
        /// <returns></returns>
        [ResponseType(typeof(Model_Response_save))]
        [AllowAnonymous]
        [Route("SaveToTableBracodeTag")]
        [HttpPost]
        public IHttpActionResult SaveToTableBracodeTag([FromBody] List<Model_Save_TBTrans_PRE_Print_Barcode> list_hd_dt){
            using (this.db)
            {
                this.conn = this.getconfig.Check_Entities(list_hd_dt[0].SITECODE);
                this.db.Connection.ConnectionString = this.conn;
                string str1 = this.RunningNewDocNo(list_hd_dt[0].SITECODE);
                int num = 0;
                TBTrans_PRE_Print_Barcode entity = new TBTrans_PRE_Print_Barcode();
                List<TBTrans_PRE_Print_BarcodeSub> prePrintBarcodeSubList = new List<TBTrans_PRE_Print_BarcodeSub>();
                Model_Response_save modelResponseSave = new Model_Response_save();
                bool flag = false;
                string str2 = "";
                if (list_hd_dt != null)
                {
                    entity.DOCNO = str1;
                    entity.DOCDATE = DateTime.Now.Date;
                    entity.BRANCHCODE = list_hd_dt[0].BRANCHCODE;
                    entity.SITECODE = list_hd_dt[0].SITECODE;
                    entity.SLOC = list_hd_dt[0].SLOC;
                    entity.CREATEDATE = DateTime.Now;
                    entity.CREATEUSER = list_hd_dt[0].REQUEST_USER;
                    entity.STATUS = "Wait";
                    entity.PRINT_NUMBER = 0;
                    entity.PRINTDATE = null;
                    entity.PRINTUSER = null;
                    foreach (Model_Save_TBTrans_PRE_Print_Barcode transPrePrintBarcode in list_hd_dt)
                    {
                        TBTrans_PRE_Print_BarcodeSub prePrintBarcodeSub = new TBTrans_PRE_Print_BarcodeSub();
                        prePrintBarcodeSub.DOCNO = str1;
                        prePrintBarcodeSub.ROWORDER = num;
                        prePrintBarcodeSub.DOCDATE = DateTime.Now.Date;
                        prePrintBarcodeSub.BRANCHCODE = transPrePrintBarcode.BRANCHCODE;
                        prePrintBarcodeSub.SITECODE = transPrePrintBarcode.SITECODE;
                        prePrintBarcodeSub.SLOC = transPrePrintBarcode.SLOC;
                        prePrintBarcodeSub.PRODUCTCODE = transPrePrintBarcode.PRODUCTCODE;
                        prePrintBarcodeSub.PRODUCTNAME = transPrePrintBarcode.PRODUCTNAME;
                        prePrintBarcodeSub.BARCODE_REQ = transPrePrintBarcode.BARCODE;
                        prePrintBarcodeSub.QUANTITY_REQ = Decimal.Parse(transPrePrintBarcode.QUANTITY_REQ);
                        prePrintBarcodeSub.UNIT_REQ = transPrePrintBarcode.UNITCODE_REQUEST;
                        prePrintBarcodeSub.UNIT_RATE_REQ = Decimal.Parse(transPrePrintBarcode.UNIT_RATE);
                        prePrintBarcodeSub.QUANTITY_TRAN = Decimal.Parse(transPrePrintBarcode.QUANTITY_REQ);
                        prePrintBarcodeSub.UNIT_TRAN = transPrePrintBarcode.UNITCODE_TRANSORT;
                        prePrintBarcodeSub.PRINTDATE = null;
                        prePrintBarcodeSub.CAUSE_REQUEST = transPrePrintBarcode.CAUSE_REQUEST;
                        prePrintBarcodeSub.GR_DOCNO = null;
                        prePrintBarcodeSub.GR_YEAR = null;
                        prePrintBarcodeSub.PRINTUSER = null;

                        if (transPrePrintBarcode.BARCODE.Length.Equals(13))
                        {
                            flag = true;
                            prePrintBarcodeSub.STATUS = "Wait";
                        }
                        else
                        prePrintBarcodeSub.STATUS = "";
                        prePrintBarcodeSub.APPROVEDATE = null;
                        prePrintBarcodeSub.APPROVEUSER = null;
                        prePrintBarcodeSub.APPROVE_REMARK = null;
                        ++num;
                        prePrintBarcodeSubList.Add(prePrintBarcodeSub);
                    }
                    try
                    {
                        this.db.Connection.Open();
                        this.db.Transaction = this.db.Connection.BeginTransaction();
                        this.db.TBTrans_PRE_Print_Barcodes.InsertOnSubmit(entity);
                        this.db.TBTrans_PRE_Print_BarcodeSubs.InsertAllOnSubmit(prePrintBarcodeSubList);
                        this.db.SubmitChanges();
                        this.db.Transaction.Commit();
                        this.db.Connection.Close();
                        this.db.Connection.Dispose();
                        if (flag)
                            str2 = "เอาสารมีรายการติดอนุมัติ ให้ผู้มีสิทธิ์อนุมัติ จึงจะสามารถพิมพ์ได้";
                        modelResponseSave.status = true;
                        modelResponseSave.msg = str2;
                        modelResponseSave.docno = str1;
                        return this.Ok(modelResponseSave);
                    }
                    catch (Exception ex)
                    {
                        string message = ex.Message;
                        this.db.Transaction.Rollback();
                        this.db.Connection.Close();
                        this.db.Connection.Dispose();
                        modelResponseSave.status = false;
                        modelResponseSave.msg = message;
                        modelResponseSave.docno = "";
                        return this.Ok(modelResponseSave);
                    }
                }
                else
                {
                    modelResponseSave.status = false;
                    modelResponseSave.msg = "Value list is null";
                    modelResponseSave.docno = "";
                    return this.Ok(modelResponseSave);
                }
            }
        }
        /// <summary>
        /// ApproveTagbarcode
        /// </summary>
        /// <param name="list_data">[FromBody] model_recive_approve list_data</param>
        /// <returns></returns>
        [ResponseType(typeof(Model_return_approve))]
        [AllowAnonymous]
        [Route("ApproveTagbarcode")]
        [HttpPost]
        public IHttpActionResult ApproveTagbarcode([FromBody] model_recive_approve list_data)
        {
            Model_return_approve modelReturnApprove = new Model_return_approve();
            this.conn = getconfig.Check_Entities(list_data.site);
            this.db.Connection.ConnectionString = this.conn;
            foreach (approve approve in list_data.list_data)
            {
                var linq = (from dt in db.TBTrans_PRE_Print_BarcodeSubs
                            where dt.DOCNO == approve.DOCNO && dt.ROWORDER == approve.ROWORDER && dt.BARCODE_REQ == approve.BARCODE_REQ
                            select new { dt }).ToList();

                if (linq.Count() > 0 && list_data.UserApprove != "" && list_data.status != "")
                {
                    List<TBTrans_PRE_Print_BarcodeSub> prePrintBarcodeSubList = new List<TBTrans_PRE_Print_BarcodeSub>();
                    foreach (var data in linq)
                    {
                        data.dt.STATUS = list_data.status;
                        data.dt.APPROVEDATE = DateTime.Now;
                        data.dt.APPROVEUSER = list_data.UserApprove;
                        data.dt.APPROVE_REMARK = list_data.reason;
                    }
                }
                else
                {
                    modelReturnApprove.status = false;
                    modelReturnApprove.msg = "ไม่พบข้อมูลที่จะอนุมัติ";
                }
            }
            try
            {
                this.db.Connection.Open();
                this.db.Transaction = this.db.Connection.BeginTransaction();
                this.db.SubmitChanges();
                this.db.Transaction.Commit();
                this.db.Connection.Close();
                this.db.Connection.Dispose();
                modelReturnApprove.status = true;
                modelReturnApprove.msg = list_data.status + " success";
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                this.db.Transaction.Rollback();
                this.db.Connection.Close();
                this.db.Connection.Dispose();
                modelReturnApprove.status = true;
                modelReturnApprove.msg = message;
            }
            return this.Ok(modelReturnApprove);
        }
        /// <summary>
        /// ใช้ดึง GETDT_APPROVE
        /// </summary>
        /// <param name="site">สาขา</param>
        /// <param name="sloc">ตำแหน่งจัดเก็บ</param>
        /// <param name="createuser">คนสร้างเอกสาร</param>
        /// <param name="docno">เลฃที่เอกสาร</param>
        /// <param name="docdate">วันที่สร้างเอกสาร</param>
        /// <param name="status_approve">ส่งเข้ามาเพื่อเช็ค mag ที่จะส่งออกไปหน้า แอพ 1 คือ จะส่งว่างๆออกไปถ้ามีการค้นหาไม่พบ 0 คือ จะส่ง msg ออกไปถ้าไม่พบข้อมูล</param>
        /// <returns></returns>
        [ResponseType(typeof(Model_Get_Approve))]
        [AllowAnonymous]
        [Route("GETDT_APPROVE")]
        [HttpGet]
        public IHttpActionResult GETDT_APPROVE(string site,string sloc,string createuser,string docno,string docdate){
            List<Get_Approve> getApproveList = new List<Get_Approve>();
            Model_Get_Approve modelGetApprove = new Model_Get_Approve();
            this.conn = this.getconfig.Check_Entities(site);
            this.db.Connection.ConnectionString = this.conn;

            if (createuser == null)
                createuser = "";
            if (docno == null)
                docno = "";
            if (docdate == null)
                docdate = "";

            if (docdate != "")
            {
                try
                {
                    DateTime oDate2 = DateTime.Parse(docdate);
                    string sDate = oDate2.ToString("yyyy-MM-dd", new CultureInfo("en-US"));

                    getApproveList = (from hd in db.TBTrans_PRE_Print_Barcodes
                                      join dt in db.TBTrans_PRE_Print_BarcodeSubs on hd.DOCNO equals dt.DOCNO
                                      where hd.SITECODE == site
                                      && hd.SLOC == sloc
                                      && hd.CREATEUSER.Contains(createuser)
                                      && hd.DOCNO.Contains(docno)
                                      && hd.DOCDATE == Convert.ToDateTime(sDate)
                                      && dt.STATUS == "Wait"
                                      select new Get_Approve
                                      {
                                          CREATEUSERCODE = hd.CREATEUSER,
                                          CREATEUSERNAME = getUsername(hd.CREATEUSER),
                                          DOCNO = dt.DOCNO,
                                          ROWORDER = dt.ROWORDER,
                                          DOCDATE = dt.DOCDATE,
                                          BRANCHCODE = dt.BRANCHCODE,
                                          SITECODE = dt.SITECODE,
                                          SLOC = dt.SLOC,
                                          PRODUCTCODE = dt.PRODUCTCODE,
                                          PRODUCTNAME = dt.PRODUCTNAME,
                                          BARCODE_REQ = dt.BARCODE_REQ,
                                          QUANTITY_REQ = dt.QUANTITY_REQ.ToString(),
                                          UNIT_REQ = dt.UNIT_REQ,
                                          UNIT_REQ_NAME = getUnitname(dt.UNIT_REQ),
                                          UNIT_RATE_REQ = dt.UNIT_RATE_REQ.ToString(),
                                          QUANTITY_TRAN = dt.QUANTITY_TRAN.ToString(),
                                          UNIT_TRAN = dt.UNIT_TRAN,
                                          UNIT_TRAN_NAME = getUnitname(dt.UNIT_TRAN),
                                          PRINTDATE = dt.PRINTDATE,
                                          CAUSE_REQUEST = dt.CAUSE_REQUEST,
                                          GR_DOCNO = dt.GR_DOCNO,
                                          GR_YEAR = dt.GR_YEAR,
                                          PRINTUSER = dt.PRINTUSER,
                                          STATUS = dt.STATUS,
                                          APPROVEDATE = dt.APPROVEDATE,
                                          APPROVEUSER = dt.APPROVEUSER,
                                          APPROVE_REMARK = dt.APPROVE_REMARK
                                      }).ToList();

                    if (getApproveList.Count == 0 )
                    {
                        modelGetApprove.status = false;
                        modelGetApprove.msg = "ไม่พบข้อมูลจากที่คุณค้นหา !";
                        modelGetApprove.list_approve = getApproveList;
                    }                  
                    else
                    {
                        modelGetApprove.status = true;
                        modelGetApprove.msg = "";
                        modelGetApprove.list_approve = getApproveList;
                    }
                }
                catch (Exception ex)
                {
                    modelGetApprove.status = false;
                    modelGetApprove.msg = ex.Message;
                }
                return this.Ok(modelGetApprove);
            }
            try
            {
                getApproveList = (from hd in db.TBTrans_PRE_Print_Barcodes
                            join dt in db.TBTrans_PRE_Print_BarcodeSubs on hd.DOCNO equals dt.DOCNO
                            where hd.SITECODE == site
                            && hd.SLOC == sloc
                            && hd.CREATEUSER.Contains(createuser)
                            && hd.DOCNO.Contains(docno)
                            && dt.STATUS == "Wait"
                            select new Get_Approve
                            {
                                CREATEUSERCODE = hd.CREATEUSER,
                                CREATEUSERNAME = getUsername(hd.CREATEUSER),
                                DOCNO = dt.DOCNO,
                                ROWORDER = dt.ROWORDER,
                                DOCDATE = dt.DOCDATE,
                                BRANCHCODE = dt.BRANCHCODE,
                                SITECODE = dt.SITECODE,
                                SLOC = dt.SLOC,
                                PRODUCTCODE = dt.PRODUCTCODE,
                                PRODUCTNAME = dt.PRODUCTNAME,
                                BARCODE_REQ = dt.BARCODE_REQ,
                                QUANTITY_REQ = dt.QUANTITY_REQ.ToString(),
                                UNIT_REQ = dt.UNIT_REQ,
                                UNIT_REQ_NAME = getUnitname(dt.UNIT_REQ),
                                UNIT_RATE_REQ = dt.UNIT_RATE_REQ.ToString(),
                                QUANTITY_TRAN = dt.QUANTITY_TRAN.ToString(),
                                UNIT_TRAN = dt.UNIT_TRAN,
                                UNIT_TRAN_NAME = getUnitname(dt.UNIT_TRAN),
                                PRINTDATE = dt.PRINTDATE,
                                CAUSE_REQUEST = dt.CAUSE_REQUEST,
                                GR_DOCNO = dt.GR_DOCNO,
                                GR_YEAR = dt.GR_YEAR,
                                PRINTUSER = dt.PRINTUSER,
                                STATUS = dt.STATUS,
                                APPROVEDATE = dt.APPROVEDATE,
                                APPROVEUSER = dt.APPROVEUSER,
                                APPROVE_REMARK = dt.APPROVE_REMARK
                            }).ToList();


                if (getApproveList.Count == 0)
                {
                    modelGetApprove.status = false;
                    modelGetApprove.msg = "ไม่พบข้อมูลจากที่คุณค้นหา !";
                    modelGetApprove.list_approve = getApproveList;
                }                
                else
                {
                    modelGetApprove.status = true;
                    modelGetApprove.msg = "";
                    modelGetApprove.list_approve = getApproveList;
                }
            }
            catch (Exception ex)
            {
                modelGetApprove.status = false;
                modelGetApprove.msg = ex.Message;
            }
            return this.Ok(modelGetApprove);
        }
        /// <summary>
        /// ใช้ดึง GETHD_STATUS
        /// </summary>
        /// <param name="site">สาขา</param>
        /// <param name="sloc">ตำแหน่งจัดเก็บ</param>
        /// <param name="createuser">คนสร้างเอกสาร</param>
        /// <param name="docno">เลฃที่เอกสาร</param>
        /// <param name="docdate">วันที่สร้างเอกสาร</param>
        /// <returns></returns>
        [ResponseType(typeof(Get_Status))]
        [AllowAnonymous]
        [Route("GETHD_STATUS")]
        [HttpGet]
        public IHttpActionResult GETHD_STATUS(string site, string sloc, string createuser, string docno, string docdate)
        {
            List<modelGet_Status> list_status = new List<modelGet_Status>();
            Get_Status get_Status = new Get_Status();
            this.conn = this.getconfig.Check_Entities(site);
            this.db.Connection.ConnectionString = this.conn;

            if (createuser == null)
                createuser = "";
            if (docno == null)
                docno = "";
            if (docdate == null)
                docdate = "";

            if (docdate != "")
            {
                try
                {
                    DateTime oDate2 = DateTime.Parse(docdate);
                    string sDate = oDate2.ToString("yyyy-MM-dd", new CultureInfo("en-US"));

                    list_status = (from hd in db.TBTrans_PRE_Print_Barcodes
                                      //join dt in db.TBTrans_PRE_Print_BarcodeSubs on hd.DOCNO equals dt.DOCNO
                                      where hd.SITECODE == site
                                      && hd.SLOC == sloc
                                      && hd.CREATEUSER.Contains(createuser)
                                      && hd.DOCNO.Contains(docno)
                                      && hd.DOCDATE == Convert.ToDateTime(sDate)
                                      //&& dt.STATUS == "Wait"
                                      select new modelGet_Status
                                      {
                                          //HD
                                          DOCNO_HD = hd.DOCNO,
                                          DOCDATE_HD = hd.DOCDATE,
                                          BRANCHCODE_HD = hd.BRANCHCODE,
                                          SITECODE_HD = hd.SITECODE,
                                          SLOC_HD  = hd.SLOC,
                                          CREATEDATE_HD = hd.CREATEDATE,
                                          CREATEUSERCODE_HD = hd.CREATEUSER,
                                          CREATEUSERNAME_HD = getUsername(hd.CREATEUSER),
                                          STATUS_HD = hd.STATUS,
                                          PRINT_NUMBER_HD = hd.PRINT_NUMBER,
                                          PRINTDATE_HD = hd.PRINTDATE,
                                          PRINTUSER_HD = hd.PRINTUSER,
                                          //DT
                                          //DOCNO = dt.DOCNO,
                                          //ROWORDER = dt.ROWORDER,
                                          //DOCDATE = dt.DOCDATE,
                                          //BRANCHCODE = dt.BRANCHCODE,
                                          //SITECODE = dt.SITECODE,
                                          //SLOC = dt.SLOC,
                                          //PRODUCTCODE = dt.PRODUCTCODE,
                                          //PRODUCTNAME = dt.PRODUCTNAME,
                                          //BARCODE_REQ = dt.BARCODE_REQ,
                                          //QUANTITY_REQ = dt.QUANTITY_REQ.ToString(),
                                          //UNIT_REQ = dt.UNIT_REQ,
                                          //UNIT_REQ_NAME = getUnitname(dt.UNIT_REQ),
                                          //UNIT_RATE_REQ = dt.UNIT_RATE_REQ.ToString(),
                                          //QUANTITY_TRAN = dt.QUANTITY_TRAN.ToString(),
                                          //UNIT_TRAN = dt.UNIT_TRAN,
                                          //UNIT_TRAN_NAME = getUnitname(dt.UNIT_TRAN),
                                          //PRINTDATE = dt.PRINTDATE,
                                          //CAUSE_REQUEST = dt.CAUSE_REQUEST,
                                          //GR_DOCNO = dt.GR_DOCNO,
                                          //GR_YEAR = dt.GR_YEAR,
                                          //PRINTUSER = dt.PRINTUSER,
                                          //STATUS = dt.STATUS,
                                          //APPROVEDATE = dt.APPROVEDATE,
                                          //APPROVEUSER = dt.APPROVEUSER,
                                          //APPROVE_REMARK = dt.APPROVE_REMARK
                                      }).ToList();

                    if (list_status.Count == 0)
                    {
                        get_Status.status = false;
                        get_Status.msg = "ไม่พบข้อมูลจากที่คุณค้นหา !";
                        get_Status.list_status = list_status;
                    }
                    else
                    {
                        get_Status.status = true;
                        get_Status.msg = "";
                        get_Status.list_status = list_status;
                    }
                }
                catch (Exception ex)
                {
                    get_Status.status = false;
                    get_Status.msg = ex.Message;
                }
                return this.Ok(get_Status);
            }
            try
            {
                list_status = (from hd in db.TBTrans_PRE_Print_Barcodes
                                  //join dt in db.TBTrans_PRE_Print_BarcodeSubs on hd.DOCNO equals dt.DOCNO
                                  where hd.SITECODE == site
                                  && hd.SLOC == sloc
                                  && hd.CREATEUSER.Contains(createuser)
                                  && hd.DOCNO.Contains(docno)
                                  //&& dt.STATUS == "Wait"
                                  select new modelGet_Status
                                  {
                                      //HD
                                      DOCNO_HD = hd.DOCNO,
                                      DOCDATE_HD = hd.DOCDATE,
                                      BRANCHCODE_HD = hd.BRANCHCODE,
                                      SITECODE_HD = hd.SITECODE,
                                      SLOC_HD = hd.SLOC,
                                      CREATEDATE_HD = hd.CREATEDATE,
                                      CREATEUSERCODE_HD = hd.CREATEUSER,
                                      CREATEUSERNAME_HD = getUsername(hd.CREATEUSER),
                                      STATUS_HD = hd.STATUS,
                                      PRINT_NUMBER_HD = hd.PRINT_NUMBER,
                                      PRINTDATE_HD = hd.PRINTDATE,
                                      PRINTUSER_HD = hd.PRINTUSER,
                                      //DT
                                      //DOCNO = dt.DOCNO,
                                      //ROWORDER = dt.ROWORDER,
                                      //DOCDATE = dt.DOCDATE,
                                      //BRANCHCODE = dt.BRANCHCODE,
                                      //SITECODE = dt.SITECODE,
                                      //SLOC = dt.SLOC,
                                      //PRODUCTCODE = dt.PRODUCTCODE,
                                      //PRODUCTNAME = dt.PRODUCTNAME,
                                      //BARCODE_REQ = dt.BARCODE_REQ,
                                      //QUANTITY_REQ = dt.QUANTITY_REQ.ToString(),
                                      //UNIT_REQ = dt.UNIT_REQ,
                                      //UNIT_REQ_NAME = getUnitname(dt.UNIT_REQ),
                                      //UNIT_RATE_REQ = dt.UNIT_RATE_REQ.ToString(),
                                      //QUANTITY_TRAN = dt.QUANTITY_TRAN.ToString(),
                                      //UNIT_TRAN = dt.UNIT_TRAN,
                                      //UNIT_TRAN_NAME = getUnitname(dt.UNIT_TRAN),
                                      //PRINTDATE = dt.PRINTDATE,
                                      //CAUSE_REQUEST = dt.CAUSE_REQUEST,
                                      //GR_DOCNO = dt.GR_DOCNO,
                                      //GR_YEAR = dt.GR_YEAR,
                                      //PRINTUSER = dt.PRINTUSER,
                                      //STATUS = dt.STATUS,
                                      //APPROVEDATE = dt.APPROVEDATE,
                                      //APPROVEUSER = dt.APPROVEUSER,
                                      //APPROVE_REMARK = dt.APPROVE_REMARK
                                  }).ToList();


                if (list_status.Count == 0)
                {
                    get_Status.status = false;
                    get_Status.msg = "ไม่พบข้อมูลจากที่คุณค้นหา !";
                    get_Status.list_status = list_status;
                }
                else
                {
                    get_Status.status = true;
                    get_Status.msg = "";
                    get_Status.list_status = list_status;
                }
            }
            catch (Exception ex)
            {
                get_Status.status = false;
                get_Status.msg = ex.Message;
            }
            return this.Ok(get_Status);
        }
        /// <summary>
        /// ใช้ดึง GETDT_STATUS
        /// </summary>
        /// <param name="site">สาขา</param>
        /// <param name="docno">เลฃที่เอกสาร</param>
        /// <returns></returns>
        [ResponseType(typeof(Get_Status))]
        [AllowAnonymous]
        [Route("GETDT_STATUS")]
        [HttpGet]
        public IHttpActionResult GETDT_STATUS(string site, string docno)
        {
            List<modelGet_Status> list_status = new List<modelGet_Status>();
            Get_Status get_Status = new Get_Status();
            this.conn = this.getconfig.Check_Entities(site);
            this.db.Connection.ConnectionString = this.conn;
    
            if (docno == null)
                docno = "";

            if (docno != "")
            {
                try
                {                
                    list_status = (from dt in db.TBTrans_PRE_Print_BarcodeSubs
                                   
                                   where dt.DOCNO == docno

                                   select new modelGet_Status
                                   {
                                       ////HD
                                       //DOCNO_HD = hd.DOCNO,
                                       //DOCDATE_HD = hd.DOCDATE,
                                       //BRANCHCODE_HD = hd.BRANCHCODE,
                                       //SITECODE_HD = hd.SITECODE,
                                       //SLOC_HD = hd.SLOC,
                                       //CREATEDATE_HD = hd.CREATEDATE,
                                       //CREATEUSERCODE_HD = hd.CREATEUSER,
                                       //CREATEUSERNAME_HD = getUsername(hd.CREATEUSER),
                                       //STATUS_HD = hd.STATUS,
                                       //PRINT_NUMBER_HD = hd.PRINT_NUMBER,
                                       //PRINTDATE_HD = hd.PRINTDATE,
                                       //PRINTUSER_HD = hd.PRINTUSER,

                                       //DT
                                       DOCNO = dt.DOCNO,
                                       ROWORDER = dt.ROWORDER,
                                       DOCDATE = dt.DOCDATE,
                                       BRANCHCODE = dt.BRANCHCODE,
                                       SITECODE = dt.SITECODE,
                                       SLOC = dt.SLOC,
                                       PRODUCTCODE = dt.PRODUCTCODE,
                                       PRODUCTNAME = dt.PRODUCTNAME,
                                       BARCODE_REQ = dt.BARCODE_REQ,
                                       QUANTITY_REQ = dt.QUANTITY_REQ.ToString(),
                                       UNIT_REQ = dt.UNIT_REQ,
                                       UNIT_REQ_NAME = getUnitname(dt.UNIT_REQ),
                                       UNIT_RATE_REQ = dt.UNIT_RATE_REQ.ToString(),
                                       QUANTITY_TRAN = dt.QUANTITY_TRAN.ToString(),
                                       UNIT_TRAN = dt.UNIT_TRAN,
                                       UNIT_TRAN_NAME = getUnitname(dt.UNIT_TRAN),
                                       PRINTDATE = dt.PRINTDATE,
                                       CAUSE_REQUEST = dt.CAUSE_REQUEST,
                                       GR_DOCNO = dt.GR_DOCNO,
                                       GR_YEAR = dt.GR_YEAR,
                                       PRINTUSER = dt.PRINTUSER,
                                       STATUS = dt.STATUS,
                                       APPROVEDATE = dt.APPROVEDATE,
                                       APPROVEUSER = dt.APPROVEUSER,
                                       APPROVE_REMARK = dt.APPROVE_REMARK
                                   }).ToList();

                    if (list_status.Count == 0)
                    {
                        get_Status.status = false;
                        get_Status.msg = "ไม่พบข้อมูลจากที่คุณค้นหา !";
                        get_Status.list_status = list_status;
                    }
                    else
                    {
                        get_Status.status = true;
                        get_Status.msg = "";
                        get_Status.list_status = list_status;
                    }
                }
                catch (Exception ex)
                {
                    get_Status.status = false;
                    get_Status.msg = ex.Message;
                }
            }
            else
            {
                get_Status.status = false;
                get_Status.msg = "ไม่พบเลขที่เอกสารที่ส่งมา!";
                get_Status.list_status = list_status;
            }
            return this.Ok(get_Status);
        }

        private string RunningNewDocNo(string site)  //UBBR-ERP-20190508-0001 
        {
            var DBTRANS = new DBTransDataContext();
            DBTRANS.Connection.ConnectionString = conn; // Set connection String โดยส่ง Site เข้าไปหา

            string sResult = "", sShortName = "";
            int docn;
            string substr = site.ToUpper().Substring(0, 2); // UB11 ---> UB
            string sdate = DateTime.Now.ToString("yyyyMMdd", new CultureInfo("en-US"));
            if (substr == "M0")
            {
                if (site == "M001")
                    sShortName = "JR";
                if (site == "M002")
                    sShortName = "ST";
                if (site == "M003")
                    sShortName = "BP";
            }
            else
            {
                sShortName = substr;
            }
            string sformat = sShortName + "BR-HH-" + sdate + "-";

            var match = DBTRANS.TBTrans_PRE_Print_Barcodes
                        .Where(x => x.DOCNO.Contains(sformat))
                        //.Where(x => x.DOCNO.Contains("CMBR-20180318-"))
                        .Select(s => new { DOCNO = s.DOCNO })
                        .OrderByDescending(v => v.DOCNO)
                        .FirstOrDefault();

            if (match != null)
            {
                int i = Int32.Parse(match.DOCNO.Substring(17, 4));  // ใส่ ตัด UBN-ERP-20190401-0001 ให้ได้ 0001
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
        private string getUsername(string usercode)
        {
            string username = "";
            var getuser = (from user in dm.TBMaster_Users
                           where user.CODE == usercode
                           select new { user }).ToList();
            if (getuser.Count > 0)               
                username = getuser[0].user.MYNAME.ToString();
            return username;
        }
        private string getUnitname(string unitcode)
        {
            string unitname = "";
            var getunit = (from user in dm.TBMaster_Units
                           where user.CODE == unitcode
                           select new { user }).ToList();
            if (getunit.Count > 0)
                unitname = getunit[0].user.MYNAME.ToString();

            return unitname;
        }
    }
}
