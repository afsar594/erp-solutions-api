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
    [Route("api/HrDocumentStatusMaster")]
    [Produces("application/json")]
    [ApiController]
    public class HrDocumentStatusMasterController : ControllerBase
    {
        private IMapper mapper;
        private IHrDocumentStatusMasterService service;
        public HrDocumentStatusMasterController(IHrDocumentStatusMasterService service, IMapper mapper)
        {
            this.service = service;
            this.mapper = mapper;
        }

        [HttpGet]
        [Route("GetAllHrDocumentStatus")]
        public List<HrDocumentStatusMasterViewModel> GetAllHrDocumentStatus()
        {
            try
            {
                return service.GetAllHrDocumentStatusMaster().Select(x => new HrDocumentStatusMasterViewModel()
                {
                    DocStatus = x.DocStatus,
                    DocStatusId = x.DocStatusId
                }).ToList();
            }
            catch (Exception ex)
            {
                return new List<HrDocumentStatusMasterViewModel>();
            }
        }

        [HttpPost]
        [Route("InsertHrDocumentStatus")]
        public List<HrDocumentStatusMasterViewModel> InsertHrDocumnetStatus([FromBody] HrDocumentStatusMasterViewModel hrDocumentStatusMasterViewModel)
        {
            try
            {
                var docStatus = mapper.Map<HrDocumentStatusMaster>(hrDocumentStatusMasterViewModel);
                return service.InsertHrDocumentStatusMaster(docStatus).Select(x => new HrDocumentStatusMasterViewModel()
                {
                    DocStatus = x.DocStatus,
                    DocStatusId = x.DocStatusId
                }
                ).ToList();
            }
            catch (Exception ex)
            {
                return new List<HrDocumentStatusMasterViewModel>();
            }
        }


        [HttpPost]
        [Route("UpdateHrDocumentStatus")]
        public List<HrDocumentStatusMasterViewModel> UpadteHrDocumnetStatus([FromBody] HrDocumentStatusMasterViewModel hrDocumentStatusMasterViewModel)
        {
            try
            {
                var docStatus = mapper.Map<HrDocumentStatusMaster>(hrDocumentStatusMasterViewModel);
                return service.UpdateHrDocumentStatusMaster(docStatus).Select(x => new HrDocumentStatusMasterViewModel()
                {
                    DocStatus = x.DocStatus,
                    DocStatusId = x.DocStatusId
                }
                ).ToList();
            }
            catch (Exception ex)
            {
                return new List<HrDocumentStatusMasterViewModel>();
            }
        }

        [HttpPost]
        [Route("DeleteHrDocumentStatus")]
        public List<HrDocumentStatusMasterViewModel> DeleteHrDocumnetStatus([FromBody] HrDocumentStatusMasterViewModel hrDocumentStatusMasterViewModel)
        {
            try
            {
                var docStatus = mapper.Map<HrDocumentStatusMaster>(hrDocumentStatusMasterViewModel);
                return service.DeleteHrDocumentStatusMaster(docStatus).Select(x => new HrDocumentStatusMasterViewModel()
                {
                    DocStatus = x.DocStatus,
                    DocStatusId = x.DocStatusId
                }
                ).ToList();
            }
            catch (Exception ex)
            {
                return new List<HrDocumentStatusMasterViewModel>();
            }
        }
    }
}
