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
using Inspire.Erp.Application.Master.Interfaces;

namespace Inspire.Erp.Web.Controllers.Document
{
    [ApiController]
    [Route("api/HrDocumentMaster")]
    public class HrDocumentMasterController : ControllerBase
    {

        //NERVER CHANGE, REPLACE OR REMOVE ANY OF THE FOLLOWING, MAY LEADS TO AN UN-EXPECTED ERROR....!
        private readonly IMapper mapper;
        private IHrDocumentMasterService hrDocumentMasterService;
        //added by irfan 08 Mar -2023
        /// <summary>
        /// Name : HrDocumentMasterController (constructor)
        /// Desc : is used to initialized the hrDocumentMasterService object to perform the CRUD operations
        /// </summary>
        /// <param name="mapper">
        /// <param name="hrDocumentMasterService">
        ///  accepts two argument of type IMapper  and IHrDocumentMasterService
        /// </param>
        public HrDocumentMasterController(IHrDocumentMasterService hrDocumentMasterService, IMapper mapper)
        {
            this.mapper = mapper;
            this.hrDocumentMasterService = hrDocumentMasterService;
        }


        //added by irfan 08 Mar -2023
        /// <summary>
        /// Name : GetAllHrDocumentMasters
        /// Desc : is used read all the salary heads from db table 
        /// </summary>
        /// <returns>
        ///  in the response returns an object of type  List<HrDocumentMasterViewModel> 
        /// </returns>
        [HttpGet]
        [Route("GetAllHrDocumentMasters")]
        public List<HrDocumentMasterViewModel> GetAllHrDocumentMasters()
        {
            try
            {
                return hrDocumentMasterService.GetAllHrDocumentMasters().Select(x => new HrDocumentMasterViewModel()
                {
                    DocumentId = x.DocumentId,
                    DocumentTypeId = x.DocumentTypeId,
                    StaffId = x.StaffId,
                    CompanyId = x.CompanyId,
                    VehicleId = x.VehicleId,
                    OtherId = x.OtherId,
                    MasterType = x.MasterType,
                    DocName = x.DocName,
                    DocNo = x.DocNo,
                    Remarks = x.Remarks,
                    IssueDate = x.IssueDate,
                    ExpDate = x.ExpDate,
                    DocImage = x.DocImage,
                    Status = x.Status,
                    DocStatusId = x.DocStatusId
                }).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpPost]
        [Route("GetReport")]
        //public IActionResult GetReport([FromBody] DtoDocumentReportParam _DtoDocumentReportParam)
        //{
        //    try
        //    {
        //        Console.WriteLine("_DtoDocumentReportParam = " + Newtonsoft.Json.JsonConvert.SerializeObject(_DtoDocumentReportParam));
        //        IQueryable data;
        //        if (_DtoDocumentReportParam.reportType == "" || _DtoDocumentReportParam.reportType == null)
        //        {
        //            return NotFound();
        //        }
        //        else if (_DtoDocumentReportParam.reportType == "GetStaffDocumentReport")
        //        {
        //            data = hrDocumentMasterService.GetStaffDocumentReport(_DtoDocumentReportParam.staffIds, _DtoDocumentReportParam.documentTypeIds, _DtoDocumentReportParam.documentStatudId);
        //            return Ok(data);
        //        }
        //        else if (_DtoDocumentReportParam.reportType == "GetVehicleDocumentReport")
        //        {
        //            data = hrDocumentMasterService.GetVehicleDocumentReport(_DtoDocumentReportParam.vehicleIds, _DtoDocumentReportParam.documentTypeIds, _DtoDocumentReportParam.documentStatudId);
        //            return Ok(data);
        //        }
        //        else if (_DtoDocumentReportParam.reportType == "GetCompanyDocumentReport")
        //        {
        //            data = hrDocumentMasterService.GetCompanyDocumentReport(_DtoDocumentReportParam.compayIds, _DtoDocumentReportParam.documentTypeIds, _DtoDocumentReportParam.documentStatudId);
        //            return Ok(data);
        //        }
        //        return Ok(null);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex);
        //        return StatusCode(500, ex.Message);
        //    }
        //}
        //added by irfan 08 Mar -2023
        /// <summary>
        /// Name : InsertHrDocumentMaster
        /// Desc : is used to insert new records to db table 
        /// </summary>
        /// <param name="hrDocumentMasterViewModel">
        ///     accepts an argument of type HrDocumentMasterViewModel
        /// </param>
        /// <returns>
        ///  in the response returns an object of type  List<HrDocumentMasterViewModel> 
        /// </returns>
        [HttpPost]
        [Route("InsertHrDocumentMaster")]
        //public List<HrDocumentMasterViewModel> InsertHrDocumentMaster([FromBody] HrDocumentMasterViewModel hrDocumentMasterViewModel)
        //{
        //    try
        //    {
        //        var hrDocumentMaster = mapper.Map<HrDocumentMaster>(hrDocumentMasterViewModel);
        //        return this.hrDocumentMasterService.InsertHrDocumentMaster(hrDocumentMaster).Select(x => new HrDocumentMasterViewModel()
        //        {
        //            DocumentId = x.DocumentId,
        //            DocumentTypeId = x.DocumentTypeId,
        //            StaffId = x.StaffId,
        //            CompanyId = x.CompanyId,
        //            VehicleId = x.VehicleId,
        //            OtherId = x.OtherId,
        //            MasterType = x.MasterType,
        //            DocName = x.DocName,
        //            DocNo = x.DocNo,
        //            Remarks = x.Remarks,
        //            IssueDate = x.IssueDate,
        //            ExpDate = x.ExpDate,
        //            DocImage = x.DocImage,
        //            Status = x.Status,
        //            DocStatusId = x.DocStatusId
        //        }).ToList();
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}


        [HttpPost]
        [Route("InsertHrDocumentMaster")]
        public ApiResponse<HrDocumentMaster> InsertHrDocumentMaster([FromBody] HrDocumentMaster HrDocumentMaster)
        {

            var InsertJobInvoiceViewModel = hrDocumentMasterService.InsertHrDocumentMaster(HrDocumentMaster);

            ApiResponse<HrDocumentMaster> apiResponse = new ApiResponse<HrDocumentMaster>
            {
                Valid = true,
                Result = mapper.Map<HrDocumentMaster>(HrDocumentMaster),
                Message = "Saved."
            };
            return apiResponse;
        }

        [HttpPost]
        [Route("UpdateHrDocumentMaster")]
        public List<HrDocumentMasterViewModel> UpdateHrDocumentMaster([FromBody] HrDocumentMasterViewModel hrDocumentMasterViewModel)
        {
            try
            {
                var hrDocumentMaster = mapper.Map<HrDocumentMaster>(hrDocumentMasterViewModel);
                return this.hrDocumentMasterService.UpdateHrDocumentMaster(hrDocumentMaster).Select(x => new HrDocumentMasterViewModel()
                {
                    DocumentId = x.DocumentId,
                    DocumentTypeId = x.DocumentTypeId,
                    StaffId = x.StaffId,
                    CompanyId = x.CompanyId,
                    VehicleId = x.VehicleId,
                    OtherId = x.OtherId,
                    MasterType = x.MasterType,
                    DocName = x.DocName,
                    DocNo = x.DocNo,
                    Remarks = x.Remarks,
                    IssueDate = x.IssueDate,
                    ExpDate = x.ExpDate,
                    DocImage = x.DocImage,
                    Status = x.Status,
                    DocStatusId = x.DocStatusId
                }).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        //added by irfan 08 Mar -2023
        /// <summary>
        /// Name : DeleteHrDocumentMaster
        /// Desc : is used to delete existing record in db table 
        /// </summary>
        /// <param name="hrDocumentMasterViewModel">
        ///     accepts an argument of type HrDocumentMasterViewModel
        /// </param>
        /// <returns>
        ///  in the response returns an object of type  List<HrDocumentMasterViewModel> 
        /// </returns>
        [HttpPost]
        [Route("DeleteHrDocumentMaster")]
        public List<HrDocumentMasterViewModel> DeleteHrDocumentMaster([FromBody] HrDocumentMasterViewModel hrDocumentMasterViewModel)
        {
            try
            {
                var hrDocumentMaster = mapper.Map<HrDocumentMaster>(hrDocumentMasterViewModel);
                return this.hrDocumentMasterService.DeleteHrDocumentMaster(hrDocumentMaster).Select(x => new HrDocumentMasterViewModel()
                {
                    DocumentId = x.DocumentId,
                    DocumentTypeId = x.DocumentTypeId,
                    StaffId = x.StaffId,
                    CompanyId = x.CompanyId,
                    VehicleId = x.VehicleId,
                    OtherId = x.OtherId,
                    MasterType = x.MasterType,
                    DocName = x.DocName,
                    DocNo = x.DocNo,
                    Remarks = x.Remarks,
                    IssueDate = x.IssueDate,
                    ExpDate = x.ExpDate,
                    DocImage = x.DocImage,
                    Status = x.Status,
                    DocStatusId = x.DocStatusId
                }).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpGet]
        [Route("GetAllStaffDocumentMaster")]
        public IActionResult GetAllStaffDocumentMaster()
        {
            try
            {
                List<HrDocumentMaster> item = hrDocumentMasterService.GetAllStaffDocumentMaster();
                return Ok(item);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpGet]
        [Route("GetAllVeicleDocumentMaster")]
        public IActionResult GetAllVeicleDocumentMaster()
        {
            try
            {
                List<HrDocumentMaster> item = hrDocumentMasterService.GetAllVeicleDocumentMaster();
                return Ok(item);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpGet]
        [Route("GetDocumentMasterByVehicleID")]
        public IActionResult GetDocumentMasterByVehicleID(long vehicleId)
        {
            try
            {
                List<HrDocumentMaster> item = hrDocumentMasterService.GetDocumentMasterByVehicleID(vehicleId);
                return Ok(item);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpGet]
        [Route("GetAllDocumentsVehicle")]
        public IActionResult GetAllDocumentsVehicle()
        {
            try
            {
                var item = hrDocumentMasterService.GetAllDocumentsVehicle();
                return Ok(item);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message.ToString());
            }
        }

        [HttpGet]
        [Route("getDocumentImagesByMasterId")]
        public IActionResult getDocumentImagesByMasterId(long Id)
        {
            try
            {
                var item = hrDocumentMasterService.getDocumentImagesByMasterId(Id);
                return Ok(item);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message.ToString());
            }
        }

        [HttpGet]
        [Route("GetAllDocumentsStaff")]
        public IActionResult GetAllDocumentsStaff()
        {
            try
            {
                var item = hrDocumentMasterService.GetAllDocumentsStaff();
                return Ok(item);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message.ToString());
            }
        }

        [HttpGet]
        [Route("GetDocumentMasterByStaffID")]
        public IActionResult GetDocumentMasterByStaffID(long staffId)
        {
            try
            {
                List<HrDocumentMaster> item = hrDocumentMasterService.GetDocumentMasterByStaffID(staffId);
                return Ok(item);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        [HttpGet]
        [Route("GetDocumentTypeReport")]
        public IActionResult GetDocumentTypeReport(HrDocumentTypeFilter docFilter)
        {
            try
            {
                var item = hrDocumentMasterService.GetDocumentTypeReport(docFilter);
                return Ok(item);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
