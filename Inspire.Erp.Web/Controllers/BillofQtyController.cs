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

namespace Inspire.Erp.Web.Controllers
{
    [Route("api")]
    [Produces("application/json")]
    [ApiController]
    public class BillofQtyController : ControllerBase
    {
        private readonly IBillofQtyService _billofQtyService;
        private readonly IMapper _mapper;
        private IRepository<BillofQtyMaster> _BillofQtyMasterRepository;
        private IRepository<BillOfQTyDetails> _BillOfQTyDetailsRepository;
        private IRepository<JobMaster> jobrepository;
        private IRepository<ItemMaster> itemrepository;
        private IRepository<UnitMaster> unitrepository;
        private readonly IRepository<PurchaseRequisitionDetails> _purchaseRequisitionDetails;

        public BillofQtyController(IBillofQtyService billofQtyService, IMapper mapper, IRepository<JobMaster> _jobrepository, IRepository<ItemMaster> _itemrepository, IRepository<UnitMaster> _unitrepository
            , IRepository<BillOfQTyDetails> BillOfQTyDetailsRepository, IRepository<BillofQtyMaster> BillofQtyMasterRepository, IRepository<PurchaseRequisitionDetails> purchaseRequisitionDetails)
        {
            _billofQtyService = billofQtyService;
            _mapper = mapper;
            jobrepository = _jobrepository; itemrepository = _itemrepository; unitrepository = _unitrepository;
            _BillofQtyMasterRepository = BillofQtyMasterRepository; _BillOfQTyDetailsRepository = BillOfQTyDetailsRepository;
            _purchaseRequisitionDetails = purchaseRequisitionDetails;
        }
        [HttpPost]
        [Route("InsertBillofQty")]
        public ApiResponse<BillofQtyMasterModel> InsertBillofQty([FromBody] BillofQtyMasterModel customerQuotationCompositeView)
        {
            ApiResponse<BillofQtyMasterModel> apiResponse = new ApiResponse<BillofQtyMasterModel>();

            try
            {
                var param1 = _mapper.Map<BillofQtyMaster>(customerQuotationCompositeView);
                var param2 = _mapper.Map<List<BillOfQTyDetails>>(customerQuotationCompositeView.BillOfQTyDetails);
                var param3 = _mapper.Map<List<EstimationCostCenter>>(customerQuotationCompositeView.EstimationCostCenter);

                var xs = _billofQtyService.InsertBillofQty(param1, param2, param3);

                BillofQtyMasterModel CustomerQuotationViewModel = new BillofQtyMasterModel();

                if (xs != null)
                {
                    CustomerQuotationViewModel = _mapper.Map<BillofQtyMasterModel>(xs);
                    CustomerQuotationViewModel.BillOfQTyDetails = _mapper.Map<List<BillOfQTyDetailsModel>>(xs.BillOfQTyDetails);
                    CustomerQuotationViewModel.EstimationCostCenter = _mapper.Map<List<EstimationCostCenterModel>>(xs.EstimationCostCenter);


                    apiResponse = new ApiResponse<BillofQtyMasterModel>
                    {
                        Valid = true,
                        Result = CustomerQuotationViewModel,
                        Message = BillofQtyMessage.SaveBillofQty
                    };
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                apiResponse = new ApiResponse<BillofQtyMasterModel>
                {
                    Valid = false,
                    Error = true,
                    Exception = ex.Message,
                    Message = BillofQtyMessage.BillofQtyError
                };
            }
            return apiResponse;

        }
        [HttpPost]
        [Route("UpdateBillofQty")]
        public ApiResponse<BillofQtyMasterModel> UpdateBillofQty([FromBody] BillofQtyMasterModel customerQuotationCompositeView)
        {
            var param1 = _mapper.Map<BillofQtyMaster>(customerQuotationCompositeView);
            var param2 = _mapper.Map<List<BillOfQTyDetails>>(customerQuotationCompositeView.BillOfQTyDetails);
            var param3 = _mapper.Map<List<EstimationCostCenter>>(customerQuotationCompositeView.EstimationCostCenter);

            var xs = _billofQtyService.UpdateBillofQty(param1, param2, param3);
            if (xs != null)
            {
                BillofQtyMasterModel CustomerQuotationViewModel = new BillofQtyMasterModel();
                CustomerQuotationViewModel = _mapper.Map<BillofQtyMasterModel>(xs);
                CustomerQuotationViewModel.BillOfQTyDetails = _mapper.Map<List<BillOfQTyDetailsModel>>(xs.BillOfQTyDetails);
                CustomerQuotationViewModel.EstimationCostCenter = _mapper.Map<List<EstimationCostCenterModel>>(xs.EstimationCostCenter);


                ApiResponse<BillofQtyMasterModel> apiResponse = new ApiResponse<BillofQtyMasterModel>
                {
                    Valid = true,
                    Result = CustomerQuotationViewModel,
                    Message = BillofQtyMessage.UpdateBillofQty
                };
                return apiResponse;
            }
            else
            {
                return null;
            }
        }
        [HttpGet]
        [Route("DeleteBillofQty/{id}")]
        public ApiResponse<int> DeleteBillofQty(decimal id)
        {
            ApiResponse<int> apiResponse = new ApiResponse<int>
            {
                Valid = true,
                Result = _mapper.Map<int>(_billofQtyService.DeleteBillofQty(id)),
                Message = ""
            };
            return apiResponse;

        }
        [HttpGet]
        [Route("GetBillofQty")]
        public ApiResponse<List<BillofQtyMaster>> GetBillofQty()
        {
            ApiResponse<List<BillofQtyMaster>> apiResponse = new ApiResponse<List<BillofQtyMaster>>
            {
                Valid = true,
                Result = _mapper.Map<List<BillofQtyMaster>>(_billofQtyService.GetBillofQty()),
                Message = ""
            };
            return apiResponse;
        }
        [HttpGet]
        [Route("GetSavedBillofQty/{id}")]
        public ApiResponse<BillofQtyMasterModel> GetSavedBillofQty(decimal id)
        {

            ApiResponse<BillofQtyMasterModel> apiResponse = new ApiResponse<BillofQtyMasterModel>
            {
                Valid = true,
                Result = _mapper.Map<BillofQtyMasterModel>(_billofQtyService.GetSavedBillofQty(id)),
                Message = ""
            };
            return apiResponse;
        }


        [HttpGet]
        [Route("GetBillofQtyTracking")]
        public IActionResult GetBillofQtyTracking([FromQuery] DateTime? fromDate,[FromQuery] DateTime? toDate, [FromQuery] string? jobNo)
        {
            return Ok(_billofQtyService.GetBillofQtyTracking(fromDate, toDate,jobNo));
        }


        [HttpGet]
        [Route("BOQLoadDropdown")]
        public ResponseInfo BOQLoadDropdown()
        {
            var objresponse = new ResponseInfo();


            var jobMasters = jobrepository.GetAsQueryable().Where(a => a.JobMasterJobDelStatus != true).Select(c => new
            {
                c.JobMasterJobId,
                JobMasterJobName = c.JobMasterJobName.Trim(),
                c.JobMasterJobNumber
            }).OrderBy(x => x.JobMasterJobName).AsEnumerable().ToList();
            var itemMaster = itemrepository.GetAsQueryable().Where(k => k.ItemMasterAccountNo != 0 && (k.ItemMasterDelStatus != true)
                    && k.ItemMasterItemType != ItemMasterStatus.Group).Select(k => new
                    {
                        k.ItemMasterItemId,
                        k.ItemMasterPartNo,
                        ItemMasterItemName = k.ItemMasterItemName.Trim()
                    }).OrderBy(x => x.ItemMasterItemName).AsEnumerable().ToList();
            var unitMasters = unitrepository.GetAsQueryable().Where(a => a.UnitMasterUnitDelStatus != true).Select(x => new
            {
                x.UnitMasterUnitId,
                UnitMasterUnitFullName = x.UnitMasterUnitFullName.Trim(),
                UnitMasterUnitShortName = x.UnitMasterUnitShortName.Trim()
            }).OrderBy(x => x.UnitMasterUnitShortName).AsEnumerable().ToList();
            objresponse.ResultSet = new
            {
                jobMasters = jobMasters,
                itemMaster = itemMaster,
                unitMasters = unitMasters,
            };
            return objresponse;
        }

        [HttpGet]
        [Route("LoadBOQ")]
        public ResponseInfo LoadBOQ(int customerEnqId)
        {
            var objResponse = new ResponseInfo();
            var detailList = _BillOfQTyDetailsRepository.GetAsQueryable().AsNoTracking().OrderBy(x => x.BillQtyId).ToList();
            var details = _BillofQtyMasterRepository.GetAsQueryable().AsNoTracking().OrderBy(x => x.BillofqtyNo).ToList();

            if (customerEnqId != 0)
            {
                details = details.Where(a => a.CustomerEnqId == customerEnqId).OrderBy(x => x.BillofqtyNo).ToList();
            }
            var billQtyIds = details.Select(x => x.Id).ToList();
            var requisitionQuantities = _purchaseRequisitionDetails.GetAsQueryable().Where(x => billQtyIds.Contains((decimal)x.boqdId))
                .AsNoTracking()
                .GroupBy(p => new { p.boqdId, p.PurchaseRequisitionDetailsMatId })
                .Select(g => new
                {
                    BillQtyId = g.Key.boqdId,
                    ItemCode = g.Key.PurchaseRequisitionDetailsMatId,
                    RecQty = g.Sum(x => x.PurchaseRequisitionDetailsQuantity)
                }).ToList();

            var data = (from d in detailList
                        join c in details on d.BillQtyId equals c.Id
                        join i in itemrepository.GetAll() on d.ItemCode equals i.ItemMasterItemId into igrData
                        from i in igrData.DefaultIfEmpty()
                        where (customerEnqId == 0 || c.CustomerEnqId == customerEnqId)
                        where d.IsEdit == true
                        let receivedQty = requisitionQuantities.FirstOrDefault(x => x.BillQtyId == d.BillQtyId && x.ItemCode == d.ItemCode)?.RecQty ?? 0
                        select new BoqDetailDto
                        {
                            Id = c.Id,
                            dId = d.Id,
                            BillofqtyNo = c.BillofqtyNo,
                            Date = c.Date,
                            SupplierId = i?.ItemMasterSupplierId ?? -1,
                            CustomerEnqId = c.CustomerEnqId,
                            ItemCode = d.ItemCode,
                            Qty = d.Qty,
                            UnitCode = d.UnitCode,
                            Rate = d.Rate,
                            Amount = d.Amount,
                            CurrRate = d.CurrencyRate ?? 1,
                            ReceivedQty = receivedQty,
                            BalanceQty = (d.Qty ?? 0m) - receivedQty,
                        }).OrderBy(x => x.BillofqtyNo).Distinct().ToList();


            objResponse.ResultSet = new
            {
                result = data.Where(x => x.BalanceQty > 0)
            };
            return objResponse;
        }

    }
}
