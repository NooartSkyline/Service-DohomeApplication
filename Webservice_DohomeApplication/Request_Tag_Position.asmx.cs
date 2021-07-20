using Newtonsoft.Json;
using SourceCode.SmartObjects.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace Webservice_DohomeApplication
{
    /// <summary>
    /// Summary description for Request_Tag_Position
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class Request_Tag_Position : System.Web.Services.WebService
    {
        DBMASTERDataContext dBMASTER = new DBMASTERDataContext();
        public string Connect = System.Configuration.ConfigurationManager.ConnectionStrings["DBMASTERConnectionString"].ConnectionString;
        string conn;
        Class_Configserver configserver = new Class_Configserver();
        private SmartObjectClientServer K2_CONNECT_SERVER()
        {
            DataSet ds = new DataSet();
            SmartObjectClientServer serverName = new SmartObjectClientServer();
            SourceCode.Hosting.Client.BaseAPI.SCConnectionStringBuilder connectionString = new SourceCode.Hosting.Client.BaseAPI.SCConnectionStringBuilder();
            connectionString.Authenticate = true;
            connectionString.Host = System.Configuration.ConfigurationManager.AppSettings["K2Server"];
            connectionString.Integrated = true;
            connectionString.IsPrimaryLogin = true;
            connectionString.Port = Convert.ToUInt32(System.Configuration.ConfigurationManager.AppSettings["Port"].ToString());
            connectionString.UserID = System.Configuration.ConfigurationManager.AppSettings["K2User"];
            connectionString.WindowsDomain = System.Configuration.ConfigurationManager.AppSettings["Domain"];
            connectionString.Password = System.Configuration.ConfigurationManager.AppSettings["K2Password"];
            connectionString.SecurityLabelName = System.Configuration.ConfigurationManager.AppSettings["SecurityLabel"];
            serverName.CreateConnection();
            serverName.Connection.Open(connectionString.ToString());
            return serverName;
        }

        // ถ้าไม่ใส่ LableTypeCode จะเป็นการค้นหาทั้งหมด
        // หรือ ถ้าอยากเปลี่ยนให้ Filter เฉพาะ Type ให้ใช้ตัวนี้แทน 
        // strsql = "select * from dbmaster..TBMaster_LabelType where Type = '" + "X" + "' "

        [WebMethod]
        public string Read_Lable_Type(string site){
            //conn = StringConn(site);
            DataSet ds = new DataSet();
            DataSet objds = new DataSet();
            try
            {
                dev.lib.SQLConnection devdb = new dev.lib.SQLConnection(Connect);
                SqlConnection cnn = devdb.getSqlConncetion;
                string strsql = "select * from dbmaster..TBMaster_LabelType ";

                bool ret = devdb.ExecuteCommand(ref ds, strsql, "TBMaster_LabelType", ref cnn);
                if (ret)
                {
                    int iRowCount = ds.Tables["TBMaster_LabelType"].Rows.Count;
                    if (iRowCount > 0)
                    {
                        DataTable dtStrBin = new DataTable();
                        dtStrBin.Columns.Add("CODE", typeof(string));
                        dtStrBin.Columns.Add("MYNAME", typeof(string));
                        dtStrBin.Columns.Add("TYPE", typeof(string));
                        dtStrBin.Columns.Add("LAST_UPDATE_DATE", typeof(string));
                        dtStrBin.Columns.Add("LAST_UPDATE_TIME", typeof(string));                      

                        foreach (DataRow objdr in ds.Tables["TBMaster_LabelType"].Rows)
                        {
                            DataRow dr = dtStrBin.NewRow();
                            dr["CODE"] = objdr["CODE"].ToString();
                            dr["MYNAME"] = objdr["MYNAME"].ToString();
                            if (objdr["TYPE"] == null)
                            {
                                dr["TYPE"] = "";
                            }
                            dr["TYPE"] = objdr["TYPE"].ToString();
                            dr["LAST_UPDATE_DATE"] = objdr["LAST_UPDATE_DATE"].ToString();
                            dr["LAST_UPDATE_TIME"] = objdr["LAST_UPDATE_TIME"].ToString();
                            dtStrBin.Rows.Add(dr);
                        }
                        dtStrBin.TableName = "TBMaster_LabelType";
                        objds.Tables.Add(dtStrBin);
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return ConvertDataSetToJSON(objds);
        }

        [WebMethod]
        public string OnSave(string Jsonstring, string sStatus, string sDocNo, string site)
        {
            string sMessage = "";
            string sRunningDocNo = "";
           
                DataSet ds = new DataSet();
                DataTable dt = this.JsonToTable(Jsonstring);
                DataRow dr = dt.Rows[0];
                if (sStatus == "AddNew" && sDocNo == "")
                {
                    //sRunningDocNo = this.RunningNewDocNo(dr["BRANCHCODE"].ToString()); // ส่งรหัสสาขาเข้าไปเพื่อนำไป Get ข้อมูลที่ Field SHORT_NAME (ให้ดูที่ฟังก์ชั่น : getBranchShortName)
                    sRunningDocNo = this.RunningNewDocNo(site); // ส่งรหัสสาขาเข้าไปเพื่อนำไป Get ข้อมูลที่ Field SHORT_NAME (ให้ดูที่ฟังก์ชั่น : getBranchShortName)
                }
                if (sStatus == "Edit" && sDocNo != "")
                {
                    sRunningDocNo = sDocNo;
                }

            //string sOnSave_TO_Master = OnSave_TO_Master(Jsonstring, sStatus, sDocNo, sRunningDocNo);
            string sOnSave_TO_Site = OnSave_TO_Site(Jsonstring, sStatus, sRunningDocNo, site);

            //if (sOnSave_TO_Master == sOnSave_TO_Site)

                sMessage = sOnSave_TO_Site;

                return sMessage;
        }
        public string OnSave_TO_Master(string Jsonstring, string sStatus, string sDocNo, string sRunningDocNo)
        {
            string sMessage = "";
            try
            {
                DataSet ds = new DataSet();
                DataTable dt = this.JsonToTable(Jsonstring);
                DataRow dr = dt.Rows[0];
                
                dev.lib.SQLConnection devdb = new dev.lib.SQLConnection(Connect);
                SqlConnection cnn = devdb.getSqlConncetion;
                DataSet objds = new DataSet();
                string strsql = "";
                #region header
                strsql = "select * from dbtrans.dbo.TBTrans_PRE_Print_StorageBin where docno = '" + sRunningDocNo + "' ";
                bool ret = devdb.ExecuteCommand(ref objds, strsql, "hd", ref cnn);
                if (ret)
                {
                    DataRow newrow = objds.Tables["hd"].NewRow();
                    if (objds.Tables["hd"].Rows.Count > 0)
                        newrow = objds.Tables["hd"].Rows[0];
                    newrow["docno"] = sRunningDocNo;

                    DateTime oDate = DateTime.Now.Date;
                    string sDate = oDate.ToString("yyyy/MM/dd", new CultureInfo("en-US"));

                    newrow["docdate"] = sDate;
                    newrow["branchcode"] = dr["BRANCHCODE"].ToString();
                    newrow["status"] = "Wait";
                    newrow["label_type"] = dr["LABEL_TYPE"].ToString();
                    // ตรงนี้ให้ใส่เงื่อนไข ให้บันทึกค่าเฉพาะเมื่อมีการสร้างเอกสารครั้งแรกเท่านั้น
                    if (sStatus == "AddNew")
                    {
                        newrow["status"] = "Wait";
                        newrow["createdate"] = DateTime.Now;
                        newrow["createuser"] = dr["CREATEUSER"].ToString();
                    }
                    //
                    if (objds.Tables["hd"].Rows.Count <= 0)
                        objds.Tables["hd"].Rows.Add(newrow);
                }
                #endregion

                #region details
                if (ret)
                {
                    strsql = "select top 0 * from dbtrans.dbo.TBTrans_PRE_Print_StorageBinSub where docno = '" + sRunningDocNo + "' ";
                    ret = devdb.ExecuteCommand(ref objds, strsql, "dt", ref cnn);
                    if (ret)
                    {
                        int index = 1;
                        foreach (DataRow drow in dt.Rows)
                        {
                            DataRow newrow = objds.Tables["dt"].NewRow();
                            newrow["docno"] = sRunningDocNo;

                            DateTime oDate = DateTime.Now.Date;
                            string sDate = oDate.ToString("yyyy/MM/dd", new CultureInfo("en-US"));

                            newrow["docdate"] = sDate;
                            newrow["roworder"] = index;
                            newrow["productcode"] = drow["PRODUCTCODE"];
                            newrow["productname"] = drow["PRODUCTNAME"];
                            newrow["sitecode"] = drow["SITECODE"];
                            newrow["sloc"] = drow["SLOC"];
                            newrow["storagebin"] = drow["STORAGEBIN"];
                            newrow["roworder_location"] = drow["ROWORDER_LOCATION"];
                            newrow["maxstock"] = drow["MAXSTOCK"];
                            newrow["unitname"] = drow["UNITNAME"];

                            objds.Tables["dt"].Rows.Add(newrow);

                            index++;
                        }
                    }
                }
                #endregion

                if (ret)
                    ret = devdb.BeginTrans(ref cnn);
                if (ret)
                {
                    strsql = "select * from dbtrans.dbo.TBTrans_PRE_Print_StorageBin where docno = '" + sRunningDocNo + "' ";
                    ret = devdb.ExcuteDataAdapterUpdate(ref objds, "hd", strsql, ref cnn);
                }
                if (ret)
                {
                    strsql = "delete from dbtrans..TBTrans_PRE_Print_StorageBinSub where docno = '" + sRunningDocNo + "' ";
                    ret = devdb.ExecuteNoneQuery(strsql, ref cnn);
                    if (ret)
                    {
                        strsql = "select * from dbtrans.dbo.TBTrans_PRE_Print_StorageBinSub where docno = '" + sRunningDocNo + "' ";
                        ret = devdb.ExcuteDataAdapterUpdate(ref objds, "dt", strsql, ref cnn);
                    }
                }
                if (ret)
                    ret = devdb.CommitTrans(ref cnn);
                if (!ret)
                    devdb.RollbackTrans(ref cnn);

                if (ret)
                    sMessage = "Saved successfully." + Environment.NewLine + "Document No. : " + sRunningDocNo;
                if (!ret)
                    sMessage = "Save failed";
            }
            catch (Exception ex)
            {
                sMessage = "Error : " + ex.Message.ToString();
            }

            return sMessage;
        }
        public string OnSave_TO_Site(string Jsonstring, string sStatus,string sRunningDocNo, string site)
        {
            string sMessage = "";
            conn = configserver.StringConn(site);
            try
            {
                DataSet ds = new DataSet();
                DataTable dt = this.JsonToTable(Jsonstring);
                DataRow dr = dt.Rows[0];

                dev.lib.SQLConnection devdb = new dev.lib.SQLConnection(conn);
                SqlConnection cnn = devdb.getSqlConncetion;
                DataSet objds = new DataSet();
                string strsql = "";
                #region header
                strsql = "select * from dbtrans.dbo.TBTrans_PRE_Print_StorageBin where docno = '" + sRunningDocNo + "' ";
                bool ret = devdb.ExecuteCommand(ref objds, strsql, "hd", ref cnn);
                if (ret)
                {
                    DataRow newrow = objds.Tables["hd"].NewRow();
                    if (objds.Tables["hd"].Rows.Count > 0)
                        newrow = objds.Tables["hd"].Rows[0];
                    newrow["docno"] = sRunningDocNo;

                    DateTime oDate = DateTime.Now.Date;
                    string sDate = oDate.ToString("yyyy/MM/dd", new CultureInfo("en-US"));

                    newrow["docdate"] = sDate;   //DateTime.Now.Date;
                    newrow["branchcode"] = dr["BRANCHCODE"].ToString();
                    newrow["status"] = "Wait";
                    newrow["label_type"] = dr["LABEL_TYPE"].ToString();
                    // ตรงนี้ให้ใส่เงื่อนไข ให้บันทึกค่าเฉพาะเมื่อมีการสร้างเอกสารครั้งแรกเท่านั้น
                    if (sStatus == "AddNew")
                    {
                        newrow["status"] = "Wait";
                        newrow["createdate"] = DateTime.Now;
                        newrow["createuser"] = dr["CREATEUSER"].ToString();
                    }
                    //
                    if (objds.Tables["hd"].Rows.Count <= 0)
                        objds.Tables["hd"].Rows.Add(newrow);
                }
                #endregion

                #region details
                if (ret)
                {
                    strsql = "select top 0 * from dbtrans.dbo.TBTrans_PRE_Print_StorageBinSub where docno = '" + sRunningDocNo + "' ";
                    ret = devdb.ExecuteCommand(ref objds, strsql, "dt", ref cnn);
                    if (ret)
                    {
                        int index = 1;
                        foreach (DataRow drow in dt.Rows)
                        {
                            DataRow newrow = objds.Tables["dt"].NewRow();
                            newrow["docno"] = sRunningDocNo;

                            DateTime oDate = DateTime.Now.Date;
                            string sDate = oDate.ToString("yyyy/MM/dd", new CultureInfo("en-US"));

                            newrow["docdate"] = sDate; //DateTime.Now.Date;
                            newrow["roworder"] = index;
                            newrow["productcode"] = drow["PRODUCTCODE"];
                            newrow["productname"] = drow["PRODUCTNAME"];
                            newrow["sitecode"] = drow["SITECODE"];
                            newrow["sloc"] = drow["SLOC"];
                            newrow["storagebin"] = drow["STORAGEBIN"];
                            newrow["roworder_location"] = drow["ROWORDER_LOCATION"];
                            newrow["maxstock"] = drow["MAXSTOCK"];
                            newrow["unitname"] = drow["UNITNAME"];

                            objds.Tables["dt"].Rows.Add(newrow);

                            index++;
                        }
                    }
                }
                #endregion

                if (ret)
                    ret = devdb.BeginTrans(ref cnn);
                if (ret)
                {
                    strsql = "select * from dbtrans.dbo.TBTrans_PRE_Print_StorageBin where docno = '" + sRunningDocNo + "' ";
                    ret = devdb.ExcuteDataAdapterUpdate(ref objds, "hd", strsql, ref cnn);
                }
                if (ret)
                {
                    strsql = "delete from dbtrans..TBTrans_PRE_Print_StorageBinSub where docno = '" + sRunningDocNo + "' ";
                    ret = devdb.ExecuteNoneQuery(strsql, ref cnn);
                    if (ret)
                    {
                        strsql = "select * from dbtrans.dbo.TBTrans_PRE_Print_StorageBinSub where docno = '" + sRunningDocNo + "' ";
                        ret = devdb.ExcuteDataAdapterUpdate(ref objds, "dt", strsql, ref cnn);
                    }
                }
                if (ret)
                    ret = devdb.CommitTrans(ref cnn);
                if (!ret)
                    devdb.RollbackTrans(ref cnn);

                if (ret)
                    sMessage = "Saved successfully." + Environment.NewLine + "Document No. : " + sRunningDocNo;
                if (!ret)
                    sMessage = "Save failed";
            }
            catch (Exception ex)
            {
                sMessage = "Error : " + ex.Message.ToString();
            }

            return sMessage;
        }
        public string OnSave_TO_Site_LINQ(string Jsonstring, string sRunningDocNo, string site)
        {
            DBTRANSDataContext dBTRANS = new DBTRANSDataContext();

            string sMessage = "";
            conn = configserver.StringConn(site);
            dBTRANS.Connection.ConnectionString = conn;
            DataTable dtable = this.JsonToTable(Jsonstring);
            DataRow dr = dtable.Rows[0];

            int i = 0;
            TBTrans_PRE_Print_StorageBin hd = new TBTrans_PRE_Print_StorageBin();
            TBTrans_PRE_Print_StorageBinSub dt;
            List<TBTrans_PRE_Print_StorageBinSub> list_dt = new List<TBTrans_PRE_Print_StorageBinSub>();

            if (dtable != null)
            {
                //Save                       

                hd.DOCNO = sRunningDocNo;
                hd.DOCTYPE = "MM";
                hd.DOCDATE = DateTime.Now.Date;
                hd.BRANCHCODE = dr["BRANCHCODE"].ToString();
                hd.LABEL_TYPE = dr["LABEL_TYPE"].ToString();
                hd.STATUS = "Wait";
                hd.CREATEDATE = DateTime.Now;
                hd.CREATEUSER = dr["CREATEUSER"].ToString();
                hd.PRINT_NUMBER = 0;
                //hd.PRINTDATE = "";
                //hd.PRINTUSER = "";


                foreach (DataRow drow in dtable.Rows)
                {
                    dt = new TBTrans_PRE_Print_StorageBinSub();
                    dt.DOCNO = sRunningDocNo;
                    dt.DOCDATE = DateTime.Now.Date;
                    dt.ROWORDER = i;
                    dt.ROWORDER_LOCATION = drow["ROWORDER_LOCATION"].ToString();
                    dt.PRODUCTCODE = drow["PRODUCTCODE"].ToString();
                    dt.PRODUCTNAME = drow["PRODUCTNAME"].ToString();
                    dt.SITECODE = drow["SITECODE"].ToString();
                    //dt.WAREHOUSE_NO = "";
                    dt.SLOC = drow["SLOC"].ToString();
                    dt.STORAGEBIN = drow["STORAGEBIN"].ToString();
                    dt.MAXSTOCK = drow["MAXSTOCK"].ToString();
                    dt.UNITNAME = drow["UNITNAME"].ToString();
                    list_dt.Add(dt);
                    i++;
                }
            }
            else
            {
                sMessage = "ไม่พบข้ฮมูลที่ส่งมาบันทึก";
            }

            try
            {
                dBTRANS.Connection.Open();
                dBTRANS.Transaction = dBTRANS.Connection.BeginTransaction();
                dBTRANS.TBTrans_PRE_Print_StorageBins.InsertOnSubmit(hd);
                dBTRANS.TBTrans_PRE_Print_StorageBinSubs.InsertAllOnSubmit(list_dt);
                dBTRANS.SubmitChanges();
                dBTRANS.Transaction.Commit();
                dBTRANS.Connection.Close();
                dBTRANS.Connection.Dispose();
                sMessage = "Saved successfully." + Environment.NewLine + "Document No. : " + sRunningDocNo;

            }
            catch (Exception ex)
            {                
                dBTRANS.Transaction.Rollback();
                dBTRANS.Connection.Close();
                dBTRANS.Connection.Dispose();
                sMessage = "Error : " + ex.Message.ToString();
            }

            return sMessage;
        }

        [WebMethod]
        public string OnDelete(string sDocNo)
        {
            string sMessage = "";
            try
            {
                dev.lib.SQLConnection devdb = new dev.lib.SQLConnection(Connect);
                SqlConnection cnn = devdb.getSqlConncetion;
                DataSet objds = new DataSet();
                string strsql = "";
                bool ret = devdb.BeginTrans(ref cnn);
                if (ret)
                {
                    strsql = "delete from dbtrans..TBTrans_PRE_Print_StorageBin where docno = '" + sDocNo + "' ";
                    ret = devdb.ExecuteNoneQuery(strsql, ref cnn);
                }
                if (ret)
                {
                    strsql = "delete from dbtrans..TBTrans_PRE_Print_StorageBinSub where docno = '" + sDocNo + "' ";
                    ret = devdb.ExecuteNoneQuery(strsql, ref cnn);
                }
                if (ret)
                    ret = devdb.CommitTrans(ref cnn);
                if (!ret)
                    ret = devdb.RollbackTrans(ref cnn);
                if (ret)
                {
                    sMessage = "Delete Successfully";
                }
            }
            catch (Exception ex)
            {
                sMessage = "Error : " + ex.Message.ToString();
            }
            return sMessage;
        }

        [WebMethod]
        public string OnSearchByDocNo(string sDocNo ,string site)
        {
            conn = configserver.StringConn(site);
            DataSet ds = new DataSet();
            DataSet objds = new DataSet();
            try
            {
                dev.lib.SQLConnection devdb = new dev.lib.SQLConnection(conn);
                SqlConnection cnn = devdb.getSqlConncetion;
                string strsql = "select srg.DOCNO, srg.BRANCHCODE, srg.DOCDATE, srg.LABEL_TYPE, srg.CREATEUSER, srg.STATUS,"
                                + " srgSub.SITECODE, srgSub.SLOC, srgSub.STORAGEBIN, srgSub.PRODUCTCODE, srgSub.PRODUCTNAME,"
                                + " srgSub.ROWORDER_LOCATION, srgSub.MAXSTOCK, srgSub.UNITNAME"
                                + " from dbtrans..TBTrans_PRE_Print_StorageBin as srg inner join"
                                + " dbtrans..TBTrans_PRE_Print_StorageBinSub as srgSub on srg.DOCNO = srgSub.DOCNO"
                                + " where srg.DOCNO = '" + sDocNo + "' ";
                bool ret = devdb.ExecuteCommand(ref ds, strsql, "tbl", ref cnn);
                if (ret)
                {
                    int iRowCount = ds.Tables["tbl"].Rows.Count;
                    if (iRowCount > 0)
                    {
                        DataTable dtStrBin = new DataTable();
                        dtStrBin.Columns.Add("DocNo", typeof(string));
                        dtStrBin.Columns.Add("DocDate", typeof(DateTime));
                        dtStrBin.Columns.Add("BranchCode", typeof(string));
                        dtStrBin.Columns.Add("Label_Type", typeof(string));
                        dtStrBin.Columns.Add("Label_Type_NAME", typeof(string));
                        dtStrBin.Columns.Add("UserCode", typeof(string));
                        dtStrBin.Columns.Add("UserName", typeof(string));
                        dtStrBin.Columns.Add("Status", typeof(string));
                        dtStrBin.Columns.Add("SiteCode", typeof(string));
                        dtStrBin.Columns.Add("Sloc", typeof(string));
                        dtStrBin.Columns.Add("Storagebin", typeof(string));
                        dtStrBin.Columns.Add("Productcode", typeof(string));
                        dtStrBin.Columns.Add("ProductName", typeof(string));
                        dtStrBin.Columns.Add("Roworder_location", typeof(string));
                        dtStrBin.Columns.Add("Maxstock", typeof(string));
                        dtStrBin.Columns.Add("Unitname", typeof(string));

                        foreach (DataRow objdr in ds.Tables["tbl"].Rows)
                        {
                            DataRow dr = dtStrBin.NewRow();
                            dr["DocNo"] = objdr["DOCNO"].ToString();
                            dr["DocDate"] = objdr["DOCDATE"].ToString();
                            dr["BranchCode"] = objdr["BRANCHCODE"].ToString();
                            dr["Label_Type"] = objdr["LABEL_TYPE"].ToString();
                            dr["Label_Type_NAME"] = this.Read_Label_Type_NAME( objdr["LABEL_TYPE"].ToString(), site);
                            dr["UserCode"] = objdr["CREATEUSER"].ToString();
                            dr["UserName"] = this.Read_Name(objdr["CREATEUSER"].ToString(), site);
                            dr["Status"] = objdr["STATUS"].ToString();
                            dr["SiteCode"] = objdr["SITECODE"].ToString();
                            dr["sloc"] = objdr["SLOC"].ToString();
                            dr["Storagebin"] = objdr["storagebin"].ToString();
                            dr["productcode"] = objdr["productcode"].ToString();
                            dr["productname"] = objdr["productname"].ToString();
                            dr["roworder_location"] = objdr["roworder_location"].ToString();
                            dr["status"] = objdr["status"].ToString();
                            dr["maxstock"] = objdr["maxstock"].ToString();
                            dr["unitname"] = objdr["unitname"].ToString();
                            dtStrBin.Rows.Add(dr);
                        }
                        dtStrBin.TableName = "TB_PRE_Print_StorageBin";
                        objds.Tables.Add(dtStrBin);
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return ConvertDataSetToJSON(objds);
        }
        private string Read_Label_Type_NAME(string Label_Type,string site)
        {
            string sResult = "";
            conn = configserver.StringConn(site);
            DataSet ds = new DataSet();
            dev.lib.SQLConnection devdb = new dev.lib.SQLConnection(conn);
            SqlConnection cnn = devdb.getSqlConncetion;
            string strsql = "select MYNAME from dbmaster..TBMaster_LabelType where code = '" + Label_Type + "' ";
            bool ret = devdb.ExecuteCommand(ref ds, strsql, "tbl", ref cnn);
            if (ret)
            {
                if (ds.Tables["tbl"].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables["tbl"].Rows[0];
                    sResult = dr["myname"].ToString();
                }
            }
            return sResult;
        }

        [WebMethod]
        public string OnSearchByStatus(string BRANCHCODE, string sDocNo, string sUserName, string sStatus ,string site)
        {
            conn = configserver.StringConn(site);
            string strsql;
            DataSet ds = new DataSet();
            DataSet objds = new DataSet();
            string sDate = DateTime.Now.ToString("yyyy/MM/dd", new CultureInfo("en-US"));
            try
            {
                dev.lib.SQLConnection devdb = new dev.lib.SQLConnection(conn);
                SqlConnection cnn = devdb.getSqlConncetion;

                if (!BRANCHCODE.Equals("") && !sDocNo.Equals("") && sUserName.Equals("") && sStatus.Equals("")) {
                    strsql = "select srg.DOCNO, srg.BRANCHCODE, srg.DOCDATE, srg.LABEL_TYPE, srg.CREATEUSER, srg.STATUS "
                                   //+ " srgSub.SITECODE, srgSub.SLOC, srgSub.STORAGEBIN, srgSub.PRODUCTCODE, srgSub.PRODUCTNAME,"
                                   //+ " srgSub.ROWORDER_LOCATION, srgSub.MAXSTOCK, srgSub.UNITNAME"
                                   + " from dbtrans..TBTrans_PRE_Print_StorageBin as srg"
                                   //+ " inner join dbtrans..TBTrans_PRE_Print_StorageBinSub as srgSub on srg.DOCNO = srgSub.DOCNO"
                                   + " where srg.BRANCHCODE = '" + BRANCHCODE + "' "
                                   + " AND srg.DOCNO like '%" + sDocNo + "%' "
                                   + " AND srg.CREATEUSER like '%" + sUserName + "%' "
                                   + " AND srg.STATUS like '%" + sStatus + "%' ";

                } else {

                    strsql = "select srg.DOCNO, srg.BRANCHCODE, srg.DOCDATE, srg.LABEL_TYPE, srg.CREATEUSER, srg.STATUS "
                                   //+ " srgSub.SITECODE, srgSub.SLOC, srgSub.STORAGEBIN, srgSub.PRODUCTCODE, srgSub.PRODUCTNAME,"
                                   //+ " srgSub.ROWORDER_LOCATION, srgSub.MAXSTOCK, srgSub.UNITNAME"
                                   + " from dbtrans..TBTrans_PRE_Print_StorageBin as srg"
                                   //+ " inner join dbtrans..TBTrans_PRE_Print_StorageBinSub as srgSub on srg.DOCNO = srgSub.DOCNO"
                                   + " where srg.DOCDATE = '" + sDate + "' "
                                   + " AND srg.BRANCHCODE = '" + BRANCHCODE + "' "
                                   + " AND srg.DOCNO like '%" + sDocNo + "%' "
                                   + " AND srg.CREATEUSER like '%" + sUserName + "%' "
                                   + " AND srg.STATUS like '%" + sStatus + "%' ";
                }
                

                bool ret = devdb.ExecuteCommand(ref ds, strsql, "tbl", ref cnn);
                if (ret)
                {
                    int iRowCount = ds.Tables["tbl"].Rows.Count;

                    DataTable dtStrBin = new DataTable();
                    dtStrBin.Columns.Add("DocNo", typeof(string));
                    dtStrBin.Columns.Add("DocDate", typeof(DateTime));
                    dtStrBin.Columns.Add("BranchCode", typeof(string));
                    dtStrBin.Columns.Add("Label_Type", typeof(string));
                    dtStrBin.Columns.Add("UserCode", typeof(string));
                    dtStrBin.Columns.Add("UserName", typeof(string));
                    dtStrBin.Columns.Add("Status", typeof(string));

                    if (iRowCount > 0)
                    {
                        
                        //dtStrBin.Columns.Add("SiteCode", typeof(string));
                        //dtStrBin.Columns.Add("Sloc", typeof(string));
                        //dtStrBin.Columns.Add("Storagebin", typeof(string));
                        //dtStrBin.Columns.Add("Productcode", typeof(string));
                        //dtStrBin.Columns.Add("ProductName", typeof(string));
                        //dtStrBin.Columns.Add("Roworder_location", typeof(string));
                        //dtStrBin.Columns.Add("Maxstock", typeof(string));
                        //dtStrBin.Columns.Add("Unitname", typeof(string));

                        foreach (DataRow objdr in ds.Tables["tbl"].Rows)
                        {
                            DataRow dr = dtStrBin.NewRow();
                            dr["DocNo"] = objdr["DOCNO"].ToString();
                            dr["DocDate"] = objdr["DOCDATE"].ToString();
                            dr["BranchCode"] = objdr["BRANCHCODE"].ToString();
                            dr["Label_Type"] = objdr["LABEL_TYPE"].ToString();
                            dr["UserCode"] = objdr["CREATEUSER"].ToString();
                            dr["UserName"] = this.Read_Name(objdr["CREATEUSER"].ToString(), site);
                            dr["Status"] = objdr["STATUS"].ToString();
                            //dr["SiteCode"] = objdr["SITECODE"].ToString();
                            //dr["sloc"] = objdr["SLOC"].ToString();
                            //dr["Storagebin"] = objdr["storagebin"].ToString();
                            //dr["productcode"] = objdr["productcode"].ToString();
                            //dr["productname"] = objdr["productname"].ToString();
                            //dr["roworder_location"] = objdr["roworder_location"].ToString();
                            //dr["status"] = objdr["status"].ToString();
                            //dr["maxstock"] = objdr["maxstock"].ToString();
                            //dr["unitname"] = objdr["unitname"].ToString();
                            dtStrBin.Rows.Add(dr);
                        }
                        
                    }
                    dtStrBin.TableName = "TB_PRE_Print_StorageBin";
                    objds.Tables.Add(dtStrBin);
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return ConvertDataSetToJSON(objds);
        }
        private string Read_Name(string sUserName ,string site)
        {
            string sResult = "";
            conn = configserver.StringConn(site);
            DataSet ds = new DataSet();
            dev.lib.SQLConnection devdb = new dev.lib.SQLConnection(conn);
            SqlConnection cnn = devdb.getSqlConncetion;
            string strsql = "select code, myname from [DBMASTER].[dbo].[TBMaster_User] where code = '" + sUserName + "' ";
            bool ret = devdb.ExecuteCommand(ref ds, strsql, "tbl", ref cnn);
            if (ret)
            {
                if (ds.Tables["tbl"].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables["tbl"].Rows[0];
                    sResult = dr["myname"].ToString();
                }
            }
            return sResult;
        }

        [WebMethod]
        public string OnSearchByArticle(string SiteCode, string Sloc, string BinCode, string ProductCode)
        {
            DataSet ds = new DataSet();
            try
            {
                if (SiteCode != "" && Sloc != "") // SiteCode และ Sloc บังคับให้ระบุ
                {
                    SmartObjectClientServer serverName = K2_CONNECT_SERVER();// Connect K2 Server 
                    SmartObject smartObject = serverName.GetSmartObject("ZHH_GET_BIN_ASSIGNLOC");//ชื่อ SmartOject
                    smartObject.MethodToExecute = "ZHH_GET_BINASSIGNLOC_ZHH_GET_BIN_ASSIGNLOC";
                    //Set parameter
                    if (ProductCode != "")
                        smartObject.ListMethods["ZHH_GET_BINASSIGNLOC_ZHH_GET_BIN_ASSIGNLOC"].InputProperties["p_I_MATNR"].Value = ProductCode;
                    if (BinCode != "")
                        smartObject.ListMethods["ZHH_GET_BINASSIGNLOC_ZHH_GET_BIN_ASSIGNLOC"].InputProperties["p_I_BINCODE"].Value = BinCode;
                    smartObject.ListMethods["ZHH_GET_BINASSIGNLOC_ZHH_GET_BIN_ASSIGNLOC"].InputProperties["p_I_WERKS"].Value = SiteCode;
                    smartObject.ListMethods["ZHH_GET_BINASSIGNLOC_ZHH_GET_BIN_ASSIGNLOC"].InputProperties["p_I_LGORT"].Value = Sloc;
                    DataTable dt = serverName.ExecuteListDataTable(smartObject);

                    dt.Columns.Add("Site");
                    dt.Columns.Add("Sloc");
                    dt.Columns.Add("StorageBin");
                    dt.Columns.Add("ArticleCode");
                    dt.Columns.Add("ArticleName");
                    dt.Columns.Add("roworder");
                    dt.Columns.Add("PriceDate");
                    foreach (DataRow row in dt.Rows)
                    {
                        DateTime dtPriceDate = DateTime.Today;

                        row["O_ASSIGNLOC_MATERIAL"] = row["O_ASSIGNLOC_MATERIAL"].ToString().TrimStart('0');

                        row["Site"] = SiteCode;
                        row["Sloc"] = Sloc;
                        row["StorageBin"] = row["O_ASSIGNLOC_BIN_CODE"].ToString();
                        row["ArticleCode"] = row["O_ASSIGNLOC_MATERIAL"].ToString();
                        row["ArticleName"] = this.Read_Product_Name(row["O_ASSIGNLOC_MATERIAL"].ToString(), SiteCode);
                        row["roworder"] = row["O_ASSIGNLOC_ROWORDER"].ToString();
                        row["PriceDate"] = dtPriceDate;
                    }
                    ds.Tables.Add(dt);
                    serverName.Connection.Close();
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return ConvertDataSetToJSON(ds);
        }

        [WebMethod]
        public string OnSearchByStorageBin(string StorageBin, string Site, string Sloc)
        {
            try
            {
                SmartObjectClientServer serverName = K2_CONNECT_SERVER();
                SmartObject smartObject = serverName.GetSmartObject("ZGET_ZLOCSTRC");

                DataSet objds = new DataSet();

                smartObject.MethodToExecute = "ZGETZLOCSTRC_ZGET_ZLOCSTRC";
                if (StorageBin != "")
                    smartObject.ListMethods["ZGETZLOCSTRC_ZGET_ZLOCSTRC"].InputProperties["P_BINLOC"].Value = this.ZGET_ZLOCSTRC_P_BINLOC(StorageBin.ToUpper());
                if (Site != "")
                    smartObject.ListMethods["ZGETZLOCSTRC_ZGET_ZLOCSTRC"].InputProperties["P_WERKS"].Value = this.ZGET_ZLOCSTRC_P_WERKS(Site);
                if (Sloc != "")
                    smartObject.ListMethods["ZGETZLOCSTRC_ZGET_ZLOCSTRC"].InputProperties["P_LGORT"].Value = this.ZGET_ZLOCSTRC_P_LGORT(Sloc);

                DataTable dt = serverName.ExecuteListDataTable(smartObject);

                DataTable dt_ret = new DataTable();
                dt_ret.Columns.Add("SITE");
                dt_ret.Columns.Add("SLOC");
                dt_ret.Columns.Add("STORAGE_BIN");

                foreach (DataRow row in dt.Rows)
                {
                    if (!String.IsNullOrEmpty(row["IT_DATA_BINLOC"].ToString()))
                    {
                        DataRow new_row = dt_ret.NewRow();
                        new_row["SITE"] = row["IT_DATA_WERKS"].ToString();
                        new_row["SLOC"] = row["IT_DATA_LGORT"].ToString();
                        new_row["STORAGE_BIN"] = row["IT_DATA_BINLOC"].ToString();

                        dt_ret.Rows.Add(new_row);
                    }
                }

                if (dt_ret.Rows.Count > 0)
                {
                    dt_ret.TableName = "TB_StorageBin";
                    objds.Tables.Add(dt_ret);

                    string str_result = ConvertDataSetToJSON(objds);

                    return str_result;
                }
                else
                {
                    return "No Data";
                }

            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }
        private string ZGET_ZLOCSTRC_P_BINLOC(string StorageBin)
        {
            try
            {
                SmartObjectClientServer serverName = K2_CONNECT_SERVER();
                SmartObject smartObject = serverName.GetSmartObject("ZGET_ZLOCSTRC_P_BINLOC");
                smartObject.MethodToExecute = "Serialize";
                SetPropertyValue(smartObject.Methods["Serialize"].InputProperties["SIGN"], "I");
                SetPropertyValue(smartObject.Methods["Serialize"].InputProperties["OPTION"], "CP");
                SetPropertyValue(smartObject.Methods["Serialize"].InputProperties["LOW"], String.IsNullOrEmpty(StorageBin) ? "*" : StorageBin);
                SetPropertyValue(smartObject.Methods["Serialize"].InputProperties["HIGH"], "");

                SmartObject returnSmartObject;
                returnSmartObject = serverName.ExecuteScalar(smartObject);
                string result_xml = returnSmartObject.Properties[4].Value;

                return result_xml;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private string ZGET_ZLOCSTRC_P_WERKS(string Site)
        {
            try
            {
                SmartObjectClientServer serverName = K2_CONNECT_SERVER();
                SmartObject smartObject = serverName.GetSmartObject("ZGET_ZLOCSTRC_P_WERKS");
                smartObject.MethodToExecute = "Serialize";
                SetPropertyValue(smartObject.Methods["Serialize"].InputProperties["SIGN"], "I");
                SetPropertyValue(smartObject.Methods["Serialize"].InputProperties["OPTION"], "EQ");
                SetPropertyValue(smartObject.Methods["Serialize"].InputProperties["LOW"], Site);
                SetPropertyValue(smartObject.Methods["Serialize"].InputProperties["HIGH"], "");

                SmartObject returnSmartObject;
                returnSmartObject = serverName.ExecuteScalar(smartObject);
                string result_xml = returnSmartObject.Properties[4].Value;

                return result_xml;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private string ZGET_ZLOCSTRC_P_LGORT(string Sloc)
        {
            try
            {
                SmartObjectClientServer serverName = K2_CONNECT_SERVER();
                SmartObject smartObject = serverName.GetSmartObject("ZGET_ZLOCSTRC_P_LGORT");
                smartObject.MethodToExecute = "Serialize";
                SetPropertyValue(smartObject.Methods["Serialize"].InputProperties["SIGN"], "I");
                SetPropertyValue(smartObject.Methods["Serialize"].InputProperties["OPTION"], "EQ");
                SetPropertyValue(smartObject.Methods["Serialize"].InputProperties["LOW"], Sloc);
                SetPropertyValue(smartObject.Methods["Serialize"].InputProperties["HIGH"], "");

                SmartObject returnSmartObject;
                returnSmartObject = serverName.ExecuteScalar(smartObject);
                string result_xml = returnSmartObject.Properties[4].Value;

                return result_xml;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private double OnGetSalePrice(string sSiteCode, string ProductCode, string UnitCode, string PaymentCode)
        {
            conn = configserver.StringConn(sSiteCode);
            double sResult = 0;
            DataSet ds = new DataSet();
            dev.lib.SQLConnection devdb = new dev.lib.SQLConnection(conn);
            SqlConnection cnn = devdb.getSqlConncetion;
            string strsql = "select top 1 saleprice ";
            strsql += "from TBMaster_Sale_Price ";
            strsql += "where sitecode='" + sSiteCode + "' ";
            strsql += "      and productcode='" + ProductCode + "' ";
            strsql += "      and unitcode='" + UnitCode + "' ";
            strsql += "      and paymentcode='" + PaymentCode + "' ";
            strsql += "      and {fn curdate()} between begindate and enddate ";
            strsql += "order by createdate desc";
            bool ret = devdb.ExecuteCommand(ref ds, strsql, "tbl", ref cnn);
            if (ret)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables["tbl"].Rows[0];
                    sResult = Convert.ToDouble(dr["saleprice"].ToString());
                }
            }
            return sResult;

        }
        private string Read_Barcode(string sProductCode, string sUnitCode,string site)
        {
            conn = configserver.StringConn(site);
            string sResult = "";
            dev.lib.SQLConnection devdb = new dev.lib.SQLConnection(conn);
            SqlConnection cnn = devdb.getSqlConncetion;
            DataSet ds = new DataSet();
            string strsql = "select *from DBMASTER..TBMaster_Barcode where productcode = '" + sProductCode + "' and unitcode = '" + sUnitCode + "'";
            bool ret = devdb.ExecuteCommand(ref ds, strsql, "tbl", ref cnn);
            cnn.Close();
            if (ret)
            {
                if (ds.Tables["tbl"].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables["tbl"].Rows[0];
                    sResult = dr["barcode"].ToString();
                }
            }


            return sResult;
        }
        private string Read_Product_Name(string sProductCode,string site)
        {
            conn = configserver.StringConn(site);
            string sResult = "";
            dev.lib.SQLConnection devdb = new dev.lib.SQLConnection(conn);
            SqlConnection cnn = devdb.getSqlConncetion;
            DataSet ds = new DataSet();
            string strsql = "select *from DBMASTER..TBMaster_Product where code = '" + sProductCode + "' ";
            bool ret = devdb.ExecuteCommand(ref ds, strsql, "tbl", ref cnn);
            cnn.Close();
            if (ret)
            {
                DataRow dr = ds.Tables["tbl"].Rows[0];
                sResult = dr["NAMETH"].ToString();
            }

            return sResult;
        }
        private string Read_Unit_Name(string sUnitCode,string site)
        {
            conn = configserver.StringConn(site);
            string sResult = "";
            dev.lib.SQLConnection devdb = new dev.lib.SQLConnection(conn);
            SqlConnection cnn = devdb.getSqlConncetion;
            DataSet ds = new DataSet();
            string strsql = "select *from DBMaster..TBMaster_Unit where code = '" + sUnitCode + "' ";
            bool ret = devdb.ExecuteCommand(ref ds, strsql, "tbl", ref cnn);
            if (ret)
            {
                if (ds.Tables["tbl"].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables["tbl"].Rows[0];
                    sResult = dr["myname"].ToString();
                }
            }

            return sResult;
        }
        private string RunningNewDocNo(string site)
        {
            conn = configserver.StringConn(site);
            string sResult = "";
            string sShortName;
            dev.lib.SQLConnection devdb = new dev.lib.SQLConnection(Connect);
            dev.lib.UtilityDataBase dbutil = new dev.lib.UtilityDataBase();
            dbutil.ConnectionString = Connect;
            dbutil.RunningFieldName = "DOCNO";
            string sformat = "yyyyMMdd-####";
            //string sShortName = this.getBranchShortName(sBranchCode); // 

            string substr = site.ToUpper().Substring(0, 2);
            if (substr == "M0")
            {
                sShortName = site;
            }
            else
            {
                sShortName = substr;
            }

            dbutil.RunningGroup = sShortName + "SB-HH-";
            dbutil.RunningFormat = sformat;
            dbutil.RunningTableName = "dbtrans.dbo.TBTrans_PRE_Print_StorageBin";
            sResult = dbutil.RunningNewDocNo(devdb);
            return sResult;
        }

        //private string getBranchShortName(string sBranchCode)
        //{
        //    //conn = StringConn(site);
        //    string sResult = "";
        //    dev.lib.SQLConnection devdb = new dev.lib.SQLConnection(Connect);
        //    SqlConnection cnn = devdb.getSqlConncetion;
        //    DataSet objds = new DataSet();
        //    string strsql = "select * from dbmaster..tbmaster_branch where CODE = '" + sBranchCode + "' ";
        //    bool ret = devdb.ExecuteCommand(ref objds, strsql, "tbl", ref cnn);
        //    cnn.Close();
        //    if (ret)
        //    {
        //        if (objds.Tables["tbl"].Rows.Count > 0)
        //        {
        //            sResult = objds.Tables["tbl"].Rows[0]["short_name"].ToString();
        //        }
        //    }
        //    return sResult;
        //}
        private static void SetPropertyValue(SmartProperty smartProperty, object value)
        {
            if (value == null)
            {
                smartProperty.ValueBehaviour = ValueBehaviour.Unchanged;
                smartProperty.Value = null;
            }
            else if (value == DBNull.Value)
            {
                smartProperty.ValueBehaviour = ValueBehaviour.Clear;
                smartProperty.Value = null;
            }
            else if (value.ToString() == string.Empty)
            {
                smartProperty.ValueBehaviour = ValueBehaviour.Empty;
                smartProperty.Value = string.Empty;
            }
            else
            {
                smartProperty.ValueBehaviour = ValueBehaviour.None;
                smartProperty.Value = value.ToString();
            }
        }
        public string ConvertDataSetToJSON(DataSet ds)
        {
            string JSONString = string.Empty;
            if (ds != null)
            {
                return JsonConvert.SerializeObject(ds, Formatting.Indented);
            }
            else
            {
                return JSONString;
            }
        }
        public DataTable JsonToTable(String JsonString)
        {
            DataTable dt = (DataTable)JsonConvert.DeserializeObject(JsonString, typeof(DataTable));
            return dt;
        }       
    }
}
