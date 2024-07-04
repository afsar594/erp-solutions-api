using AutoMapper;
using Inspire.Erp.Application.Procurement.Interface;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Modals.Common;
using Inspire.Erp.Domain.Models;
using Inspire.Erp.Domain.Models.Procurement.PurchaseOrderTracking;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NPOI.OpenXmlFormats.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inspire.Erp.Web.Controllers.Procurement
{
    [Route("api/PurchassOrderTracking")]
    [Produces("application/json")]
    [ApiController]
    public class PurchassOrderTrackingController : Controller
    {

        private IPurchaseOrdertracking purchaseOrdertracking;
        private IRepository<UserTracking> _UserTracking;
        private IRepository<UserFile> _UserFile;
        private IMapper _mapper;
        public PurchassOrderTrackingController(IRepository<UserFile> UserFile,IPurchaseOrdertracking purchaseOrdertracking, IRepository<UserTracking> UserTracking, IMapper mapper)
        {
            this.purchaseOrdertracking = purchaseOrdertracking;
            _mapper = mapper;
            _UserTracking = UserTracking;
            _UserFile = UserFile;
        }

        //Irfan 08 Dec 2023
        /// <summary>
        /// Name : getPurchaseOrderTrackingDetails
        /// Desc : used to get purchase order tracking details 
        /// </summary>
        /// <param name="reportFilter">
        /// need to set these attrubutes to load data accordingly
        /// PONO, fromDate, toDate, SupplierId, JobId, LocationId, ItemId
        /// </param>
        /// <returns> 
        /// </returns>
        [HttpPost]
        [Route("GetPurchaseOrderTrackingDetails")]
        public async Task<IActionResult> getPurchaseOrderTrackingDetails(ReportFilter reportFilter)
        {
            return Ok(await purchaseOrdertracking.GetPurchaseOrderTrackingDetails(reportFilter));
        }

        //Irfan 08 Dec 2023
        /// <summary>
        /// Name : GetPurchaseOrderTrackingSummary
        /// Desc : used to get purchase order tracking Summary 
        /// </summary>
        /// <param name="reportFilter">
        ///  need to set these attrubutes to load data accordingly
        /// PONO, fromDate, toDate, SupplierId, JobId, LocationId, ItemId</param>
        /// <returns> 
        /// </returns>
        [HttpPost]
        [Route("GetPurchaseOrderTrackingSummary")]
        public async Task<IActionResult> getPurchaseOrderTrackingSummary(ReportFilter reportFilter)
        {
            return Ok(await purchaseOrdertracking.GetPurchaseOrderTrackingSummary(reportFilter));
        }

        [HttpPost]
        [Route("GetUserTrackingDetails")]
        public async Task<IActionResult> GetUserTrackingDetails(UserTrackFilter reportFilter)
        {
            return Ok(await purchaseOrdertracking.GetUserTrackingDetails(reportFilter));
        }

        [HttpGet("GetTrackingUserVPType")]
        public ResponseInfo GetTrackingUserVPType()
        {
            var objectresponse = new ResponseInfo();
            var response = _UserTracking.GetAsQueryable().Select(ut => new
            {
                VpType = ut.UserTrackingUserVpType ?? "",
            }).Distinct().ToList();            
            objectresponse.ResultSet = new
            {
                response = response
            };
            return objectresponse;
        }
        [HttpGet("GetTrackingUserVPAction")]
        public ResponseInfo GetTrackingUserVPAction()
        {
            var objectresponse = new ResponseInfo();
            var response = _UserTracking.GetAsQueryable().Select(ut => new
            {
                UserTrackingUserVpAction = ut.UserTrackingUserVpAction ?? ""
                
            }).Distinct().ToList();
            objectresponse.ResultSet = new
            {
                response = response
            };
            return objectresponse;
        }
        [HttpGet("GetTrackingUserUserFile")]
        public ResponseInfo GetTrackingUserUserFile()
        {
            var objectresponse = new ResponseInfo();
            var response = _UserFile.GetAsQueryable().Select(ut => new
            {
                LogInId  = ut.UserId,
                username = ut.UserName

            }).Distinct().ToList();
            objectresponse.ResultSet = new
            {
                response = response
            };
            return objectresponse;
        }

    }
}
