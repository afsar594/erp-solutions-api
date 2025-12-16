using Inspire.Erp.Application.Account.Implementations;
using Inspire.Erp.Application.Account.Interfaces;
using Inspire.Erp.Domain.DTO.ItemMaster;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Modals.AccountStatement;
using Inspire.Erp.Domain.Modals.Common;
using Inspire.Erp.Domain.Models;
using Inspire.Erp.Domain.Models.ViewModels;
using Inspire.Erp.Infrastructure.Database;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inspire.Erp.Web.Controllers.Accounts
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountStatementService _accountStatService;
        private readonly IVoucherPrintService _iVoucherPrintService;
        private IRepository<AccountSettings> _accountsSettingsRepo;
        private readonly IAccountStatementMultiAccountService _multiAccountService;
        private readonly IDaybookService _daybookService;
        private IRepository<AccountsTransactions> _accountTransactionRepository;
        private readonly IVoucherAllocationService _voucherAllocation;
        private readonly IOutstandingStatementService _outstandingStatement;
        private readonly IOldBalanceSheetService _oldBalanceSheet;
        private IRepository<CustomerMaster> _customerMasterRepository;
        private readonly IAgeingReport _ageingReport;
        private IRepository<SalesManMaster> salesmanrepository;
        private readonly IAssetsOpeningService _assetsOpeningService;
        private readonly IVatStatementService _vatStatementService;
        private IRepository<SalesVoucherDetails> _salesVoucherDetailsRepository;
        private readonly IAssetsMasterService _assetsMasterService;
        private IRepository<SalesVoucher> _salesVoucherRepository;
        private readonly IAccountTransactionsServices _accountTransactionsServices;
        private IRepository<ItemMaster> _itemrepository;
        public readonly InspireErpDBContext _Context;
        public AccountController(IAccountStatementService accountStatService, IDaybookService daybookService, IAssetsOpeningService assetsOpeningService, IRepository<AccountsTransactions> accountTransactionRepository,
            IVoucherAllocationService voucherAllocation, IOutstandingStatementService outstandingStatement, IOldBalanceSheetService oldBalanceSheet, IRepository<CustomerMaster> customerMasterRepository,
            IVoucherPrintService iVoucherPrintService, IAccountStatementMultiAccountService multiAccountService, IAgeingReport ageingReport, IRepository<SalesManMaster> _salesmanrepository,
            IVatStatementService vatStatementService, IAssetsMasterService assetsMasterService, IAccountTransactionsServices accountTransactionsServices, IRepository<AccountSettings> accountsSettingsRepo,
            IRepository<SalesVoucher> salesVoucherRepository, IRepository<SalesVoucherDetails> salesVoucherDetailsRepository, IRepository<ItemMaster> itemrepository, InspireErpDBContext Context)
        {
            _accountStatService = accountStatService;
            _iVoucherPrintService = iVoucherPrintService;
            _multiAccountService = multiAccountService; _accountsSettingsRepo = accountsSettingsRepo;
            _daybookService = daybookService; _accountTransactionRepository = accountTransactionRepository;
            _voucherAllocation = voucherAllocation;
            _outstandingStatement = outstandingStatement;
            _oldBalanceSheet = oldBalanceSheet; _customerMasterRepository = customerMasterRepository;
            _ageingReport = ageingReport; salesmanrepository = _salesmanrepository;
            _assetsOpeningService = assetsOpeningService;
            _vatStatementService = vatStatementService; _salesVoucherRepository = salesVoucherRepository;
            _salesVoucherDetailsRepository = salesVoucherDetailsRepository;
            _assetsMasterService = assetsMasterService; _itemrepository = itemrepository;
            _accountTransactionsServices = accountTransactionsServices;
            _Context = Context;
        }

        #region Account Statement 
        [HttpGet("GetAccountMasterDropdown")]
        public async Task<IActionResult> GetAccountMasterDropdown()
        {
            return Ok(await _accountStatService.GetAccountMasterDropdown());
        }
        [HttpGet("GetJobMasterDropdown")]
        public async Task<IActionResult> GetJobMasterDropdown()
        {
            return Ok(await _accountStatService.GetJobMasterDropdown());
        }
        [HttpPost("GetAccountTransactions")]
        public async Task<IActionResult> GetAccountTransactions(GenericGridViewModel model)
        {
            return Ok(await _accountStatService.GetAccountTransactions(model));
        }
        [HttpGet("AccountTransactionsHistory")]
        public void AccountTransactionsHistory(string accountNumber, string dateFrom, string dateTo, int location, int costcenterId, int jobId)
        {
            try
            {
                _Context.Database.ExecuteSqlRaw(
                    "EXEC sp_InsertAccountTransactions {0}, {1}, {2},{3},{4},{5}",
                    accountNumber, dateFrom, dateTo, location, costcenterId, jobId
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SQL Error: {ex.Message}");
                throw;
            }
        }

        [HttpPost("DownloadAccountStatement")]
        public async Task<IActionResult> DownloadAccountStatement(GenericGridViewModel model)
        {
            var result = await _accountStatService.AccountSTatementPrint(model);
            if (result.Valid)
            {
                return File(result.Result.stream, result.Result.ContentType, result.Result.Path);
            }
            return File(result.Result.stream, result.Result.ContentType, result.Result.Path);
        }

        #endregion

        #region Account Statement MultiAccount
        [HttpGet("GetAccountMaster")]
        public async Task<IActionResult> GetAccountMaster(string filter)
        {
            return Ok(await _multiAccountService.GetAccountMaster(filter));
        }
        [HttpGet("GetAccountMasterGroup")]
        public async Task<IActionResult> GetAccountMasterGroup()
        {
            return Ok(await _multiAccountService.GetAccountMasterGroup());
        }
        [HttpPost("GetAccountTransactionsMultiAccountSummary")]
        public async Task<IActionResult> GetAccountTransactionsMultiAccountSummary(GenericGridViewModel model)
        {
            return Ok(await _multiAccountService.GetAccountTransactionsMultiAccountSummary(model));
        }
        [HttpPost("GetAccountTransactionsMultiAccountDetail")]
        public async Task<IActionResult> GetAccountTransactionsMultiAccountDetail(GenericGridViewModel model)
        {
            return Ok(await _multiAccountService.GetAccountTransactionsMultiAccountDetail(model));
        }


        [HttpPost("GetAccountTransactionsForeignCurrencyDetail")]
        public async Task<IActionResult> GetAccountTransactionsForeignCurrencyDetail(GenericGridViewModel model)
        {
            return Ok(await _multiAccountService.GetAccountTransactionsForeignCurrencyDetail(model));
        }

        [HttpPost("GetAccountTransactionsForeignCurrencySummary")]
        public async Task<IActionResult> GetAccountTransactionsForeignCurrencySummary(GenericGridViewModel model)
        {
            return Ok(await _multiAccountService.GetAccountTransactionsForeignCurrencySummary(model));
        }

        [HttpPost("DownloadMultiAccountPrint")]
        public async Task<IActionResult> DownloadMultiAccountPrint(GenericGridViewModel model)
        {
            var result = await _multiAccountService.VoucherPrint(model);
            if (result.Valid)
            {
                return File(result.Result.stream, result.Result.ContentType, result.Result.Path);
            }
            return File(result.Result.stream, result.Result.ContentType, result.Result.Path);
        }
        #endregion

        #region Voucher Printing
        [HttpGet("GetVoucherTypeDropdown")]
        public async Task<IActionResult> GetVoucherTypeDropdown()
        {
            return Ok(await _iVoucherPrintService.GetVoucherTypeDropdown());
        }
        [HttpGet("GetVoucherNoDropdown")]
        public async Task<IActionResult> GetVoucherNoDropdown()
        {
            return Ok(await _iVoucherPrintService.GetVoucherNoDropdown());
        }
        [HttpPost("GetVoucherNoAutoComplete")]
        public async Task<IActionResult> GetVoucherNoAutoComplete(PagedSearchRequest request)
        {
            return Ok(await _iVoucherPrintService.GetVoucherNoAutoComplete(request));
        }
        [HttpPost("GetAccountTransactionsVoucherPrint")]
        public async Task<IActionResult> GetAccountTransactionsVoucherPrint(GenericGridViewModel model)
        {
            return Ok(await _iVoucherPrintService.GetAccountTransactions(model));
        }
        [HttpPost("DownloadVoucherPrint")]
        public async Task<IActionResult> DownloadVoucherPrint(GenericGridViewModel model)
        {
            var result = await _iVoucherPrintService.DownloadMultiAccountPrint(model);
            if (result.Valid)
            {
                return File(result.Result.stream, result.Result.ContentType, result.Result.Path);
            }
            return File(result.Result.stream, result.Result.ContentType, result.Result.Path);
        }
        #endregion

        #region Day book
        [HttpPost("GetDaybookAccountTransactions")]
        public async Task<IActionResult> GetDaybookAccountTransactions(GenericGridViewModel model)
        {
            return Ok(await _daybookService.GetDaybookAccountTransactions(model));
        }
        [HttpPost("DownloadDaybookAccount")]
        public async Task<IActionResult> DownloadDaybookAccount(GenericGridViewModel model)
        {
            var result = await _daybookService.DaybookPrint(model);
            if (result.Valid)
            {
                return File(result.Result.stream, result.Result.ContentType, result.Result.Path);
            }
            return File(result.Result.stream, result.Result.ContentType, result.Result.Path);
        }
        #endregion

        #region Voucher Allocation

        [HttpPost]
        [Route("GetAllocationDetailsByAccNoRefNo")]
        public async Task<IActionResult> GetAllocationDetailsByAccNoRefNo(string accNo, string refNO, string vType)
        {
            return Ok(await _accountTransactionsServices.GetReceiptVoucherAllocations(accNo, refNO, vType));
        }

        [HttpPost]
        [Route("GetAllocationDetailsByAccNoRefNoPayment")]
        public async Task<IActionResult> GetAllocationDetailsByAccNoRefNoPayment(string accNo, string refNO, string vType)
        {
            return Ok(await _accountTransactionsServices.GetPaymentVoucherAllocations(accNo, refNO, vType));
        }

        [HttpGet]
        [Route("getAllocationforPrint")]
        public async Task<ResponseInfo> getAllocationforPrint(int? Id)
        {
            //return await _accountTransactionsServices.getAllocationforPrint(accNo, date,vType);
            return await _accountTransactionsServices.getAllocationforPrint(Id);
        }

        [HttpGet("GetCustomerMasterDropdown")]
        public async Task<IActionResult> GetCustomerMasterDropdown()
        {
            return Ok(await _voucherAllocation.GetCustomerMasterDropdown());
        }
        [HttpGet("GetSupplierMasterDropdown")]
        public async Task<IActionResult> GetSupplierMasterDropdown()
        {
            return Ok(await _voucherAllocation.GetSupplierMasterDropdown());
        }
        [HttpPost("GetAccountTransactionsVoucherAllocation")]
        public async Task<IActionResult> GetAccountTransactionsVoucherAllocation(GenericGridViewModel model)
        {
            return Ok(await _voucherAllocation.GetAccountTransactions(model));
        }
        [HttpGet("GetSpecificVoucherAllocation")]
        public async Task<IActionResult> GetSpecificVoucherAllocation(string id)
        {
            return Ok(await _voucherAllocation.GetSpecificVoucherAllocation(id));
        }
        [HttpPost("AddEditVoucherAllocation")]
        public async Task<IActionResult> AddEditVoucherAllocation(AllocationVoucher model)
        {
            try
            {
                return Ok(await _voucherAllocation.AddEditVoucherAllocation(model));
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, ex.Message.ToString());
            }

        }

        [HttpPost("release-allocations")]
        public async Task<IActionResult> ReleaseAllocations([FromBody] AllocationVoucher model)
        {
            return Ok(await _voucherAllocation.ReleaseVoucherAllocation(model));
        }

        [HttpPost("GetVoucherAllocationsList")]
        public async Task<IActionResult> GetVoucherAllocationsList(GenericGridViewModel model)
        {
            return Ok(await _voucherAllocation.GetVoucherAllocationsList(model));
        }
        [HttpGet("GetVoucherAllocation")]
        public IActionResult GetVoucherAllocation()
        {
            try
            {
                return Ok(_voucherAllocation.GetVoucherAllocation());
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        [HttpDelete("DeleteAllocationVoucher")]
        public IActionResult DeleteAllocationVoucher(string id, string type)
        {
            try
            {
                return Ok(_voucherAllocation.DeleteAllocationVoucher(id, type));
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Outstanding Statement

        [HttpPost("GetOutstandingStatementAccountTransactions")]
        public async Task<IActionResult> GetOutstandingStatementAccountTransactions(GenericGridViewModel model)
        {
            return Ok(await _outstandingStatement.GetOutstandingStatementAccountTransactions(model));
        }
        [HttpPost("GetOutstandingReportGrid")]
        public async Task<IActionResult> GetOutstandingStatementAccountTransactions(OutStandingReprortFilter model)
        {
            return Ok(await _outstandingStatement.GetOutstandingStatementAccountTransactions(model));
        }
        [HttpPost("OutstandingStatementPrint")]
        public async Task<IActionResult> OutstandingStatementPrint(GenericGridViewModel model)
        {
            var result = await _outstandingStatement.OutstandingStatementPrint(model);
            if (result.Valid)
            {
                return File(result.Result.stream, result.Result.ContentType, result.Result.Path);
            }
            return File(result.Result.stream, result.Result.ContentType, result.Result.Path);
        }

        [HttpGet("GetCustomerSupplierMasterGroup")]
        public async Task<IActionResult> GetCustomerSupplierMasterGroup()
        {
            return Ok(await _outstandingStatement.GetCustomerSupplierMasterGroup());
        }
        #endregion

        #region Old Balance Sheet
        [HttpPost("AddEditRecordsInBalanceSheet")]
        public async Task<IActionResult> AddEditRecordsInBalanceSheet(AddOldBalanceSheetResponse model)
        {
            return Ok(await _oldBalanceSheet.AddEditRecordsInBalanceSheet(model));
        }
        [HttpPost("GetBalanceSheetList")]
        public async Task<IActionResult> GetBalanceSheetList(GenericGridViewModel model)
        {
            return Ok(await _oldBalanceSheet.GetBalanceSheetList(model));
        }
        [HttpPost("DownloadOldBalanceSheetAccount")]
        public async Task<IActionResult> DownloadOldBalanceSheetAccount(GenericGridViewModel model)
        {
            var result = await _oldBalanceSheet.OldBalanceSheetPrint(model);
            if (result.Valid)
            {
                return File(result.Result.stream, result.Result.ContentType, result.Result.Path);
            }
            return File(result.Result.stream, result.Result.ContentType, result.Result.Path);
        }
        #endregion

        #region Ageing Report

        [HttpPost("GetAgeingReport")]
        public async Task<IActionResult> GetAgeingReport(GenericGridViewModel model)
        {
            return Ok(await _ageingReport.GetAgeingReport(model));
        }



        [HttpPost("AgeingReportPrint")]
        public async Task<IActionResult> AgeingReportPrint(GenericGridViewModel model)
        {
            var result = await _outstandingStatement.OutstandingStatementPrint(model);
            if (result.Valid)
            {
                return File(result.Result.stream, result.Result.ContentType, result.Result.Path);
            }
            return File(result.Result.stream, result.Result.ContentType, result.Result.Path);
        }

        #endregion

        #region Assets Opening
        [HttpGet("GetCurrencyMaster")]
        public async Task<IActionResult> GetCurrencyMaster()
        {
            return Ok(await _assetsOpeningService.GetCurrencyMaster());
        }
        [HttpGet("GetAssetMaster")]
        public async Task<IActionResult> GetAssetMaster()
        {
            return Ok(await _assetsOpeningService.GetAssetMaster());
        }
        [HttpGet("GetAssetOpening")]
        public async Task<IActionResult> GetAssetOpening(int id = 0)
        {
            return Ok(await _assetsOpeningService.GetAssetOpening(id));
        }
        [HttpPost("AddEditAssetOpening")]
        public async Task<IActionResult> AddEditAssetOpening(AddEditAssetPurchaseVoucher model)
        {
            return Ok(await _assetsOpeningService.AddEditAssetOpening(model));
        }
        [HttpPost("GetAssetOpeningList")]
        public async Task<IActionResult> GetAssetOpeningList(GenericGridViewModel model)
        {
            return Ok(await _assetsOpeningService.GetAssetOpeningList(model));
        }
        #endregion

        #region Vat Statement

        [HttpPost("GetVatStatementDetail")]
        public async Task<IActionResult> GetVatStatementDetail(GenericGridViewModel model)
        {
            return Ok(await _vatStatementService.GetVatStatementDetail(model));
        }
        [HttpPost("GetVatStatementSummary")]
        public async Task<IActionResult> GetVatStatementSummary(GenericGridViewModel model)
        {
            return Ok(await _vatStatementService.GetVatStatementSummary(model));
        }

        [HttpGet("GetDistinctVoucherType")]
        public async Task<IActionResult> GetDistinctVoucherType()
        {
            return Ok(await _vatStatementService.GetDistinctVoucherType());
        }



        #endregion

        #region ASSET MASTER 
        [HttpPost("GetFixedAccountForAssetMaster")]
        public async Task<IActionResult> GetFixedAccountForAssetMaster(GenericGridViewModel model)
        {
            return Ok(await _assetsMasterService.GetFixedAccountForAssetMaster(model));
        }
        [HttpGet("GetRelativeAccountAssetMaster")]
        public async Task<IActionResult> GetRelativeAccountAssetMaster(string accNo)
        {
            return Ok(await _assetsMasterService.GetRelativeAccountAssetMaster(accNo));
        }
        [HttpGet("GetRelativeAssetMasterRecord")]
        public async Task<IActionResult> GetRelativeAssetMasterRecord(int id)
        {
            return Ok(await _assetsMasterService.GetRelativeAssetMasterRecord(id));
        }

        [HttpGet("GetAssetsMaster")]
        public async Task<IActionResult> GetAssetMaster(int id)
        {
            return Ok(await _assetsMasterService.GetAssetMaster(id));
        }

        [HttpPost("AddEditAssetMaster")]
        public async Task<IActionResult> AddEditAssetMaster(AssetMasterViewModel model)
        {
            return Ok(await _assetsMasterService.AddEditAssetMaster(model));
        }

        [HttpPost("SaveNewAssetGroup")]
        public async Task<IActionResult> SaveNewAssetGroup(AssetGroupViewModel model)
        {
            return Ok(await _assetsMasterService.SaveNewAssetGroup(model));
        }

        [HttpPost("EditAssetGroup")]
        public async Task<IActionResult> EditAssetGroup(AssetGroupEditViewModel model)
        {
            return Ok(await _assetsMasterService.EditAssetGroup(model));
        }

        [HttpPost("EditAssetHead")]
        public async Task<IActionResult> EditAssetHead(AssetGroupEditViewModel model)
        {
            return Ok(await _assetsMasterService.EditAssetHead(model));
        }

        [HttpPost("SaveNewAssetHead")]
        public async Task<IActionResult> SaveNewAssetHead(AssetHeaderViewModel model)
        {
            return Ok(await _assetsMasterService.SaveNewAssetHead(model));
        }

        //[HttpGet("GetAllAssets")]
        //public async Task<IActionResult> GetAllAssets()
        //{
        //    return Ok(_assetsMasterService.GetAllAssets());
        //}


        #endregion

        #region invoice Profit Analysis

        [HttpGet]
        [Route("ProfitAnalysisLoadDropdown")]
        public ResponseInfo ProfitAnalysisLoadDropdown()
        {
            var objresponse = new ResponseInfo();


            var saleManList = salesmanrepository.GetAsQueryable().AsNoTracking().Where(a => a.SalesManMasterSalesManDelStatus != true).Select(c => new
            {
                c.SalesManMasterSalesManId,
                c.SalesManMasterSalesManName,
            }).OrderBy(o => o.SalesManMasterSalesManName).AsEnumerable().ToList();

            var customerMasters = _customerMasterRepository.GetAsQueryable().AsNoTracking().Where(k => k.CustomerMasterCustomerDelStatus != true).Select(a => new
            {
                a.CustomerMasterCustomerNo,
                a.CustomerMasterCustomerName,
            }).OrderBy(o => o.CustomerMasterCustomerName).AsEnumerable().ToList();
            objresponse.ResultSet = new
            {
                customerMasters = customerMasters,
                saleManList = saleManList
            };

            return objresponse;
        }

        [HttpPost]
        [Route("InvoiceProfitAnalysis")]
        public ResponseInfo InvoiceProfitAnalysis(InvoiceProfitAnalysis model)
        {
            var objresponse = new ResponseInfo();

            var rawData = (from sv in _salesVoucherRepository.GetAsQueryable().AsNoTracking()
                           join svd in _salesVoucherDetailsRepository.GetAsQueryable().AsNoTracking()
                               on sv.SalesVoucherId equals svd.SalesVoucherId
                           join cust in _customerMasterRepository.GetAsQueryable().AsNoTracking()
                               on sv.SalesVoucherPartyId equals cust.CustomerMasterCustomerNo
                           where sv.SalesVoucherDelStatus != true
                                 && (model.salemanId == 0 || sv.SalesVoucherSalesManID == model.salemanId)
                                 && (model.customerId == 0 || sv.SalesVoucherPartyId == model.customerId)
                                 && (!model.isDate || (sv.SalesVoucherDate.Value.Date >= model.fromdate.Value.Date
                                                       && sv.SalesVoucherDate.Value.Date <= model.todate.Value.Date))
                           select new
                           {
                               sv.SalesVoucherId,
                               sv.SalesVoucherNo,
                               sv.SalesVoucherDate,
                               sv.SalesVoucherPartyId,
                               CustomerName = cust.CustomerMasterCustomerName,
                               SalesAmount = svd.SalesVoucherDetailsNetAmt,
                               CostPrice = svd.SalesVoucherDetailsCostPrice
                           }).AsNoTracking().ToList();

            var result = rawData
                .GroupBy(x => new
                {
                    x.SalesVoucherId,
                    x.SalesVoucherNo,
                    x.SalesVoucherDate,
                    x.SalesVoucherPartyId,
                    x.CustomerName
                })
                .Select(g =>
                {
                    var salesAmount = g.Sum(x => x.SalesAmount);
                    var costPrice = g.Sum(x => x.CostPrice);
                    var profit = salesAmount - costPrice;
                    var profitPercent = salesAmount == 0 ? 0 : (profit * 100) / salesAmount;

                    return new
                    {
                        InvoiceId = g.Key.SalesVoucherId,
                        InvoiceNo = g.Key.SalesVoucherNo,
                        Date = g.Key.SalesVoucherDate,
                        CustomerId = g.Key.SalesVoucherPartyId,
                        CustomerName = g.Key.CustomerName,
                        SalesAmount = salesAmount,
                        CostPrice = costPrice,
                        Profit = profit,
                        ProfitPercent = Math.Round((double)profitPercent, 2)
                    };
                })
                .OrderBy(x => x.InvoiceNo)
                .ToList();

            objresponse.ResultSet = new { result };
            return objresponse;
        }

        #endregion

        #region Assets Consumtions

        [HttpGet("GetAssetsCodes")]
        public async Task<IActionResult> GetAssetCodes()
        {
            return Ok(await _assetsMasterService.GetAssetsCodes());
        }
        #endregion



        [HttpPost]
        [Route("ASSummReport")]
        public async Task<IActionResult> ASSummReport([FromBody] AccStReport voucherCompositeView)
        {
            try
            {
                return Ok(await _accountStatService.AccountStatementSummaryReport(voucherCompositeView));
            }
            catch (Exception ex)
            {
                // Log the full exception details here
                return StatusCode(500, $"An error occurred while retrieving AliAnsariReport: {ex.Message}");
            }
        }


        [HttpPost]
        [Route("ASDetailReport")]
        public async Task<IActionResult> ASDetailReport([FromBody] AccStReport voucherCompositeView)
        {
            try
            {
                return Ok(await _accountStatService.AccountStatementDetailReport(voucherCompositeView));
            }
            catch (Exception ex)
            {
                // Log the full exception details here
                return StatusCode(500, $"An error occurred while retrieving AliAnsariReport: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("SaveTempStatement")]
        public async Task<IActionResult> SaveTempStatement([FromBody] List<TempStatementModel> tempStatements)
        {
            try
            {
                foreach (var tempStatement in tempStatements)
                {
                    await _accountStatService.SaveTempStatement(tempStatement);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving AliAnsariReport: {ex.Message}");
            }
        }

    }
}
