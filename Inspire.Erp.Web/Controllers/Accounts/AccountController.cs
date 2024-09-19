using Inspire.Erp.Application.Account.Interfaces;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Modals.AccountStatement;
using Inspire.Erp.Domain.Modals.Common;
using Inspire.Erp.Domain.Models;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inspire.Erp.Web.Controllers.Accounts
{
    ////[Route("api/[controller]")]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountStatementService _accountStatService;
        private readonly IVoucherPrintService _iVoucherPrintService; private IRepository<AccountSettings> _accountsSettingsRepo;
        private readonly IAccountStatementMultiAccountService _multiAccountService;
        private readonly IDaybookService _daybookService; private IRepository<AccountsTransactions> _accountTransactionRepository;
        private readonly IVoucherAllocationService _voucherAllocation;
        private readonly IOutstandingStatementService _outstandingStatement;
        private readonly IOldBalanceSheetService _oldBalanceSheet; private IRepository<CustomerMaster> _customerMasterRepository;
        private readonly IAgeingReport _ageingReport; private IRepository<SalesManMaster> salesmanrepository;
        private readonly IAssetsOpeningService _assetsOpeningService;
        private readonly IVatStatementService _vatStatementService; private IRepository<SalesVoucherDetails> _salesVoucherDetailsRepository;
        private readonly IAssetsMasterService _assetsMasterService; private IRepository<SalesVoucher> _salesVoucherRepository;
        private readonly IAccountTransactionsServices _accountTransactionsServices;
        public AccountController(IAccountStatementService accountStatService, IDaybookService daybookService, IAssetsOpeningService assetsOpeningService, IRepository<AccountsTransactions> accountTransactionRepository,
            IVoucherAllocationService voucherAllocation, IOutstandingStatementService outstandingStatement, IOldBalanceSheetService oldBalanceSheet, IRepository<CustomerMaster> customerMasterRepository,
            IVoucherPrintService iVoucherPrintService, IAccountStatementMultiAccountService multiAccountService, IAgeingReport ageingReport, IRepository<SalesManMaster> _salesmanrepository,
            IVatStatementService vatStatementService, IAssetsMasterService assetsMasterService, IAccountTransactionsServices accountTransactionsServices, IRepository<AccountSettings> accountsSettingsRepo,
            IRepository<SalesVoucher> salesVoucherRepository, IRepository<SalesVoucherDetails> salesVoucherDetailsRepository)
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
            _assetsMasterService = assetsMasterService;
            _accountTransactionsServices = accountTransactionsServices;
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
        //[HttpPost("GetAccountTransactionsForeignCurrencySummary")]
        //public async Task<IActionResult> GetAccountTransactionsForeignCurrencySummary([FromBody] AccountTransactionsFilterReport model)
        //{
        //    return Ok(await _multiAccountService.GetAccountTransactionsForeignCurrencySummary(model));
        //}

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
            }).ToList();
            var customerMasters = _customerMasterRepository.GetAsQueryable().AsNoTracking().Where(k => k.CustomerMasterCustomerDelStatus != true).Select(a => new
            {
                a.CustomerMasterCustomerNo,
                a.CustomerMasterCustomerName,
            }).ToList();
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

            var query = _salesVoucherRepository.GetAsQueryable().AsNoTracking()
                            .Where(a => a.SalesVoucherDelStatus != true &&
                                        (model.salemanId == 0 || a.SalesVoucherSalesManID == model.salemanId) &&
                                        (model.customerId == 0 || a.SalesVoucherPartyId == model.customerId));

            if (model.isDate)
            {
                query = query.Where(c => c.SalesVoucherDate >= model.fromdate && c.SalesVoucherDate <= model.todate);
            }

            var result = query.ToList();
            var accountsSettings = _accountsSettingsRepo.GetAsQueryable().AsNoTracking().Where(a => a.AccountSettingsAccountDelStatus != true && (a.AccountSettingsAccountDescription.Contains("Cost Of Finished Goods A/C") || a.AccountSettingsAccountDescription.Contains("Sales Account"))).Select(c => new
            {
                c.AccountSettingsAccountId,
                AccountSettingsAccountDescription = c.AccountSettingsAccountDescription.Trim(),
                c.AccountSettingsAccountTextValue
            }).ToList();
            List<string> ids = new List<string>();
            string sale = "";
            string cost = "";
            if (accountsSettings.Count > 0)
            {
                var salesAccountTextValue = accountsSettings.Where(a => a.AccountSettingsAccountDescription.Trim() == "Sales Account").Select(a => a.AccountSettingsAccountTextValue)
                 .FirstOrDefault();

                var costAccountTextValue = accountsSettings.Where(a => a.AccountSettingsAccountDescription.Trim() == "Cost Of Finished Goods A/C").Select(a => a.AccountSettingsAccountTextValue)
                 .FirstOrDefault();


                if (!string.IsNullOrEmpty(salesAccountTextValue))
                {
                    var parts = salesAccountTextValue.Split(new[] { "::" }, StringSplitOptions.None);

                    if (parts.Length > 1)
                    {
                        sale = parts[1].Trim();
                        ids.Add(parts[1]);
                    }
                }
                if (!string.IsNullOrEmpty(costAccountTextValue))
                {
                    var parts = costAccountTextValue.Split(new[] { "::" }, StringSplitOptions.None);

                    if (parts.Length > 1)
                    {
                        cost = parts[1].Trim();
                        ids.Add(parts[1]);
                    }
                }
            }
            var acc = _accountTransactionRepository.GetAsQueryable().AsNoTracking()
                .Where(a => a.AccountstransactionsDelStatus != true
                            && a.AccountsTransactionsVoucherType == "SalesVoucher")
                .ToList();

            var account = acc.Where(a => ids.Any(id =>
                            string.Equals(a.AccountsTransactionsAccNo.Trim(), id.Trim(), StringComparison.OrdinalIgnoreCase)))
                            .ToList();
            var data = from q in result
                       join s in _salesVoucherDetailsRepository.GetAsQueryable().AsNoTracking()
                           on q.SalesVoucherId equals s.SalesVoucherId
                       join m in account
                           on q.SalesVoucherNo equals m.AccountsTransactionsVoucherNo
                       group new { s, m } by new
                       {
                           q.SalesVoucherNo,
                           q.SalesVoucherDate,
                           s.SalesVoucherDetailsQuantity,
                           s.SalesVoucherDetailsMatId
                       } into grouped
                       let invoiceAmount = grouped.Sum(g => g.m.AccountsTransactionsCredit)
                       let cost1 = grouped.Sum(g => g.m.AccountsTransactionsDebit)
                       let profit = invoiceAmount - cost1
                       let profitPercentage = cost1 == 0 ? 0 : (invoiceAmount / cost1) * 100
                       select new
                       {
                           InvoiceNo = grouped.Key.SalesVoucherNo,
                           Date = grouped.Key.SalesVoucherDate,
                           Quantity = grouped.Key.SalesVoucherDetailsQuantity,
                           Item = grouped.Key.SalesVoucherDetailsMatId,
                           InvoiceAmount = invoiceAmount,
                           Cost = cost1,
                           Profit = profit,
                           ProfitPercentage = Math.Round(profitPercentage, 2)
        };

        objresponse.ResultSet = new
            {
                result = data.ToList()
    };
            return objresponse;
        }

        #endregion
    }
}
