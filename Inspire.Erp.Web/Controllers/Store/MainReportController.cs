using AutoMapper;
using Inspire.Erp.Application.Account;
using Inspire.Erp.Application.Account.Implementations;
using Inspire.Erp.Application.Account.Interfaces;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Modals.Common;
using Inspire.Erp.Domain.Models.ViewModels;
using Inspire.Erp.Infrastructure.Database;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using Inspire.Erp.Web.Common;
using Inspire.Erp.Web.ViewModels;
using Inspire.Erp.Web.ViewModels.Procurement;
using Inspire.Erp.Web.ViewModels.Store;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;




namespace Inspire.Erp.Web.Controllers.Store
{

    [Route("api/MainReport")]
    [Produces("application/json")]
    [ApiController]
    public class MainReportController : ControllerBase
    {
        private IMainReportService _mainReportService;
        private readonly IMapper _mapper;
        public readonly InspireErpDBContext _Context;
        private IRepository<CostCenterMaster> costcenterrepository;

        private IRepository<PaymentVoucher> _paymentVoucher;
        private IRepository<BankPaymentVoucher> _bankPaymentVoucher;
        private IRepository<ReceiptVoucherMaster> _receiptVoucher;
        private IRepository<BankReceiptVoucher> _bankReceiptVoucher;
        private IRepository<SalesVoucher> _salesVoucher;
        private IRepository<SalesReturn> _salesReturnVoucher;
        private IRepository<PurchaseVoucher> _purchaseVoucher;
        private IRepository<PurchaseReturn> _purchaseReturnVoucher;
        private IRepository<JournalVoucher> _journalVoucher;
        private IRepository<ContraVoucher> _contraVoucher;
        private IRepository<CreditNote> _creditNote;
        private IRepository<DebitNote> _debitNote;

        private IRepository<JobMaster> jobrepository; private IRepository<LocationMaster> locationrepository; private IRepository<CurrencyMaster> currencyrepository;
        private IChartofAccountsService chartofAccountsService; private IRepository<ViewAccountsTransactionsVoucherType> _viewAccountsTransactionsVoucherTypeRepository;
        public MainReportController(IMainReportService mainReportService, IMapper mapper, IRepository<CostCenterMaster> _countryrepository
            , IRepository<JobMaster> _jobrepository, IRepository<LocationMaster> _locationrepository, IChartofAccountsService _chartofAccountsService,
            IRepository<ViewAccountsTransactionsVoucherType> viewAccountsTransactionsVoucherTypeRepository, InspireErpDBContext Context, IRepository<PaymentVoucher> paymentVoucher, IRepository<BankPaymentVoucher> bankPaymentVoucher, IRepository<ReceiptVoucherMaster> receiptVoucher, IRepository<BankReceiptVoucher> bankReceiptVoucher, IRepository<SalesVoucher> salesVoucher, IRepository<SalesReturn> salesReturnVoucher, IRepository<PurchaseVoucher> purchaseVoucher, IRepository<PurchaseReturn> purchaseReturnVoucher, IRepository<JournalVoucher> journalVoucher, IRepository<ContraVoucher> contraVoucher, IRepository<CreditNote> creditNote, IRepository<DebitNote> debitNote)
        {
            _mainReportService = mainReportService;
            _mapper = mapper; _viewAccountsTransactionsVoucherTypeRepository = viewAccountsTransactionsVoucherTypeRepository;
            costcenterrepository = _countryrepository;
            jobrepository = _jobrepository; locationrepository = _locationrepository;
            chartofAccountsService = _chartofAccountsService;
            _Context = Context;
            _paymentVoucher = paymentVoucher;
            _bankPaymentVoucher = bankPaymentVoucher;
            _receiptVoucher = receiptVoucher;
            _bankReceiptVoucher = bankReceiptVoucher;
            _salesVoucher = salesVoucher;
            _salesReturnVoucher = salesReturnVoucher;
            _purchaseVoucher = purchaseVoucher;
            _purchaseReturnVoucher = purchaseReturnVoucher;
            _journalVoucher = journalVoucher;
            _contraVoucher = contraVoucher;
            _creditNote = creditNote;
            _debitNote = debitNote;
        }

        [HttpGet]
        [Route("MainReport_GetReportAccountsTransactions/{AccNo}/{Location}/{Job}/{CostCenter}")]
        public ApiResponse<List<ReportAccountsTransactions>> MainReport_GetReportAccountsTransactions(string AccNo, int Location, int Job, int CostCenter)
        {



            ApiResponse<List<ReportAccountsTransactions>> apiResponse = new ApiResponse<List<ReportAccountsTransactions>>
            {
                Valid = true,
                Result = _mapper.Map<List<ReportAccountsTransactions>>(_mainReportService.MainReport_GetReportAccountsTransactions(
                     AccNo, Location, Job, CostCenter
                )),
                Message = ""
            };
            return apiResponse;
        }

        [HttpPost]
        [Route("MainReport_GetReportAccountsTransactions_PARAM_CLASS")]
        public ApiResponse<List<ReportAccountsTransactions>> MainReport_GetReportAccountsTransactions_PARAM_CLASS([FromBody] AccountStatementParametersViewModels voucherCompositeView)
        {
            var param1 = _mapper.Map<AccountStatementParameters>(voucherCompositeView);

            ApiResponse<List<ReportAccountsTransactions>> apiResponse = new ApiResponse<List<ReportAccountsTransactions>>
            {

                Valid = true,
                Result = _mapper.Map<List<ReportAccountsTransactions>>(_mainReportService.MainReport_GetReportAccountsTransactions_PARAM_CLASS(
                     param1
                )),
                Message = ""
            };
            return apiResponse;
        }
        [HttpPost]
        [Route("CustomerStattemnetbySummery")]
        public ApiResponse<List<ReportAccountsTransactions>> CustomerStattemnetbySummery([FromBody] AccountStatementParametersViewModels voucherCompositeView)
        {
            var param1 = _mapper.Map<AccountStatementParameters>(voucherCompositeView);
            ApiResponse<List<ReportAccountsTransactions>> apiResponse = new ApiResponse<List<ReportAccountsTransactions>>
            {
                Valid = true,
                Result = _mapper.Map<List<ReportAccountsTransactions>>(_mainReportService.CustomerStattemnetbySummery(
                     param1
                )),
                Message = ""
            };
            return apiResponse;
        }

        [HttpGet]
        [Route("MainReport_GetReportStockRegister")]
        public ApiResponse<List<ReportStockRegister>> MainReport_GetReportStockRegister()
        {
            ApiResponse<List<ReportStockRegister>> apiResponse = new ApiResponse<List<ReportStockRegister>>
            {
                Valid = true,
                Result = _mapper.Map<List<ReportStockRegister>>(_mainReportService.MainReport_GetReportStockRegister()),
                Message = ""
            };
            return apiResponse;
        }

        [HttpPost]
        [Route("MainReport_GetReportStockLedger_PARAM_CLASS")]
        public ApiResponse<List<ReportStockBaseUnit>> MainReport_GetReportStockLedger_PARAM_CLASS([FromBody] StockLedgerParametersViewModel voucherCompositeView)
        {
            var param1 = _mapper.Map<StockLedgerParameters>(voucherCompositeView);
            var param2 = _mapper.Map<List<ViewStockTransferType>>(voucherCompositeView.viewStockTransferType);
            ApiResponse<List<ReportStockBaseUnit>> apiResponse = new ApiResponse<List<ReportStockBaseUnit>>
            {
                Valid = true,
                Result = _mapper.Map<List<ReportStockBaseUnit>>(_mainReportService.MainReport_GetReportStockLedger_PARAM_CLASS(
                     param1, param2
                )),
                Message = ""
            };
            return apiResponse;
        }

        [HttpPost]
        [Route("MainReport_GetReportStockLedger_Location_PARAM_CLASS")]
        public ApiResponse<List<ReportStockBaseUnit>> MainReport_GetReportStockLedger_Location_PARAM_CLASS([FromBody] StockLedgerParametersViewModel voucherCompositeView)
        {
            var param1 = _mapper.Map<StockLedgerParameters>(voucherCompositeView);
            var param2 = _mapper.Map<List<ViewStockTransferType>>(voucherCompositeView.viewStockTransferType);

            ApiResponse<List<ReportStockBaseUnit>> apiResponse = new ApiResponse<List<ReportStockBaseUnit>>
            {
                Valid = true,
                Result = _mapper.Map<List<ReportStockBaseUnit>>(_mainReportService.MainReport_GetReportStockLedger_Location_PARAM_CLASS(
                     param1, param2
                )),
                Message = ""
            };
            return apiResponse;
        }

        [HttpPost]
        [Route("MainReport_GetReportSalesVoucher_PARAM_CLASS")]
        public ApiResponse<List<ReportSalesVoucher>> MainReport_GetReportSalesVoucher_PARAM_CLASS([FromBody] SalesvoucherreportParametersViewModel voucherCompositeView)
        {
            var param1 = _mapper.Map<SalesvoucherreportParameters>(voucherCompositeView);
            var param2 = _mapper.Map<List<ViewStockTransferType>>(voucherCompositeView.viewStockTransferType);

            ApiResponse<List<ReportSalesVoucher>> apiResponse = new ApiResponse<List<ReportSalesVoucher>>
            {
                Valid = true,
                Result = _mapper.Map<List<ReportSalesVoucher>>(_mainReportService.MainReport_GetReportSalesVoucher_PARAM_CLASS(
                     param1, param2
                )),
                Message = ""
            };
            return apiResponse;
        }

        [HttpPost]
        [Route("MainReport_GetReportPurchaseVoucher_PARAM_CLASS")]
        public ApiResponse<List<ReportPurchaseVoucher>> MainReport_GetReportPurchaseVoucher_PARAM_CLASS([FromBody] PurchasevoucherreportParametersViewModel voucherCompositeView)
        {
            var param1 = _mapper.Map<PurchasevoucherreportParameters>(voucherCompositeView);
            var param2 = _mapper.Map<List<ViewStockTransferType>>(voucherCompositeView.viewStockTransferType);

            ApiResponse<List<ReportPurchaseVoucher>> apiResponse = new ApiResponse<List<ReportPurchaseVoucher>>
            {
                Valid = true,
                Result = _mapper.Map<List<ReportPurchaseVoucher>>(_mainReportService.MainReport_GetReportPurchaseVoucher_PARAM_CLASS(
                     param1, param2
                )),
                Message = ""
            };
            return apiResponse;
        }

        [HttpPost]
        [Route("MainReport_GetReportPurchaseOrder_PARAM_CLASS")]
        public ApiResponse<List<ReportPurchaseOrder>> MainReport_GetReportPurchaseOrder_PARAM_CLASS([FromBody] PurchaseOrderReportParametersViewModel voucherCompositeView)
        {
            var param1 = _mapper.Map<PurchaseOrderReportParameters>(voucherCompositeView);
            var param2 = _mapper.Map<List<ViewStockTransferType>>(voucherCompositeView.viewStockTransferType);

            ApiResponse<List<ReportPurchaseOrder>> apiResponse = new ApiResponse<List<ReportPurchaseOrder>>
            {
                Valid = true,

                Result = _mapper.Map<List<ReportPurchaseOrder>>(_mainReportService.MainReport_GetReportPurchaseOrder_PARAM_CLASS(
                     param1, param2
                )),
                Message = ""
            };
            return apiResponse;

        }

        [HttpPost]
        [Route("MainReport_GetReportPurchaseRequisition_PARAM_CLASS")]
        public ApiResponse<List<ReportPurchaseRequisition>> MainReport_GetReportPurchaseRequisition_PARAM_CLASS([FromBody] PurchaserequisitionReportParametersViewModel voucherCompositeView)
        {
            var param1 = _mapper.Map<PurchaserequisitionReportParameters>(voucherCompositeView);
            var param2 = _mapper.Map<List<ViewStockTransferType>>(voucherCompositeView.viewStockTransferType);
            ApiResponse<List<ReportPurchaseRequisition>> apiResponse = new ApiResponse<List<ReportPurchaseRequisition>>
            {
                Valid = true,
                Result = _mapper.Map<List<ReportPurchaseRequisition>>(_mainReportService.MainReport_GetReportPurchaseRequisition_PARAM_CLASS(
                     param1, param2
                )),
                Message = ""
            };
            return apiResponse;
        }

        [HttpPost]
        [Route("MainReport_GetReportProjectStatus_PARAM_CLASS")]
        public ApiResponse<List<ReportProjectStatus>> MainReport_GetReportProjectStatus_PARAM_CLASS([FromBody] ProjectStatusReportParameters projectStatusCompositeView)
        {
              if (projectStatusCompositeView.JobId != null && projectStatusCompositeView.JobId != -1)
                {
                    _Context.Database.ExecuteSqlRaw("EXEC Spr_UpdateProjectStatus {0}", projectStatusCompositeView.JobId);
                }
           
            var param1 = _mapper.Map<ProjectStatusReportParameters>(projectStatusCompositeView);
           

            ApiResponse<List<ReportProjectStatus>> apiResponse = new ApiResponse<List<ReportProjectStatus>>
            {
                Valid = true,
                Result = _mapper.Map<List<ReportProjectStatus>>(_mainReportService.MainReport_GetReportProjectStatus_PARAM_CLASS(
                     param1
                )),
                Message = ""
            };
            return apiResponse;
        }



        [HttpPost]
        [Route("MainReport_GetReportPurchaseRequisition")]
        public async Task<IActionResult> MainReport_GetReportPurchaseRequisition([FromBody] PurchaserequisitionReportParametersViewModel voucherCompositeView)
        {
            try
            {
                var param1 = _mapper.Map<PurchaserequisitionReportParameters>(voucherCompositeView);
                var param2 = _mapper.Map<List<ViewStockTransferType>>(voucherCompositeView.viewStockTransferType);
                return Ok(await _mainReportService.MainReport_GetReportPurchaseRequisition(param1, param2));
            }
            catch (Exception ex)
            {
                // Log the full exception details here
                return StatusCode(500, $"An error occurred while retrieving GetReportPurchaseRequisition: {ex.Message}");
            }
        }


     
        [HttpPost]
        [Route("GetReportPurchaseRequisition")]
        public async Task<IActionResult> GetReportPurchaseRequisition([FromBody] ReportPRVM voucherCompositeView)
        {
            try
            {
                return Ok(await _mainReportService.GetReportPurchaseRequisition(voucherCompositeView));
            }
            catch (Exception ex)
            {
                // Log the full exception details here
                return StatusCode(500, $"An error occurred while retrieving GetReportPurchaseRequisition: {ex.Message}");
            }
        }


    

        [HttpGet]
        [Route("MainReport_GetAllItemMaster")]
        public List<ItemMaster> MainReport_GetAllItemMaster()
        {
            return _mapper.Map<List<ItemMaster>>(_mainReportService.MainReport_GetAllItemMaster());
        }

        [HttpGet]
        [Route("MainReport_GetAllItemMaster_Group")]
        public List<ItemMaster> MainReport_GetAllItemMaster_Group()
        {
            return _mapper.Map<List<ItemMaster>>(_mainReportService.MainReport_GetAllItemMaster_Group());
        }

        [HttpGet]
        [Route("MainReport_GetAccountsTransactions_VoucherType")]
        public List<ViewAccountsTransactionsVoucherType> MainReport_GetAccountsTransactions_VoucherType()
        {
            return _mapper.Map<List<ViewAccountsTransactionsVoucherType>>(_mainReportService.MainReport_GetAccountsTransactions_VoucherType());
        }

        [HttpGet]
        [Route("AccountsTransactionsLoadDropdown")]
        public ResponseInfo AccountsTransactionsLoadDropdown()
        {
            var objectresponse = new ResponseInfo();

            var costcenterMasters = costcenterrepository.GetAsQueryable().Where(a => a.CostCenterMasterCostCenterDelStatus != true).Select(c => new
            {
                c.CostCenterMasterCostCenterId,
                c.CostCenterMasterCostCenterName,
            }).OrderBy(x => x.CostCenterMasterCostCenterName).ToList();
            var jobMasters = jobrepository.GetAsQueryable().Where(a => a.JobMasterJobDelStatus != true).Select(c => new
            {
                c.JobMasterJobId,
                c.JobMasterJobName,
            }).OrderBy(x => x.JobMasterJobName).ToList();
            var locationMaster = locationrepository.GetAsQueryable().Where(a => a.LocationMasterLocationDelStatus != true).Select(c => new
            {
                c.LocationMasterLocationId,
                c.LocationMasterLocationName,
            }).OrderBy(x => x.LocationMasterLocationName).ToList();

            var masterAccountsTables = chartofAccountsService.GetAllAccounts().Where(a => a.MaAccType == "A" && a.MaDelStatus != true).Select(c => new
            {
                c.MaAccNo,
                c.MaAccName,
                c.MaRelativeNo,
                c.MaSno
            }).OrderBy(x => x.MaAccName).ToList();

            var vouchers = _viewAccountsTransactionsVoucherTypeRepository.GetAsQueryable().ToList();
            objectresponse.ResultSet = new
            {
                costcenterMasters = costcenterMasters,
                masterAccountsTables = masterAccountsTables,
                jobMasters = jobMasters,
                locationMaster = locationMaster,
                vouchers = vouchers
            };
            return objectresponse;
        }

        [HttpPost]
        [Route("GetAccountIdByVoucherNo")]
        public async Task<IActionResult> GetAccountIdByVoucherNo(string type, string voucherNo)
        {
            if (string.IsNullOrWhiteSpace(type) || string.IsNullOrWhiteSpace(voucherNo))
                return BadRequest("Invalid request data.");

            try
            {
                int? id = type.ToLower() switch
                {
                    "paymentvoucher" => await _paymentVoucher.GetAsQueryable()
                        .Where(x => x.PaymentVoucherVoucherNo == voucherNo)
                        .Select(x => (int?)x.PaymentVoucherSno)
                        .FirstOrDefaultAsync(),

                    "bankpaymentvoucher" => await _bankPaymentVoucher.GetAsQueryable()
                        .Where(x => x.BankPaymentVoucherVoucherNo == voucherNo)
                        .Select(x => (int?)x.BankPaymentVoucherSno)
                        .FirstOrDefaultAsync(),

                    "receiptvoucher" => await _receiptVoucher.GetAsQueryable()
                        .Where(x => x.ReceiptVoucherMasterVoucherNo == voucherNo)
                        .Select(x => (int?)x.ReceiptVoucherMasterSno)
                        .FirstOrDefaultAsync(),

                    "bankreceiptvoucher" => await _bankReceiptVoucher.GetAsQueryable()
                        .Where(x => x.BankReceiptVoucherVoucherNo == voucherNo)
                        .Select(x => (int?)x.BankReceiptVoucherSno)
                        .FirstOrDefaultAsync(),

                    "salesvoucher" => await _salesVoucher.GetAsQueryable()
                        .Where(x => x.SalesVoucherNo == voucherNo)
                        .Select(x => (int?)x.SalesVoucherId)
                        .FirstOrDefaultAsync(),

                    "salesreturn" => await _salesReturnVoucher.GetAsQueryable()
                        .Where(x => x.SalesReturnNo == voucherNo)
                        .Select(x => (int?)x.SalesReturnId)
                        .FirstOrDefaultAsync(),

                    "purchasevoucher" => await _purchaseVoucher.GetAsQueryable()
                        .Where(x => x.PurchaseVoucherVoucherNo == voucherNo)
                        .Select(x => (int?)x.PurchaseVoucherPurID)
                        .FirstOrDefaultAsync(),

                    "purchasereturn" => await _purchaseReturnVoucher.GetAsQueryable()
                        .Where(x => x.PurchaseReturnNo == voucherNo)
                        .Select(x => (int?)x.PurchaseReturnId)
                        .FirstOrDefaultAsync(),

                    "journalvoucher" => await _journalVoucher.GetAsQueryable()
                        .Where(x => x.JournalVoucherVrefNo == voucherNo)
                        .Select(x => (int?)x.JournalVoucherId)
                        .FirstOrDefaultAsync(),

                    "contravoucher" => await _contraVoucher.GetAsQueryable()
                        .Where(x => x.ContraVoucherRefNo == voucherNo)
                        .Select(x => (int?)x.ContraVoucherId)
                        .FirstOrDefaultAsync(),

                    "creditnote" => await _creditNote.GetAsQueryable()
                        .Where(x => x.CreditNoteVno == voucherNo)
                        .Select(x => (int?)x.CreditNoteId)
                        .FirstOrDefaultAsync(),

                    "debitnote" => await _debitNote.GetAsQueryable()
                        .Where(x => x.DebitNoteVno == voucherNo)
                        .Select(x => (int?)x.DebitNoteId)
                        .FirstOrDefaultAsync(),

                    _ => null
                };

                if (id == null)
                    return NotFound($"No record found for type '{type}' and voucher no '{voucherNo}'.");

                return Ok(new { Id = id });
            }
            catch (Exception ex)
            {
                throw;
            }
        }





    }
}








