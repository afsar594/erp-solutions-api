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
    [Route("api/HrDocumentType")]
    [Produces("application/json")]
    [ApiController]
    public class HrDocumentTypeController : ControllerBase
    {

            private readonly IMapper mapper;
            private IHrDocumentTypeService hrDocumentTypeService;

            public HrDocumentTypeController(IHrDocumentTypeService _hrDocumentTypeService, IMapper mapper)
            {
                this.mapper = mapper;
                this.hrDocumentTypeService = _hrDocumentTypeService;
            }

        [HttpGet]
        [Route("GetDocumentType")]
        public ApiResponse<List<HrDocumentType>> GetDocumentType()
        {
            ApiResponse<List<HrDocumentType>> apiResponse = new ApiResponse<List<HrDocumentType>>
            {
                Valid = true,
                Result = mapper.Map<List<HrDocumentType>>(hrDocumentTypeService.GetAllHrDocumentType()),
                Message = ""
            };
            return apiResponse;
        }
        [HttpGet("GetDocumentTypeById/{id}")]
        public ApiResponse<HrDocumentType> GetDocumentTypeById(int id)
        {
            var DocumentType = hrDocumentTypeService.GetDocumentTypeById(id);

            ApiResponse<HrDocumentType> apiResponse = new ApiResponse<HrDocumentType>
            {
                Valid = true,
                Result = DocumentType,
                Message = ""
            };
            return apiResponse;
        }

        [HttpPost]
        [Route("InsertDocumentType")]
        public ApiResponse<HrDocumentType> InsertDocumentType([FromBody] HrDocumentType DocumentTypeViewModel)
        {
            var param1 = mapper.Map<HrDocumentType>(DocumentTypeViewModel);
            var UpdateJobInvoiceViewModel = hrDocumentTypeService.InsertDocumentType(param1);


            ApiResponse<HrDocumentType> apiResponse = new ApiResponse<HrDocumentType>
            {
                Valid = true,
                Result = mapper.Map<HrDocumentType>(UpdateJobInvoiceViewModel),
                Message = "Document Type has been saved."

            };
            return apiResponse;
        }
        [HttpPost]
        [Route("UpdateHrDocumentType")]
        public ApiResponse<HrDocumentType> UpdateHrDocumentType([FromBody] HrDocumentType DocumentTypeViewModel)
        {
            var param1 = mapper.Map<HrDocumentType>(DocumentTypeViewModel);
            var UpdateJobInvoiceViewModel = hrDocumentTypeService.UpdateDocumentType(param1);


            ApiResponse<HrDocumentType> apiResponse = new ApiResponse<HrDocumentType>
            {
                Valid = true,
                Result = mapper.Map<HrDocumentType>(UpdateJobInvoiceViewModel),
                Message = "Document Type has been Updated."

            };
            return apiResponse;
        }
        [HttpDelete]
        [Route("DeleteHrDocumentType")]
        public IActionResult DeleteHrDocumentType(long Id)
        {
            try
            {
                return Ok(hrDocumentTypeService.DeleteDocumentType(Id));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message.ToString());
            }
        }
     
        [HttpGet]
        [Route("GetAllCompanies")]
        public List<StationMasterList> GetAllCompanies()
        {
            try
            {
                return hrDocumentTypeService.GetAllCompanies().Select(x => new StationMasterList()
                {
                    StationMasterCode = x.StationMasterCode,
                    StationMasterStationName = x.StationMasterStationName,
                   
                }).ToList();
            }
            catch (Exception ex)
            {
                return new List<StationMasterList>();
            }
        }
        [HttpGet]
        [Route("GetAlldocumentType")]
        public List<HrDocumentTypeList> GetAlldocumentType()
        {
            try
            {
                return hrDocumentTypeService.GetAlldocumentType().Select(x => new HrDocumentTypeList()
                {
                    DocumentType = x.DocumentType,
                    DocumentTypeId = x.DocumentTypeId,

                }).ToList();
            }
            catch (Exception ex)
            {
                return new List<HrDocumentTypeList>();
            }
        }
    }
 }

