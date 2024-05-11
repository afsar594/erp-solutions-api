using Inspire.Erp.Application.Sales.Interfaces;
using Inspire.Erp.Application.Account.Implementations;
using Inspire.Erp.Application.Common;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Enums;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Inspire.Erp.Application.MODULE;
using Inspire.Erp.Domain.Modals.Sales;
using Inspire.Erp.Domain.Modals.Common;
using Inspire.Erp.Domain.Modals;
using Inspire.Erp.Infrastructure.Database;
using SendGrid.Helpers.Mail;
using Inspire.Erp.Domain.Entities.POS;


namespace Inspire.Erp.Application.Sales.Implementations
{
    public class SalesVoucherService : ISalesVoucherService
    {
        private IRepository<StockRegister> _stockRegisterRepository;
        private IRepository<CurrencyMaster> _currencyMasterRepository;
        private IRepository<AccountsTransactions> _accountTransactionRepository;
        private IRepository<SalesVoucher> _salesVoucherRepository;
        private IRepository<SalesVoucherDetails> _salesVoucherDetailsRepository;
        private IRepository<ProgramSettings> _programsettingsRepository;
        private IRepository<VouchersNumbers> _voucherNumbersRepository;
        private readonly ILogger<PaymentVoucherService> _logger;

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


        /// <summary>
        /// Master accounts
        /// </summary>
        private IRepository<MasterAccountsTable> accMaster;

        private IRepository<POS_Sales_Voucher> _posSalesVoucherRepository;
        private IRepository<POS_Sales_Voucher_Details> _posSalesVoucherDetailsRepository;


        //private IRepository<ReportSalesVoucher> _reportSalesVoucherRepository;
        public SalesVoucherService(
            //IRepository<ReportSalesVoucher> reportSalesVoucherRepository,
            IRepository<CustomerMaster> customerMasterRepository,
            IRepository<SalesManMaster> salesManMasterRepository,
            IRepository<DepartmentMaster> departmentMasterRepository,
            IRepository<ItemMaster> itemMasterRepository, IRepository<UnitMaster> unitMasterRepository,
            IRepository<SuppliersMaster> suppliersMasterRepository, IRepository<LocationMaster> locationMasterRepository,

            IRepository<AccountsTransactions> accountTransactionRepository, IRepository<StockRegister> stockRegisterRepository, IRepository<ProgramSettings> programsettingsRepository,
             IRepository<VouchersNumbers> voucherNumbers, ILogger<PaymentVoucherService> logger,

            IRepository<SalesVoucher> salesVoucherRepository, IRepository<SalesVoucherDetails> salesVoucherDetailsRepository, IRepository<CurrencyMaster> currencyMasterRepository, IRepository<UnitDetails> unitDetailsRepository, IRepository<MasterAccountsTable> accMaster, IRepository<SalesReturn> salesReturn, IRepository<SalesReturnDetails> salesReturnDtls, IRepository<OptionsMaster> optionMaster, IRepository<POS_Sales_Voucher> posSalesVoucherRepository, IRepository<POS_Sales_Voucher_Details> posSalesVoucherDetailsRepository)
        {
            this._accountTransactionRepository = accountTransactionRepository;
            this._stockRegisterRepository = stockRegisterRepository;
            this._salesVoucherRepository = salesVoucherRepository;
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
            this.accMaster = accMaster;
            _salesReturn = salesReturn;
            _salesReturnDtls = salesReturnDtls;
            _optionMaster = optionMaster;
            _posSalesVoucherDetailsRepository = posSalesVoucherDetailsRepository;
            _posSalesVoucherRepository = posSalesVoucherRepository;
            

            //_reportSalesVoucherRepository = reportSalesVoucherRepository;
        }

        //public IEnumerable<ReportSalesVoucher> SalesVoucher_GetReportSalesVoucher()
        //{
        //    return _reportSalesVoucherRepository.GetAll();
        //}



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

        public SalesVoucherModel UpdateSalesVoucher(SalesVoucher salesVoucher, List<AccountsTransactions> accountsTransactions,
            List<SalesVoucherDetails> salesVoucherDetails
           , List<StockRegister> stockRegister
            )
        {

            try
            {
                _salesVoucherRepository.BeginTransaction();

                //=================================
                clsCommonFunctions.Delete_OldEntry_StockRegister(salesVoucher.SalesVoucherNo, VoucherType.SalesVoucher_TYPE, _stockRegisterRepository);
                clsCommonFunctions.Delete_OldEntry_AccountsTransactions(salesVoucher.SalesVoucherNo, VoucherType.SalesVoucher_TYPE, _accountTransactionRepository);
                //=================================
                salesVoucher.SalesVoucherDetails = salesVoucherDetails.Select((k) =>
                {
                    //if (k.SalesVoucherDetailsId == 0)
                    //{
                    k.SalesVoucherId = salesVoucher.SalesVoucherId;
                    k.SalesVoucherDetailsNo = salesVoucher.SalesVoucherNo;
                    //k.SalesVoucherDetailsId = 0;
                    //}

                    return k;
                }).ToList();

                _salesVoucherRepository.Update(salesVoucher);





                //_salesVoucherRepository.Update(salesVoucher);
                accountsTransactions = accountsTransactions.Select((k) =>
                {
                    if (k.AccountsTransactionsTransSno == 0)
                    {
                        k.AccountsTransactionsTransDate = salesVoucher.SalesVoucherDate;
                        k.AccountsTransactionsVoucherNo = salesVoucher.SalesVoucherNo;
                        k.AccountsTransactionsVoucherType = VoucherType.SalesVoucher_TYPE;
                        k.AccountsTransactionsStatus = AccountStatus.Approved;
                    }

                    return k;
                }).Where(x=>x.AccountsTransactionsTransSno == 0).ToList();
                _accountTransactionRepository.UpdateList(accountsTransactions);


                stockRegister = stockRegister.Select((k) =>
                {
                    if (k.StockRegisterStoreID == 0)
                    {
                        k.StockRegisterVoucherDate = salesVoucher.SalesVoucherDate;
                        k.StockRegisterRefVoucherNo = salesVoucher.SalesVoucherNo;
                        k.StockRegisterTransType = VoucherType.SalesVoucher_TYPE;
                        k.StockRegisterStatus = AccountStatus.Approved;
                    }

                    return k;
                }).ToList();
                _stockRegisterRepository.UpdateList(stockRegister);






                _salesVoucherRepository.TransactionCommit();


            }
            catch (Exception ex)
            {
                _salesVoucherRepository.TransactionRollback();
                throw ex;
            }

            return this.GetSavedSalesVoucherDetails(salesVoucher.SalesVoucherNo);
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
                }).ToList();

                //_salesVoucherDetailsRepository.UpdateList(salesVoucherDetails);

                accountsTransactions = accountsTransactions.Select((k) =>
                {
                    k.AccountstransactionsDelStatus = true;
                    return k;
                }).ToList();
                _accountTransactionRepository.UpdateList(accountsTransactions);




                stockRegister = stockRegister.Select((k) =>
                {
                    k.StockRegisterDelStatus = true;
                    return k;
                }).ToList();
                _stockRegisterRepository.UpdateList(stockRegister);




                salesVoucher.SalesVoucherDetails = salesVoucherDetails;

                _salesVoucherRepository.Update(salesVoucher);

                var vchrnumer = _voucherNumbersRepository.GetAsQueryable().Where(k => k.VouchersNumbersVNo == salesVoucher.SalesVoucherNo).FirstOrDefault();

                _voucherNumbersRepository.Update(vchrnumer);

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
        private decimal getConverionTypebyUnitId(int? unitDetailsid)
        {
            try
            {
                return (decimal)this._UnitDetailsRepository.GetAsQueryable().FirstOrDefault(x => x.UnitDetailsUnitId == unitDetailsid).UnitDetailsConversionType;
            }
            catch
            {
                return 1;
            }
        }

        public SalesVoucherModel InsertSalesVoucher(SalesVoucher salesVoucher, List<AccountsTransactions> accountsTransactions,
            List<SalesVoucherDetails> salesVoucherDetails
            , List<StockRegister> stockRegister
            )
        {
            try
            {
                _salesVoucherRepository.BeginTransaction();
                string salesVoucherNumber = this.GenerateVoucherNo(salesVoucher.SalesVoucherDate).VouchersNumbersVNo;
                salesVoucher.SalesVoucherNo = salesVoucherNumber;


                int maxcount = 0;
                maxcount = Convert.ToInt32(
                    _salesVoucherRepository.GetAsQueryable()
                    .DefaultIfEmpty().Max(o => o == null ? 0 : o.SalesVoucherId) + 1);

                salesVoucher.SalesVoucherId = maxcount;

                // var currencyMaster = _currencyMasterRepository.GetAsQueryable().Where(c => c.CurrencyMasterCurrencyId == salesVoucher.SalesVoucherCurrencyId).FirstOrDefault();
                //foreach (var saleVoucherDetail in salesVoucherDetails)
                //{
                //    saleVoucherDetail.SalesVoucherDetailsRate = saleVoucherDetail.SalesVoucherDetailsRate * Convert.ToDecimal(currencyMaster.CurrencyMasterCurrencyRate);
                //}
                // salesVoucher.SalesVoucherFcRate = salesVoucher.SalesVoucherFcRate * Convert.ToDecimal(currencyMaster.CurrencyMasterCurrencyRate);
                salesVoucherDetails = salesVoucherDetails.Select((x) =>
                {
                    x.SalesVoucherId = maxcount;
                    x.SalesVoucherDetailsNo = salesVoucherNumber;
                    return x;
                }).ToList();
                //_salesVoucherDetailsRepository.InsertList(salesVoucherDetails);

                //accunt transaction entries for sales
                //createAccountTransactionEntries(salesVoucher, accountsTransactions);

                accountsTransactions = accountsTransactions.Select((k) =>
                {
                    k.AccountsTransactionsVoucherNo = salesVoucherNumber;
                    k.AccountsTransactionsAccNo = k.AccountsTransactionsAccNo ?? string.Empty;
                    k.AccountsTransactionsVoucherType = VoucherType.SalesVoucher_TYPE;
                    k.AccountsTransactionsStatus = AccountStatus.Approved;
                    return k;
                }).ToList();
                _accountTransactionRepository.InsertList(accountsTransactions);

                stockRegister = stockRegister.Select((k) =>
                {
                    k.StockRegisterVoucherDate = salesVoucher.SalesVoucherDate;
                    k.StockRegisterRefVoucherNo = salesVoucherNumber;
                    k.StockRegisterTransType = VoucherType.SalesVoucher_TYPE;
                    k.StockRegisterStatus = AccountStatus.Approved;
                    return k;
                }).ToList();



                var stockReg = new List<StockRegister>();
                foreach (var item in salesVoucher.SalesVoucherDetails)
                {

                 var converontype=   this.getConverionTypebyUnitId(item.SalesVoucherDetailsUnitId);
                 
                    stockReg.Add(new StockRegister()
                    {
                        StockRegisterPurchaseID = salesVoucher.SalesVoucherNo,
                        StockRegisterRefVoucherNo = salesVoucher.SalesVoucherNo,
                        StockRegisterVoucherDate = DateTime.Now.Date,
                        StockRegisterMaterialID = item.SalesVoucherDetailsMatId,
                        StockRegisterQuantity = item.SalesVoucherDetailsQuantity * (decimal)(converontype) ?? 0,
                        StockRegisterSIN = 0,
                        StockRegisterSout = item.SalesVoucherDetailsQuantity * (decimal)(converontype) ?? 0,
                        StockRegisterRate = item.SalesVoucherDetailsRate,
                        StockRegisterAmount = item.SalesVoucherDetailsActualAmount ?? 0,
                        StockRegisterFCAmount = item.SalesVoucherDetailsActualAmount ?? 0 * this.getCurrencyRate(salesVoucher.SalesVoucherCurrencyId),
                        StockRegisterAssignedDate = DateTime.Now.Date,
                        StockRegisterStatus = "A",
                        StockRegisterTransType = salesVoucher.SalesVoucherType,
                        StockRegisterUnitID = item.SalesVoucherDetailsUnitId,
                        StockRegisterLocationID = salesVoucher.SalesVoucherLocationID,
                        StockRegisterJobID = salesVoucher.SalesVoucherJobId,
                        StockRegisterDepID = salesVoucher.SalesVoucherDptID,
                        StockRegisterDelStatus = false
                    });
                }
                _stockRegisterRepository.InsertList(stockReg);



                _salesVoucherRepository.Insert(salesVoucher);
                //_salesVoucherDetailsRepository.InsertList(salesVoucherDetails);
                _salesVoucherRepository.TransactionCommit();
                return this.GetSavedSalesVoucherDetails(salesVoucher.SalesVoucherNo);

            }
            catch (Exception ex)
            {
                _salesVoucherRepository.TransactionRollback();
                throw ex;
            }

        }
        /// <summary>
        /// Name : createAccountTransactionEntries
        /// Desc : this is used to create transaction entries again sales customer 
        ///  not to change/ remove OR replace any field in below method, may leads to an unexpected errors/
        ///  may be create a wrong entry in account transaction table
        /// </summary>
        /// <param name="salesVoucher"></param>
        /// <param name="accountsTransactions"></param>
        /// <param name="salesVoucherNumber"></param>
        private void createAccountTransactionEntries(SalesVoucher salesVoucher, List<AccountsTransactions> accountsTransactions)
        {
            decimal currencyRate = getCurrencyRate(salesVoucher.SalesVoucherCurrencyId);

            //Credit Entry

            //Debit Entry
            if (salesVoucher.SalesVoucherType == "Credit")//here there are two entries for invoice type
                                                          //need to replace SalesVoucherType by invoice type
            {

            }
            else
            {

            }
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
            }).ToList();
            _accountTransactionRepository.InsertList(accountsTransactions);
        }

        private string getAccountNoByPartyName(string partyName)
        {
            try
            {

                string maAcc = this.accMaster.GetAsQueryable().Where(x => x.MaAccName.ToUpper().Equals(partyName.ToUpper())).Select(x => x.MaAccNo).FirstOrDefault();

                return maAcc == null ? "" : maAcc;
                //return this.accMaster.GetAsQueryable().FirstOrDefault(x => x.MaAccName.ToUpper().Equals(partyName.ToUpper())).MaAccNo??"";
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
            //IEnumerable<SalesVoucher> salesVoucher_ALL = _salesVoucherRepository.GetAll().Where(k => k.SalesVoucherDelStatus == false || k.SalesVoucherDelStatus == null).ToList();
            //IEnumerable<CurrencyMaster> _currencyMaster = _currencyMasterRepository.GetAll().ToList();

            //foreach (var salesVoucher in salesVoucher_ALL)
            //{
            //    salesVoucher.CurrencyMasterCurrencyName = _currencyMaster
            //                                                 .Where(c => c.CurrencyMasterCurrencyId == salesVoucher.SalesVoucherCurrencyId)
            //                                                 .FirstOrDefault() != null ? _currencyMaster
            //                                                 .Where(c => c.CurrencyMasterCurrencyId == salesVoucher.SalesVoucherCurrencyId)
            //                                                 .FirstOrDefault().CurrencyMasterCurrencyName : null;
            //}


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
                        }).ToList();





            return salV;
        }
        public SalesVoucherModel GetSavedSalesVoucherDetails(string pvno)
        {
            SalesVoucherModel salesVoucherModel = new SalesVoucherModel();
            //salesVoucherModel.salesVoucher = _salesVoucherRepository.GetAsQueryable().Where(k => k.SalesVoucherNo == pvno && k.SalesVoucherDelStatus == false).SingleOrDefault();
            salesVoucherModel.salesVoucher = _salesVoucherRepository.GetAsQueryable().Where(k => k.SalesVoucherNo == pvno).SingleOrDefault();
            salesVoucherModel.salesVoucherDetails = _salesVoucherDetailsRepository.GetAsQueryable().Where(x => x.SalesVoucherDetailsNo == pvno && (x.SalesVoucherDetailsDelStatus == false || x.SalesVoucherDetailsDelStatus == null)).ToList();

            salesVoucherModel.accountsTransactions = _accountTransactionRepository.GetAsQueryable().Where(x => x.AccountsTransactionsVoucherNo == pvno).ToList();

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


                //var prefix = "CN";
                //int vnoMaxVal = 1;


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



        //using Microsoft.Extensions.Logging;
        //private readonly ILogger<PaymentVoucherService> _logger;
        //IRepository<VouchersNumbers> voucherNumbers, ILogger<PaymentVoucherService> logger,
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

        public IQueryable GetSalesVoucherDetailsByMasterNo(string SalesVoucherNo)
        {
            try
            {
                var Details = _salesVoucherDetailsRepository.GetAll().Where(o => o.SalesVoucherDetailsNo == SalesVoucherNo).AsQueryable();
                return Details;
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
                }).ToList());
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

        public IQueryable GetSalesReportSummaryWise()
        {
            try
            {
                var SalesVoucherList = _salesVoucherRepository.GetAll();
                var SalesManMasterList = _salesManMasterRepository.GetAll();// context.SalesManMaster.ToList();

                var SalesReturnDetails = _salesReturnDtls.GetAll().Select(o => new
                {
                    SalRetAmt = o.SalesReturnDetailsNetAmt,
                    SalesReturnDetailsNo = o.SalesReturnDetailsNo,
                }).ToList().GroupBy(g => new
                {
                    SalesReturnDetailsNo = g.SalesReturnDetailsNo
                }).Select(o => new
                {
                    SalesReturnDetailsNo = o.Key.SalesReturnDetailsNo,
                    SalRetAmt = o.Sum(s => s.SalRetAmt)
                }).ToList();

                var SalesReturnMaster = _salesReturn.GetAll().Select(o => new
                {
                    SalRetVatAmt = o.SalesReturnVat_RountAmt,
                    SalesReturnNo = o.SalesReturnNo,
                    SalRetAmt = o.SalesReturnNetAmount,
                    SalRetRountAmt = o.SalesReturnVat_RoundSign == "-" ? o.SalesReturnVat_RountAmt * -1 : o.SalesReturnVat_RountAmt ,
                }).ToList().GroupBy(g => new
                {
                    SalesReturnNo = g.SalesReturnNo,
                }).Select(o => new
                {
                    SalesReturnNo = o.Key.SalesReturnNo,
                    SalRetVatAmt = o.Sum(s => s.SalRetVatAmt),
                    SalRetAmt = o.FirstOrDefault().SalRetAmt,
                    SalRetRountAmt = o.FirstOrDefault().SalRetRountAmt,
                }).ToList();
                var Include_Sales_Return_Details_In_Reports = _optionMaster.GetAsQueryable().FirstOrDefault(o => o.OptionsMasterDescription == "Include_Sales_Return_Details_In_Reports" && o.OptionsMasterType == "Yes");
                if (Include_Sales_Return_Details_In_Reports != null)
                {
                    var data1 = (from hdr in SalesVoucherList
                                 join dtl in SalesReturnDetails on hdr.SalesVoucherNo equals dtl.SalesReturnDetailsNo into ldtl
                                 from retDtl in ldtl.DefaultIfEmpty()
                                 join mstr in SalesReturnMaster on hdr.SalesVoucherNo equals mstr.SalesReturnNo into lmstr
                                 from retMstr in lmstr.DefaultIfEmpty()
                                 where hdr.SalesVoucherNo != null
                                 select new
                                 {
                                     voucherNo = hdr.SalesVoucherNo,
                                     voucherDate = hdr.SalesVoucherDate,
                                     voucherType = hdr.SalesVoucherType,
                                    CustomerName =_customerMasterRepository.GetAsQueryable().FirstOrDefault(x=>x.CustomerMasterCustomerNo == hdr.SalesVoucherPartyId).CustomerMasterCustomerName,//  hdr.SalesVoucherPartyName,// hdr.SalesVoucherContPerson == null ? hdr.SalesVoucherPartyName : hdr.SalesVoucherContPerson,
                                     SalesmanName = SalesManMasterList.FirstOrDefault(o => o.SalesManMasterSalesManId == hdr.SalesVoucherSalesManID) != null ? SalesManMasterList.FirstOrDefault(o => o.SalesManMasterSalesManId == hdr.SalesVoucherSalesManID).SalesManMasterSalesManName : "",
                                     GrossAmount = hdr.SalesVoucherTotalGrossAmount,
                                     Discount = hdr.SalesVoucherNetDisAmount??0 + hdr.SalesVoucherTotalDisAmount ?? 0,
                                     SalRetAmt = retMstr != null ? retMstr.SalRetAmt : 0,
                                     VATAmount =hdr.SalesVoucherVat_AMT - (retMstr != null ? (decimal)retMstr.SalRetVatAmt : 0),
                                     NetAmount = hdr.SalesVoucherNetAmount - (retMstr != null ? (decimal)retMstr.SalRetAmt : 0),
                                 }).ToList();

                    var data2 = (from hdr in SalesVoucherList
                                 join mstr in SalesReturnMaster on hdr.SalesVoucherNo equals mstr.SalesReturnNo into lmstr
                                 from retMstr in lmstr.DefaultIfEmpty()
                                 where hdr.SalesVoucherNo != null
                                 select new
                                 {
                                     voucherNo = hdr.SalesVoucherNo,
                                     voucherDate = hdr.SalesVoucherDate,
                                     voucherType = hdr.SalesVoucherType,
                                     CustomerName =_customerMasterRepository.GetAsQueryable().FirstOrDefault(x=>x.CustomerMasterCustomerNo == hdr.SalesVoucherPartyId).CustomerMasterCustomerName,//  hdr.SalesVoucherPartyName,// hdr.SalesVoucherContPerson == null ? hdr.SalesVoucherPartyName : hdr.SalesVoucherContPerson,
                                     
                                     SalesmanName = SalesManMasterList.FirstOrDefault(o => o.SalesManMasterSalesManId == hdr.SalesVoucherSalesManID) != null ? SalesManMasterList.FirstOrDefault(o => o.SalesManMasterSalesManId == hdr.SalesVoucherSalesManID).SalesManMasterSalesManName : "",
                                     GrossAmount = hdr.SalesVoucherTotalGrossAmount,
                                     Discount = hdr.SalesVoucherNetDisAmount ?? 0 + hdr.SalesVoucherTotalDisAmount ?? 0,
                                     SalRetAmt = retMstr != null ? retMstr.SalRetAmt : 0,
                                     VATAmount = hdr.SalesVoucherVat_AMT - (retMstr != null ? (decimal)retMstr.SalRetVatAmt : 0),
                                     NetAmount = hdr.SalesVoucherNetAmount - (retMstr != null ? (decimal)retMstr.SalRetAmt : 0),
                                 }).ToList();


                    var finalData = data1.Union(data2).AsQueryable();
                    return finalData;
                }
                else
                {
                    var data = (from hdr in SalesVoucherList
                                join saleman in SalesManMasterList on hdr.SalesVoucherSalesManID equals saleman.SalesManMasterSalesManId into lsmm
                                from smm in lsmm.DefaultIfEmpty()
                                where hdr.SalesVoucherNo != null
                                select new
                                {
                                    SalesmanName = SalesManMasterList.FirstOrDefault(o => o.SalesManMasterSalesManId == hdr.SalesVoucherSalesManID) != null ? SalesManMasterList.FirstOrDefault(o => o.SalesManMasterSalesManId == hdr.SalesVoucherSalesManID).SalesManMasterSalesManName : "",
                                    voucherNo = hdr.SalesVoucherNo,
                                    voucherDate = hdr.SalesVoucherDate,
                                    voucherType = hdr.SalesVoucherType,
                                    CustomerName =_customerMasterRepository.GetAsQueryable().FirstOrDefault(x=>x.CustomerMasterCustomerNo == hdr.SalesVoucherPartyId).CustomerMasterCustomerName,//  hdr.SalesVoucherPartyName,// hdr.SalesVoucherContPerson == null ? hdr.SalesVoucherPartyName : hdr.SalesVoucherContPerson,
                                    GrossAmount = hdr.SalesVoucherTotalGrossAmount,
                                    Discount = hdr.SalesVoucherNetDisAmount??0 + hdr.SalesVoucherTotalDisAmount??0,
                                    SalRetAmt = 0,
                                    VATAmount = hdr.SalesVoucherVat_AMT,
                                    NetAmount = hdr.SalesVoucherNetAmount,
                                }).AsQueryable();
                    return data;
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public IQueryable GetSalesReportDetailWise()
        {
            try
            {
                var data = (from svd in _salesVoucherDetailsRepository.GetAll()
                            join sv in _salesVoucherRepository.GetAll() on svd.SalesVoucherDetailsNo equals sv.SalesVoucherNo into lsv
                            from salesVou in lsv.DefaultIfEmpty()
                            join itemMas in _itemMasterRepository.GetAll() on (long)svd.SalesVoucherDetailsMatId equals itemMas.ItemMasterItemId into litemm
                            from itemmaster in litemm.DefaultIfEmpty()
                            join um in _unitMasterRepository.GetAll() on svd.SalesVoucherDetailsUnitId equals um.UnitMasterUnitId into lum
                            from unitMaster in lum.DefaultIfEmpty()
                            join unitDet in _UnitDetailsRepository.GetAll() on new { UnitId = (int)svd.SalesVoucherDetailsUnitId, ItemId = (int)svd.SalesVoucherDetailsMatId }
                            equals new { UnitId = unitDet.UnitDetailsUnitId, ItemId = (int)unitDet.UnitDetailsItemId } into lud
                            from unitDetail in lud.DefaultIfEmpty()
                            where svd.SalesVoucherDetailsNo != null
                            select new
                            {
                                NetAmount = svd.SalesVoucherDetailsNetAmt,
                                Discount = svd.SalesVoucherDetailsDiscAmount,
                                GrossAmt = svd.SalesVoucherDetailsGrossAmount,
                                //UnitPrice = svd.SalesVoucherDetailsCostPrice,
                                UnitDes = unitMaster.UnitMasterUnitShortName,
                                MatDes = itemmaster.ItemMasterItemName,
                                //PartNo = itemmaster.ItemMasterPartNo,
                                VoucherDate = salesVou.SalesVoucherDate,
                                VoucherNo = salesVou.SalesVoucherNo,
                                QuantitySold = svd.SalesVoucherDetailsQuantity ?? 0,
                                //unitPrice = itemmaster.ItemMasterUnitPrice ?? 0,
                                baseUnit = svd.SalesVoucherDetailsQuantity * (decimal)(unitDetail==null?1:unitDetail.UnitDetailsConversionType ?? 1)   //SD.Sold_Qty * ISNULL(UD.ConversionType, 1)
                                // ItemGroup = 
                            }).AsQueryable();
                return data;
            }
            catch (System.Exception ex)
            {
                throw;
            }
        }

        public POS_Sales_Voucher InsertPosSalesVoucher(POS_Sales_Voucher salesVoucher, List<POS_Sales_Voucher_Details> posSalesVoucherDetails)
        {
            try
            {
                _posSalesVoucherRepository.BeginTransaction();
                string salesVoucherNumber = this.GeneratePosVoucherNo(salesVoucher.VoucherDate).VouchersNumbersVNo;
                salesVoucher.VoucherNo = salesVoucherNumber;


                int maxcount = 0;
                maxcount = Convert.ToInt32(
                    _posSalesVoucherRepository.GetAsQueryable()
                    .DefaultIfEmpty().Max(o => o == null ? 0 : o.V_Id) + 1);

                posSalesVoucherDetails = posSalesVoucherDetails.Select((x) =>
                {
                    x.SNO = maxcount;
                    x.VoucherNo = salesVoucherNumber;
                    return x;
                }).ToList();

                var stockReg = new List<StockRegister>();
                foreach (var item in salesVoucher.posSalesVoucherDetails)
                {
                    stockReg.Add(new StockRegister()
                    {
                        StockRegisterPurchaseID = salesVoucher.VoucherNo,
                        StockRegisterRefVoucherNo = salesVoucher.VoucherNo,
                        StockRegisterVoucherDate = DateTime.Now.Date,
                        StockRegisterMaterialID = Convert.ToInt32(item.ItemId),
                        StockRegisterQuantity = item.Sold_Qty,
                        StockRegisterSIN = 0,
                        StockRegisterSout = item.Sold_Qty,
                        StockRegisterRate = item.UnitPrice,
                        StockRegisterAmount = item.GrossAmt ?? 0,
                        StockRegisterFCAmount = item.GrossAmt ,
                        StockRegisterAssignedDate = DateTime.Now.Date,
                        StockRegisterStatus = "A",
                        StockRegisterTransType = salesVoucher.Voucher_Type,
                        StockRegisterUnitID = Convert.ToInt32(item.UnitId),
                        StockRegisterLocationID = salesVoucher.Location,
                        StockRegisterDepID = Convert.ToInt32(salesVoucher.DeptID),
                        StockRegisterDelStatus = false
                    });
                }
                _stockRegisterRepository.InsertList(stockReg);


                _posSalesVoucherRepository.Insert(salesVoucher);
                _posSalesVoucherDetailsRepository.InsertList(posSalesVoucherDetails);
                _posSalesVoucherRepository.TransactionCommit();
                return this.GetSavedPosSalesVoucherDetails(salesVoucher.VoucherNo);

            }
            catch (Exception ex)
            {
                _posSalesVoucherRepository.TransactionRollback();
                throw;
            }

        }

        public POS_Sales_Voucher GetSavedPosSalesVoucherDetails(string pvno)
        {
            POS_Sales_Voucher salesVoucher = new POS_Sales_Voucher();

            salesVoucher = _posSalesVoucherRepository.GetAsQueryable().Where(k => k.VoucherNo == pvno).SingleOrDefault();
            salesVoucher.posSalesVoucherDetails = _posSalesVoucherDetailsRepository.GetAsQueryable().Where(x => x.VoucherNo == pvno).ToList();


            return salesVoucher;
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

        public POS_Sales_Voucher UpdatePosSalesVoucher(POS_Sales_Voucher salesVoucher, List<POS_Sales_Voucher_Details> PosSalesVoucherDetails)
        {
            try
            {
                _posSalesVoucherRepository.BeginTransaction();
                _posSalesVoucherRepository.Update(salesVoucher);
                _posSalesVoucherDetailsRepository.UpdateList(PosSalesVoucherDetails);
                _posSalesVoucherRepository.TransactionCommit();

                return this.GetSavedPosSalesVoucherDetails(salesVoucher.VoucherNo);
            }
            catch (Exception ex)
            {
                _posSalesVoucherRepository.TransactionRollback();
                throw;
            }
        }

        public IEnumerable<POS_Sales_Voucher> GetPosSalesVoucher()
        {
            var salV = (from s in _posSalesVoucherRepository.GetAsQueryable()
                        join c in _customerMasterRepository.GetAsQueryable() on s.Customer_ID equals c.CustomerMasterCustomerNo
                        select new POS_Sales_Voucher()
                        {
                            V_Id = s.V_Id,
                            VoucherNo = s.VoucherNo,
                            VoucherDate = s.VoucherDate,
                            Voucher_Type = s.Voucher_Type,
                            Customer_ID = c.CustomerMasterCustomerNo,
                            CustomerName = c.CustomerMasterCustomerName,
                            refno = s.refno,
                            DeptID = s.DeptID,
                            Location= s.Location,
                            UserId = s.UserId,
                            CompanyId = s.CompanyId,
                            DiscountPer = s.DiscountPer,
                            Vat_Per = s.Vat_Per,
                            Vat_RoundSign = s.Vat_RoundSign,
                            Vat_RountAmt = s.Vat_RountAmt,
                            NetDiscount = s.NetDiscount,
                            NetAmount = s.NetAmount,
                            Remarks = s.Remarks,
                            ShortNo = s.ShortNo,
                            CashCustName = s.CashCustName,
                            Counter_ID_N = s.Counter_ID_N,
                            Discount = s.Discount,
                            GrossAmount = s.GrossAmount,
                            HOLD_B = s.HOLD_B,
                            WorkPeriodID = s.WorkPeriodID,
                            InvoiceType  = s.InvoiceType,
                            PaymentMode = s.PaymentMode,
                            Vat_TaxableAmt = s.Vat_TaxableAmt,
                            Refrence = s.Refrence,
                            Salesman = s.Salesman,
                            Retail_B = s.Retail_B,
                            VatAmount = s.VatAmount,
                            SV_SM_VoucherNo = s.SV_SM_VoucherNo,
                            Shift_ID_N = s.Shift_ID_N,
                            StationID = s.StationID,
                            SV_NoVat_B = s.SV_NoVat_B,
                            SV_SM_Bal = s.SV_SM_Bal,
                            Vat_AMT = s.Vat_AMT,
                            Vatable_TotAmt = s.Vatable_TotAmt
                        }).ToList();


     
            return salV;
        }

    }
}
