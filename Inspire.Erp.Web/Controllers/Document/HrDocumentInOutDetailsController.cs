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
    [Produces("application/json")]
    [Route("api")]
    [ApiController]
    public class HrDocumentInOutDetailsController : ControllerBase
    {
        private IMapper mapper;
        private IHrDocumentInOutDetailsService service;
        public HrDocumentInOutDetailsController(IHrDocumentInOutDetailsService service, IMapper mapper)
        {
            this.service = service;
            this.mapper = mapper;
        }


        [HttpGet]
        [Route("GetAllHrDocumentInOutDetails")]
        public List<HrDocumentInOutDetailsViewModel> GetAllHrDocumentInOutDetails()
        {
            try
            {
                return service.GetAllHrDocumentInOutDetails().Select(x => new HrDocumentInOutDetailsViewModel()
                {
                    AuthorisedStaffId = x.AuthorisedStaffId,
                    DocResubDys = x.DocResubDys,
                    DocTypeId = x.DocTypeId,
                    Id = x.Id,
                    InDate = x.InDate,
                    IssueStaffid = x.IssueStaffid,
                    Lcdate = x.Lcdate,
                    Lcno = x.Lcno,
                    LeaveId = x.LeaveId,
                    OutDate = x.OutDate,
                    Purpose = x.Purpose,
                    Remarks = x.Remarks,
                    StaffId = x.StaffId,
                    Status = x.Status
                }).ToList();
            }
            catch (Exception ex)
            {
                return new List<HrDocumentInOutDetailsViewModel>();
            }
        }

        [HttpPost]
        [Route("InsertHrDocumentInOutDetail")]
        public List<HrDocumentInOutDetailsViewModel> InsertHrDocumentInOutDetail([FromBody] HrDocumentInOutDetailsViewModel hrDocumentInOutDetailsViewModel)
        {
            try
            {
                var docInOutDetail = mapper.Map<HrDocumentInOutDetails>(hrDocumentInOutDetailsViewModel);
                return service.InsertHrDocumentInOutDetail(docInOutDetail).Select(x => new HrDocumentInOutDetailsViewModel()
                {
                    AuthorisedStaffId = x.AuthorisedStaffId,
                    DocResubDys = x.DocResubDys,
                    DocTypeId = x.DocTypeId,
                    Id = x.Id,
                    InDate = x.InDate,
                    IssueStaffid = x.IssueStaffid,
                    Lcdate = x.Lcdate,
                    Lcno = x.Lcno,
                    LeaveId = x.LeaveId,
                    OutDate = x.OutDate,
                    Purpose = x.Purpose,
                    Remarks = x.Remarks,
                    StaffId = x.StaffId,
                    Status = x.Status
                }).ToList();
            }
            catch (Exception ex)
            {
                return new List<HrDocumentInOutDetailsViewModel>();
            }
        }

        [HttpPost]
        [Route("DeleteHrDocumentInOutDetail")]
        public List<HrDocumentInOutDetailsViewModel> DeleteHrDocumentInOutDetail([FromBody] HrDocumentInOutDetailsViewModel hrDocumentInOutDetailsViewModel)
        {
            try
            {
                var docInOutDetail = mapper.Map<HrDocumentInOutDetails>(hrDocumentInOutDetailsViewModel);
                return service.DeleteHrDocumentInOutDetail(docInOutDetail).Select(x => new HrDocumentInOutDetailsViewModel()
                {
                    AuthorisedStaffId = x.AuthorisedStaffId,
                    DocResubDys = x.DocResubDys,
                    DocTypeId = x.DocTypeId,
                    Id = x.Id,
                    InDate = x.InDate,
                    IssueStaffid = x.IssueStaffid,
                    Lcdate = x.Lcdate,
                    Lcno = x.Lcno,
                    LeaveId = x.LeaveId,
                    OutDate = x.OutDate,
                    Purpose = x.Purpose,
                    Remarks = x.Remarks,
                    StaffId = x.StaffId,
                    Status = x.Status
                }).ToList();
            }
            catch (Exception ex)
            {
                return new List<HrDocumentInOutDetailsViewModel>();
            }
        }

        [HttpPost]
        [Route("UpdateHrDocumentInOutDetail")]
        public List<HrDocumentInOutDetailsViewModel> UpdateHrDocumentInOutDetail([FromBody] HrDocumentInOutDetailsViewModel hrDocumentInOutDetailsViewModel)
        {
            try
            {
                var docInOutDetail = mapper.Map<HrDocumentInOutDetails>(hrDocumentInOutDetailsViewModel);
                return service.UpdateHrDocumentInOutDetail(docInOutDetail).Select(x => new HrDocumentInOutDetailsViewModel()
                {
                    AuthorisedStaffId = x.AuthorisedStaffId,
                    DocResubDys = x.DocResubDys,
                    DocTypeId = x.DocTypeId,
                    Id = x.Id,
                    InDate = x.InDate,
                    IssueStaffid = x.IssueStaffid,
                    Lcdate = x.Lcdate,
                    Lcno = x.Lcno,
                    LeaveId = x.LeaveId,
                    OutDate = x.OutDate,
                    Purpose = x.Purpose,
                    Remarks = x.Remarks,
                    StaffId = x.StaffId,
                    Status = x.Status
                }).ToList();
            }
            catch (Exception ex)
            {
                return new List<HrDocumentInOutDetailsViewModel>();
            }
        }
    }
}
