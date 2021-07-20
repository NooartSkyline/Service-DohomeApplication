using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Webservice_DohomeApplication
{
    public class Class_Configserver
    {
        public string StringConn(string site)
        {         
            string substr = site.ToUpper().Substring(0, 2);
            if (substr == "M0") {
                return System.Configuration.ConfigurationManager.AppSettings[site];
            }
            else
            {
                return System.Configuration.ConfigurationManager.AppSettings[substr];
            }
                
        }
    }
}