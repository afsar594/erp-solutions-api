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
        private IRepository<CustomerMaster> _salesManMasterRepository;
        private IRepository<DepartmentMaster> _departmentMasterRepository;
        private IRepository<UnitDetails> _UnitDetailsRepository;


        //private IRepository<ReportSalesVoucher> _reportSalesVoucherRepository;
        public SalesVoucherService(
            //IRepository<ReportSalesVoucher> reportSalesVoucherRepository,
            IRepository<CustomerMaster> customerMasterRepository,
            IRepository<CustomerMaster> salesManMasterRepository,
            IRepository<DepartmentMaster> departmentMasterRepository,
            IRepository<ItemMaster> itemMasterRepository, IRepository<UnitMaster> unitMasterRepository,
            IRepository<SuppliersMaster> suppliersMasterRepository, IRepository<LocationMaster> locationMasterRepository,

            IRepository<AccountsTransactions> accountTransactionRepository, IRepository<StockRegister> stockRegisterRepository, IRepository<ProgramSettings> programsettingsRepository,
             IRepository<VouchersNumbers> voucherNumbers, ILogger<PaymentVoucherService> logger,

            IRepository<SalesVoucher> salesVoucherRepository, IRepository<SalesVoucherDetails> salesVoucherDetailsRepository, IRepository<CurrencyMaster> currencyMasterRepository, IRepository<UnitDetails> unitDetailsRepository)
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







        public IEnumerable<CustomerMaster> SalesVoucher_GetAllSalesManMaster()
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
                }).ToList();
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

                accountsTransactions = accountsTransactions.Select((k) =>
                {
                    //k.AccountsTransactionsTransDate = salesVoucher.SalesVoucherDate;
                    k.AccountsTransactionsVoucherNo = salesVoucherNumber;
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
                _stockRegisterRepository.InsertList(stockRegister);





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
        public IEnumerable<AccountsTransactions> GetAllTransaction()
        {
            return _accountTransactionRepository.GetAll();
        }
        public IEnumerable<SalesVoucher> GetSalesVoucher()
        {
            IEnumerable<SalesVoucher> salesVoucher_ALL = _salesVoucherRepository.GetAll().Where(k => k.SalesVoucherDelStatus == false || k.SalesVoucherDelStatus == null).ToList();
            IEnumerable<CurrencyMaster> _currencyMaster = _currencyMasterRepository.GetAll().ToList();

            foreach (var salesVoucher in salesVoucher_ALL)
            {
                salesVoucher.CurrencyMasterCurrencyName = _currencyMaster
                                                             .Where(c => c.CurrencyMasterCurrencyId == salesVoucher.SalesVoucherCurrencyId)
                                                             .FirstOrDefault() != null? _currencyMaster
                                                             .Where(c => c.CurrencyMasterCurrencyId == salesVoucher.SalesVoucherCurrencyId)
                                                             .FirstOrDefault().CurrencyMasterCurrencyName:null;
            }
            return salesVoucher_ALL;
        }
        public SalesVoucherModel GetSavedSalesVoucherDetails(string pvno)
        {
            SalesVoucherModel salesVoucherModel = new SalesVoucherModel();
            //salesVoucherModel.salesVoucher = _salesVoucherRepository.GetAsQueryable().Where(k => k.SalesVoucherNo == pvno && k.SalesVoucherDelStatus == false).SingleOrDefault();

            salesVoucherModel.salesVoucher = _salesVoucherRepository.GetAsQueryable().Where(k => k.SalesVoucherNo == pvno).SingleOrDefault();



            salesVoucherModel.accountsTransactions = _accountTransactionRepository.GetAsQueryable().Where(c => c.AccountsTransactionsVoucherNo == pvno && c.AccountsTransactionsVoucherType == VoucherType.SalesVoucher_TYPE && (c.AccountstransactionsDelStatus == false || c.AccountstransactionsDelStatus == null)).ToList();
            salesVoucherModel.salesVoucherDetails = _salesVoucherDetailsRepository.GetAsQueryable().Where(x => x.SalesVoucherDetailsNo == pvno && (x.SalesVoucherDetailsDelStatus == false || x.SalesVoucherDetailsDelStatus == null)).ToList();
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




    }
}
