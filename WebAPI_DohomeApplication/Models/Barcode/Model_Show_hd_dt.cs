using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI_DohomeApplication.Models
{
    public class Model_Show_hd_dt
    {
        public string _DOCNO { get; set; }

        public System.Nullable<System.DateTime> _DOCDATE { get; set; }

        public string _BRANCHCODE { get; set; }

        public string _SITECODE { get; set; }

        public string _SLOC { get; set; }

        public System.Nullable<System.DateTime> _CREATEDATE { get; set; }

        public string _CREATEUSERCODE { get; set; }
        public string _CREATEUSERNAME { get; set; }

        public string _STATUS { get; set; }

        public int _PRINT_NUMBER { get; set; }

        public System.Nullable<System.DateTime> _PRINTDATE { get; set; }

        public string _PRINTUSER { get; set; }


        public int _ROWORDER { get; set; }

        public string _PRODUCTCODE { get; set; }

        public string _PRODUCTNAME { get; set; }

        public string _BARCODE_REQ { get; set; }

        public System.Nullable<decimal> _QUANTITY_REQ { get; set; }

        public string _UNIT_REQ { get; set; }

        public System.Nullable<decimal> _UNIT_RATE_REQ { get; set; }

        public System.Nullable<decimal> _QUANTITY_TRAN { get; set; }

        public string _UNIT_TRAN { get; set; }

        public string _CAUSE_REQUEST { get; set; }

        public string _GR_DOCNO { get; set; }

        public string _GR_YEAR { get; set; }

        public string _STATUS_APPROVE { get; set; }

        public System.Nullable<System.DateTime> _APPROVEDATE { get; set; }

        public string _APPROVEUSER { get; set; }

        public string _APPROVE_REMARK { get; set; }
    }
}