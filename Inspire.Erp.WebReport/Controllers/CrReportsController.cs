using CrystalReportWebAPI.Utilities;
using Inspire.Erp.WebReport.Models;
using Inspire.Erp.WebReport.ReportEngine;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web.Http;
namespace Inspire.Erp.WebReport.Controllers
{
    [RoutePrefix("api/Reports")]
    public class CrReportsController : ApiController
    {
        /// <summary>
        /// Name : getReportPrint
        /// Desc : This is used to generate report and convert to pdf
        /// </summary>
        /// <param name="reportFilterModel"> 
        ///  used as a parameter
        ///  accpected format as given balow 
        /// </param>
        /// <returns></returns>
        /// 
        /************************* Accepted Parameter Format ***************************/
        /**
                                {
                                  "ReportName":"crpJobCardEntryTable",
                                  "SelectionFormula":"{tbl_JobCard.JC_Id_N}=1006",
                                  "Parameters": [
                                    {
                                      "Key": "Add1",
                                      "Value": "Address test 1"
                                    },
                                    {
                                      "Key": "Add2",
                                      "Value": "Address test 2"
                                    },
                                    {
                                      "Key": "Email",
                                      "Value": "email@gamil.com"
                                    },
                                    {
                                      "Key": "Heading",
                                      "Value": "Main Heading"
                                    },
                                    {
                                      "Key": "Company",
                                      "Value": "Company Name"
                                    },
                                    {
                                      "Key": "subHeading",
                                      "Value": "sub Heading"
                                    }
                                  ]
                                }         
         
         */
        [AllowAnonymous]
        [Route("getReportPrint")]
        [HttpPut]
        [ClientCacheWithEtag(60)]  //1 min client side caching
        public HttpResponseMessage getReportPrint([FromBody] ReportFilterModel reportFilterModel)
        {
            string exportPath = AppDomain.CurrentDomain.BaseDirectory + @"FileExports";
            string filePath = AppDomain.CurrentDomain.BaseDirectory + $@"Reports\{reportFilterModel.ReportName}.rpt";
            HttpResponseMessage result;
            try
            {

                CrystalReport report = new CrystalReport();
                result = report.RenderReport(
                                                                    PathReport: filePath,
                                                                    SelectionFormula: reportFilterModel.SelectionFormula,
                                                                    Namefile: reportFilterModel.ReportName  + ".pdf", reportFilterModel.Parameters
                                                               );
                return result;
            }
            catch (Exception ex)
            {
                var errorResponse = Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
                return errorResponse;
            }
        }

     
    }
}