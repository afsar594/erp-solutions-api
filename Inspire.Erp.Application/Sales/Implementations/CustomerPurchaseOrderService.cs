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
    public class CustomerPurchaseOrderService : ICustomerPurchaseOrderService
    {
        private IRepository<StockRegister> _stockRegisterRepository;
        private IRepository<AccountsTransactions> _accountTransactionRepository;
        private IRepository<CustomerPurchaseOrder> _customerPurchaseOrderRepository;
        private IRepository<CustomerPurchaseOrderDetails> _customerPurchaseOrderDetailsRepository;
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
        public readonly InspireErpDBContext _Context;
        //private IRepository<ReportSalesVoucher> _reportSalesVoucherRepository;
        public CustomerPurchaseOrderService(
            //IRepository<ReportSalesVoucher> reportSalesVoucherRepository,
            IRepository<CustomerMaster> customerMasterRepository,
            IRepository<CustomerMaster> salesManMasterRepository,
            IRepository<DepartmentMaster> departmentMasterRepository,
            IRepository<ItemMaster> itemMasterRepository, IRepository<UnitMaster> unitMasterRepository,
            IRepository<SuppliersMaster> suppliersMasterRepository, IRepository<LocationMaster> locationMasterRepository,

            IRepository<AccountsTransactions> accountTransactionRepository, IRepository<StockRegister> stockRegisterRepository, IRepository<ProgramSettings> programsettingsRepository,
             IRepository<VouchersNumbers> voucherNumbers, ILogger<PaymentVoucherService> logger,

            IRepository<CustomerPurchaseOrder> customerPurchaseOrderRepository, IRepository<CustomerPurchaseOrderDetails> customerPurchaseOrderDetailsRepository, InspireErpDBContext Context)
        {
            this._accountTransactionRepository = accountTransactionRepository;
            this._stockRegisterRepository = stockRegisterRepository;
            this._customerPurchaseOrderRepository = customerPurchaseOrderRepository;
            this._customerPurchaseOrderDetailsRepository = customerPurchaseOrderDetailsRepository;
            _programsettingsRepository = programsettingsRepository;
            _voucherNumbersRepository = voucherNumbers;
            _Context = Context;

            _itemMasterRepository = itemMasterRepository;
            _unitMasterRepository = unitMasterRepository;
            _suppliersMasterRepository = suppliersMasterRepository;
            _locationMasterRepository = locationMasterRepository;


            _customerMasterRepository = customerMasterRepository;

            _salesManMasterRepository = salesManMasterRepository;

            _departmentMasterRepository = departmentMasterRepository;

            //_reportSalesVoucherRepository = reportSalesVoucherRepository;
        }



        //public IEnumerable<LocationMaster> SalesVoucher_GetAllLocationMaster()
        //{
        //    return _locationMasterRepository.GetAll();
        //}



        //public IEnumerable<SuppliersMaster> SalesVoucher_GetAllSuppliersMaster()
        //{
        //    return _suppliersMasterRepository.GetAll();
        //}


        //public IEnumerable<DepartmentMaster> SalesVoucher_GetAllDepartmentMaster()
        //{
        //    return _departmentMasterRepository.GetAll();
        //}




        //public IEnumerable<CustomerMaster> SalesVoucher_GetAllCustomerMaster()
        //{
        //    return _customerMasterRepository.GetAll();
        //}







        //public IEnumerable<CustomerMaster> SalesVoucher_GetAllSalesManMaster()
        //{
        //    return _salesManMasterRepository.GetAll();
        //}


        //public IEnumerable<UnitMaster> SalesVoucher_GetAllUnitMaster()
        //{
        //    return _unitMasterRepository.GetAll();
        //}
        //public IEnumerable<ItemMaster> SalesVoucher_GetAllItemMaster()
        //{
        //    return _itemMasterRepository.GetAll();
        //}

        public CustomerPurchaseOrderModel UpdateSalesOrder(CustomerPurchaseOrder customerPurchaseOrder, List<AccountsTransactions> accountsTransactions,
            List<CustomerPurchaseOrderDetails> customerPurchaseOrderDetails
           , List<StockRegister> stockRegister
            )
        {

            try
            {
                _customerPurchaseOrderRepository.BeginTransaction();

                //=================================
                clsCommonFunctions.Delete_OldEntry_StockRegister(customerPurchaseOrder.CustomerPurchaseOrderVoucherNo, VoucherType.CustomerPurchaseOrder_TYPE, _stockRegisterRepository);
                clsCommonFunctions.Delete_OldEntry_AccountsTransactions(customerPurchaseOrder.CustomerPurchaseOrderVoucherNo, VoucherType.CustomerPurchaseOrder_TYPE, _accountTransactionRepository);
                //=================================
                customerPurchaseOrder.CustomerPurchaseOrderDetails = customerPurchaseOrder.CustomerPurchaseOrderDetails.Select((k) =>
                {
                    //if (k.SalesVoucherDetailsId == 0)
                    //{
                    k.CustomerPurchaseOrderDetailsOrderId = customerPurchaseOrder.CustomerPurchaseOrderId;
                    k.CustomerPurchaseOrderDetailsVoucherNo = customerPurchaseOrder.CustomerPurchaseOrderVoucherNo;
                    //k.SalesVoucherDetailsId = 0;
                    //}

                    return k;
                }).ToList();

                _customerPurchaseOrderRepository.Update(customerPurchaseOrder);





                //_customerPurchaseOrderRepository.Update(customerPurchaseOrder);
                accountsTransactions = accountsTransactions.Select((k) =>
                {
                    if (k.AccountsTransactionsTransSno == 0)
                    {
                        k.AccountsTransactionsTransDate = customerPurchaseOrder.CustomerPurchaseOrderDate;
                        k.AccountsTransactionsVoucherNo = customerPurchaseOrder.CustomerPurchaseOrderVoucherNo;
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
                        k.StockRegisterVoucherDate = customerPurchaseOrder.CustomerPurchaseOrderDate;
                        k.StockRegisterRefVoucherNo = customerPurchaseOrder.CustomerPurchaseOrderVoucherNo;
                        k.StockRegisterTransType = VoucherType.CustomerPurchaseOrder_TYPE;
                        k.StockRegisterStatus = AccountStatus.Approved;
                    }

                    return k;
                }).ToList();
                _stockRegisterRepository.UpdateList(stockRegister);
                _customerPurchaseOrderRepository.TransactionCommit();

            }
            catch (Exception ex)
            {
                _customerPurchaseOrderRepository.TransactionRollback();
                throw ex;
            }

            return this.GetSavedCustomerPurchaseOrderDetails(customerPurchaseOrder.CustomerPurchaseOrderVoucherNo);
        }

        public int DeleteSalesOrder(CustomerPurchaseOrder customerPurchaseOrder, List<AccountsTransactions> accountsTransactions,
            List<CustomerPurchaseOrderDetails> customerPurchaseOrderDetails
            , List<StockRegister> stockRegister
            )
        {
            int i = 0;
            try
            {
                _customerPurchaseOrderRepository.BeginTransaction();

                //=================================
                clsCommonFunctions.Delete_OldEntry_StockRegister(customerPurchaseOrder.CustomerPurchaseOrderVoucherNo, VoucherType.SalesVoucher_TYPE, _stockRegisterRepository);
                clsCommonFunctions.Delete_OldEntry_AccountsTransactions(customerPurchaseOrder.CustomerPurchaseOrderVoucherNo, VoucherType.SalesVoucher_TYPE, _accountTransactionRepository);
                //=================================


                customerPurchaseOrder.CustomerPurchaseOrderDelStatus = true;

                customerPurchaseOrder.CustomerPurchaseOrderDetails = customerPurchaseOrder.CustomerPurchaseOrderDetails.Select((k) =>
                {
                    k.CustomerPurchaseOrderDetailsDelStatus = true;
                    return k;
                }).ToList();

                //_customerPurchaseOrderDetailsRepository.UpdateList(customerPurchaseOrderDetails);

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




                //customerPurchaseOrder.SalesVoucherDetails = customerPurchaseOrderDetails;

                _customerPurchaseOrderRepository.Update(customerPurchaseOrder);

                var vchrnumer = _voucherNumbersRepository.GetAsQueryable().Where(k => k.VouchersNumbersVNo == customerPurchaseOrder.CustomerPurchaseOrderVoucherNo).FirstOrDefault();

                _voucherNumbersRepository.Update(vchrnumer);

                _customerPurchaseOrderRepository.TransactionCommit();
                i = 1;
            }
            catch (Exception ex)
            {
                _customerPurchaseOrderRepository.TransactionRollback();
                i = 0;
                throw ex;
            }

            return i;

        }
        public CustomerPurchaseOrderModel InsertSalesOrder(CustomerPurchaseOrder customerPurchaseOrder, List<AccountsTransactions> accountsTransactions,
            List<CustomerPurchaseOrderDetails> customerPurchaseOrderDetails
            , List<StockRegister> stockRegister
            )
        {
            try
            {
                _customerPurchaseOrderRepository.BeginTransaction();
                string customerPurchaseOrderNumber = this.GenerateVoucherNo(customerPurchaseOrder.CustomerPurchaseOrderDate).VouchersNumbersVNo;
                customerPurchaseOrder.CustomerPurchaseOrderVoucherNo = customerPurchaseOrderNumber;


                int maxcount = 0;
                maxcount = Convert.ToInt32(
                    _customerPurchaseOrderRepository.GetAsQueryable()
                    .DefaultIfEmpty().Max(o => o == null ? 0 : o.CustomerPurchaseOrderId) + 1);

                customerPurchaseOrder.CustomerPurchaseOrderId = maxcount;

                customerPurchaseOrder.CustomerPurchaseOrderDetails = customerPurchaseOrder.CustomerPurchaseOrderDetails.Select((x) =>
                {
                    x.CustomerPurchaseOrderDetailsOrderId = maxcount;
                    x.CustomerPurchaseOrderDetailsVoucherNo = customerPurchaseOrderNumber;
                    return x;
                }).ToList();
                //_customerPurchaseOrderDetailsRepository.InsertList(customerPurchaseOrderDetails);

                accountsTransactions = accountsTransactions.Select((k) =>
                {
                    //k.AccountsTransactionsTransDate = customerPurchaseOrder.SalesVoucherDate;
                    k.AccountsTransactionsVoucherNo = customerPurchaseOrderNumber;
                    k.AccountsTransactionsVoucherType = VoucherType.CustomerPurchaseOrder_TYPE;
                    k.AccountsTransactionsStatus = AccountStatus.Approved;
                    return k;
                }).ToList();
                _accountTransactionRepository.InsertList(accountsTransactions);


                //List<StockRegister> sr = new List<StockRegister>();

                //foreach (var item in customerPurchaseOrderDetails)
                //{
                //    sr.Add(new StockRegister()
                //    {
                //        StockRegisterRefVoucherNo = customerPurchaseOrderNumber,
                //        StockRegisterAssignedDate = DateTime.Now,
                //        StockRegisterTransType = VoucherType.SalesVoucher_TYPE,
                //        StockRegisterStatus = AccountStatus.Approved,
                //        StockRegisterVoucherDate = customerPurchaseOrder.CustomerPurchaseOrderDate\,
                //        StockRegisterSout = item.SalesVoucherDetailsQuantity,
                //        StockRegisterMaterialID = item.SalesVoucherDetailsMatId,
                //    });
                //}
                //stockRegister = stockRegister.Select((k) =>
                //{
                //   k.StockRegisterVoucherDate = customerPurchaseOrder.SalesVoucherDate;
                //   k.StockRegisterRefVoucherNo = customerPurchaseOrderNumber;
                //   k.StockRegisterTransType = VoucherType.SalesVoucher_TYPE;
                //   k.StockRegisterStatus = AccountStatus.Approved;
                //    k.StockRegisterSout = 
                //   return k;
                //}).ToList();
               // _stockRegisterRepository.InsertList(sr);





                _customerPurchaseOrderRepository.Insert(customerPurchaseOrder);
                _customerPurchaseOrderRepository.TransactionCommit();
                return this.GetSavedCustomerPurchaseOrderDetails(customerPurchaseOrder.CustomerPurchaseOrderVoucherNo);

            }
            catch (Exception ex)
            {
                _customerPurchaseOrderRepository.TransactionRollback();
                throw ex;
            }

        }
        public IEnumerable<AccountsTransactions> GetAllTransaction()
        {
            return _accountTransactionRepository.GetAll();
        }
        public IEnumerable<CustomerPurchaseOrder> GetCustomerPurchaseOrders()
        {
            IEnumerable<CustomerPurchaseOrder> customerPurchaseOrder_ALL = _customerPurchaseOrderRepository.GetAll().Where(k => k.CustomerPurchaseOrderDelStatus == false || k.CustomerPurchaseOrderDelStatus == null);
            return customerPurchaseOrder_ALL;
        }
        public CustomerPurchaseOrderModel GetSavedCustomerPurchaseOrderDetails(string pvno)
        {
            CustomerPurchaseOrderModel customerPurchaseOrderModel = new CustomerPurchaseOrderModel();
            //customerPurchaseOrderModel.customerPurchaseOrder = _customerPurchaseOrderRepository.GetAsQueryable().Where(k => k.SalesVoucherNo == pvno && k.SalesVoucherDelStatus == false).SingleOrDefault();

            customerPurchaseOrderModel.customerPurchaseOrder = _customerPurchaseOrderRepository.GetAsQueryable().Where(k => k.CustomerPurchaseOrderVoucherNo == pvno).SingleOrDefault();



            customerPurchaseOrderModel.accountsTransactions = _accountTransactionRepository.GetAsQueryable().Where(c => c.AccountsTransactionsVoucherNo == pvno && c.AccountsTransactionsVoucherType == VoucherType.SalesVoucher_TYPE && (c.AccountstransactionsDelStatus == false || c.AccountstransactionsDelStatus == null)).ToList();
            customerPurchaseOrderModel.customerPurchaseOrderDetails = _customerPurchaseOrderDetailsRepository.GetAsQueryable().Where(x => x.CustomerPurchaseOrderDetailsVoucherNo == pvno && (x.CustomerPurchaseOrderDetailsDelStatus == false || x.CustomerPurchaseOrderDetailsDelStatus == null)).ToList();
            return customerPurchaseOrderModel;





        }
        public VouchersNumbers GenerateVoucherNo(DateTime? VoucherGenDate)
        {
            try
            {

                var prefix = this._programsettingsRepository.GetAsQueryable().Where(k => k.ProgramSettingsKeyValue == Prefix.CustomerPurchaseOrder_Prefix).FirstOrDefault().ProgramSettingsTextValue;
                int vnoMaxVal = Convert.ToInt32(_voucherNumbersRepository.GetAsQueryable()
                                                        .Where(x => x.VouchersNumbersVNoNu > 0 && x.VouchersNumbersVType == VoucherType.CustomerPurchaseOrder_TYPE)
                                                        .DefaultIfEmpty()
                                                        .Max(o => o == null ? 0 : o.VouchersNumbersVNoNu)) + 1;


                //var prefix = "CN";
                //int vnoMaxVal = 1;


                VouchersNumbers vouchersNumbers = new VouchersNumbers
                {
                    VouchersNumbersVNo = prefix + vnoMaxVal,
                    VouchersNumbersVNoNu = vnoMaxVal,
                    VouchersNumbersVType = VoucherType.CustomerPurchaseOrder_TYPE,
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
                var Details = _Context.SalesVoucherDetails.Where(o => o.SalesVoucherDetailsNo == SalesVoucherNo).AsQueryable();
                return Details;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
    }
}
