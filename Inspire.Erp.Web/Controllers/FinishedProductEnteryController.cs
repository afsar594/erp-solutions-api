using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Inspire.Erp.Application.Sales.Interfaces;
using Inspire.Erp.Application.Common;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Enums;
using Inspire.Erp.Web.Common;
using Inspire.Erp.Web.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Inspire.Erp.Web.ViewModels.Accounts;
using Inspire.Erp.Domain.Modals.Sales;
using Inspire.Erp.Web.ViewModels.Procurement;
using Inspire.Erp.Application.Store.Implementation;
using Inspire.Erp.Domain.Modals.Stock;
using Inspire.Erp.Application.Account;
using Inspire.Erp.Domain.Modals.Common;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using Microsoft.EntityFrameworkCore;
using Inspire.Erp.Application.Sales.Implementations;

namespace Inspire.Erp.Web.Controllers
{
    [Route("api")]
    [Produces("application/json")]
    [ApiController]
    public class FinishedProductController : ControllerBase
    {
        private IFinishedProduct FinishedProductService;
        private readonly IMapper _mapper;
        private IRepository<FinishedProductMaster> _BillofQtyMasterRepository; private IRepository<FinishedProductDetails> _BillOfQTyDetailsRepository;
        private IRepository<JobMaster> jobrepository; private IRepository<ItemMaster> itemrepository; private IRepository<UnitMaster> unitrepository;
        public FinishedProductController(IFinishedProduct FinishedProductService, IMapper mapper, IRepository<JobMaster> _jobrepository, IRepository<ItemMaster> _itemrepository, IRepository<UnitMaster> _unitrepository
            , IRepository<FinishedProductDetails> BillOfQTyDetailsRepository, IRepository<FinishedProductMaster> BillofQtyMasterRepository)
        {
            FinishedProductService = FinishedProductService;
            _mapper = mapper;
            jobrepository = _jobrepository; itemrepository = _itemrepository; unitrepository = _unitrepository;
            _BillofQtyMasterRepository = BillofQtyMasterRepository; _BillOfQTyDetailsRepository = BillOfQTyDetailsRepository;
        }
        [HttpPost]
        [Route("InsertFinishedProduct")]
        public ApiResponse<FinishedProductMasterModel> InsertFinishProduct([FromBody] FinishedProductMasterModel customerQuotationCompositeView)
        {
            ApiResponse<FinishedProductMasterModel> apiResponse = new ApiResponse<FinishedProductMasterModel>();

            try
            {
                // Mapping input to entities
                var param1 = _mapper.Map<FinishedProductMaster>(customerQuotationCompositeView);
                var param2 = _mapper.Map<List<FinishedProductDetails>>(customerQuotationCompositeView.FinishedProductDetails);

                // Calling the service to insert the data
                var xs = FinishedProductService.InsertFinishProduct(param1, param2);

                if (xs != null)
                {
                    var finishedProductMasterModel = _mapper.Map<FinishedProductMasterModel>(xs);
                    //finishedProductMasterModel.FinishedProductDetails = _mapper.Map<List<FinishedProductDetailsModel>>(xs.FinishedProductDetails);

                    apiResponse = new ApiResponse<FinishedProductMasterModel>
                    {
                        Valid = true,
                        Result = finishedProductMasterModel,
                        Message = BillofQtyMessage.SaveBillofQty
                    };
                }
                else
                {
                    apiResponse = new ApiResponse<FinishedProductMasterModel>
                    {
                        Valid = false,
                        Error = true,
                        Message = "Failed to insert the Bill of Quantity."
                    };
                }
            }
            catch (Exception ex)
            {
                apiResponse = new ApiResponse<FinishedProductMasterModel>
                {
                    Valid = false,
                    Error = true,
                    Exception = ex.ToString(),
                    Message = BillofQtyMessage.BillofQtyError
                };
            }

            return apiResponse;
        }

        //[HttpPost]
        //[Route("UpdateBillofQty")]
        //public ApiResponse<BillofQtyMasterModel> UpdateBillofQty([FromBody] BillofQtyMasterModel customerQuotationCompositeView)
        //{
        //    var param1 = _mapper.Map<BillofQtyMaster>(customerQuotationCompositeView);
        //    var param2 = _mapper.Map<List<BillOfQTyDetails>>(customerQuotationCompositeView.BillOfQTyDetails);
        //    var xs = _customerQuotationService.UpdateBillofQty(param1, param2);
        //    if (xs != null)
        //    {
        //        BillofQtyMasterModel CustomerQuotationViewModel = new BillofQtyMasterModel();
        //        CustomerQuotationViewModel = _mapper.Map<BillofQtyMasterModel>(xs);
        //        CustomerQuotationViewModel.BillOfQTyDetails = _mapper.Map<List<BillOfQTyDetailsModel>>(xs.BillOfQTyDetails);

        //        ApiResponse<BillofQtyMasterModel> apiResponse = new ApiResponse<BillofQtyMasterModel>
        //        {
        //            Valid = true,
        //            Result = CustomerQuotationViewModel,
        //            Message = BillofQtyMessage.UpdateBillofQty
        //        };
        //        return apiResponse;
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}
        //[HttpDelete]
        //[Route("DeleteBillofQty/{id}")]
        //public ApiResponse<int> DeleteBillofQty(decimal id)
        //{
        //    ApiResponse<int> apiResponse = new ApiResponse<int>
        //    {
        //        Valid = true,
        //        Result = _mapper.Map<int>(_customerQuotationService.DeleteBillofQty(id)),
        //        Message = ""
        //    };
        //    return apiResponse;

        //}
        [HttpGet]
        [Route("GetallFinishedProduct")]
        public ApiResponse<List<FinishedProductMaster>> GetIFinishedProduct()
        {
            ApiResponse<List<FinishedProductMaster>> apiResponse = new ApiResponse<List<FinishedProductMaster>>
            {
                Valid = true,
                Result = _mapper.Map<List<FinishedProductMaster>>(FinishedProductService.GetIFinishedProduct()),
                Message = ""
            };
            return apiResponse;
        }
        //[HttpGet]
        //[Route("GetSavedBillofQty/{id}")]
        //public ApiResponse<BillofQtyMasterModel> GetSavedBillofQty(decimal id)
        //{

        //    ApiResponse<BillofQtyMasterModel> apiResponse = new ApiResponse<BillofQtyMasterModel>
        //    {
        //        Valid = true,
        //        Result = _mapper.Map<BillofQtyMasterModel>(_customerQuotationService.GetSavedBillofQty(id)),
        //        Message = ""
        //    };
        //    return apiResponse;
        //}

        //[HttpGet]
        //[Route("BOQLoadDropdown")]
        //public ResponseInfo BOQLoadDropdown()
        //{
        //    var objresponse = new ResponseInfo();


        //    var jobMasters = jobrepository.GetAsQueryable().Where(a => a.JobMasterJobDelStatus != true).Select(c => new
        //    {
        //        c.JobMasterJobId,
        //        JobMasterJobName = c.JobMasterJobName.Trim(),
        //    }).OrderBy(x=>x.JobMasterJobName).AsEnumerable().ToList();
        //    var itemMaster = itemrepository.GetAsQueryable().Where(k => k.ItemMasterAccountNo != 0 && (k.ItemMasterDelStatus != true)
        //            && k.ItemMasterItemType != ItemMasterStatus.Group).Select(k => new
        //            {
        //                k.ItemMasterItemId,
        //                k.ItemMasterPartNo,
        //                ItemMasterItemName = k.ItemMasterItemName.Trim()
        //            }).OrderBy(x => x.ItemMasterItemName).AsEnumerable().ToList();
        //    var unitMasters = unitrepository.GetAsQueryable().Where(a => a.UnitMasterUnitDelStatus != true).Select(x => new
        //    {
        //        x.UnitMasterUnitId,
        //        UnitMasterUnitFullName = x.UnitMasterUnitFullName.Trim(),
        //        UnitMasterUnitShortName = x.UnitMasterUnitShortName.Trim()
        //    }).OrderBy(x => x.UnitMasterUnitShortName).AsEnumerable().ToList();
        //    objresponse.ResultSet = new
        //    {
        //        jobMasters = jobMasters,
        //        itemMaster = itemMaster,
        //        unitMasters = unitMasters,
        //    };
        //    return objresponse;
        //}

        //[HttpGet]
        //[Route("LoadBOQ")]
        //public ResponseInfo LoadBOQ(int customerEnqId)
        //{
        //    var objResponse = new ResponseInfo();
        //    var detailList = _BillOfQTyDetailsRepository.GetAsQueryable().AsNoTracking().OrderBy(x => x.BillQtyId).ToList();
        //    var details = _BillofQtyMasterRepository.GetAsQueryable().AsNoTracking().OrderBy(x => x.BillofqtyNo).ToList();
        //    if (customerEnqId != 0)
        //    {
        //        details = details.Where(a => a.CustomerEnqId == customerEnqId).OrderBy(x => x.BillofqtyNo).AsEnumerable().ToList();
        //    }
        //    // Filtering detailList based on PurchaseOrderNos and customerEnqId
        //    var data = (from d in detailList
        //                join c in details on d.BillQtyId equals c.Id
        //                where (customerEnqId == 0 || c.CustomerEnqId == customerEnqId)
        //                where d.IsEdit == true
        //                select new BoqDetailDto
        //                {
        //                    Id = c.Id,
        //                    dId = d.Id,
        //                    BillofqtyNo = c.BillofqtyNo,
        //                    Date = c.Date,
        //                    CustomerEnqId = c.CustomerEnqId,
        //                    ItemCode = d.ItemCode,
        //                    Qty = d.Qty,
        //                    UnitCode = d.UnitCode
        //                }).OrderBy(x => x.dId)
        //        .OrderBy(x => x.BillofqtyNo).AsEnumerable().ToList();

        //    objResponse.ResultSet = new
        //    {
        //        result = data
        //    };
        //    return objResponse;
        //}

    }
}
