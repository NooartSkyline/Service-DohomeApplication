using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI_DohomeApplication.Models
{
    public class GetconfigConnection
    {
        public string Check_Entities(string site)
        {
            string substr = site.ToUpper().Substring(0, 2);
            if (substr == "M0")
            {
                return System.Configuration.ConfigurationManager.AppSettings[site];
            }
            else
            {
                return System.Configuration.ConfigurationManager.AppSettings[substr];
            }
        }
    }
}