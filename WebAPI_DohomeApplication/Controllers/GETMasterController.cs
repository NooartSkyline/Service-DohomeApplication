using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using WebAPI_DohomeApplication.Models;
using WebAPI_DohomeApplication.Models.GetMaster;

namespace WebAPI_DohomeApplication.Controllers
{
    public class GETMasterController : ApiController
    {

        DBMasterDataContext DBM = new DBMasterDataContext();

        /// <summary>
        /// ใช้ดึง Branch Site Sloc ทั้งหมด
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [ResponseType(typeof(Model_input))]
        [AllowAnonymous]
        [Route("GetMaster")]
        [HttpGet]
        public IHttpActionResult GetMaster(string search,string status)
        {            
            List<TB_product> products = new List<TB_product>();
            List<TB_unit> units = new List<TB_unit>();
            List<TBMaster_Pre_Print_Barcode_Reason> Print_Barcode_Reasons = new List<TBMaster_Pre_Print_Barcode_Reason>();

            Model_input response = new Model_input();
            try
            {
                if (status == "0")
                {
                    products = (from tbpu in DBM.TBMaster_Products
                                where tbpu.CODE == search
                                select new TB_product { procuctcode = tbpu.CODE, productname = tbpu.NAMETH }).ToList();

                    units = (from br in DBM.TBMaster_Barcodes
                             join un in DBM.TBMaster_Units on br.UNITCODE equals un.CODE
                             join pro in DBM.TBMaster_Product_Units on new { br.PRODUCTCODE, br.UNITCODE } equals new { pro.PRODUCTCODE, pro.UNITCODE }
                             where br.PRODUCTCODE == search
                             select new TB_unit { unitcode = un.CODE, unitname = un.MYNAME, barcode = br.BARCODE , unitrate = pro.UNITRATE.ToString()}).ToList();

                    Print_Barcode_Reasons = (from pbr in DBM.TBMaster_Pre_Print_Barcode_Reasons select pbr).ToList();

                    if (products.Count == 0)
                    {
                        response.Message = "ไม่พบรหัสสินค้านี้ " + search;
                        response.Status = false;
                        response.products = products;
                        response.units = units;
                        response.Print_Barcode_Reasons = Print_Barcode_Reasons;
                    }
                    else
                    {
                        response.Message = "";
                        response.Status = true;
                        response.products = products;
                        response.units = units;
                        response.Print_Barcode_Reasons = Print_Barcode_Reasons;
                    }
                }
                    if (status == "1")
                    {
                        products = (from bar in DBM.TBMaster_Barcodes
                                    join pd in DBM.TBMaster_Products on bar.PRODUCTCODE equals pd.CODE
                                    where bar.BARCODE == search
                                    select new TB_product { procuctcode = pd.CODE, productname = pd.NAMETH }).ToList();

                        units = (from br in DBM.TBMaster_Barcodes
                                 join un in DBM.TBMaster_Units on br.UNITCODE equals un.CODE
                                 join pro in DBM.TBMaster_Product_Units on new { br.PRODUCTCODE, br.UNITCODE } equals new { pro.PRODUCTCODE, pro.UNITCODE }
                                 where br.BARCODE == search
                                 select new TB_unit { unitcode = un.CODE, unitname = un.MYNAME, barcode = br.BARCODE, unitrate = pro.UNITRATE.ToString()}).ToList();

                        Print_Barcode_Reasons = (from pbr in DBM.TBMaster_Pre_Print_Barcode_Reasons select pbr).ToList();

                    if (products.Count == 0)
                    {
                        response.Message = "ไม่พบรหัสสินค้านี้ " + search;
                        response.Status = false;
                        response.products = products;
                        response.units = units;
                        response.Print_Barcode_Reasons = Print_Barcode_Reasons;
                    }
                    else
                    {
                        response.Message = "";
                        response.Status = true;
                        response.products = products;
                        response.units = units;
                        response.Print_Barcode_Reasons = Print_Barcode_Reasons;
                    }
                }
                }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = ex.Message; 
            }
            return Ok(response);
        }
    }
}