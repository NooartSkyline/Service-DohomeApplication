using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI_DohomeApplication.Models.Message
{
    public class Model_GETHDDT
    {
        public string _DOCNO { get; set; }

        public System.Nullable<System.DateTime> _DOCDATE { get; set; }

        public string _BRANCHCODE { get; set; }

        public System.DateTime _CREATEDATE { get; set; }

        public string _CREATEUSERCODE { get; set; }
        public string _CREATEUSERNAME { get; set; }

        public string _MAT_SELLER { get; set; }

        public int _ROWORDER { get; set; }

        public string _MESSAGE { get; set; }

        public System.Nullable<decimal> _QUANTITY { get; set; }

        public System.Nullable<System.DateTime> _PRINTDATE { get; set; }

        public string _PRINTUSER { get; set; }
    }
}