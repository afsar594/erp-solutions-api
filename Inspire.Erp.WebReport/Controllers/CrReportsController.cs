using CrystalReportWebAPI.Utilities;
using Inspire.Erp.WebReport.Cores;
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
    [Authorize]
    [AllowCrossSite]
    public class CrReportsController : ApiController
    {
        [AllowCrossSite]
        [AllowAnonymous]
        [Route("getReportPrint/{folderName}")]
        [HttpPut]
        [ClientCacheWithEtag(60)]
        public HttpResponseMessage getReportPrint([FromUri] string folderName, [FromBody] ReportFilterModel reportFilterModel)
        {
            HttpResponseMessage result;
            try
            {
                Logger.LogInfo($"getReportPrint called with folderName: {folderName} and ReportName: {reportFilterModel.ReportName}");

                string driPath = AppDomain.CurrentDomain.BaseDirectory;
                CrystalReport report = new CrystalReport();
                var folderPath = Path.Combine(driPath, "bin", folderName);
                var dsnName = Path.Combine(folderPath, "DSN.json");
                var dbSettings = report.getDbSettingsFromJSonFle(dsnName);
                string filePath = Path.Combine(folderPath, "Reports", $"{reportFilterModel.ReportName}.rpt");

                result = report.RenderReport(
                    PathReport: filePath,
                    SelectionFormula: reportFilterModel.SelectionFormula,
                    Namefile: reportFilterModel.ReportName + ".pdf",
                    reportFilterModel.Parameters,
                    dbSettings
                );

                Logger.LogInfo($"Report successfully generated for {reportFilterModel.ReportName}");
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError("Failed to generate report in getReportPrint", ex);
                var errorResponse = Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
                return errorResponse;
            }
        }

        [AllowCrossSite]
        [AllowAnonymous]
        [Route("getPrintByQuery/{folderName}")]
        [HttpPut]
        [ClientCacheWithEtag(60)]
        public HttpResponseMessage getPrintByQuery([FromUri] string folderName, [FromBody] ReportFilterModel reportFilterModel)
        {
            HttpResponseMessage result;
            try
            {
                Logger.LogInfo($"getPrintByQuery called with folderName: {folderName} and ReportName: {reportFilterModel.ReportName}");

                string driPath = AppDomain.CurrentDomain.BaseDirectory;
                CrystalReport report = new CrystalReport();

                var folderPath = Path.Combine(driPath, "bin", folderName);
                var dsnName = Path.Combine(folderPath, "DSN.json");
                var dbSettings = report.getDbSettingsFromJSonFle(dsnName);
                string filePath = Path.Combine(folderPath, "Reports", $"{reportFilterModel.ReportName}.rpt");

                result = report.RenderReport(filePath, reportFilterModel, dbSettings);

                Logger.LogInfo($"Report successfully generated for {reportFilterModel.ReportName}");
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError("Failed to generate report in getPrintByQuery", ex);
                var errorResponse = Request.CreateResponse(HttpStatusCode.InternalServerError, "An error occurred: " + ex.Message);
                return errorResponse;
            }
        }
    }
}
