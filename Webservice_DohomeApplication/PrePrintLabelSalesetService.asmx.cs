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
    /// Summary description for PrePrintLabelSalesetService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class PrePrintLabelSalesetService : System.Web.Services.WebService
    {

        public string Connect = System.Configuration.ConfigurationManager.ConnectionStrings["DBMASTERConnectionString"].ConnectionString;
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
        public struct SalesetDetails
        {
            public DateTime pricedate;
            public DateTime component_pricedate;
        }

        [WebMethod]
        public string OnSave(string JsonstringHD, string JsonstringDT, string sStatus, string sDocNo, string sBranchCode ,string site)//เพิ่่มส่ง Site เข้ามาเพิ่ม
        {
            string sMessage = "";
            string sRunningDocNo = "";
            string stringcon = configserver.StringConn(site);
            try
            {
                // Header
                DataTable dtHD = this.JsonToTable(JsonstringHD);
                DataRow drHD = dtHD.Rows[0];

                // Detail
                DataTable dtDT = this.JsonToTable(JsonstringDT);
                DataRow drDT = dtDT.Rows[0];


                if (sStatus == "AddNew" && sDocNo == "")
                {
                    sRunningDocNo = this.RunningNewDocNo(sBranchCode, site); // ส่งรหัสสาขาเข้าไปเพื่อนำไป Get ข้อมูลที่ Field SHORT_NAME (ให้ดูที่ฟังก์ชั่น : getBranchShortName)
                }
                if (sStatus == "Edit" && sDocNo != "")
                {
                    sRunningDocNo = sDocNo;
                }
                //dev.lib.SQLConnection devdb = new dev.lib.SQLConnection(Connect);
                dev.lib.SQLConnection devdb = new dev.lib.SQLConnection(stringcon);
                SqlConnection cnn = devdb.getSqlConncetion;
                DataSet objds = new DataSet();
                string strsql = "";
                #region header
                strsql = "select * from dbtrans.dbo.TBTrans_PRE_Print_Label_saleset where docno = '" + sRunningDocNo + "' ";
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
                    newrow["branchcode"] = drHD["BRANCHCODE"].ToString(); // เปลี่ยน
                    newrow["sitecode"] = drHD["SITECODE"].ToString(); // เปลี่ยน
                    newrow["sloc"] = drHD["SLOC"].ToString(); // เปลี่ยน
                    newrow["request_user"] = drHD["REQUEST_USER"].ToString(); //เปลี่ยน
                    newrow["status"] = "Wait";
                    newrow["label_type"] = drHD["LABEL_TYPE"].ToString(); // เปลี่ยน
                    newrow["cause_of_request"] = drHD["CAUSE_OF_REQUEST"].ToString(); // เปลี่ยน
                    // ตรงนี้ให้ใส่เงื่อนไข ให้บันทึกค่าเฉพาะเมื่อมีการสร้างเอกสารครั้งแรกเท่านั้น
                    if (sStatus == "AddNew")
                    {
                        newrow["create_user"] = drHD["CREATE_USER"].ToString();
                    }
                    if (objds.Tables["hd"].Rows.Count <= 0)
                        objds.Tables["hd"].Rows.Add(newrow);
                }
                #endregion

                #region details
                // ref pricedate
                SalesetDetails dtSale = new SalesetDetails();
                if (ret)
                {
                    strsql = "select top 0 * from dbtrans.dbo.tbtrans_pre_print_label_saleset_sub where docno = '" + sRunningDocNo + "' ";
                    ret = devdb.ExecuteCommand(ref objds, strsql, "dt", ref cnn);
                    if (ret)
                    {
                        int Index = 0;
                        int sLastParentRowOrder = 0;
                        foreach (DataRow drow in dtDT.Rows)
                        {
                            DataRow newrow = objds.Tables["dt"].NewRow();
                            newrow["docno"] = sRunningDocNo;

                            DateTime oDate = DateTime.Now.Date;
                            string sDate = oDate.ToString("yyyy/MM/dd", new CultureInfo("en-US"));

                            newrow["docdate"] = sDate;
                            newrow["rowtype"] = drow["rowtype"].ToString();
                            newrow["roworder"] = Index;
                            if (drow["rowtype"].ToString() == "PARENT")
                            {
                                sLastParentRowOrder = Index;
                                newrow["child_roworder"] = newrow["roworder"];
                            }
                            else
                            {
                                newrow["child_roworder"] = sLastParentRowOrder;
                            }
                            //Parent
                            newrow["productcode"] = drow["ProductCode"].ToString();
                            newrow["productname"] = drow["ProductName"].ToString();
                            newrow["barcode"] = drow["Barcode"].ToString();
                            newrow["unitcode"] = drow["UNITCODE"].ToString();
                            newrow["unitprice"] = drow["unitprice"].ToString();
                            if (Convert.ToDouble(drow["Unitprice"]) > 0)
                                newrow["pricedate"] = DateTime.Now;
                            newrow["quantity"] = drow["Quantity"].ToString();
                            newrow["location"] = drow["Location"].ToString();
                            newrow["matkl"] = "";
                            //Child
                            newrow["component_code"] = drow["Component_code"].ToString();
                            newrow["component_name"] = drow["Component_name"].ToString();
                            newrow["component_barcode"] = drow["Component_barcode"].ToString();
                            newrow["component_unitcode"] = drow["Component_unitcode"].ToString();
                            newrow["component_unitprice"] = drow["Component_unitprice"].ToString();
                            if (Convert.ToDouble(drow["Component_unitprice"].ToString()) > 0)
                                newrow["component_pricedate"] = DateTime.Now;
                            newrow["component_quantity"] = drow["Component_quantity"].ToString();

                            newrow["branchcode"] = sBranchCode; // ให้แก้ไขให้ตรงตามรหัสสาขาที่ถูกส่งเข้ามา ตัวอย่าง dr["BranchCode"].ToString
                            newrow["sitecode"] = drow["SITECODE"].ToString(); // ให้แก้ไขให้ตรงตาม Site ที่ถูกส่งเข้ามา
                            newrow["sloc"] = drow["SLOC"].ToString(); // ให้แก้ไขให้ตรงตาม Sloc ที่ถูกส่งเข้ามา
                            objds.Tables["dt"].Rows.Add(newrow);
                            Index++;
                        }
                    }
                }
                #endregion
                if (ret)
                    ret = devdb.BeginTrans(ref cnn);
                if (ret)
                {
                    strsql = "select * from dbtrans.dbo.tbtrans_pre_print_label_saleset where docno = '" + sRunningDocNo + "' ";
                    ret = devdb.ExcuteDataAdapterUpdate(ref objds, "hd", strsql, ref cnn);
                }
                if (ret)
                {
                    strsql = "delete from dbtrans..tbtrans_pre_print_label_saleset_sub where docno = '" + sRunningDocNo + "' ";
                    ret = devdb.ExecuteNoneQuery(strsql, ref cnn);
                    if (ret)
                    {
                        strsql = "select * from dbtrans.dbo.tbtrans_pre_print_label_saleset_sub where docno = '" + sRunningDocNo + "' ";
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

        [WebMethod]
        public string GetLabelType()
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            dev.lib.SQLConnection devdb = new dev.lib.SQLConnection(Connect);
            SqlConnection cnn = devdb.getSqlConncetion;
            string strsql = "select * from dbmaster..tbmaster_print_label_type where ltype = 2";
            bool ret = devdb.ExecuteCommand(ref ds, strsql, "tbl", ref cnn);
            if (ret)
            {
                if (ds.Tables["tbl"].Rows.Count > 0)
                {
                    dt = ds.Tables["tbl"];
                }
            }
            return ConvertDataSetToJSON(ds);
        }

        [WebMethod]
        public string Read_Storage_Bin(string sProductCode, string sSiteCode, string sSloc)
        {
            try
            {
                SmartObjectClientServer serverName = K2_CONNECT_SERVER();// Connect K2 Server 
                SmartObject smartObject = serverName.GetSmartObject("ZHH_GET_BIN_ASSIGNLOC");//ชื่อ SmartOject
                smartObject.MethodToExecute = "ZHH_GET_BINASSIGNLOC_ZHH_GET_BIN_ASSIGNLOC";
                //Set parameter
                if (sProductCode != "")
                    smartObject.ListMethods["ZHH_GET_BINASSIGNLOC_ZHH_GET_BIN_ASSIGNLOC"].InputProperties["p_I_MATNR"].Value = sProductCode;
                //smartObject.ListMethods["ZHH_GET_BINASSIGNLOC_ZHH_GET_BIN_ASSIGNLOC"].InputProperties["p_I_BINCODE"].Value = BinCode.ToUpper();
                smartObject.ListMethods["ZHH_GET_BINASSIGNLOC_ZHH_GET_BIN_ASSIGNLOC"].InputProperties["p_I_WERKS"].Value = sSiteCode;
                smartObject.ListMethods["ZHH_GET_BINASSIGNLOC_ZHH_GET_BIN_ASSIGNLOC"].InputProperties["p_I_LGORT"].Value = sSloc;
                DataTable dt = serverName.ExecuteListDataTable(smartObject);
                DataSet ds = new DataSet();
                ds.Tables.Add(dt);
                serverName.Connection.Close();

                return ConvertDataSetToJSON(ds);
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }

        [WebMethod]// ค้นหาหน้า Status 
        public string Read_Doc_Status(string BRANCHCODE, string SITECODE, string SLOC, string sDocNo, string sCreateUser, string sStatus)
        {
            string strsql = "";
            DataSet ds = new DataSet();
            DataSet objds = new DataSet();
            DataTable dt = new DataTable();
            string stringcon = configserver.StringConn(SITECODE);
            string sDate = DateTime.Now.ToString("yyyy/MM/dd", new CultureInfo("en-US"));
            try
            {
                //dev.lib.SQLConnection devdb = new dev.lib.SQLConnection(Connect);
                dev.lib.SQLConnection devdb = new dev.lib.SQLConnection(stringcon);
                SqlConnection cnn = devdb.getSqlConncetion;

                if (!BRANCHCODE.Equals("") && !SITECODE.Equals("") && !SLOC.Equals("") && !sDocNo.Equals("") && sCreateUser.Equals("") && sStatus.Equals("")) {
                    strsql = "select top 100 * from dbtrans..TBTrans_PRE_Print_Label_Saleset"
                                  + " where BRANCHCODE = '" + BRANCHCODE + "'"
                                  + " and SITECODE = '" + SITECODE + "'"
                                  + " and SLOC = '" + SLOC + "'"
                                  + " and docno like '%" + sDocNo + "%'"
                                  + " and create_user like '%" + sCreateUser + "%'"
                                  + " and status like '%" + sStatus + "%'";

                } else {
                    strsql = "select top 100 * from dbtrans..TBTrans_PRE_Print_Label_Saleset"
                                + " where DOCDATE = '" + sDate + "'"
                                + " and BRANCHCODE = '" + BRANCHCODE + "'"
                                + " and SITECODE = '" + SITECODE + "'"
                                + " and SLOC = '" + SLOC + "'"
                                + " and docno like '%" + sDocNo + "%'"
                                + " and create_user like '%" + sCreateUser + "%'"
                                + " and status like '%" + sStatus + "%'";
                }

                bool ret = devdb.ExecuteCommand(ref ds, strsql, "TB_Status", ref cnn);
                if (ret)
                {
                    if (ds.Tables["TB_Status"].Rows.Count > 0)
                    {
                        int iRowCount = ds.Tables["TB_Status"].Rows.Count;
                        DataTable objdt = ds.Tables["TB_Status"];

                        dt.Columns.Add("DocNo", typeof(string));
                        dt.Columns.Add("BranchCode", typeof(string));
                        dt.Columns.Add("BranchName", typeof(string));
                        dt.Columns.Add("SiteCode", typeof(string));
                        dt.Columns.Add("SiteName", typeof(string));
                        dt.Columns.Add("CreateUserCode", typeof(string));
                        dt.Columns.Add("CreateUserName", typeof(string));
                        dt.Columns.Add("Status", typeof(string));

                        foreach (DataRow row in objdt.Rows)
                        {
                            DataRow newrow = dt.NewRow();



                            newrow["DocNo"] = row["DocNo"].ToString();
                            newrow["BranchCode"] = row["BranchCode"].ToString();
                            newrow["BranchName"] = this.Read_Branch_Name(row["BranchCode"].ToString());
                            newrow["SiteCode"] = row["sitecode"].ToString();
                            newrow["SiteName"] = this.Read_Site_Name(row["sitecode"].ToString());
                            newrow["CreateUserCode"] = row["CREATE_USER"].ToString();
                            newrow["CreateUserName"] = this.Read_Name(row["CREATE_USER"].ToString());
                            newrow["Status"] = row["Status"].ToString();
                            dt.Rows.Add(newrow);
                        }
                    }
                    dt.TableName = "TB_Status";
                    objds.Tables.Add(dt);
                }
                return ConvertDataSetToJSON(objds);
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }
        // รหัสสาขาใหม่ เช่น 0001
        private string Read_Branch_Name(string sBranchCode)
        {
            string sResult = "";
            DataSet ds = new DataSet();
            try
            {
                dev.lib.SQLConnection devdb = new dev.lib.SQLConnection(Connect);
                SqlConnection cnn = devdb.getSqlConncetion;
                string strsql = "select *from [DBMASTER].[dbo].[TBMaster_Branch] where code = '" + sBranchCode + "' ";
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
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }
        private string Read_Site_Name(string sSiteCode)
        {
            string sResult = "";
            DataSet ds = new DataSet();
            try
            {
                dev.lib.SQLConnection devdb = new dev.lib.SQLConnection(Connect);
                SqlConnection cnn = devdb.getSqlConncetion;
                string strsql = "select *from [DBMASTER].[dbo].[TBMaster_Site] where code = '" + sSiteCode + "' ";
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
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }
        private string Read_Name(string sUserName)
        {
            string sResult = "";
            DataSet ds = new DataSet();
            try
            {
                dev.lib.SQLConnection devdb = new dev.lib.SQLConnection(Connect);
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
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }
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

        [WebMethod]
        public string GetLabelReason()
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            try
            {
                dev.lib.SQLConnection devdb = new dev.lib.SQLConnection(Connect);
                SqlConnection cnn = devdb.getSqlConncetion;
                string strsql = "select * from dbmaster..TBMaster_Print_Label_Reason";
                bool ret = devdb.ExecuteCommand(ref ds, strsql, "tbl", ref cnn);
                if (ret)
                {
                    if (ds.Tables["tbl"].Rows.Count > 0)
                    {
                        dt = ds.Tables["tbl"];
                    }
                }
                return ConvertDataSetToJSON(ds);
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }

        [WebMethod]
        public string OnDelete(string sDocNo)
        {
            string sMessage = "";
            try
            {
                dev.lib.SQLConnection devdb = new dev.lib.SQLConnection(Connect);
                SqlConnection cnn = devdb.getSqlConncetion;
                DataSet ds = new DataSet();
                string strsql = "";
                bool ret = devdb.BeginTrans(ref cnn);
                if (ret)
                {
                    strsql = "delete from DBTRANS..TBTrans_PRE_Print_Label_Saleset where DOCNO = '" + sDocNo + "' ";
                    ret = devdb.ExecuteNoneQuery(strsql, ref cnn);
                }
                if (ret)
                {
                    strsql = "delete from dbtrans..tbtrans_pre_print_label_sub where docno = '" + sDocNo + "' ";
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
        public string OnSearchByProductCode(string sProductCode, string _Site, string _Sloc)
        {
            string _ProductCode = "";
            string _ProductName = "";
            //
            DataSet ds = new DataSet();
            DataSet objds = new DataSet();
            try
            {
                dev.lib.DDUtil util = new dev.lib.DDUtil();
                util.ConnectionString = Connect;
                dev.lib.SQLConnection devdb = new dev.lib.SQLConnection(Connect);
                SqlConnection cnn = devdb.getSqlConncetion;
                string strsql = "select * from dbmaster..tbmaster_articleset " +
                                "where article = '" + sProductCode + "' ";
                bool ret = devdb.ExecuteCommand(ref objds, strsql, "tbl", ref cnn);
                cnn.Close();
                if (ret)
                {
                    if (objds.Tables["tbl"].Rows.Count > 0)
                    {
                        //สินค้าตัวแม่
                        DataRow drow = objds.Tables["tbl"].Rows[0];
                        //DataTable dtUnit = this.getDataSourceForSelectUnit(drow["article"].ToString()); // แสดงหน่วยของสินค้าทั้งหมดที่ค้นหาเจอไว้ใน DataTable
                        _ProductCode = drow["article"].ToString();
                        _ProductName = this.getProductName(drow["article"].ToString());
                        // ref pricedate
                        SalesetDetails dtSale = new SalesetDetails();

                        #region Columns
                        DataTable dtParent = new DataTable();
                        dtParent.Columns.Add("RowTypeParent", typeof(string));
                        dtParent.Columns.Add("Productcode", typeof(string));
                        dtParent.Columns.Add("Productname", typeof(string));
                        dtParent.Columns.Add("Unitcode", typeof(string));
                        dtParent.Columns.Add("Barcode", typeof(string));
                        dtParent.Columns.Add("Quantity", typeof(double));
                        dtParent.Columns.Add("Pricedate", typeof(string));
                        dtParent.Columns.Add("Unitname", typeof(string));
                        dtParent.Columns.Add("Unitprice", typeof(double));
                        dtParent.Columns.Add("Location", typeof(string));
                        dtParent.Columns.Add("RowOrder", typeof(int));
                        dtParent.Columns.Add("StorageBin", typeof(int));

                        DataTable dtChild = new DataTable();
                        dtChild.Columns.Add("RowTypeChild", typeof(string));
                        dtChild.Columns.Add("ChildRowOrder", typeof(int));
                        dtChild.Columns.Add("Component_code", typeof(string));
                        dtChild.Columns.Add("Component_name", typeof(string));
                        dtChild.Columns.Add("Component_unitcode", typeof(string));
                        dtChild.Columns.Add("Component_barcode", typeof(string));
                        dtChild.Columns.Add("Component_quantity", typeof(double));
                        dtChild.Columns.Add("Component_unitname", typeof(string));
                        dtChild.Columns.Add("Component_unitprice", typeof(double));
                        dtChild.Columns.Add("Component_pricedate", typeof(string));
                        #endregion


                        DataRow drParent = dtParent.NewRow();
                        drParent["RowTypeParent"] = "PARENT";
                        drParent["Productcode"] = drow["article"].ToString();
                        drParent["Productname"] = this.getProductName(drow["article"].ToString());
                        drParent["Unitcode"] = this.getUnit(drow["article"].ToString()); // 
                        drParent["Barcode"] = this.getBarcode(drow["article"].ToString(), this.getUnit(drow["article"].ToString())); //
                        drParent["Quantity"] = 1;
                        drParent["Pricedate"] = DateTime.Now.ToShortDateString();
                        drParent["Unitname"] = this.getUnitName(this.getUnit(drow["article"].ToString()));

                        //drParent["Unitprice"] = util.Read_Saleprice(_Site, drow["article"].ToString(), drow["BOMUOM"].ToString(), "0000", ref new_date);
                        drParent["Unitprice"] = Read_Saleprice(_Site, drow["article"].ToString(), drow["BOMUOM"].ToString());

                        drParent["Location"] = "";  // Storage bin
                        drParent["RowOrder"] = 0;

                        //drParent["ChildRowOrder"] = drParent["RowOrder"];
                        dtParent.Rows.Add(drParent);
                        dtParent.TableName = "TB_ArticleParent";
                        ds.Tables.Add(dtParent);

                        //สินค้าตัวลูก
                        int irow = 0;
                        foreach (DataRow objdr in objds.Tables["tbl"].Rows)
                        {
                            DataRow drChild = dtChild.NewRow();
                            drChild["RowTypeChild"] = "CHILD";
                            drChild["ChildRowOrder"] = irow;
                            drChild["Component_code"] = objdr["ComponentArticle"].ToString();
                            drChild["Component_name"] = this.getProductName(objdr["ComponentArticle"].ToString());
                            drChild["Component_unitcode"] = objdr["bomuom"].ToString();
                            drChild["Component_barcode"] = this.getBarcode(drChild["Component_code"].ToString(), drChild["Component_unitcode"].ToString());
                            drChild["Component_quantity"] = Convert.ToDouble(objdr["ComponentArticleQuantityRatio"].ToString());
                            drChild["Component_unitname"] = this.getUnitName(drow["BOMUOM"].ToString());
                            //drChild["Component_unitprice"] = util.Read_Saleprice(_Site, drParent["Productcode"].ToString(), drParent["Unitcode"].ToString(), "0000", ref dtSale.component_pricedate);
                            drChild["Component_unitprice"] = Read_Saleprice(_Site, drParent["Productcode"].ToString(), drParent["Unitcode"].ToString());
                            drChild["Component_pricedate"] = DateTime.Now.ToShortDateString();
                            dtChild.Rows.Add(drChild);

                            irow++;
                        }
                        dtChild.TableName = "TB_ArticleChild";
                        ds.Tables.Add(dtChild);

                    }
                    else {
                        DataTable dtParent = new DataTable();
                        dtParent.TableName = "TB_ArticleParent";
                        ds.Tables.Add(dtParent);

                        DataTable dtChild = new DataTable();
                        dtChild.TableName = "TB_ArticleChild";
                        ds.Tables.Add(dtChild);

                    }
                }
                return ConvertDataSetToJSON(ds);
            }
            catch (Exception ex)
            {
                return ex.StackTrace;
            }
        }
        private string Read_Saleprice(string SITE,string PRODUCTCODE,string UNITCODE) //สร้างใหม่ Arty
        {
            string sResult = "0.0";
            try
            {
                dev.lib.SQLConnection devdb = new dev.lib.SQLConnection(Connect);
                SqlConnection cnn = devdb.getSqlConncetion;
                DataSet objds = new DataSet();
                string strsql = "select top 1 * from dbmaster.dbo.tbmaster_sale_price "
                    + " where GETDATE() between begindate and enddate"
                    + " and sitecode = '" + SITE + "'"
                    + " and productcode = '" + PRODUCTCODE + "'"
                    + " and unitcode = '" + UNITCODE + "'"
                    + " and paymentcode = '0000'"
                    + " and isnull(saleprice,0) > 0"
                    + " order by createdate desc";


                bool ret = devdb.ExecuteCommand(ref objds, strsql, "tbmaster_sale_price", ref cnn);
                cnn.Close();
                if (ret)
                {
                    if (objds.Tables["tbmaster_sale_price"].Rows.Count > 0)
                    {
                        sResult = objds.Tables["tbmaster_sale_price"].Rows[0]["SALEPRICENV"].ToString();
                    }
                }
                return sResult;
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }

        [WebMethod]
        public string OnSearchByDocNo(string sDocNo ,string site)
        {
            DataSet ds = new DataSet();
            DataSet objds = new DataSet();
            string stringcon = configserver.StringConn(site);
            try
            {
                //dev.lib.SQLConnection devdb = new dev.lib.SQLConnection(Connect);
                dev.lib.SQLConnection devdb = new dev.lib.SQLConnection(stringcon);
                SqlConnection cnn = devdb.getSqlConncetion;
                string strsql = "select sale.DOCNO, sale.DOCDATE, sale.BRANCHCODE, sale.SITECODE, sale.SLOC,"
                                + " sale.CREATE_USER, sale.LABEL_TYPE, sale.CAUSE_OF_REQUEST, saleSub.rowtype,sale.STATUS,"
                                + " saleSub.RowOrder, saleSub.Child_RowOrder, saleSub.productcode, saleSub.productname,"
                                + " saleSub.unitcode, saleSub.barcode, saleSub.quantity, un.MYNAME as unitname,"
                                + " saleSub.unitprice, saleSub.component_code, saleSub.component_name, saleSub.component_unitcode,"
                                + " saleSub.component_barcode, saleSub.component_quantity, saleSub.component_unitprice,saleSub.LOCATION,saleSub.PRICEDATE"
                                + " from DBTRANS..tbtrans_pre_print_label_saleset as sale inner join"
                                + " DBTRANS..tbtrans_pre_print_label_saleset_sub as saleSub on sale.DOCNO = saleSub.DOCNO inner join"
                                + " DBMASTER.dbo.TBMaster_Unit as un ON saleSub.UNITCODE = un.CODE"
                                + " where sale.DOCNO = '" + sDocNo + "' ";
                bool ret = devdb.ExecuteCommand(ref ds, strsql, "tbl", ref cnn);
                if (ret)
                {
                    int iRowCount = ds.Tables["tbl"].Rows.Count;
                    if (iRowCount > 0)
                    {
                        DataTable dtSaleset = new DataTable();
                        dtSaleset.Columns.Add("DocNo", typeof(string));
                        dtSaleset.Columns.Add("DocDate", typeof(DateTime));
                        dtSaleset.Columns.Add("BranchCode", typeof(string));
                        dtSaleset.Columns.Add("SiteCode", typeof(string));
                        dtSaleset.Columns.Add("Sloc", typeof(string));
                        dtSaleset.Columns.Add("Create_User", typeof(string));
                        dtSaleset.Columns.Add("Create_User_NAME", typeof(string));
                        dtSaleset.Columns.Add("Label_Type", typeof(string));
                        dtSaleset.Columns.Add("Label_Type_NAME", typeof(string));
                        dtSaleset.Columns.Add("Cause_Of_Request", typeof(string));
                        dtSaleset.Columns.Add("RowType", typeof(string));
                        dtSaleset.Columns.Add("STATUS", typeof(string));
                        dtSaleset.Columns.Add("RowOrder", typeof(int));
                        dtSaleset.Columns.Add("Child_RowOrder", typeof(int));
                        dtSaleset.Columns.Add("Productcode", typeof(string));
                        dtSaleset.Columns.Add("ProductName", typeof(string));
                        dtSaleset.Columns.Add("Unitcode", typeof(string));
                        dtSaleset.Columns.Add("Barcode", typeof(string));
                        dtSaleset.Columns.Add("Quantity", typeof(double));
                        dtSaleset.Columns.Add("Unitname", typeof(string));
                        dtSaleset.Columns.Add("Unitprice", typeof(string));
                        dtSaleset.Columns.Add("PRICEDATE", typeof(string));
                        dtSaleset.Columns.Add("LOCATION", typeof(string));
                        dtSaleset.Columns.Add("Component_Code", typeof(string));
                        dtSaleset.Columns.Add("Component_Name", typeof(string));
                        dtSaleset.Columns.Add("Component_Unitcode", typeof(string));
                        dtSaleset.Columns.Add("Component_Barcode", typeof(string));
                        dtSaleset.Columns.Add("Component_Quantity", typeof(string));
                        dtSaleset.Columns.Add("Component_UnitName", typeof(string));
                        dtSaleset.Columns.Add("Component_Unitprice", typeof(string));

                        DataTable dt = ds.Tables["tbl"];
                        foreach (DataRow dr in dt.Rows)
                        {
                            DataRow objdr = dtSaleset.NewRow();
                            objdr["DocNo"] = dr["DOCNO"].ToString();
                            objdr["DocDate"] = dr["DOCDATE"].ToString();
                            objdr["BranchCode"] = dr["BRANCHCODE"].ToString();
                            objdr["SiteCode"] = dr["SITECODE"].ToString();
                            objdr["Sloc"] = dr["SLOC"].ToString();
                            objdr["Create_User"] = dr["CREATE_USER"].ToString();
                            objdr["Create_User_NAME"] = this.Read_Name(dr["CREATE_USER"].ToString());
                            objdr["Label_Type"] = dr["LABEL_TYPE"].ToString();
                            objdr["Label_Type_NAME"] = this.GetLabelType_NAME(dr["LABEL_TYPE"].ToString());
                            objdr["Cause_Of_Request"] = dr["CAUSE_OF_REQUEST"].ToString();
                            objdr["RowType"] = dr["rowtype"].ToString();
                            objdr["STATUS"] = dr["STATUS"].ToString();
                            objdr["RowOrder"] = dr["RowOrder"].ToString();
                            objdr["Child_RowOrder"] = dr["Child_RowOrder"].ToString();
                            objdr["Productcode"] = dr["productcode"].ToString();
                            objdr["ProductName"] = dr["ProductName"].ToString();
                            objdr["Unitcode"] = dr["unitcode"].ToString();
                            objdr["Barcode"] = dr["barcode"].ToString();
                            objdr["Quantity"] = dr["quantity"].ToString();
                            objdr["Unitname"] = dr["unitname"].ToString();
                            objdr["Unitprice"] = dr["unitprice"].ToString();
                            objdr["PRICEDATE"] = dr["PRICEDATE"].ToString();
                            objdr["LOCATION"] = dr["LOCATION"].ToString();
                            objdr["Component_code"] = dr["component_code"].ToString();
                            objdr["component_Name"] = dr["component_name"].ToString();
                            objdr["Component_Unitcode"] = dr["component_unitcode"].ToString();
                            objdr["Component_barcode"] = dr["component_barcode"].ToString();
                            objdr["Component_quantity"] = dr["component_quantity"].ToString();
                            objdr["Component_UnitName"] = this.getUnitName(dr["component_unitcode"].ToString());
                            objdr["Component_unitprice"] = dr["component_unitprice"].ToString();
                            dtSaleset.Rows.Add(objdr);
                        }
                        dtSaleset.TableName = "tbtrans_pre_print_label_saleset";
                        objds.Tables.Add(dtSaleset);
                    }
                }
                return ConvertDataSetToJSON(objds);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }
        private string RunningNewDocNo(string sBranchCode,string site)
        {
            string sResult = "";
            string stringcon = configserver.StringConn(site);
            //dev.lib.SQLConnection devdb = new dev.lib.SQLConnection(Connect);
            dev.lib.SQLConnection devdb = new dev.lib.SQLConnection(stringcon);
            dev.lib.UtilityDataBase dbutil = new dev.lib.UtilityDataBase();
            dbutil.ConnectionString = Connect;
            dbutil.RunningFieldName = "DOCNO";
            string sformat = "yyyyMMdd-####";
            string sShortName = this.getBranchShortName(sBranchCode); // 
            dbutil.RunningGroup = sShortName + "S-";
            dbutil.RunningFormat = sformat;
            dbutil.RunningTableName = "dbtrans.dbo.TBTrans_PRE_Print_Label_Saleset";
            sResult = dbutil.RunningNewDocNo(devdb);
            return sResult;
        }
        private string getBranchShortName(string sBranchCode)
        {
            string sResult = "";
            try
            {
                //dev.lib.SQLConnection devdb = new dev.lib.SQLConnection(Connect);
                dev.lib.SQLConnection devdb = new dev.lib.SQLConnection(Connect);
                SqlConnection cnn = devdb.getSqlConncetion;
                DataSet objds = new DataSet();
                string strsql = "select * from dbmaster..tbmaster_branch where code = '" + sBranchCode + "' ";
                bool ret = devdb.ExecuteCommand(ref objds, strsql, "tbl", ref cnn);
                cnn.Close();
                if (ret)
                {
                    if (objds.Tables["tbl"].Rows.Count > 0)
                    {
                        sResult = objds.Tables["tbl"].Rows[0]["short_name"].ToString();
                    }
                }
                return sResult;
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }
        public string GetLabelType_NAME(string CODE)
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            dev.lib.SQLConnection devdb = new dev.lib.SQLConnection(Connect);
            SqlConnection cnn = devdb.getSqlConncetion;
            string strsql = "select * from dbmaster..tbmaster_print_label_type where ltype = 2 and CODE = '" + CODE + "'";
            bool ret = devdb.ExecuteCommand(ref ds, strsql, "label_type", ref cnn);
            string sResult = "";
            if (ret)
            {
                if (ds.Tables["label_type"].Rows.Count > 0)
                {
                    sResult = ds.Tables["label_type"].Rows[0]["MYNAME"].ToString();
                }
            }
            return sResult;
        }
        // ค้นหารายการสินค้าทุกหน่วย
        public DataTable getDataSourceForSelectUnit(string sProductCode)
        {
            dev.lib.SQLConnection devdb = new dev.lib.SQLConnection(Connect);
            SqlConnection cnn = devdb.getSqlConncetion;
            DataSet objds = new DataSet();
            string strsql = "select prd.code as productcode, prd.nameth as productname, pun.unitcode, unt.myname as unitname, pun.unitrate, bar.barcode " +
                            "   , 0.00 as unitprice, '' as location, '' as matkl, {fn curdate()} as pricedate " +
                            "from dbmaster..tbmaster_product prd " +
                            "   inner join dbmaster..TBMaster_Product_Unit pun on prd.code = pun.productcode and prd.stockunit = pun.unitcode " +
                            "   left join dbmaster..TBMaster_Unit unt on pun.unitcode = unt.code " +
                            "   left join dbmaster..TBMaster_barcode bar on pun.productcode = bar.productcode and pun.unitcode = bar.unitcode " +
                            "where prd.code = '" + sProductCode + "' " +
                            "order by pun.unitrate ";
            bool ret = devdb.ExecuteCommand(ref objds, strsql, "tbl", ref cnn);
            cnn.Close();
            if (ret)
            {
                return objds.Tables["tbl"];
            }
            else
                return null;
        }
        private string getProductName(string sProductCode)
        {
            string sResult = "";
            try
            {
                dev.lib.SQLConnection devdb = new dev.lib.SQLConnection(Connect);
                SqlConnection cnn = devdb.getSqlConncetion;
                DataSet objds = new DataSet();
                string strsql = "select * from dbmaster..tbmaster_product where code = '" + sProductCode + "' ";
                bool ret = devdb.ExecuteCommand(ref objds, strsql, "tbl", ref cnn);
                cnn.Close();
                if (ret)
                {
                    if (objds.Tables["tbl"].Rows.Count > 0)
                    {
                        sResult = objds.Tables["tbl"].Rows[0]["nameth"].ToString();
                    }
                }
                return sResult;
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }
        private string getUnitName(string sUnitCode)
        {
            string sResult = "";
            try
            {
                dev.lib.SQLConnection devdb = new dev.lib.SQLConnection(Connect);
                SqlConnection cnn = devdb.getSqlConncetion;
                DataSet objds = new DataSet();
                string strsql = "select * from dbmaster..tbmaster_unit where code = '" + sUnitCode + "' ";
                bool ret = devdb.ExecuteCommand(ref objds, strsql, "tbl", ref cnn);
                cnn.Close();
                if (ret)
                {
                    if (objds.Tables["tbl"].Rows.Count > 0)
                    {
                        sResult = objds.Tables["tbl"].Rows[0]["myname"].ToString();
                    }
                }
                return sResult;
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }
        private string getBarcode(string sProductCode, string sUnitCode)
        {
            string sResult = "";
            try
            {
                dev.lib.SQLConnection devdb = new dev.lib.SQLConnection(Connect);
                SqlConnection cnn = devdb.getSqlConncetion;
                DataSet objds = new DataSet();
                string strsql = "select * from dbmaster..tbmaster_barcode where productcode = '" + sProductCode + "' and unitcode = '" + sUnitCode + "' ";
                bool ret = devdb.ExecuteCommand(ref objds, strsql, "tbl", ref cnn);
                cnn.Close();
                if (ret)
                {
                    if (objds.Tables["tbl"].Rows.Count > 0)
                    {
                        sResult = objds.Tables["tbl"].Rows[0]["barcode"].ToString();
                    }
                }
                return sResult;
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }

        }
        private string getUnit(string sProductCode)
        {
            string sResult = "";
            try
            {
                dev.lib.SQLConnection devdb = new dev.lib.SQLConnection(Connect);
                SqlConnection cnn = devdb.getSqlConncetion;
                DataSet objds = new DataSet();
                string strsql = "select * from dbmaster..TBMaster_Product where CODE = '" + sProductCode + "' ";
                bool ret = devdb.ExecuteCommand(ref objds, strsql, "tbl", ref cnn);
                cnn.Close();
                if (ret)
                {
                    if (objds.Tables["tbl"].Rows.Count > 0)
                    {
                        sResult = objds.Tables["tbl"].Rows[0]["STOCKUNIT"].ToString();
                    }
                }
                return sResult;
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
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
