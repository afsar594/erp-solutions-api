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
using Inspire.Erp.Domain.Modals.Stock;
using NPOI.SS.Formula.Functions;
namespace Inspire.Erp.Web.Controllers.Document
{   
    [ApiController]
    [Route("api/HrDocumentMaster")]
    public class HrDocumentMasterController : ControllerBase
    {
        private readonly IMapper mapper;
        private IHrDocumentMasterService hrDocumentMasterService;
        public HrDocumentMasterController(IHrDocumentMasterService hrDocumentMasterService, IMapper mapper)
        {
            this.mapper = mapper;
            this.hrDocumentMasterService = hrDocumentMasterService;
        }


        [HttpGet]
        [Route("GetAllDocumentMaster")]
        public ApiResponse<List<DocumentMaster>> GetAllHrDocumentMasters()
        {
            ApiResponse<List<DocumentMaster>> apiResponse = new ApiResponse<List<DocumentMaster>>
            {
                Valid = true,
                Result = mapper.Map<List<DocumentMaster>>(hrDocumentMasterService.GetAllHrDocumentMasters()),
                Message = ""
            };
            return apiResponse;
        }

        [HttpPost]
        [Route("InsertDocumentMaster")]
        public ApiResponse<DocumentMaster> InsertDocumentMaster([FromBody] DocumentMaster DocumentMaster)
        {

            var DocumentMasterView = hrDocumentMasterService.InsertHrDocumentMaster(DocumentMaster);

            ApiResponse<DocumentMaster> apiResponse = new ApiResponse<DocumentMaster>
            {
                Valid = true,
                Result = mapper.Map<DocumentMaster>(DocumentMasterView),
                Message = "Document Master has been Saved."
            };
            return apiResponse;
        }

        [HttpPost]
        [Route("UpdateDocumentMaster")]
        public ApiResponse<DocumentMaster> UpdateHrDocumentMaster([FromBody] DocumentMaster DocumentMaster)
        {
            var UpdateDocumentMasterl = hrDocumentMasterService.UpdateHrDocumentMaster(DocumentMaster);


            ApiResponse<DocumentMaster> apiResponse = new ApiResponse<DocumentMaster>
            {
                Valid = true,
                Result = mapper.Map<DocumentMaster>(UpdateDocumentMasterl),
                Message = "Document Master has been Updated."

            };
            return apiResponse;
        }
        [HttpDelete]

        [Route("DeleteDocumentMaster")]
        public IActionResult DeleteHrDocumentMaster(long Id)
        {
            try
            {
                return Ok(hrDocumentMasterService.DeleteDocumentMaster(Id));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message.ToString());
            }
        }

        [HttpGet("GetDocumentMasterById/{id}")]
        public ApiResponse<DocumentMaster> GetDocumentMasterById(int id)
        {
            var DocumentMasters = hrDocumentMasterService.GetDocumentMasterById(id);

            ApiResponse<DocumentMaster> apiResponse = new ApiResponse<DocumentMaster>
            {
                Valid = true,
                Result = DocumentMasters,
                Message = ""
            };
            return apiResponse;
        }
        [HttpPost]
        [Route("GetDocumentReport")]
        public IActionResult GetDocumentReport(Documentfilter model)
        {
            return Ok(hrDocumentMasterService.GetDocumentReport(model));
        }
    }
}
