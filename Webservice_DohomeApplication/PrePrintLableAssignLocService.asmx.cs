using Newtonsoft.Json;
using SourceCode.SmartObjects.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace Webservice_DohomeApplication
{
    /// <summary>
    /// Summary description for PrePrintLableAssignLocService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class PrePrintLableAssignLocService : System.Web.Services.WebService
    {
        DBMASTERDataContext db = new DBMASTERDataContext();

        public string Connect = System.Configuration.ConfigurationManager.ConnectionStrings["DBMASTERConnectionString"].ConnectionString;
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

        [WebMethod]
        public string ZDD_HH_SET_ZMM_ASSIGNLOC_MULTI(string Jsonstring)
        {
            string strXml = "";
            string sMessage = "";
            try
            {
                DataTable dt = this.JsonToTable(Jsonstring);
                strXml = "<connect>";
                foreach (DataRow dr in dt.Rows)
                {

                    strXml += "<I_ARTICLE_ASSIGNLOC>";
                    //
                    strXml += "<IN_MATNR>" + dr["O_ASSIGNLOC_MATERIAL"].ToString() + "</IN_MATNR>";
                    strXml += "<IN_UNITOFMEASURE>" + dr["O_ASSIGNLOC_UNITOFMEASURE"].ToString() + "</IN_UNITOFMEASURE>";
                    strXml += "<IN_BIN_CODE>" + dr["O_ASSIGNLOC_BIN_CODE"].ToString() + "</IN_BIN_CODE>";
                    strXml += "<IN_LOCTYPE>" + dr["O_ASSIGNLOC_LOCTYPE"].ToString() + "</IN_LOCTYPE>";
                    strXml += "<IN_SITE>" + dr["O_ASSIGNLOC_WERKS"].ToString() + "</IN_SITE>";
                    strXml += "<IN_STORAGE_LOC>" + dr["O_ASSIGNLOC_STORAGE_LOC"].ToString() + "</IN_STORAGE_LOC>";
                    strXml += "<IN_DEFAULT_BIN>" + (dt.Columns.Contains("O_ASSIGNLOC_DEFAULT_BIN") ? dr["O_ASSIGNLOC_DEFAULT_BIN"].ToString() : "") + "</IN_DEFAULT_BIN>";
                    //
                    if (dr["O_ASSIGNLOC_ROWORDER"].ToString() == "")
                        strXml += "<IN_ROWORDER>" + "0" + "</IN_ROWORDER>";
                    else
                        strXml += "<IN_ROWORDER>" + dr["O_ASSIGNLOC_ROWORDER"].ToString() + "</IN_ROWORDER>";
                    //
                    if (dr["O_ASSIGNLOC_PUTQTY"].ToString() == "")
                        strXml += "<IN_PUTQTY>" + "0" + "</IN_PUTQTY>";
                    else
                        strXml += "<IN_PUTQTY>" + dr["O_ASSIGNLOC_PUTQTY"].ToString() + "</IN_PUTQTY>";
                    //
                    if (dr["O_ASSIGNLOC_PUTLEVEL"].ToString() == "")
                        strXml += "<IN_PUTLEVEL>" + "0" + "</IN_PUTLEVEL>";
                    else
                        strXml += "<IN_PUTLEVEL>" + dr["O_ASSIGNLOC_PUTLEVEL"].ToString() + "</IN_PUTLEVEL>";
                    //
                    if (dr["O_ASSIGNLOC_MAXSTOCK"].ToString() == "")
                        strXml += "<IN_MAXSTOCK>" + "0" + "</IN_MAXSTOCK>";
                    else
                        strXml += "<IN_MAXSTOCK>" + dr["O_ASSIGNLOC_MAXSTOCK"].ToString() + "</IN_MAXSTOCK>";
                    //
                    strXml += "</I_ARTICLE_ASSIGNLOC>";
                }
                strXml += "</connect>";

                SmartObjectClientServer serverName = K2_CONNECT_SERVER();// Connect K2 Server 
                SmartObject smartObject = serverName.GetSmartObject("ZDD_HH_SET_ZMM_ASSIGNLOC_MULTI_I_ARTICLE_ASSIGNLOC");
                smartObject.MethodToExecute = "ZDD_HH_SET_ZMM_ASSIGNLOC_MULTII_ARTICLE_ASSIGNLOC_ZDD_HH_SET_ZMM_ASSIGNLOC_MULTI";
                SetPropertyValue(smartObject.ListMethods["ZDD_HH_SET_ZMM_ASSIGNLOC_MULTII_ARTICLE_ASSIGNLOC_ZDD_HH_SET_ZMM_ASSIGNLOC_MULTI"].InputProperties["I_ARTICLE_ASSIGNLOC"], strXml); // รับ XML

             
                DataTable dt_result = serverName.ExecuteListDataTable(smartObject);
                serverName.Connection.Close();

                //loop check each article have been set to SAP
                bool check = false;
                dev.lib.Utilities devlib = new dev.lib.Utilities();
                List<string> list = new List<string>();

                foreach (DataRow row in dt.Rows)
                {
                    foreach (DataRow result in dt_result.Rows)
                    {
                        if (devlib.RemoveZero(result["TBL_LOCATION_MATERIAL"].ToString()).Equals(row["O_ASSIGNLOC_MATERIAL"].ToString()) &&
                            result["TBL_LOCATION_BIN_CODE"].ToString().Equals(row["O_ASSIGNLOC_BIN_CODE"].ToString()) &&
                            result["TBL_LOCATION_UNITOFMEASURE"].ToString().Equals(row["O_ASSIGNLOC_UNITOFMEASURE"].ToString()) &&
                            result["TBL_LOCATION_WERKS"].ToString().Equals(row["O_ASSIGNLOC_WERKS"].ToString()) &&
                            result["TBL_LOCATION_STORAGE_LOC"].ToString().Equals(row["O_ASSIGNLOC_STORAGE_LOC"].ToString()))
                        {
                            check = true;
                            break;
                        }

                    }
                    if (!check)
                    {
                        list.Add(row["O_ASSIGNLOC_MATERIAL"].ToString());
                    }

                }

                if (check)
                {
                    sMessage = "S";
                }
                else
                {
                    sMessage = "Article : ";
                    foreach (var i in list)
                    {
                        sMessage += i + ", ";
                    }
                    sMessage = " doesn't set by SAP! Please try again.";

                }
                return sMessage;


            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }

        }

        //[WebMethod]
        //public string ZDD_HANDHELD_SET_ZMM_ASSIGNLOC_TBL_LOCATION_XML(
        //    string sMANDT, string sMATERIAL, string sUNITOFMEASURE,
        //    string sBIN_CODE, string sLOCTYPE, string sWERKS, string sSTORAGE_LOC,
        //    string sDEFAULT_BIN, double dMAXSTOCK, double dROWORDER, double dPUTQTY,
        //    double dPUTLEVEL, double dMINSTOCK, string sSPANTYPE, double dSPANWIDE)
        //{
        //    try
        //    {
        //        SmartObjectClientServer serverName = K2_CONNECT_SERVER();
        //        SmartObject smartObject = serverName.GetSmartObject("ZDD_HANDHELD_SET_ZMM_ASSIGNLOC_TBL_LOCATION");
        //        smartObject.MethodToExecute = "Serialize";
        //        SetPropertyValue(smartObject.Methods["Serialize"].InputProperties["MANDT"], sMANDT);
        //        SetPropertyValue(smartObject.Methods["Serialize"].InputProperties["MATERIAL"], sMATERIAL);
        //        SetPropertyValue(smartObject.Methods["Serialize"].InputProperties["UNITOFMEASURE"], sUNITOFMEASURE);
        //        SetPropertyValue(smartObject.Methods["Serialize"].InputProperties["BIN_CODE"], sBIN_CODE);
        //        SetPropertyValue(smartObject.Methods["Serialize"].InputProperties["LOCTYPE"], sLOCTYPE);
        //        SetPropertyValue(smartObject.Methods["Serialize"].InputProperties["WERKS"], sWERKS);
        //        SetPropertyValue(smartObject.Methods["Serialize"].InputProperties["STORAGE_LOC"], sSTORAGE_LOC);
        //        SetPropertyValue(smartObject.Methods["Serialize"].InputProperties["DEFAULT_BIN"], sDEFAULT_BIN);
        //        SetPropertyValue(smartObject.Methods["Serialize"].InputProperties["MAXSTOCK"], dMAXSTOCK);
        //        SetPropertyValue(smartObject.Methods["Serialize"].InputProperties["ROWORDER"], dROWORDER);
        //        SetPropertyValue(smartObject.Methods["Serialize"].InputProperties["PUTQTY"], dPUTQTY);
        //        SetPropertyValue(smartObject.Methods["Serialize"].InputProperties["PUTLEVEL"], dPUTLEVEL);
        //        SetPropertyValue(smartObject.Methods["Serialize"].InputProperties["MINSTOCK"], dMINSTOCK);
        //        SetPropertyValue(smartObject.Methods["Serialize"].InputProperties["SPANTYPE"], sSPANTYPE);
        //        SetPropertyValue(smartObject.Methods["Serialize"].InputProperties["SPANWIDE"], dSPANWIDE);

        //        SmartObject returnSmartObject;
        //        returnSmartObject = serverName.ExecuteScalar(smartObject);
        //        string result_xml = SetProperty_Serialized_Item__TBL_LOCATION_(returnSmartObject.Properties["Serialized_Item__TBL_LOCATION_"].Value);

        //        serverName.Connection.Close();

        //        return result_xml;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        [WebMethod]
        public string ZDD_HANDHELD_DEL_ZMM_ASSIGNLOC(string sIN_BIN_CODE, string sIN_STORAGE_LOC, string sIN_WERKS)
        {
            string sMessage = "";
            try
            {
                SmartObjectClientServer serverName = K2_CONNECT_SERVER();// Connect K2 Server 
                SmartObject smartObject = serverName.GetSmartObject("ZDD_HANDHELD_DEL_ZMM_ASSIGNLOC");
                smartObject.MethodToExecute = "ZDD_HANDHELDDEL_ZMM_ASSIGNLOC_ZDD_HANDHELD_DEL_ZMM_ASSIGNLOC";
                SetPropertyValue(smartObject.ListMethods["ZDD_HANDHELDDEL_ZMM_ASSIGNLOC_ZDD_HANDHELD_DEL_ZMM_ASSIGNLOC"].InputProperties["p_IN_BIN_CODE"], sIN_BIN_CODE);
                SetPropertyValue(smartObject.ListMethods["ZDD_HANDHELDDEL_ZMM_ASSIGNLOC_ZDD_HANDHELD_DEL_ZMM_ASSIGNLOC"].InputProperties["p_IN_STORAGE_LOC"], sIN_STORAGE_LOC);
                SetPropertyValue(smartObject.ListMethods["ZDD_HANDHELDDEL_ZMM_ASSIGNLOC_ZDD_HANDHELD_DEL_ZMM_ASSIGNLOC"].InputProperties["p_IN_WERKS"], sIN_WERKS);
                serverName.ExecuteListDataTable(smartObject);

                sMessage = "Successfully";
                return sMessage;
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }

        [WebMethod]
        public string GETPRODUCT(string barcode ,out string msg) {
            model_listmaster model_Listmaster = new model_listmaster();
            List<TBMaster_Product_Unit> list_pro_unit = new List<TBMaster_Product_Unit>();
            List<TBMaster_Unit> list_unit = new List<TBMaster_Unit>();
            List<TBMaster_Product> list_pro = new List<TBMaster_Product>();
            TBMaster_Barcode productcode = Getproductbybarcode(barcode);
            try
            {
                list_pro_unit = (from product in db.TBMaster_Product_Units
                            where product.PRODUCTCODE == productcode.PRODUCTCODE && product.UNITCODE == productcode.UNITCODE
                            select product).ToList();

                list_unit = (from unit in db.TBMaster_Units
                             where unit.CODE == list_pro_unit[0].UNITCODE
                             select unit).ToList();

                list_pro = (from pr in db.TBMaster_Products
                            where pr.CODE == productcode.PRODUCTCODE
                            select pr).ToList();

                model_Listmaster.TBMaster_Product_Unit = list_pro_unit;
                model_Listmaster.TBMaster_Unit = list_unit;
                model_Listmaster.TBMaster_Product = list_pro;
               
            }
            catch(Exception ex) {
                msg = ex.Message;
            } 

            msg = "";
            return JsonConvert.SerializeObject(model_Listmaster);
        }
        public TBMaster_Barcode Getproductbybarcode(string barcode) {
            TBMaster_Barcode _Barcode = new TBMaster_Barcode();
            _Barcode = (from br in db.TBMaster_Barcodes
                        where br.BARCODE == barcode
                        select br).SingleOrDefault();

            return _Barcode;
        }

        public DataTable JsonToTable(String JsonString)
        {
            DataTable dt = (DataTable)JsonConvert.DeserializeObject(JsonString, typeof(DataTable));
            return dt;
        }

        protected static string SetProperty_Serialized_Item__TBL_LOCATION_(string Serialized_Item__TBL_LOCATION_)
        {
            if (Serialized_Item__TBL_LOCATION_ == null)
            {
                return null;
            }


            if (Serialized_Item__TBL_LOCATION_ == string.Empty)
            {
                return null;
            }

            return Serialized_Item__TBL_LOCATION_;
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
    }
}
