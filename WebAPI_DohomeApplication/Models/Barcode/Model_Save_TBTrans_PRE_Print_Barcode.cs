using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI_DohomeApplication.Models
{
    public class Model_Save_TBTrans_PRE_Print_Barcode
    {
        public int NOROW { get; set; }
        public string BRANCHCODE { get; set; }
        public string SITECODE { get; set; }
        public string SLOC { get; set; }
        public string REQUEST_USER { get; set; }
        public string CAUSE_REQUEST { get; set; }
        public string PRODUCTCODE { get; set; }
        public string PRODUCTNAME { get; set; }
        public string BARCODE { get; set; }
        public string UNITCODE_TRANSORT { get; set; }
        public string UNITCODE_REQUEST { get; set; }
        public string QUANTITY_REQ { get; set; }
        public string UNIT_RATE { get; set; }
    }
}