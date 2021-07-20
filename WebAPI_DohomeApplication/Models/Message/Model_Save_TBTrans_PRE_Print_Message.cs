using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI_DohomeApplication.Models.Message
{
    public class Model_Save_TBTrans_PRE_Print_Message
    {
        public string DOCNO { get; set; }

        public System.Nullable<System.DateTime> DOCDATE { get; set; }

        public string BRANCHCODE { get; set; }

        public System.DateTime _CREATEDATE { get; set; }

        public string REQUEST_USER { get; set; }

        public string MAT_SELLER { get; set; }

        public int ROWORDER { get; set; }

        public string MESSAGE { get; set; }

        public System.Nullable<decimal> QTYREQUEST { get; set; }

        public System.Nullable<System.DateTime> PRINTDATE { get; set; }

        public string PRINTUSER { get; set; }
    }
}