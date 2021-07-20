using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI_DohomeApplication.Models.GetMaster
{
    public class Model_input
    {
        public bool Status;
        public string Message;
        public List<TB_product> products;
        public List<TB_unit> units;
        public List<TBMaster_Pre_Print_Barcode_Reason> Print_Barcode_Reasons;
    }
    public class TB_product {
        public string procuctcode;
        public string productname;
    }
   public class TB_unit
    {
        public string unitcode;
        public string unitname;
        public string barcode;
        public string unitrate;
    }
}