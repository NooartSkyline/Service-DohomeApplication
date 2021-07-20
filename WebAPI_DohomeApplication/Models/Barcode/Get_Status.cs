using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI_DohomeApplication.Models.Barcode
{
    public class Get_Status
    {
        public bool status;
        public string msg;
        public List<modelGet_Status> list_status;
    }
    public class modelGet_Status
    {
        public string STATUSCHECK { get; set; }
        //HD     

        public string DOCNO_HD { get; set; }

        public System.Nullable<System.DateTime> DOCDATE_HD { get; set; }

        public string BRANCHCODE_HD { get; set; }

        public string SITECODE_HD { get; set; }

        public string SLOC_HD { get; set; }

        public System.Nullable<System.DateTime> CREATEDATE_HD { get; set; }

        public string CREATEUSERCODE_HD { get; set; }
        public string CREATEUSERNAME_HD { get; set; }

        public string STATUS_HD { get; set; }

        public int PRINT_NUMBER_HD { get; set; }

        public System.Nullable<System.DateTime> PRINTDATE_HD { get; set; }

        public string PRINTUSER_HD { get; set; }

        //DT
        public string DOCNO { get; set; }
        public int ROWORDER { get; set; }
        public System.Nullable<System.DateTime> DOCDATE { get; set; }
        public string BRANCHCODE { get; set; }

        public string SITECODE { get; set; }

        public string SLOC { get; set; }

        public string PRODUCTCODE { get; set; }

        public string PRODUCTNAME { get; set; }

        public string BARCODE_REQ { get; set; }

        public string QUANTITY_REQ { get; set; }

        public string UNIT_REQ { get; set; }
        public string UNIT_REQ_NAME { get; set; }

        public string UNIT_RATE_REQ { get; set; }

        public string QUANTITY_TRAN { get; set; }

        public string UNIT_TRAN { get; set; }
        public string UNIT_TRAN_NAME { get; set; }

        public System.Nullable<System.DateTime> PRINTDATE { get; set; }

        public string CAUSE_REQUEST { get; set; }

        public string GR_DOCNO { get; set; }

        public string GR_YEAR { get; set; }

        public string PRINTUSER { get; set; }

        public string STATUS { get; set; }

        public System.Nullable<System.DateTime> APPROVEDATE { get; set; }

        public string APPROVEUSER { get; set; }

        public string APPROVE_REMARK { get; set; }
    }
}
