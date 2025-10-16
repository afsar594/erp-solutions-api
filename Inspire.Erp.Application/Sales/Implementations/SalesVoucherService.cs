using Dapper;
using Inspire.Erp.Application.Account.Implementations;
using Inspire.Erp.Application.Common;
using Inspire.Erp.Application.Helpers;
using Inspire.Erp.Application.Master;
using Inspire.Erp.Application.MODULE;
using Inspire.Erp.Application.Sales.Interfaces;
using Inspire.Erp.Application.Store.Implementation;
using Inspire.Erp.Application.Store.Interfaces;
using Inspire.Erp.Domain.DTO;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Entities.POS;
using Inspire.Erp.Domain.Enums;
using Inspire.Erp.Domain.Modals;
using Inspire.Erp.Domain.Modals.Common;
using Inspire.Erp.Domain.Modals.Sales;
using Inspire.Erp.Domain.Models;
using Inspire.Erp.Infrastructure;
using Inspire.Erp.Infrastructure.Database;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid.Helpers.Mail;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.IO.Pipelines;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Inspire.Erp.Application.Sales.Implementations
{
    public class SalesVoucherService : ISalesVoucherService
    {
        private IRepository<StockRegister> _stockRegisterRepository;
        private IRepository<CurrencyMaster> _currencyMasterRepository;
        private IRepository<AccountsTransactions> _accountTransactionRepository;
        private IRepository<SalesVoucher> _salesVoucherRepository;
        private IRepository<VanSales> _vanSalesRepository;
        private IRepository<VanSalesDetails> _vanSalesDetailsRepository;

        private IRepository<SalesVoucherDetails> _salesVoucherDetailsRepository;
        private IRepository<Domain.Entities.ProgramSettings> _programsettingsRepository;
        private IRepository<VouchersNumbers> _voucherNumbersRepository;
        private readonly ILogger<PaymentVoucherService> _logger;
        private IRepository<AccountSettings> _accountsSettingssvcs;
        private IRepository<ItemMaster> _itemMasterRepository;
        private IRepository<UnitMaster> _unitMasterRepository;
        private IRepository<SuppliersMaster> _suppliersMasterRepository;
        private IRepository<LocationMaster> _locationMasterRepository;
        private IRepository<CustomerMaster> _customerMasterRepository;
        private IRepository<SalesManMaster> _salesManMasterRepository;
        private IRepository<DepartmentMaster> _departmentMasterRepository;
        private IRepository<UnitDetails> _UnitDetailsRepository;
        private IRepository<SalesReturn> _salesReturn;
        private IRepository<SalesReturnDetails> _salesReturnDtls;
        private IRepository<OptionsMaster> _optionMaster;
        private InspireErpDBContext _context;
        private IRepository<MasterAccountsTable> accMaster;
        private IRepository<SalesHold> _salesHoldRepository;
        private IRepository<SalesHoldDetails> _salesHoldDetailsRepository;
        private IRepository<POS_WorkPeriod> _POSWorkPeriodRepository;
        private IRepository<UserTracking> _UserTracking;
        private IStockService _openingStockService;
        private IRepository<SalesCharges> _salesChargesRepository;
        private IRepository<OtherChargesMaster> _otherChargesMasterRepository;


        private readonly IDbConnection _dbConnection;

        public SalesVoucherService(
            IRepository<UserTracking> UserTracking,
            IRepository<CustomerMaster> customerMasterRepository,
            IRepository<SalesManMaster> salesManMasterRepository,
            IRepository<DepartmentMaster> departmentMasterRepository,
            IRepository<ItemMaster> itemMasterRepository, IRepository<UnitMaster> unitMasterRepository,
            IRepository<SuppliersMaster> suppliersMasterRepository, IRepository<LocationMaster> locationMasterRepository,
            IRepository<AccountsTransactions> accountTransactionRepository, IRepository<StockRegister> stockRegisterRepository, IRepository<Domain.Entities.ProgramSettings> programsettingsRepository,
             IRepository<VouchersNumbers> voucherNumbers, ILogger<PaymentVoucherService> logger, IRepository<AccountSettings> accountsSettingssvcsRepository,
            IRepository<SalesVoucher> salesVoucherRepository, IRepository<VanSales> VanSalesRepository, IRepository<VanSalesDetails> VanSalesDetailsRepository, IRepository<SalesVoucherDetails> salesVoucherDetailsRepository, IRepository<CurrencyMaster> currencyMasterRepository,
            IRepository<UnitDetails> unitDetailsRepository, IRepository<MasterAccountsTable> accMaster, IRepository<SalesReturn> salesReturn, IRepository<SalesReturnDetails> salesReturnDtls, IRepository<OptionsMaster> optionMaster
            , IRepository<SalesHold> salesHoldRepository, IRepository<SalesHoldDetails> salesHoldDetailsRepository, IRepository<POS_WorkPeriod> POSWorkPeriodRepository, InspireErpDBContext context
            , IStockService openingStockService, IOptions<ApplicationSettings> applicationSettings,
            IRepository<SalesCharges> salesChargesRepository,IRepository<OtherChargesMaster> otherChargesMasterRepository)
        {
            this._accountTransactionRepository = accountTransactionRepository;
            this._stockRegisterRepository = stockRegisterRepository;
            this._salesVoucherRepository = salesVoucherRepository;
            this._vanSalesRepository = VanSalesRepository;
            this._vanSalesDetailsRepository = VanSalesDetailsRepository;


            this._salesVoucherDetailsRepository = salesVoucherDetailsRepository;
            _programsettingsRepository = programsettingsRepository;
            _voucherNumbersRepository = voucherNumbers;
            _itemMasterRepository = itemMasterRepository;
            _unitMasterRepository = unitMasterRepository;
            _suppliersMasterRepository = suppliersMasterRepository;
            _locationMasterRepository = locationMasterRepository;
            _customerMasterRepository = customerMasterRepository;
            _salesManMasterRepository = salesManMasterRepository;
            _departmentMasterRepository = departmentMasterRepository;
            _currencyMasterRepository = currencyMasterRepository;
            _UnitDetailsRepository = unitDetailsRepository;
            this.accMaster = accMaster; _accountsSettingssvcs = accountsSettingssvcsRepository;
            _salesReturn = salesReturn;
            _salesReturnDtls = salesReturnDtls;
            _optionMaster = optionMaster;
            _salesHoldRepository = salesHoldRepository;
            _salesHoldDetailsRepository = salesHoldDetailsRepository;
            _POSWorkPeriodRepository = POSWorkPeriodRepository;
            _UserTracking = UserTracking;
            _context = context;
            _openingStockService = openingStockService;
            _dbConnection = new SqlConnection(applicationSettings.Value.ConnectionString);
            _salesChargesRepository = salesChargesRepository;
            _otherChargesMasterRepository = otherChargesMasterRepository;
        }
        public IEnumerable<LocationMaster> SalesVoucher_GetAllLocationMaster()
        {
            return _locationMasterRepository.GetAll();
        }
        public IEnumerable<SuppliersMaster> SalesVoucher_GetAllSuppliersMaster()
        {
            return _suppliersMasterRepository.GetAll();
        }
        public IEnumerable<DepartmentMaster> SalesVoucher_GetAllDepartmentMaster()
        {
            return _departmentMasterRepository.GetAll();
        }
        public IEnumerable<CustomerMaster> SalesVoucher_GetAllCustomerMaster()
        {
            return _customerMasterRepository.GetAll();
        }
        public IEnumerable<SalesManMaster> SalesVoucher_GetAllSalesManMaster()
        {
            return _salesManMasterRepository.GetAll();
        }
        public IEnumerable<UnitMaster> SalesVoucher_GetAllUnitMaster()
        {
            return _unitMasterRepository.GetAll();
        }
        public IEnumerable<ItemMaster> SalesVoucher_GetAllItemMaster()
        {
            return _itemMasterRepository.GetAll();
        }
        public async Task<SalesVoucherModel> UpdateSalesVoucher(SalesVoucher salesVoucher, List<AccountsTransactions> accountsTransactions,
            List<SalesVoucherDetails> salesVoucherDetails
           , List<StockRegister> stockRegister
           , List<SalesCharges> salesChargesList
            )
        {
            try
            {
                _salesVoucherRepository.BeginTransaction();

                //=================================
                clsCommonFunctions.Delete_OldEntryOf_StockRegister(salesVoucher.SalesVoucherNo, VoucherType.SalesVoucher_TYPE, _stockRegisterRepository);
                clsCommonFunctions.Delete_OldEntry_AccountsTransactions(salesVoucher.SalesVoucherNo, VoucherType.SalesVoucher_TYPE, _accountTransactionRepository);
                //=================================
              
                var deteletDetails = _salesVoucherDetailsRepository.GetAsQueryable().Where(i => i.SalesVoucherDetailsNo == salesVoucher.SalesVoucherNo).ToList();
                _salesVoucherDetailsRepository.DeleteList(deteletDetails);
                //var itemdetails = salesVoucherDetails.Where(x => x.SalesVoucherDetailsId == 0).ToList();
                //_salesVoucherDetailsRepository.InsertList(itemdetails);
                if (salesChargesList != null && salesChargesList.Any())
                {
                    await _salesChargesRepository.UpdateListAsync(salesChargesList);
                    decimal totalServiceCharges = salesChargesList.Sum(x => x.TaxIncAmount);
                    salesVoucher.ServiceCharges = totalServiceCharges;
                }
                _salesVoucherRepository.Update(salesVoucher);


                var currencyRate = clsCommonFunctions.getConverionCurrencyRate(salesVoucher.SalesVoucherCurrencyId, _currencyMasterRepository);
                var rate = (decimal)currencyRate;

                var stockReg = new List<StockRegister>();
                foreach (var item in salesVoucher.SalesVoucherDetails)
                {
                    var avgrate = item.SalesVoucherDetailsCostPrice ?? 1;

                    var converontype = this.getConverionTypebyUnitId(item.SalesVoucherDetailsMatId, item.SalesVoucherDetailsUnitId);


                    var expense = 0;
                    if (salesVoucher.SalesVoucherExpense > 0)
                    {
                        //expense calculation
                        var exp = item.SalesVoucherDetailsGrossAmount / salesVoucher.SalesVoucherTotalGrossAmount;

                        var expen = exp * salesVoucher.SalesVoucherExpense;
                        expense = (int)((expen + item.SalesVoucherDetailsGrossAmount) / item.SalesVoucherDetailsQuantity);

                    }

                    stockReg.Add(new StockRegister()
                    {
                        StockRegisterPurchaseID = salesVoucher.SalesVoucherNo,
                        StockRegisterRefVoucherNo = salesVoucher.SalesVoucherNo,
                        StockRegisterVoucherDate = DateTime.Now.Date,
                        StockRegisterMaterialID = item.SalesVoucherDetailsMatId,
                        StockRegisterQuantity = item.SalesVoucherDetailsQuantity * (decimal)(converontype) ?? 0,
                        StockRegisterSIN = 0,
                        StockRegisterSout = item.SalesVoucherDetailsQuantity * (decimal)(converontype) ?? 0,
                        StockRegisterRate = avgrate,
                        StockRegisterAmount = item.SalesVoucherDetailsQuantity * (decimal)(converontype) * avgrate ?? 0,
                        StockRegisterFCAmount = item.SalesVoucherDetailsQuantity * (decimal)(converontype) * avgrate * rate,
                        StockRegisterAssignedDate = DateTime.Now.Date,
                        StockRegisterStatus = AccountStatus.Approved,
                        StockRegisterTransType = VoucherType.SalesVoucher_TYPE,
                        StockRegisterUnitID = item.SalesVoucherDetailsUnitId,
                        StockRegisterLocationID = salesVoucher.SalesVoucherLocationID,
                        StockRegisterJobID = salesVoucher.SalesVoucherJobId,
                        StockRegisterDepID = salesVoucher.SalesVoucherDptID,
                        StockRegisterDelStatus = false,
                        StockRegisterLandingCost = expense
                    });
                }
                _stockRegisterRepository.UpdateList(stockReg);
                accountsTransactions = accountsTransactions.Select((k) =>
                {
                    k.AccountsTransactionsVoucherNo = salesVoucher.SalesVoucherNo;
                    k.AccountsTransactionsAccNo = k.AccountsTransactionsAccNo ?? string.Empty;
                    k.AccountsTransactionsVoucherType = VoucherType.SalesVoucher_TYPE;
                    k.AccountsTransactionsStatus = AccountStatus.Approved;
                    return k;
                }).OrderBy(x => x.AccountsTransactionsVoucherNo).AsEnumerable().ToList();
                _accountTransactionRepository.InsertList(accountsTransactions);

                UserTracking trackingData = new UserTracking();
                trackingData.UserTrackingUserUserId = 1;
                trackingData.UserTrackingUserVpAction = "Update";
                trackingData.UserTrackingUserVpNo = salesVoucher.SalesVoucherNo;
                trackingData.UserTrackingUserChangeDt = DateTime.Now;
                trackingData.UserTrackingUserChangeTime = DateTime.Now;
                trackingData.UserTrackingUserVpType = VoucherType.SalesVoucher_TYPE;
                _UserTracking.Insert(trackingData);

                _salesVoucherRepository.TransactionCommit();
            }
            catch (Exception ex)
            {
                _salesVoucherRepository.TransactionRollback();
                throw ex;
            }

            return this.GetSavedSalesVoucherDetails((decimal)salesVoucher.SalesVoucherId);
        }
        public int DeleteSalesVoucher(SalesVoucher salesVoucher, List<AccountsTransactions> accountsTransactions,
            List<SalesVoucherDetails> salesVoucherDetails
            , List<StockRegister> stockRegister
            )
        {
            int i = 0;
            try
            {
                _salesVoucherRepository.BeginTransaction();

                //=================================
                clsCommonFunctions.Delete_OldEntry_StockRegister(salesVoucher.SalesVoucherNo, VoucherType.SalesVoucher_TYPE, _stockRegisterRepository);
                clsCommonFunctions.Delete_OldEntry_AccountsTransactions(salesVoucher.SalesVoucherNo, VoucherType.SalesVoucher_TYPE, _accountTransactionRepository);
                //=================================


                salesVoucher.SalesVoucherDelStatus = true;

                salesVoucherDetails = salesVoucherDetails.Select((k) =>
                {
                    k.SalesVoucherDetailsDelStatus = true;
                    return k;
                }).OrderBy(x => x.SalesVoucherDetailsItemName).AsEnumerable().ToList();

                //_salesVoucherDetailsRepository.UpdateList(salesVoucherDetails);

                stockRegister = stockRegister.Select((k) =>
                {
                    k.StockRegisterDelStatus = true;
                    return k;
                }).OrderBy(x => x.StockRegisterRefVoucherNo).AsEnumerable().ToList();
                _stockRegisterRepository.UpdateList(stockRegister);
                salesVoucher.SalesVoucherDetails = salesVoucherDetails;
                _salesVoucherRepository.Update(salesVoucher);
                var vchrnumer = _voucherNumbersRepository.GetAsQueryable().Where(k => k.VouchersNumbersVNo == salesVoucher.SalesVoucherNo).FirstOrDefault();
                _voucherNumbersRepository.Update(vchrnumer);

                UserTracking trackingData = new UserTracking();
                trackingData.UserTrackingUserUserId = 1;
                trackingData.UserTrackingUserVpAction = "Delete";
                trackingData.UserTrackingUserVpNo = salesVoucher.SalesVoucherNo;
                trackingData.UserTrackingUserChangeDt = DateTime.Now;
                trackingData.UserTrackingUserChangeTime = DateTime.Now;
                trackingData.UserTrackingUserVpType = VoucherType.SalesVoucher_TYPE;
                _UserTracking.Insert(trackingData);

                _salesVoucherRepository.TransactionCommit();
                i = 1;
            }
            catch (Exception ex)
            {
                _salesVoucherRepository.TransactionRollback();
                i = 0;
                throw ex;
            }
            return i;
        }
        //private decimal getConverionTypebyUnitId(int? itemid, int? unitDetailsid)
        //{
        //    try
        //    {
        //        return (decimal)_UnitDetailsRepository.GetAsQueryable().FirstOrDefault(x => x.UnitDetailsItemId == itemid && x.UnitDetailsUnitId == unitDetailsid).UnitDetailsConversionType;
        //    }
        //    catch
        //    {
        //        return 1;
        //    }
        //}
        private decimal getConverionTypebyUnitId(int? itemid, int? unitDetailsid)
        {
            try
            {
                if (itemid.HasValue && itemid.Value != 0 && unitDetailsid.HasValue && unitDetailsid.Value != 0)
                {
                    return (decimal)_UnitDetailsRepository.GetAsQueryable().FirstOrDefault(x => x.UnitDetailsItemId == itemid && x.UnitDetailsUnitId == unitDetailsid).UnitDetailsConversionType;
                }
                else
                {
                    return 1;
                }
            }
            catch
            {
                return 1;
            }
        }

        public async Task<SalesVoucherModel> InsertSalesVoucher(SalesVoucher salesVoucher, List<AccountsTransactions> accountsTransactions,
            List<SalesVoucherDetails> salesVoucherDetails
            , List<StockRegister> stockRegister
            , List<SalesCharges> salesChargesList)
        {
            try
            {
                _salesVoucherRepository.BeginTransaction();

                string salesVoucherNumber = clsCommonFunctions.GenrateVoucherNo(VoucherType.SalesVoucher_TYPE, Prefix.SalesVoucher_Prefix, _programsettingsRepository, _voucherNumbersRepository);
                salesVoucher.SalesVoucherNo = salesVoucherNumber;

                decimal? maxcount = 0;
                maxcount =
                    _salesVoucherRepository.GetAsQueryable()
                    .DefaultIfEmpty().Max(o => o == null ? 0 : o.SalesVoucherId) + 1;

                salesVoucher.SalesVoucherId = maxcount;
                salesVoucher.SalesVoucherDetails.Clear();
                salesVoucherDetails = salesVoucherDetails.Select((x) =>
                {
                    x.SalesVoucherId = maxcount;
                    x.SalesVoucherDetailsNo = salesVoucherNumber;
                    salesVoucher.SalesVoucherDetails.Add(x);
                    return x;
                }).OrderBy(x => x.SalesVoucherDetailsItemName).AsEnumerable().ToList();

                accountsTransactions = accountsTransactions.Select((k) =>
                {
                    k.AccountsTransactionsVoucherNo = salesVoucherNumber;
                    k.AccountsTransactionsAccNo = k.AccountsTransactionsAccNo ?? string.Empty;
                    k.AccountsTransactionsVoucherType = VoucherType.SalesVoucher_TYPE;
                    k.AccountsTransactionsStatus = AccountStatus.Approved;
                    return k;
                }).OrderBy(x => x.AccountsTransactionsVoucherNo).AsEnumerable().ToList();
                _accountTransactionRepository.InsertList(accountsTransactions);

                var currencyRate = clsCommonFunctions.getConverionCurrencyRate(salesVoucher.SalesVoucherCurrencyId, _currencyMasterRepository);
                var rate = (decimal)currencyRate;

                var stockReg = new List<StockRegister>();
                foreach (var item in salesVoucher.SalesVoucherDetails)
                {
                    var avgrate = item.SalesVoucherDetailsCostPrice ?? 1;
                    var converontype = this.getConverionTypebyUnitId(item.SalesVoucherDetailsMatId, item.SalesVoucherDetailsUnitId);

                    var expense = 0;
                    if (salesVoucher.SalesVoucherExpense > 0)
                    {
                        //expense calculation
                        var exp = item.SalesVoucherDetailsGrossAmount / salesVoucher.SalesVoucherTotalGrossAmount;

                        var expen = exp * salesVoucher.SalesVoucherExpense;
                        expense = (int)((expen + item.SalesVoucherDetailsGrossAmount) / item.SalesVoucherDetailsQuantity);
                    }
                    stockReg.Add(new StockRegister()
                    {
                        StockRegisterPurchaseID = salesVoucher.SalesVoucherNo,
                        StockRegisterRefVoucherNo = salesVoucher.SalesVoucherNo,
                        StockRegisterVoucherDate = DateTime.Now.Date,
                        StockRegisterMaterialID = item.SalesVoucherDetailsMatId ?? 0,
                        StockRegisterQuantity = item.SalesVoucherDetailsQuantity * (decimal)(converontype) ?? 0,
                        StockRegisterSIN = 0,
                        StockRegisterSout = item.SalesVoucherDetailsQuantity * (decimal)(converontype) ?? 1,
                        StockRegisterRate = avgrate,
                        StockRegisterAmount = item.SalesVoucherDetailsQuantity * (decimal)(converontype) * avgrate ?? 1,
                        StockRegisterFCAmount = item.SalesVoucherDetailsQuantity * (decimal)(converontype) * avgrate * rate ?? 1,
                        StockRegisterAssignedDate = DateTime.Now.Date,
                        StockRegisterStatus = "A",
                        StockRegisterTransType = VoucherType.SalesVoucher_TYPE,
                        StockRegisterUnitID = item.SalesVoucherDetailsUnitId ?? 0,
                        StockRegisterLocationID = salesVoucher.SalesVoucherLocationID ?? 0,
                        StockRegisterJobID = salesVoucher.SalesVoucherJobId ?? 0,
                        StockRegisterDepID = salesVoucher.SalesVoucherDptID ?? 0,
                        StockRegisterDelStatus = false,
                        StockRegisterLandingCost = expense
                    });
                }
                _stockRegisterRepository.InsertList(stockReg);
                if (salesChargesList != null && salesChargesList.Any())
                {
                    await _salesChargesRepository.UpdateListAsync(salesChargesList);
                    decimal totalServiceCharges = salesChargesList.Sum(x => x.TaxIncAmount);
                    salesVoucher.ServiceCharges = totalServiceCharges;
                }

                _salesVoucherRepository.Insert(salesVoucher);

                UserTracking trackingData = new UserTracking();
                trackingData.UserTrackingUserUserId = 1;
                trackingData.UserTrackingUserVpAction = "Insert";
                trackingData.UserTrackingUserVpNo = salesVoucher.SalesVoucherNo;
                trackingData.UserTrackingUserChangeDt = DateTime.Now;
                trackingData.UserTrackingUserChangeTime = DateTime.Now;
                trackingData.UserTrackingUserVpType = VoucherType.SalesVoucher_TYPE;
                _UserTracking.Insert(trackingData);

                _salesVoucherRepository.TransactionCommit();

                return this.GetSavedSalesVoucherDetails((decimal)salesVoucher.SalesVoucherId);

            }
            catch (Exception ex)
            {
                _salesVoucherRepository.TransactionRollback();
                throw ex;
            }
        }
        public async Task<VanSales> InsertVanSalesAsync(VanSales model)
        {
            var vanSales = new VanSales
            {
                Mode = model.Mode,
                Product = model.Product,
                Barcode = model.Barcode,
                MRP = model.MRP,
                CutOffRate = model.CutOffRate,
                DiscountPercent = model.DiscountPercent,
                UnitExclV = model.UnitExclV,
                FOC = model.FOC,
                DiscountAmount = model.DiscountAmount,
                UnitInclV = model.UnitInclV,
                Quantity = model.Quantity,
                VatPercent = model.VatPercent,
                Total = model.Total,
                SummaryDiscountPercent = model.SummaryDiscountPercent,
                VatExcl = model.VatExcl,
                TenderedAmount = model.TenderedAmount,
                SummaryDiscountAmount = model.SummaryDiscountAmount,
                Vat = model.Vat,
                CardReceived = model.CardReceived,
                Balance = model.Balance,
                SummaryTotal = model.SummaryTotal,
                AddedToCredit = model.AddedToCredit,
                Remark = model.Remark,
                PrintInvoice = model.PrintInvoice,
                CreatedDate = model.CreatedDate,
                CustomerName = model.CustomerName,
                CustomerId = model.CustomerId,
                CurrencyId = model.CurrencyId,
                LocationId = model.LocationId,
            };

            _vanSalesRepository.Insert(vanSales);
            await _context.SaveChangesAsync();

            if (model.Details?.Count > 0)
            {
                var detailToAdd = model.Details.Select(x => new VanSalesDetails
                {
                    SalesId = vanSales.SalesId,
                    SI_No = x.SI_No,
                    Item_Name = x.Item_Name,
                    Item_Id = x.Item_Id,
                    Unit_Id = x.Unit_Id,
                    Qty = x.Qty,
                    Rate = x.Rate,
                    Amount = x.Amount,
                    DiscountAmount = x.DiscountAmount,
                    GrossAmt = x.GrossAmt,
                    CreatedDate = x.CreatedDate
                }).ToList();

                _vanSalesDetailsRepository.InsertList(detailToAdd);
                await _context.SaveChangesAsync();
            }

            try
            {
                var salesVoucher = new SalesVoucher
                {
                    VanSalesId = vanSales.SalesId,
                    SalesVoucherCurrencyId = vanSales.CurrencyId ?? 0,
                    SalesVoucherLocationID = vanSales.LocationId ?? 0,
                    // SalesVoucherTotalGrossAmount = vanSales.Total ?? 0,
                    SalesVoucherExpense = 0,
                    SalesVoucherJobId = 0,
                    SalesVoucherDptID = 0,
                    SalesVoucherPartyId = vanSales.CustomerId ?? 0,
                    SalesVoucherPartyName = vanSales.CustomerName ?? string.Empty,
                    SalesVoucherType = vanSales.Mode ?? string.Empty,
                    SalesVoucherDate = DateTime.Now,
                    SalesVoucherTotalGrossAmount = vanSales.TenderedAmount,
                    SalesVoucherTotalDisPer = vanSales.SummaryDiscountPercent,
                    SalesVoucherTotalDisAmount = vanSales.SummaryDiscountAmount,
                    SalesVoucherNetAmount = vanSales.SummaryTotal,
                    SalesVoucherVat_Per = vanSales.Vat,

                    SalesVoucherDetails = new List<SalesVoucherDetails>()
                };

                var salesVoucherDetails = model.Details?.Select(d => new SalesVoucherDetails
                {
                    SalesVoucherDetailsItemName = d.Item_Name,
                    SalesVoucherDetailsQuantity = d.Qty,
                    SalesVoucherDetailsRate = d.Rate,
                    SalesVoucherDetailsGrossAmount = d.GrossAmt,
                    SalesVoucherDetailsCostPrice = d.Rate,
                    SalesVoucherDetailsMatId = d.Item_Id,
                    SalesVoucherDetailsUnitId = d.Unit_Id,
                    SalesVoucherDetailsNetAmt = d.Amount,
                    SalesVoucherDetailsDiscAmount = d.DiscountAmount
                }).ToList() ?? new List<SalesVoucherDetails>();
                foreach (var detail in salesVoucherDetails)
                {
                    salesVoucher.SalesVoucherDetails.Add(detail);
                }
                var accountsTransactions = new List<AccountsTransactions>
        {
            new AccountsTransactions
            {
                               AccountsTransactionsAccNo = "01001014",
                    AccountsTransactionsAllocBalance = 0,
                    AccountsTransactionsCredit = 0,
                    AccountsTransactionsCurrencyId = 5,
                    AccountsTransactionsDescription = string.Empty,
                    AccountsTransactionsFcAllocBalance = 0,
                    AccountsTransactionsFcCredit = 0,
                    AccountsTransactionsParticulars = string.Empty,
                    AccountsTransactionsStatus = AccountStatus.Approved,
                    AccountsTransactionsTransDate = DateTime.Now,
                    AccountsTransactionsTransSno = 0,
                    AccountsTransactionsTstamp = DateTime.Now,
                    AccountsTransactionsUserId = 1,
                    AccountsTransactionsVoucherNo = salesVoucher.SalesVoucherNo,
                    AccountsTransactionsVoucherType = VoucherType.SalesVoucher_TYPE,
            }
        };

                var stockRegister = salesVoucherDetails.Select(d => new StockRegister
                {
                    StockRegisterMaterialID = d.SalesVoucherDetailsMatId,
                    StockRegisterQuantity = d.SalesVoucherDetailsQuantity ?? 0,
                    StockRegisterRate = d.SalesVoucherDetailsRate ?? 0,
                    StockRegisterAmount = (d.SalesVoucherDetailsQuantity ?? 0) * (d.SalesVoucherDetailsRate ?? 0),
                    StockRegisterVoucherDate = DateTime.Now
                }).ToList();
                var saleschargesList = new List<SalesCharges>();    
                await InsertSalesVoucher(salesVoucher, accountsTransactions, salesVoucherDetails, stockRegister, saleschargesList);
            }
            catch (Exception ex)
            {
                throw new Exception("Van Sales saved but failed to save Sales Voucher: " + ex.Message, ex);
            }

            return vanSales;
        }



        public async Task<Response<(List<decimal> PurchasePrices, List<decimal> SalesPrices)>> GetLast3PurchaseAndSalesPricesAsync(int itemId, int? locationId)
        {
            try
            {
                // Get last 3 purchase prices
                var PurchaseVoucherIds = await _context.PurchaseVoucher
                    .Where(x => x.PurchaseVoucherStatus == "P")
                    .Select(o => o.PurchaseVoucherPurID)
                    .ToListAsync();

                var last3PurchasePrices = await (from pvd in _context.PurchaseVoucherDetails
                                                 join pv in _context.PurchaseVoucher on (long)pvd.PurchaseVoucherDetailsPrId equals pv.PurchaseVoucherPurID
                                                 where pvd.PurchaseVoucherDetailsMaterialId == itemId
                                                 && PurchaseVoucherIds.Contains((long)pvd.PurchaseVoucherDetailsPrId)
                                                 orderby pv.PurchaseVoucherPurchaseDate descending
                                                 select (decimal)(pvd.PurchaseVoucherDetailsRate ?? 0))
                                                 .Take(3)
                                                 .ToListAsync();

                // Get last 3 sales prices
                var SalesVoucherIds = await _context.SalesVoucher
                    .Where(x => x.SalesVoucherStatus == "S")
                    .Select(o => o.SalesVoucherId)
                    .ToListAsync();

                var last3SalesPrices = await (from svd in _context.SalesVoucherDetails
                                              join sv in _context.SalesVoucher on (long)svd.SalesVoucherDetailsId equals sv.SalesVoucherId
                                              where svd.SalesVoucherDetailsMatId == itemId
                                              && SalesVoucherIds.Contains((long)svd.SalesVoucherDetailsId)
                                              orderby sv.SalesVoucherDate descending
                                              select (decimal)(svd.SalesVoucherDetailsRate ?? 0))
                                              .Take(3)
                                              .ToListAsync();

                return Response<(List<decimal> PurchasePrices, List<decimal> SalesPrices)>.Success((last3PurchasePrices, last3SalesPrices), "Data found");
            }
            catch (Exception ex)
            {
                return Response<(List<decimal> PurchasePrices, List<decimal> SalesPrices)>.Fail((new List<decimal>(), new List<decimal>()), ex.Message);
            }
        }

        public async Task<Response<decimal>> GetAveragePurchasePrice(int itemId, int? locationId)
        {
            try
            {
                var totalQuantity = (from sr in _context.StockRegister
                                     where (locationId == null || sr.StockRegisterLocationID == locationId)
                                     && sr.StockRegisterMaterialID == itemId
                                     group sr by 1 into og
                                     select new
                                     {
                                         Total = og.Sum(item => (int)(item.StockRegisterSIN ?? 0 - item.StockRegisterSout ?? 0)),

                                     }).FirstOrDefault().Total;

                var rateQuantiyFromStock = await (from sr in _context.StockRegister
                                                  where (locationId == null || sr.StockRegisterLocationID == locationId)
                                                  && sr.StockRegisterMaterialID == itemId

                                                  select new PurVoucherRateQuantityModel
                                                  {
                                                      Quantity = sr.StockRegisterQuantity,
                                                      Rate = sr.StockRegisterRate,
                                                      PurchaseDate = sr.StockRegisterVoucherDate
                                                  }).OrderByDescending(y => y.PurchaseDate).ToListAsync();

                var PurchaseVoucherIds = await _context.PurchaseVoucher.Where(x => x.PurchaseVoucherStatus == "P").Select(o => o.PurchaseVoucherPurID).ToListAsync();
                var purchseVouchers = await (from pvd in _context.PurchaseVoucherDetails
                                             join pv in _context.PurchaseVoucher on (long)pvd.PurchaseVoucherDetailsPrId equals pv.PurchaseVoucherPurID
                                             where pvd.PurchaseVoucherDetailsMaterialId == itemId
                                            && PurchaseVoucherIds.Contains((long)pvd.PurchaseVoucherDetailsPrId)
                                             select new PurVoucherRateQuantityModel
                                             {
                                                 Rate = (long)(pvd.PurchaseVoucherDetailsRate ?? 0),
                                                 Quantity = pvd.PurchaseVoucherDetailsQuantity ?? 0,
                                                 PurchaseDate = pv.PurchaseVoucherPurchaseDate

                                             }).OrderByDescending(y => y.PurchaseDate).ToListAsync();

                decimal actualQuantity = 0;
                decimal actualRate = 0;
                decimal quantity = 0;
                decimal rate = 0;
                actualQuantity = totalQuantity;

                if (rateQuantiyFromStock.Count() > 0)
                {
                    foreach (var pvd in rateQuantiyFromStock)
                    {
                        rate = pvd.Rate ?? 0;
                        quantity = pvd.Quantity ?? 0;
                        if (actualQuantity > 0)
                        {
                            if (pvd.Quantity >= actualQuantity)
                            {
                                quantity = actualQuantity;
                                actualRate = actualRate + (rate * quantity);
                                actualQuantity = actualQuantity - quantity;
                            }
                        }
                    }
                }
                else if (purchseVouchers.Count() > 0)
                {
                    foreach (var pvd in purchseVouchers)
                    {
                        rate = pvd.Rate ?? 0;
                        quantity = pvd.Quantity ?? 0;
                        if (actualQuantity > 0)
                        {
                            if (pvd.Quantity >= actualQuantity)
                            {
                                quantity = actualQuantity;
                                actualRate = actualRate + (rate * quantity);
                                actualQuantity = actualQuantity - quantity;
                            }
                        }
                    }
                }
                else
                {
                    //get from item master
                    var im = _context.ItemMaster.Where(k => k.ItemMasterItemId == itemId).FirstOrDefault();
                    actualRate = im.ItemMasterLastPurchasePrice ?? 0;
                }

                decimal returnAvgPrice = 0;
                if (totalQuantity <= 0)
                {
                    totalQuantity = 1;
                }
                returnAvgPrice = actualRate / totalQuantity;
                return Response<decimal>.Success(returnAvgPrice, "Data found");
            }
            catch (Exception ex)
            {
                return Response<decimal>.Fail(0, ex.Message);
            }
        }
        private void createAccountTransactionEntries(SalesVoucher salesVoucher, List<AccountsTransactions> accountsTransactions)
        {
            decimal currencyRate = getCurrencyRate(salesVoucher.SalesVoucherCurrencyId);


            accountsTransactions = accountsTransactions.Select((k) =>
            {
                k.AccountsTransactionsAccNo = getAccountNoByPartyName(salesVoucher.SalesVoucherPartyName); // need to modify for its original acc no, this is just to test the field
                k.AccountsTransactionsVoucherNo = salesVoucher.SalesVoucherNo;
                k.AccountsTransactionsVoucherType = VoucherType.SalesVoucher_TYPE;
                k.AccountsTransactionsStatus = AccountStatus.Approved;
                k.AccountsTransactionsTransDate = DateTime.Now.Date;
                k.AccountsTransactionsParticulars = $"{VoucherType.SalesVoucher_TYPE} {salesVoucher.CurrencyMasterCurrencyName}"; //need to replace salesVoucher.CurrencyMasterCurrencyName by customer name
                k.AccountsTransactionsDebit = (decimal)salesVoucher.SalesVoucherNetAmount * currencyRate; //need replace by its original value
                k.AccountsTransactionsCredit = 0;
                k.AccountsTransactionsDescription = string.Empty;//need to add description here if needed
                k.AccountsTransactionsUserId = salesVoucher.SalesVoucherUserID ?? k.AccountsTransactionsUserId;
                k.AccountsTransactionsStatus = "A"; //you can change it if needed
                k.AccountsTransactionsTstamp = DateTime.Now; // can be changable if needed for date and time
                k.RefNo = string.Empty; // you can add here the Ref no if needed
                k.AccountsTransactionsFsno = salesVoucher.SalesVoucherFSNO ?? 0m;
                k.AccountsTransactionsFcAllocDebit = 0;
                k.AccountsTransactionsFcAllocCredit = 0;
                k.AccountsTransactionsAllocCredit = 0;
                k.AccountsTransactionsAllocDebit = 0;
                k.AccountsTransactionsAllocBalance = (decimal)salesVoucher.SalesVoucherNetAmount * currencyRate;//need to replace by its original value
                k.AccountsTransactionsFcAllocBalance = (decimal)salesVoucher.SalesVoucherNetAmount * currencyRate;//need to replace by its original value
                k.AccountsTransactionsFcCredit = 0;
                k.AccountsTransactionsFcDebit = (decimal)salesVoucher.SalesVoucherNetAmount * currencyRate;//need to replace by its original value
                k.AccountsTransactionsLocation = salesVoucher.SalesVoucherLocationID;
                k.AccountsTransactionsJobNo = salesVoucher.SalesVoucherJobId;
                k.AccountsTransactionsCurrencyId = salesVoucher.SalesVoucherCurrencyId;
                k.AccountsTransactionsFcRate = currencyRate;
                k.AccountsTransactionsLpoNo = string.Empty; // change it to desired option if needed
                k.AccountsTransactionsDeptId = -1; // change it to desired option if needed
                return k;
            }).OrderBy(x => x.AccountsTransactionsVoucherNo).AsEnumerable().ToList();
            _accountTransactionRepository.InsertList(accountsTransactions);
        }
        private string getAccountNoByPartyName(string partyName)
        {
            try
            {

                string maAcc = this.accMaster.GetAsQueryable().Where(x => x.MaAccName.ToUpper().Equals(partyName.ToUpper())).Select(x => x.MaAccNo).FirstOrDefault();

                return maAcc == null ? "" : maAcc;
            }
            catch
            {
                return string.Empty;
            }
        }
        private decimal getCurrencyRate(int? salesVoucherCurrencyId)
        {
            try
            {
                return (decimal)this._currencyMasterRepository.GetAsQueryable().FirstOrDefault(x => x.CurrencyMasterCurrencyId == salesVoucherCurrencyId).CurrencyMasterCurrencyRate;
            }
            catch
            {
                return 1;
            }
        }
        public IEnumerable<AccountsTransactions> GetAllTransaction()
        {
            return _accountTransactionRepository.GetAll();
        }
        public IEnumerable<SalesVoucher> GetSalesVoucher()
        {
            var salV = (from s in _salesVoucherRepository.GetAsQueryable()
                        join c in _customerMasterRepository.GetAsQueryable() on s.SalesVoucherPartyId equals c.CustomerMasterCustomerNo
                        join cr in _currencyMasterRepository.GetAsQueryable() on s.SalesVoucherCurrencyId equals cr.CurrencyMasterCurrencyId
                        where s.SalesVoucherDelStatus == null || s.SalesVoucherDelStatus == false
                        select new SalesVoucher()
                        {
                            SalesVoucherId = s.SalesVoucherId,
                            SalesVoucherNo = s.SalesVoucherNo,
                            SalesVoucherDate = s.SalesVoucherDate,
                            SalesVoucherType = s.SalesVoucherType,
                            SalesVoucherPartyId = c.CustomerMasterCustomerNo,
                            SalesVoucherPartyName = c.CustomerMasterCustomerName,
                            SalesVoucherPartyAddress = c.CustomerMasterCustomerLocation,
                            SalesVoucherPartyVatNo = c.CustomerMasterCustomerVatNo,
                            SalesVoucherRefNo = s.SalesVoucherRefNo,
                            SalesVoucherContPerson = s.SalesVoucherContPerson,
                            SalesVoucherDlvNo = s.SalesVoucherDlvNo,
                            SalesVoucherDlvDate = s.SalesVoucherDlvDate,
                            SalesVoucherSONo = s.SalesVoucherSONo,
                            SalesVoucherSODate = s.SalesVoucherSODate,
                            SalesVoucherQtnNo = s.SalesVoucherQtnNo,
                            SalesVoucherQtnDate = s.SalesVoucherQtnDate,
                            SalesVoucherSalesManID = s.SalesVoucherSalesManID,
                            SalesVoucherDptID = s.SalesVoucherDptID,
                            SalesVoucherEnqNo = s.SalesVoucherEnqNo,
                            SalesVoucherEnqDate = s.SalesVoucherEnqDate,
                            SalesVoucherDescription = s.SalesVoucherDescription,
                            SalesVoucherExcludeVAT = s.SalesVoucherExcludeVAT,
                            SalesVoucherLocationID = s.SalesVoucherLocationID,
                            SalesVoucherUserID = s.SalesVoucherUserID,
                            SalesVoucherCurrencyId = cr.CurrencyMasterCurrencyId,
                            CurrencyMasterCurrencyName = cr.CurrencyMasterCurrencyName,
                            SalesVoucherCompanyId = s.SalesVoucherCompanyId,
                            SalesVoucherJobId = s.SalesVoucherJobId,
                            SalesVoucherFSNO = s.SalesVoucherFSNO,
                            SalesVoucherFc_Rate = s.SalesVoucherFc_Rate,
                            SalesVoucherStatus = s.SalesVoucherStatus,
                            SalesVoucherTotalGrossAmount = s.SalesVoucherTotalGrossAmount,
                            SalesVoucherTotalItemDisAmount = s.SalesVoucherTotalItemDisAmount,
                            SalesVoucherTotalActualAmount = s.SalesVoucherTotalActualAmount,
                            SalesVoucherTotalDisPer = s.SalesVoucherTotalDisPer,
                            SalesVoucherTotalDisAmount = s.SalesVoucherTotalDisAmount,
                            SalesVoucherVat_AMT = s.SalesVoucherVat_AMT,
                            SalesVoucherVat_Per = s.SalesVoucherVat_Per,
                            SalesVoucherVat_RoundSign = s.SalesVoucherVat_RoundSign,
                            SalesVoucherVat_RountAmt = s.SalesVoucherVat_RountAmt,
                            SalesVoucherNetDisAmount = s.SalesVoucherNetDisAmount,
                            SalesVoucherNetAmount = s.SalesVoucherNetAmount,
                            SalesVoucherHeader = s.SalesVoucherHeader,
                            SalesVoucherDelivery = s.SalesVoucherDelivery,
                            SalesVoucherNotes = s.SalesVoucherNotes,
                            SalesVoucherFooter = s.SalesVoucherFooter,
                            SalesVoucherPaymentTerms = s.SalesVoucherPaymentTerms,
                            SalesVoucherSubject = s.SalesVoucherSubject,
                            SalesVoucherContent = s.SalesVoucherContent,
                            SalesVoucherRemarks1 = s.SalesVoucherRemarks1,
                            SalesVoucherRemarks2 = s.SalesVoucherRemarks2,
                            SalesVoucherTermsAndCondition = s.SalesVoucherTermsAndCondition,
                            SalesVoucherShippinAddress = s.SalesVoucherShippinAddress,
                            SalesVoucherDelStatus = s.SalesVoucherDelStatus,
                            SalesVoucherShortNo = s.SalesVoucherShortNo,
                        }).OrderBy(x => x.SalesVoucherNo).AsEnumerable().ToList();

            return salV;
        }


        public async Task<Response<GridWrapperResponse<List<SalesVoucher>>>> GetSalesVoucherList(GenericGridViewModel model)
        {
            try
            {
                var baseQuery = (from s in _salesVoucherRepository.GetAsQueryable()
                                 join c in _customerMasterRepository.GetAsQueryable() on s.SalesVoucherPartyId equals c.CustomerMasterCustomerNo
                                 join cr in _currencyMasterRepository.GetAsQueryable() on s.SalesVoucherCurrencyId equals cr.CurrencyMasterCurrencyId
                                 where s.SalesVoucherDelStatus == null || s.SalesVoucherDelStatus == false
                                 select new SalesVoucher()
                                 {
                                     SalesVoucherId = s.SalesVoucherId,
                                     SalesVoucherNo = s.SalesVoucherNo,
                                     SalesVoucherDate = s.SalesVoucherDate,
                                     SalesVoucherType = s.SalesVoucherType,
                                     SalesVoucherPartyId = c.CustomerMasterCustomerNo,
                                     SalesVoucherPartyName = c.CustomerMasterCustomerName,
                                     SalesVoucherPartyAddress = c.CustomerMasterCustomerLocation,
                                     SalesVoucherPartyVatNo = c.CustomerMasterCustomerVatNo,
                                     SalesVoucherRefNo = s.SalesVoucherRefNo,
                                     SalesVoucherContPerson = s.SalesVoucherContPerson,
                                     SalesVoucherDlvNo = s.SalesVoucherDlvNo,
                                     SalesVoucherDlvDate = s.SalesVoucherDlvDate,
                                     SalesVoucherSONo = s.SalesVoucherSONo,
                                     SalesVoucherSODate = s.SalesVoucherSODate,
                                     SalesVoucherQtnNo = s.SalesVoucherQtnNo,
                                     SalesVoucherQtnDate = s.SalesVoucherQtnDate,
                                     SalesVoucherSalesManID = s.SalesVoucherSalesManID,
                                     SalesVoucherDptID = s.SalesVoucherDptID,
                                     SalesVoucherEnqNo = s.SalesVoucherEnqNo,
                                     SalesVoucherEnqDate = s.SalesVoucherEnqDate,
                                     SalesVoucherDescription = s.SalesVoucherDescription,
                                     SalesVoucherExcludeVAT = s.SalesVoucherExcludeVAT,
                                     SalesVoucherLocationID = s.SalesVoucherLocationID,
                                     SalesVoucherUserID = s.SalesVoucherUserID,
                                     SalesVoucherCurrencyId = cr.CurrencyMasterCurrencyId,
                                     CurrencyMasterCurrencyName = cr.CurrencyMasterCurrencyName,
                                     SalesVoucherCompanyId = s.SalesVoucherCompanyId,
                                     SalesVoucherJobId = s.SalesVoucherJobId,
                                     SalesVoucherFSNO = s.SalesVoucherFSNO,
                                     SalesVoucherFc_Rate = s.SalesVoucherFc_Rate,
                                     SalesVoucherStatus = s.SalesVoucherStatus,
                                     SalesVoucherTotalGrossAmount = s.SalesVoucherTotalGrossAmount,
                                     SalesVoucherTotalItemDisAmount = s.SalesVoucherTotalItemDisAmount,
                                     SalesVoucherTotalActualAmount = s.SalesVoucherTotalActualAmount,
                                     SalesVoucherTotalDisPer = s.SalesVoucherTotalDisPer,
                                     SalesVoucherTotalDisAmount = s.SalesVoucherTotalDisAmount,
                                     SalesVoucherVat_AMT = s.SalesVoucherVat_AMT,
                                     SalesVoucherVat_Per = s.SalesVoucherVat_Per,
                                     SalesVoucherVat_RoundSign = s.SalesVoucherVat_RoundSign,
                                     SalesVoucherVat_RountAmt = s.SalesVoucherVat_RountAmt,
                                     SalesVoucherNetDisAmount = s.SalesVoucherNetDisAmount,
                                     SalesVoucherNetAmount = s.SalesVoucherNetAmount,
                                     SalesVoucherHeader = s.SalesVoucherHeader,
                                     SalesVoucherDelivery = s.SalesVoucherDelivery,
                                     SalesVoucherNotes = s.SalesVoucherNotes,
                                     SalesVoucherFooter = s.SalesVoucherFooter,
                                     SalesVoucherPaymentTerms = s.SalesVoucherPaymentTerms,
                                     SalesVoucherSubject = s.SalesVoucherSubject,
                                     SalesVoucherContent = s.SalesVoucherContent,
                                     SalesVoucherRemarks1 = s.SalesVoucherRemarks1,
                                     SalesVoucherRemarks2 = s.SalesVoucherRemarks2,
                                     SalesVoucherTermsAndCondition = s.SalesVoucherTermsAndCondition,
                                     SalesVoucherShippinAddress = s.SalesVoucherShippinAddress,
                                     SalesVoucherDelStatus = s.SalesVoucherDelStatus,
                                     SalesVoucherShortNo = s.SalesVoucherShortNo,
                                 });

                // Filtering
                if (!string.IsNullOrEmpty(model.Filter))
                    baseQuery = baseQuery.Where(model.Filter);

                // Search
                if (!string.IsNullOrEmpty(model.Search))
                {
                    baseQuery = baseQuery.Where(x =>
                        x.SalesVoucherNo.Contains(model.Search) ||
                        x.SalesVoucherPartyName.Contains(model.Search) ||
                        x.SalesVoucherRefNo.Contains(model.Search) ||
                        x.SalesVoucherType.Contains(model.Search));
                }

                // Date range
                if (model.FromDate.HasValue)
                    baseQuery = baseQuery.Where(x => x.SalesVoucherDate >= model.FromDate.Value);

                if (model.ToDate.HasValue)
                    baseQuery = baseQuery.Where(x => x.SalesVoucherDate <= model.ToDate.Value);

                // Count AFTER filters
                var totalCount = await baseQuery.CountAsync();

                // Sorting
                IOrderedQueryable<SalesVoucher> orderedQuery;
                if (!string.IsNullOrEmpty(model.Field))
                {
                    // Map column name from UI to entity property
                    string sortField = model.Field.ToLower() switch
                    {
                        "salesvoucherid" => "SalesVoucherId",
                        "salesvoucherno" => "SalesVoucherNo",
                        "salesvouchertype" => "SalesVoucherType",
                        "salesvoucherpartyid" => "SalesVoucherPartyId",
                        "salesvoucherpartyname" => "SalesVoucherPartyName",
                        "salesvoucherdate" => "SalesVoucherDate",
                        _ => "SalesVoucherNo"
                    };

                    string sortDir = model.Dir?.ToLower() == "desc" ? "descending" : "ascending";
                    orderedQuery = baseQuery.OrderBy($"{sortField} {sortDir}");
                }
                else
                {
                    orderedQuery = baseQuery.OrderBy(x => x.SalesVoucherNo);
                }


                // Pagination
                var result = await orderedQuery
                    .Skip(model.Skip)
                    .Take(model.Take)
                    .ToListAsync();
                var gridResponse = new GridWrapperResponse<List<SalesVoucher>>();
                gridResponse.Data = result;
                gridResponse.Total = totalCount;

                return Response<GridWrapperResponse<List<SalesVoucher>>>.Success(gridResponse, "Data retrieved successfully");

                //return Response<List<SalesVoucher>>.Success(result, $"Data retrieved successfully. Total records: {totalCount}");
            }
            catch (Exception ex)
            {
                var result = new List<SalesVoucher>();
                var gridResponse = new GridWrapperResponse<List<SalesVoucher>>();
                gridResponse.Data = result;
                gridResponse.Total = 0;
                return Response<GridWrapperResponse<List<SalesVoucher>>>.Fail(gridResponse, ex.Message);
            }
        }
        public SalesVoucherModel GetSavedSalesVoucherDetails(decimal pvno)
        {
            SalesVoucherModel salesVoucherModel = new SalesVoucherModel();
            //salesVoucherModel.salesVoucher = _salesVoucherRepository.GetAsQueryable().Where(k => k.SalesVoucherNo == pvno && k.SalesVoucherDelStatus == false).SingleOrDefault();
            salesVoucherModel.salesVoucher = _salesVoucherRepository.GetAsQueryable().Where(k => k.SalesVoucherId == pvno).SingleOrDefault();

            if (salesVoucherModel.salesVoucher != null)
            {
                salesVoucherModel.salesVoucherDetails = _salesVoucherDetailsRepository.GetAsQueryable().Where(x => x.SalesVoucherDetailsNo == salesVoucherModel.salesVoucher.SalesVoucherNo && (x.SalesVoucherDetailsDelStatus == false || x.SalesVoucherDetailsDelStatus == null)).OrderBy(x => x.SalesVoucherDetailsItemName).AsEnumerable().ToList();

                salesVoucherModel.accountsTransactions = _accountTransactionRepository.GetAsQueryable().Where(x => x.AccountsTransactionsVoucherNo == salesVoucherModel.salesVoucher.SalesVoucherNo && x.AccountstransactionsDelStatus != true).OrderBy(x => x.AccountsTransactionsVoucherNo).ToList();

                if (salesVoucherModel.salesVoucherDetails.Count > 0)
                {
                    foreach (var item in salesVoucherModel.salesVoucherDetails)
                    {
                        decimal sumReturnedQty = 0;
                        var stockQuery = _stockRegisterRepository.GetAsQueryable()
                        .Where(a => a.StockRegisterMaterialID == item.SalesVoucherDetailsMatId &&
                                    a.StockRegisterTransType == "SalesReturn" &&
                                    a.StockRegisterRefVoucherNo == item.SalesVoucherDetailsNo);

                        sumReturnedQty = stockQuery.Any()
                                           ? stockQuery.Sum(s => (decimal?)s.StockRegisterSout) ?? 0
                                           : 0;

                        if (sumReturnedQty != null)
                        {
                            item.SalesReturnQtyTill = sumReturnedQty;
                        }
                    }
                }
            }
            return salesVoucherModel;
        }
        public SalesVoucherModel GetSavedSalesVoucherDetailsBySaleVoucher(string svNo)
        {
            SalesVoucherModel salesVoucherModel = new SalesVoucherModel();
            //salesVoucherModel.salesVoucher = _salesVoucherRepository.GetAsQueryable().Where(k => k.SalesVoucherNo == pvno && k.SalesVoucherDelStatus == false).SingleOrDefault();
            salesVoucherModel.salesVoucher = _salesVoucherRepository.GetAsQueryable().Where(k => k.SalesVoucherNo == svNo).SingleOrDefault();

            if (salesVoucherModel.salesVoucher != null)
            {
                salesVoucherModel.salesVoucherDetails = _salesVoucherDetailsRepository.GetAsQueryable().Where(x => x.SalesVoucherDetailsNo == salesVoucherModel.salesVoucher.SalesVoucherNo && (x.SalesVoucherDetailsDelStatus == false || x.SalesVoucherDetailsDelStatus == null)).OrderBy(x => x.SalesVoucherDetailsItemName).AsEnumerable().ToList();

                salesVoucherModel.accountsTransactions = _accountTransactionRepository.GetAsQueryable().Where(x => x.AccountsTransactionsVoucherNo == salesVoucherModel.salesVoucher.SalesVoucherNo && x.AccountstransactionsDelStatus != true).OrderBy(x => x.AccountsTransactionsVoucherNo).ToList();

                if (salesVoucherModel.salesVoucherDetails.Count > 0)
                {
                    foreach (var item in salesVoucherModel.salesVoucherDetails)
                    {
                        decimal sumReturnedQty = 0;
                        var stockQuery = _stockRegisterRepository.GetAsQueryable()
                        .Where(a => a.StockRegisterMaterialID == item.SalesVoucherDetailsMatId &&
                                    a.StockRegisterTransType == "SalesReturn" &&
                                    a.StockRegisterRefVoucherNo == item.SalesVoucherDetailsNo);

                        sumReturnedQty = stockQuery.Any()
                                           ? stockQuery.Sum(s => (decimal?)s.StockRegisterSout) ?? 0
                                           : 0;

                        if (sumReturnedQty != null)
                        {
                            item.SalesReturnQtyTill = sumReturnedQty;
                        }
                    }
                }
            }
            return salesVoucherModel;
        }
        public VouchersNumbers GenerateVoucherNo(DateTime? VoucherGenDate)
        {
            try
            {


                var prefix = this._programsettingsRepository.GetAsQueryable().Where(k => k.ProgramSettingsKeyValue == Prefix.SalesVoucher_Prefix).FirstOrDefault().ProgramSettingsTextValue;
                int vnoMaxVal = Convert.ToInt32(_voucherNumbersRepository.GetAsQueryable()
                                                        .Where(x => x.VouchersNumbersVNoNu > 0 && x.VouchersNumbersVType == VoucherType.SalesVoucher_TYPE)
                                                        .DefaultIfEmpty()
                                                        .Max(o => o == null ? 0 : o.VouchersNumbersVNoNu)) + 1;


                VouchersNumbers vouchersNumbers = new VouchersNumbers
                {
                    VouchersNumbersVNo = prefix + vnoMaxVal,
                    VouchersNumbersVNoNu = vnoMaxVal,
                    VouchersNumbersVType = VoucherType.SalesVoucher_TYPE,
                    VouchersNumbersFsno = 1,
                    VouchersNumbersStatus = AccountStatus.Pending,
                    VouchersNumbersVoucherDate = VoucherGenDate

                };
                _voucherNumbersRepository.Insert(vouchersNumbers);
                return vouchersNumbers;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public VouchersNumbers GetVouchersNumbers(string pvno)
        {
            try
            {
                VouchersNumbers vouchersNumbers = _voucherNumbersRepository.GetAsQueryable().Where(k => k.VouchersNumbersVNo == pvno).SingleOrDefault();
                return vouchersNumbers;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex;
            }

        }

        public SalesVoucher GetSalesVoucherDetailsByMasterNo(string SalesVoucherNo)
        {
            try
            {
                SalesVoucher voucherModel = new SalesVoucher();
                voucherModel = _salesVoucherRepository.GetAsQueryable().Where(a => a.SalesVoucherNo == SalesVoucherNo).FirstOrDefault();
                voucherModel.SalesVoucherDetails = _salesVoucherDetailsRepository.GetAsQueryable().Where(o => o.SalesVoucherDetailsNo == SalesVoucherNo && o.SalesVoucherDetailsDelStatus != true).OrderBy(x => x.SalesVoucherDetailsItemName).AsEnumerable().ToList();


                if (voucherModel.SalesVoucherDetails.Count > 0)
                {

                    foreach (var item in voucherModel.SalesVoucherDetails)
                    {
                        item.SalesVoucherDetailsNoNavigation = null;
                        decimal sumReturnedQty = 0;
                        var stockQuery = _stockRegisterRepository.GetAsQueryable()
                        .Where(a => a.StockRegisterMaterialID == item.SalesVoucherDetailsMatId &&
                                    a.StockRegisterTransType == "SalesReturn" &&
                                    a.StockRegisterRefVoucherNo == item.SalesVoucherDetailsNo);

                        sumReturnedQty = stockQuery.Any()
                                           ? stockQuery.Sum(s => (decimal?)s.StockRegisterSIN) ?? 0
                                           : 0;

                        if (sumReturnedQty != null)
                        {
                            item.SalesReturnQtyTill = sumReturnedQty;
                        }

                    }
                }

                return voucherModel;


            }

            catch (System.Exception ex)
            {
                throw ex;
            }
        }
        public async Task<Response<GridWrapperResponse<List<GetSalesVoucherResponse>>>> GetSalesVoucher(GenericGridViewModel model)
        {
            try
            {
                List<GetSalesVoucherResponse> response = new List<GetSalesVoucherResponse>();
                var orders = await _salesVoucherRepository.GetAsQueryable().Where(x => x.SalesVoucherEnqNo != null).Skip(model.Skip).Take(model.Take).ToListAsync();
                response.AddRange(orders.Select(x => new GetSalesVoucherResponse
                {
                    SalesVoucherId = (decimal)x.SalesVoucherId,
                    SalesVoucherNo = x.SalesVoucherNo
                }).OrderBy(x => x.SalesVoucherNo).AsEnumerable().ToList());
                var gridResponse = new GridWrapperResponse<List<GetSalesVoucherResponse>>();
                gridResponse.Data = response;
                gridResponse.Total = 0;
                return Response<GridWrapperResponse<List<GetSalesVoucherResponse>>>.Success(gridResponse, "Data found");
            }
            catch (Exception ex)
            {
                return Response<GridWrapperResponse<List<GetSalesVoucherResponse>>>.Fail(new GridWrapperResponse<List<GetSalesVoucherResponse>>(), ex.Message);
            }
        }

        public object GetSalesReportSummaryWise(salesReportFilter filter)
        {

            try
            {



                var salesVocherReport = (from sv in _salesVoucherRepository.GetAsQueryable()
                                         join c in _customerMasterRepository.GetAsQueryable()
                                         on sv.SalesVoucherPartyId equals c.CustomerMasterCustomerNo
                                         where
                           (sv.SalesVoucherDelStatus == false || sv.SalesVoucherDelStatus == null)
                           && (filter.CustomerId == null || sv.SalesVoucherPartyId == filter.CustomerId)
                           && (filter.LocationId == null || sv.SalesVoucherLocationID == filter.LocationId)
                           && (!filter.IsDate
                               || (sv.SalesVoucherDate >= filter.Fromdate
                                   && sv.SalesVoucherDate <= filter.Todate))
                                         //where ((sv.SalesVoucherDelStatus == false || sv.SalesVoucherDelStatus == null) && sv.SalesVoucherPartyId == filter.CustomerId && sv.SalesVoucherLocationID == filter.LocationId )
                                         select new
                                         {
                                             voucherNo = sv.SalesVoucherNo,
                                             voucherDate = sv.SalesVoucherDate,
                                             voucherType = sv.SalesVoucherType,
                                             customerName = c.CustomerMasterCustomerName,
                                             grossAmount = sv.SalesVoucherTotalGrossAmount,
                                             discount = sv.SalesVoucherTotalDisAmount,
                                             //vatAmount = (sv.SalesVoucherNetAmount - sv.SalesVoucherTotalDisAmount) - sv.SalesVoucherTotalGrossAmount,
                                             vatAmount = sv.SalesVoucherVat_AMT,
                                             netAmount = sv.SalesVoucherNetAmount
                                         }).ToList();

                return salesVocherReport;
            }


            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public object GetSalesReportDetailWise(salesReportFilter filter)
        {
            try
            {
                var customerDict = _customerMasterRepository.GetAsQueryable()
                                        .ToDictionary(x => x.CustomerMasterCustomerNo, x => x.CustomerMasterCustomerName);

                var data = (from svd in _salesVoucherDetailsRepository.GetAsQueryable()
                            join sv in _salesVoucherRepository.GetAsQueryable()
                                on svd.SalesVoucherDetailsNo equals sv.SalesVoucherNo into lsv
                            from salesVou in lsv.DefaultIfEmpty()

                            join itemMas in _itemMasterRepository.GetAsQueryable()
                                on (long)svd.SalesVoucherDetailsMatId equals itemMas.ItemMasterItemId into litemm
                            from itemmaster in litemm.DefaultIfEmpty()

                            join um in _unitMasterRepository.GetAsQueryable()
                                on svd.SalesVoucherDetailsUnitId equals um.UnitMasterUnitId into lum
                            from unitMaster in lum.DefaultIfEmpty()

                            join um1 in _locationMasterRepository.GetAsQueryable()
                                on salesVou.SalesVoucherLocationID equals um1.LocationMasterLocationId into lum1
                            from locationMater in lum1.DefaultIfEmpty()

                            join unitDet in _UnitDetailsRepository.GetAsQueryable()
                                on new { UnitId = (int)svd.SalesVoucherDetailsUnitId, ItemId = (int)svd.SalesVoucherDetailsMatId }
                                equals new { UnitId = unitDet.UnitDetailsUnitId, ItemId = (int)unitDet.UnitDetailsItemId } into lud
                            from unitDetail in lud.DefaultIfEmpty()

                            where (svd.SalesVoucherDetailsNo != null && (filter.CustomerId == null || salesVou.SalesVoucherPartyId == filter.CustomerId) &&
                            (filter.LocationId == null || salesVou.SalesVoucherLocationID == filter.LocationId) &&
                            (filter.VType == null || salesVou.SalesVoucherType == filter.VType) && (filter.ItemId == null || svd.SalesVoucherDetailsMatId == filter.ItemId) && (!filter.IsDate
     || (salesVou.SalesVoucherDate >= filter.Fromdate
         && salesVou.SalesVoucherDate <= filter.Todate)))

                            select new
                            {
                                InvoiceType = salesVou.SalesVoucherType,
                                InvoiceStatus = salesVou.SalesVoucherStatus,
                                NetAmount = svd.SalesVoucherDetailsNetAmt,
                                Discount = svd.SalesVoucherDetailsDiscAmount,
                                GrossAmt = svd.SalesVoucherDetailsGrossAmount,
                                UnitPrice = svd.SalesVoucherDetailsCostPrice,

                                // FIXED: replace ?. with conditional operator
                                UnitDes = unitMaster == null ? "" : unitMaster.UnitMasterUnitShortName,
                                MatDes = itemmaster == null ? "" : itemmaster.ItemMasterItemName,
                                PartNo = itemmaster == null ? "N/A" : itemmaster.ItemMasterPartNo,
                                ItemId = itemmaster == null ? 0 : itemmaster.ItemMasterItemId,

                                VoucherDate = salesVou.SalesVoucherDate,
                                VoucherNo = salesVou.SalesVoucherNo,
                                QuantitySold = svd.SalesVoucherDetailsQuantity ?? 0,
                                baseUnit = svd.SalesVoucherDetailsQuantity
                                           * (decimal)(unitDetail == null ? 1 : unitDetail.UnitDetailsConversionType),

                                CustomerName = customerDict.ContainsKey(Convert.ToInt32(salesVou.SalesVoucherPartyId))
                                             ? customerDict[Convert.ToInt32(salesVou.SalesVoucherPartyId)]
                                             : "",

                                LocationMasterId = locationMater == null ? 0 : locationMater.LocationMasterLocationId,
                                LocationMasterName = locationMater == null ? "N/A" : locationMater.LocationMasterLocationName
                            }).ToList();

                return data;
            }
            catch (System.Exception ex)
            {
                throw;
            }
        }

        public VouchersNumbers GeneratePosVoucherNo(DateTime? VoucherGenDate)
        {
            try
            {
                // var prefix = this._programsettingsRepository.GetAsQueryable().Where(k => k.ProgramSettingsKeyValue == Prefix.SalesVoucher_Prefix).FirstOrDefault().ProgramSettingsTextValue;
                int vnoMaxVal = Convert.ToInt32(_voucherNumbersRepository.GetAsQueryable()
                                                        .Where(x => x.VouchersNumbersVNoNu > 0 && x.VouchersNumbersVType == VoucherType.PosSalesVoucher_TYPE)
                                                        .DefaultIfEmpty()
                                                        .Max(o => o == null ? 0 : o.VouchersNumbersVNoNu)) + 1;



                VouchersNumbers vouchersNumbers = new VouchersNumbers
                {
                    VouchersNumbersVNo = "POSSV" + vnoMaxVal,
                    VouchersNumbersVNoNu = vnoMaxVal,
                    VouchersNumbersVType = VoucherType.PosSalesVoucher_TYPE,
                    VouchersNumbersFsno = 1,
                    VouchersNumbersStatus = AccountStatus.Pending,
                    VouchersNumbersVoucherDate = VoucherGenDate

                };
                _voucherNumbersRepository.Insert(vouchersNumbers);
                return vouchersNumbers;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        #region Pos Sales Hold
        public VouchersNumbers GeneratePosSaleHoldVoucherNo(DateTime? VoucherGenDate)
        {
            try
            {
                // var prefix = this._programsettingsRepository.GetAsQueryable().Where(k => k.ProgramSettingsKeyValue == Prefix.SalesVoucher_Prefix).FirstOrDefault().ProgramSettingsTextValue;
                int vnoMaxVal = Convert.ToInt32(_voucherNumbersRepository.GetAsQueryable()
                                                        .Where(x => x.VouchersNumbersVNoNu > 0 && x.VouchersNumbersVType == VoucherType.PosSalesVoucher_TYPE)
                                                        .DefaultIfEmpty()
                                                        .Max(o => o == null ? 0 : o.VouchersNumbersVNoNu)) + 1;



                VouchersNumbers vouchersNumbers = new VouchersNumbers
                {
                    VouchersNumbersVNo = "POSSH" + vnoMaxVal,
                    VouchersNumbersVNoNu = vnoMaxVal,
                    VouchersNumbersVType = VoucherType.PosSalesVoucher_TYPE,
                    VouchersNumbersFsno = 1,
                    VouchersNumbersStatus = AccountStatus.Pending,
                    VouchersNumbersVoucherDate = VoucherGenDate

                };
                _voucherNumbersRepository.Insert(vouchersNumbers);
                return vouchersNumbers;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public SalesHold InsertPosSalesHold(SalesHold salesHold, List<SalesHoldDetails> salesHoldDetails)
        {
            try
            {
                _salesHoldRepository.BeginTransaction();

                string salesVoucherNumber = this.GeneratePosSaleHoldVoucherNo(salesHold.VoucherDate).VouchersNumbersVNo;
                salesHold.VoucherNo = salesVoucherNumber;


                decimal maxcount = 0;
                maxcount = (
                    _salesHoldRepository.GetAsQueryable()
                    .DefaultIfEmpty().Max(o => o == null ? 0 : (decimal)o.V_ID) + 1);

                salesHold.V_ID = maxcount;
                salesHoldDetails = salesHoldDetails.Select((x) =>
                {
                    x.SNO = maxcount;
                    x.VoucherNo = salesVoucherNumber;
                    return x;
                }).OrderBy(x => x.VoucherNo).AsEnumerable().ToList();

                salesHold.SalesHoldDetails.Clear();

                long max1count = 0;
                max1count =
                    _salesHoldDetailsRepository.GetAsQueryable()
                    .DefaultIfEmpty().Max(o => o == null ? 0 : (long)o.SDet_ID) + 1;
                foreach (var item in salesHoldDetails)
                {
                    item.SDet_ID = max1count;
                    item.SNO = salesHold.V_ID;
                    item.VoucherNo = salesVoucherNumber ?? "";
                    salesHold.SalesHoldDetails.Add(item);
                    max1count++;
                }

                _salesHoldRepository.Insert(salesHold);

                _salesHoldRepository.TransactionCommit();

                return salesHold;
            }
            catch (Exception ex)
            {
                _salesHoldRepository.TransactionRollback();
                throw ex;
            }
        }

        public List<SalesHold> DeletePosSalesHold(decimal id)
        {
            try
            {
                _salesHoldRepository.BeginTransaction();

                SalesHold SalesHold = new SalesHold();
                SalesHold = this.GetPosSalesHold(id);
                if (SalesHold != null)
                {
                    _salesHoldDetailsRepository.DeleteList(SalesHold.SalesHoldDetails.OrderBy(x => x.VoucherNo).ToList());



                    _salesHoldRepository.Delete(SalesHold);
                    _salesHoldRepository.TransactionCommit();
                }

                return _salesHoldRepository.GetAll().Where(a => a.V_ID > 0).OrderBy(x => x.VoucherNo).AsEnumerable().ToList();
            }
            catch (Exception ex)
            {
                _salesHoldRepository.TransactionRollback();
                throw ex;
            }
        }

        public SalesHold GetPosSalesHold(decimal id)
        {
            SalesHold SalesHold = new SalesHold();

            try
            {

                SalesHold = _salesHoldRepository.GetAsQueryable().Where(k => k.V_ID == id).FirstOrDefault();
                if (SalesHold != null)
                {
                    SalesHold.SalesHoldDetails = _salesHoldDetailsRepository.GetAsQueryable().Where(x => x.SNO == id).OrderBy(x => x.VoucherNo).AsEnumerable().ToList();
                }
                return SalesHold;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public IEnumerable<SalesHold> GetPosSalesHoldList()
        {

            try
            {

                IEnumerable<SalesHold> SalesHold = _salesHoldRepository.GetAll();
                return SalesHold;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion
        public POS_WorkPeriod StartPosWP(POS_WorkPeriod wp)
        {
            try
            {
                _POSWorkPeriodRepository.BeginTransaction();

                wp.StartDate = DateTime.Now;
                wp.Enddate = null;
                wp.StartTime = DateTime.Now;
                wp.EndTime = null;
                wp.LoginComputerName = Environment.MachineName;
                var hostName = Dns.GetHostName();
                IPAddress[] localIPs = Dns.GetHostAddresses(hostName);
                IPAddress ipv4Address = localIPs.FirstOrDefault(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);

                if (ipv4Address != null)
                {
                    wp.LoginSystemIP = ipv4Address.ToString();
                }
                else
                {
                    wp.LoginSystemIP = null;
                }

                wp.OpeningCash = 0;//query
                wp.OpeningCashUser = 0;
                wp.UserOpeningBal = 0; //text box



                _POSWorkPeriodRepository.Insert(wp);
                _POSWorkPeriodRepository.TransactionCommit();

                return wp;
            }
            catch (Exception ex)
            {
                _POSWorkPeriodRepository.TransactionRollback();
                throw ex;
            }
        }

        public POS_WorkPeriod EndPosWP(POS_WorkPeriod wp)
        {
            try
            {
                _POSWorkPeriodRepository.BeginTransaction();

                wp.Enddate = DateTime.Now;
                wp.StartDate = null;
                wp.EndTime = DateTime.Now;
                wp.StartTime = null;
                wp.LoginComputerName = Environment.MachineName;
                var hostName = Dns.GetHostName();
                IPAddress[] localIPs = Dns.GetHostAddresses(hostName);
                IPAddress ipv4Address = localIPs.FirstOrDefault(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);

                if (ipv4Address != null)
                {
                    wp.LoginSystemIP = ipv4Address.ToString();
                }
                else
                {
                    wp.LoginSystemIP = null;
                }


                wp.ClosingCash = 0;  //query + POS_SalesTransaction_Details.amount
                wp.ClosingCashUser = 0;
                wp.UserClosebal = 0; //text box

                wp.DifferenceAmount = 0; //query + POS_SalesTransaction_Details.amount-text box

                _POSWorkPeriodRepository.Insert(wp);
                _POSWorkPeriodRepository.TransactionCommit();

                return wp;
            }
            catch (Exception ex)
            {
                _POSWorkPeriodRepository.TransactionRollback();
                throw ex;
            }
        }

        public async Task<ApiResponse<List<SalesVoucherDto>>> GetSalesVoucherForGrnBrowse(GeneralFilter model)
        {
            try
            {
                //--and cm.Customer_Master_Customer_No = 1
                //-- and lm.Location_Master_Location_ID = 10


                var filterString = string.Empty;
                if (model.IsDate && model.FromDate.HasValue && model.ToDate.HasValue)
                    filterString = $" and cast(sv.SalesVoucherDate as Date) between '{model.FromDate.Value:yyyy-MM-dd}' and '{model.ToDate.Value:yyyy-MM-dd}'";
                if (!string.IsNullOrEmpty(model.Filter))
                    filterString += model.Filter;

                var sql = $@" select sv.SalesVoucherId Id, sv.SalesVoucherNo VoucherNo, cast(sv.SalesVoucherDate as Date) VoucherDate,cm.Customer_Master_Customer_Name CustomerName
                                         ,im.Item_Master_Item_ID ItemId ,im.Item_Master_Item_Name ItemName, sd.SalesVoucherDetailsQuantity Quantity,um.Unit_Master_Unit_ID UnitId,um.Unit_Master_Unit_Short_Name Unit, lm.Location_Master_Location_Name locationName
                                         from SalesVoucher sv
                                         join SalesVoucherDetails sd on sv.SalesVoucherId = sd.SalesVoucherId
                                         join Customer_Master cm on sv.SalesVoucherPartyId = cm.Customer_Master_Customer_No
                                         join Item_Master im on sd.SalesVoucherDetailsMatID = im.Item_Master_Item_ID
                                         join Location_Master lm on sv.SalesVoucherLocationID = lm.Location_Master_Location_ID
                                         join Unit_Master um on sd.SalesVoucherDetailsUnitId = um.Unit_Master_Unit_ID
                                         where 1=1 and sd.SalesVoucherDetailsDelStatus = 0 
                                    {filterString}";

                var result = (await _dbConnection.QueryAsync<SalesVoucherDto>(sql)).ToList();
                if (!result.Any())
                    return ApiResponse<List<SalesVoucherDto>>.Fail("No Data found!");
                return ApiResponse<List<SalesVoucherDto>>.Success(result, "Data found!");
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IQueryable GetCustomerDetailWiseSalesReport(int? customerId, DateTime? fromDate, DateTime? toDate)
        {

            try
            {
                var customerDict = _customerMasterRepository.GetAsQueryable()
                    .ToDictionary(x => x.CustomerMasterCustomerNo, x => x.CustomerMasterCustomerName);

                var data = (from svd in _salesVoucherDetailsRepository.GetAll()
                            join sv in _salesVoucherRepository.GetAll() on svd.SalesVoucherDetailsNo equals sv.SalesVoucherNo into lsv

                            from salesVou in lsv.DefaultIfEmpty()
                            join itemMas in _itemMasterRepository.GetAll() on (long)svd.SalesVoucherDetailsMatId equals itemMas.ItemMasterItemId into litemm

                            from itemmaster in litemm.DefaultIfEmpty()
                            join um in _unitMasterRepository.GetAll() on svd.SalesVoucherDetailsUnitId equals um.UnitMasterUnitId into lum

                            from unitMaster in lum.DefaultIfEmpty()
                            where svd.SalesVoucherDetailsNo != null
                                  // ✅ Customer filter
                                  && (!customerId.HasValue || salesVou.SalesVoucherPartyId == customerId)

                                  // ✅ FromDate filter
                                  && (!fromDate.HasValue || salesVou.SalesVoucherDate >= fromDate.Value)

                                  // ✅ ToDate filter
                                  && (!toDate.HasValue || salesVou.SalesVoucherDate <= toDate.Value)

                            select new
                            {
                                netAmount = svd.SalesVoucherDetailsNetAmt,
                                discount = svd.SalesVoucherDetailsDiscAmount,
                                grossAmount = svd.SalesVoucherDetailsGrossAmount,
                                unitPrice = svd.SalesVoucherDetailsCostPrice,
                                unitDes = unitMaster?.UnitMasterUnitShortName,
                                itemName = itemmaster?.ItemMasterItemName ?? "Unknown",
                                partNo = itemmaster?.ItemMasterPartNo ?? "N/A",
                                itemId = itemmaster?.ItemMasterItemId ?? 0,
                                voucherDate = salesVou.SalesVoucherDate,
                                voucherNo = salesVou.SalesVoucherNo,
                                soldQty = svd.SalesVoucherDetailsQuantity ?? 0,
                                customerName = customerDict.ContainsKey(Convert.ToInt32(salesVou.SalesVoucherPartyId))
                                               ? customerDict[Convert.ToInt32(salesVou.SalesVoucherPartyId)]
                                               : ""
                            }).AsQueryable();

                return data;
            }
            catch (System.Exception ex)
            {
                throw;
            }

        }

        public IQueryable GetCustomerMaterialWiseSalesReport(int? customerId, DateTime? fromDate, DateTime? toDate)
        {
            try
            {
                var query = from sv in _salesVoucherRepository.GetAll()
                            join c in _customerMasterRepository.GetAll()
                            on sv.SalesVoucherPartyId equals c.CustomerMasterCustomerNo
                            join sd in _salesVoucherDetailsRepository.GetAll()
                            on sv.SalesVoucherNo equals sd.SalesVoucherDetailsNo
                            where (sv.SalesVoucherDelStatus == false || sv.SalesVoucherDelStatus == null)
                            select new { sv, c, sd };

                // ✅ Apply Customer Filter (if provided)
                if (customerId.HasValue)
                {
                    query = query.Where(x => x.c.CustomerMasterCustomerNo == customerId.Value);
                }

                if (fromDate.HasValue)
                {
                    query = query.Where(x => x.sv.SalesVoucherDate >= fromDate.Value.Date);
                }
                if (toDate.HasValue)
                {
                    var endDate = toDate.Value.Date.AddDays(1);
                    query = query.Where(x => x.sv.SalesVoucherDate < endDate);
                }

                var customerWiseReport = query
                    .Select(g => new
                    {
                        voucherNo = g.sv.SalesVoucherNo,
                        voucherDate = g.sv.SalesVoucherDate,
                        itemName = g.sd.SalesVoucherDetailsItemName,
                        soldQty = g.sd.SalesVoucherDetailsQuantity,
                        voucherType = g.sv.SalesVoucherType,
                        customerName = g.c.CustomerMasterCustomerName,
                        grossAmount = g.sv.SalesVoucherTotalGrossAmount,
                        discount = g.sv.SalesVoucherTotalDisAmount,
                        netAmount = g.sv.SalesVoucherNetAmount,

                    })
                    .AsQueryable();

                return customerWiseReport;
            }
            catch (System.Exception ex)
            {
                throw;
            }
        }

        public IQueryable GetCustomerSummaryWiseSalesReport(int? customerId, DateTime? fromDate, DateTime? toDate)
        {
            try
            {
                var query = from sv in _salesVoucherRepository.GetAll()
                            join c in _customerMasterRepository.GetAll()
                            on sv.SalesVoucherPartyId equals c.CustomerMasterCustomerNo
                            where (sv.SalesVoucherDelStatus == false || sv.SalesVoucherDelStatus == null)
                            select new { sv, c };

                // ✅ Apply Customer Filter (if provided)
                if (customerId.HasValue)
                {
                    query = query.Where(x => x.c.CustomerMasterCustomerNo == customerId.Value);
                }

                if (fromDate.HasValue)
                {
                    query = query.Where(x => x.sv.SalesVoucherDate >= fromDate.Value.Date);
                }
                if (toDate.HasValue)
                {
                    var endDate = toDate.Value.Date.AddDays(1);
                    query = query.Where(x => x.sv.SalesVoucherDate < endDate);
                }

                var customerWiseReport = query
                    .Select(g => new
                    {
                        voucherNo = g.sv.SalesVoucherNo,
                        voucherDate = g.sv.SalesVoucherDate,
                        voucherType = g.sv.SalesVoucherType,
                        customerName = g.c.CustomerMasterCustomerName,
                        grossAmount = g.sv.SalesVoucherTotalGrossAmount,
                        discount = g.sv.SalesVoucherTotalDisAmount,
                        //vatAmount = (sv.SalesVoucherNetAmount - sv.SalesVoucherTotalDisAmount) - sv.SalesVoucherTotalGrossAmount,
                        vatAmount = g.sv.SalesVoucherVat_AMT,
                        netAmount = g.sv.SalesVoucherNetAmount,
                        retAmount = g.sv.SalesVoucherVat_RountAmt,
                        net = g.sv.SalesVoucherNetAmount
                    })
                    .AsQueryable();

                return customerWiseReport;
            }
            catch (System.Exception ex)
            {
                throw;
            }



        }


    }
}

