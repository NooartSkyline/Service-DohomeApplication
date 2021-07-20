using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI_DohomeApplication.Models.Barcode
{
    public class model_recive_approve
    {
        public List<approve> list_data { get; set; }

        public string status { get; set; }

        public string reason { get; set; }

        public string UserApprove { get; set; }

        public string site { get; set; }
    }
}