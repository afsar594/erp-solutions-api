using AutoMapper;
using Inspire.Erp.Application.Authentication.Inerfaces;
using Inspire.Erp.Application.Common;
using Inspire.Erp.Application.Master;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Entities.documents;
using Inspire.Erp.Domain.Enums;
using Inspire.Erp.Domain.Modals;
using Inspire.Erp.Domain.Modals.Common;
using Inspire.Erp.Domain.Models;
using Inspire.Erp.Web.ViewModels;
using Inspire.Erp.Web.ViewModels.Procurement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inspire.Erp.Application.Document.Interfaces;
namespace Inspire.Erp.Web.Controllers.Document
{
    [Route("api")]
    [Produces("application/json")]
    [ApiController]
    public class HrDocumentRequestController : ControllerBase
    {
        private IMapper mapper;
        private IHrDocumentRequestService service;
        public HrDocumentRequestController(IHrDocumentRequestService service, IMapper mapper)
        {
            this.service = service;
            this.mapper = mapper;
        }

        [HttpGet]
        [Route("GetAllHrDocumentRequests")]
        public List<HrDocumentRequestViewModel> GetAllHrDocumentRequests()
        {
            try
            {
                return service.GetAllHrDocumentRequest().Select(x => new HrDocumentRequestViewModel()
                {
                    AppByStaffId = x.AppByStaffId,
                    AppDate = x.AppDate,
                    Approved = x.Approved,
                    ApproveDate = x.ApproveDate,
                    AppStatus = x.AppStatus,
                    DocTypeId = x.DocTypeId,
                    InOut = x.InOut,
                    Purpose = x.Purpose,
                    Remarks = x.Remarks,
                    ReqId = x.ReqId,
                    StaffId = x.StaffId
                }).ToList();
            }
            catch (Exception ex)
            {
                return new List<HrDocumentRequestViewModel>();
            }
        }

        [HttpPost]
        [Route("InsertHrDocumentRequest")]
        public List<HrDocumentRequestViewModel> InsertHrDocumentRequest([FromBody] HrDocumentRequestViewModel model)
        {
            try
            {
                var docRequest = mapper.Map<HrDocumentRequest>(model);
                return service.InsertHrDocumentRequest(docRequest).Select(x => new HrDocumentRequestViewModel()
                {
                    AppByStaffId = x.AppByStaffId,
                    AppDate = x.AppDate,
                    Approved = x.Approved,
                    ApproveDate = x.ApproveDate,
                    AppStatus = x.AppStatus,
                    DocTypeId = x.DocTypeId,
                    InOut = x.InOut,
                    Purpose = x.Purpose,
                    Remarks = x.Remarks,
                    ReqId = x.ReqId,
                    StaffId = x.StaffId
                }).ToList();
            }
            catch (Exception ex)
            {
                return new List<HrDocumentRequestViewModel>();
            }
        }


        [HttpPost]
        [Route("UpdateHrDocumentRequest")]
        public List<HrDocumentRequestViewModel> UpdateHrDocumentRequest([FromBody] HrDocumentRequestViewModel model)
        {
            try
            {
                var docRequest = mapper.Map<HrDocumentRequest>(model);
                return service.UpdateHrDocumentRequest(docRequest).Select(x => new HrDocumentRequestViewModel()
                {
                    AppByStaffId = x.AppByStaffId,
                    AppDate = x.AppDate,
                    Approved = x.Approved,
                    ApproveDate = x.ApproveDate,
                    AppStatus = x.AppStatus,
                    DocTypeId = x.DocTypeId,
                    InOut = x.InOut,
                    Purpose = x.Purpose,
                    Remarks = x.Remarks,
                    ReqId = x.ReqId,
                    StaffId = x.StaffId
                }).ToList();
            }
            catch (Exception ex)
            {
                return new List<HrDocumentRequestViewModel>();
            }
        }

        [HttpPost]
        [Route("DeleteHrDocumentRequest")]
        public List<HrDocumentRequestViewModel> DeleteHrDocumentRequest([FromBody] HrDocumentRequestViewModel model)
        {
            try
            {
                var docRequest = mapper.Map<HrDocumentRequest>(model);
                return service.InsertHrDocumentRequest(docRequest).Select(x => new HrDocumentRequestViewModel()
                {
                    AppByStaffId = x.AppByStaffId,
                    AppDate = x.AppDate,
                    Approved = x.Approved,
                    ApproveDate = x.ApproveDate,
                    AppStatus = x.AppStatus,
                    DocTypeId = x.DocTypeId,
                    InOut = x.InOut,
                    Purpose = x.Purpose,
                    Remarks = x.Remarks,
                    ReqId = x.ReqId,
                    StaffId = x.StaffId
                }).ToList();
            }
            catch (Exception ex)
            {
                return new List<HrDocumentRequestViewModel>();
            }
        }
    }
}