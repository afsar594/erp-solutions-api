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
using Inspire.Erp.Domain.Modals;
using Inspire.Erp.Domain.Modals.Common;

using Inspire.Erp.Infrastructure.Database;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.AspNetCore.Mvc.ApplicationModels;


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
        private readonly IRepository<SalesVoucher> _salesVoucherRepo;
        private readonly IRepository<SalesVoucherDetails> _salesVoucherDetailRepo;
        private readonly ILogger<PaymentVoucherService> _logger;
        private IRepository<ItemMaster> _itemMasterRepository;
        private IRepository<UnitMaster> _unitMasterRepository;
        private IRepository<SuppliersMaster> _suppliersMasterRepository;
        private IRepository<LocationMaster> _locationMasterRepository;
        private IRepository<CustomerMaster> _customerMasterRepository;
        private IRepository<CustomerMaster> _salesManMasterRepository;
        private IRepository<DepartmentMaster> _departmentMasterRepository;
        public readonly InspireErpDBContext _Context;
        private IRepository<TrackingRegister> _trackingRegisterRepository;
        private IRepository<PurOrderRegister> _purOrderRegisterRepository;
        private IRepository<CustomerOrderRegister> _custOrderRegRepository;

        public CustomerPurchaseOrderService(
            //IRepository<ReportSalesVoucher> reportSalesVoucherRepository,
            IRepository<CustomerMaster> customerMasterRepository,
            IRepository<CustomerMaster> salesManMasterRepository,
            IRepository<DepartmentMaster> departmentMasterRepository,
            IRepository<ItemMaster> itemMasterRepository, IRepository<UnitMaster> unitMasterRepository,
            IRepository<SuppliersMaster> suppliersMasterRepository, IRepository<LocationMaster> locationMasterRepository,
            IRepository<AccountsTransactions> accountTransactionRepository, IRepository<StockRegister> stockRegisterRepository, IRepository<ProgramSettings> programsettingsRepository,
             IRepository<VouchersNumbers> voucherNumbers, ILogger<PaymentVoucherService> logger,
            IRepository<CustomerPurchaseOrder> customerPurchaseOrderRepository, IRepository<CustomerPurchaseOrderDetails> customerPurchaseOrderDetailsRepository,
            IRepository<TrackingRegister> trackingRegisterRepository, IRepository<PurOrderRegister> purOrderRegisterRepository,
            IRepository<CustomerOrderRegister> custOrderRegRepository, IRepository<SalesVoucher> salesVoucherRepo, IRepository<SalesVoucherDetails> salesVoucherDetailRepo,
            InspireErpDBContext Context)
        {
            this._accountTransactionRepository = accountTransactionRepository;
            this._stockRegisterRepository = stockRegisterRepository;
            this._customerPurchaseOrderRepository = customerPurchaseOrderRepository;
            this._customerPurchaseOrderDetailsRepository = customerPurchaseOrderDetailsRepository;
            _programsettingsRepository = programsettingsRepository;
            _voucherNumbersRepository = voucherNumbers;
            _Context = Context;
            _salesVoucherDetailRepo = salesVoucherDetailRepo;
            _salesVoucherRepo = salesVoucherRepo;
            _itemMasterRepository = itemMasterRepository;
            _unitMasterRepository = unitMasterRepository;
            _suppliersMasterRepository = suppliersMasterRepository;
            _locationMasterRepository = locationMasterRepository;
            _customerMasterRepository = customerMasterRepository;
            _salesManMasterRepository = salesManMasterRepository;
            _departmentMasterRepository = departmentMasterRepository;
            _trackingRegisterRepository = trackingRegisterRepository;
            _purOrderRegisterRepository = purOrderRegisterRepository;
            _custOrderRegRepository = custOrderRegRepository;
        }
        public CustomerPurchaseOrderModel UpdateSalesOrder(CustomerPurchaseOrder customerPurchaseOrder, List<AccountsTransactions> accountsTransactions,
            List<CustomerPurchaseOrderDetails> customerPurchaseOrderDetails
           , List<StockRegister> stockRegister
            )
        {

            try
            {
                _customerPurchaseOrderRepository.BeginTransaction();

                var deleteTrackingRegister = _trackingRegisterRepository.GetAsQueryable().Where(x => x.TrackingRegisterVoucherType == VoucherType.CustomerPurchaseOrder_TYPE && x.TrackingRegisterVoucherNo == customerPurchaseOrder.CustomerPurchaseOrderVoucherNo).FirstOrDefault();
                _trackingRegisterRepository.Delete(deleteTrackingRegister);
                _trackingRegisterRepository.SaveChangesAsync();

                var deletePurOrderRegister = _purOrderRegisterRepository.GetAsQueryable().Where(x => x.PurOrderRegisterTransType == VoucherType.CustomerPurchaseOrder_TYPE && x.PurOrderRegisterRefVoucherNo == customerPurchaseOrder.CustomerPurchaseOrderVoucherNo).FirstOrDefault();
                _purOrderRegisterRepository.Delete(deletePurOrderRegister);
                _purOrderRegisterRepository.SaveChangesAsync();

                var deletecustOrderRegister = _custOrderRegRepository.GetAsQueryable().Where(x => x.CustomerOrderRegisterTransType == VoucherType.CustomerPurchaseOrder_TYPE && x.CustomerOrderRegisterRefVoucherNo == customerPurchaseOrder.CustomerPurchaseOrderVoucherNo).FirstOrDefault();
                _custOrderRegRepository.Delete(deletecustOrderRegister);
                _custOrderRegRepository.SaveChangesAsync();

                //=================================
                clsCommonFunctions.Delete_OldEntry_StockRegister(customerPurchaseOrder.CustomerPurchaseOrderVoucherNo, VoucherType.CustomerPurchaseOrder_TYPE, _stockRegisterRepository);
                clsCommonFunctions.Delete_OldEntry_AccountsTransactions(customerPurchaseOrder.CustomerPurchaseOrderVoucherNo, VoucherType.CustomerPurchaseOrder_TYPE, _accountTransactionRepository);
                //=================================
                customerPurchaseOrder.CustomerPurchaseOrderDetails = customerPurchaseOrder.CustomerPurchaseOrderDetails.Select((k) =>
                {
                    k.CustomerPurchaseOrderId = customerPurchaseOrder.CustomerPurchaseOrderId;
                    k.CustomerPurchaseOrderDetailsVoucherNo = customerPurchaseOrder.CustomerPurchaseOrderVoucherNo;
                    return k;
                }).OrderBy(x => x.CustomerPurchaseOrderDetailsVoucherNo).AsEnumerable().ToList();

                _customerPurchaseOrderRepository.Update(customerPurchaseOrder);

                accountsTransactions = accountsTransactions.Select((k) =>
                {
                    if (k.AccountsTransactionsTransSno == 0)
                    {
                        k.AccountsTransactionsTransDate = customerPurchaseOrder.CustomerPurchaseOrderDate;
                        k.AccountsTransactionsVoucherNo = customerPurchaseOrder.CustomerPurchaseOrderVoucherNo;
                        k.AccountsTransactionsVoucherType = VoucherType.CustomerPurchaseOrder_TYPE;
                        k.AccountsTransactionsStatus = AccountStatus.Approved;
                    }

                    return k;
                }).OrderBy(x => x.AccountsTransactionsVoucherNo).AsEnumerable().ToList();
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
                }).OrderBy(x => x.StockRegisterRefVoucherNo).AsEnumerable().ToList();
                _stockRegisterRepository.UpdateList(stockRegister);

                //insert tracking register
                var listTrackRegs = new List<TrackingRegister>();
                var listTrackPurchOrderReg = new List<PurOrderRegister>();
                var listCustOrderRegisterReg = new List<CustomerOrderRegister>();
                foreach (var purOrderDet in customerPurchaseOrder.CustomerPurchaseOrderDetails)
                {
                    var tr = new TrackingRegister
                    {
                        TrackingRegisterDetailsId = (int)purOrderDet.CustomerPurchaseOrderDetailsId,
                        TrackingRegisterVoucherNo = purOrderDet.CustomerPurchaseOrderDetailsVoucherNo,
                        TrackingRegisterMaterialId = (int)purOrderDet.CustomerPurchaseOrderDetailsItemId,
                        TrackingRegisterVoucherType = VoucherType.CustomerPurchaseOrder_TYPE,
                        TrackingRegisterQty = (int)purOrderDet.CustomerPurchaseOrderDetailsQty,
                        TrackingRegisterQtyin = (int)purOrderDet.CustomerPurchaseOrderDetailsQty,
                        TrackingRegisterQtyout = 0,
                        TrackingRegisterRate = purOrderDet.CustomerPurchaseOrderDetailsAmount,
                        TrackingRegisterTrackDate = customerPurchaseOrder.CustomerPurchaseOrderDate,
                        TrackingRegisterFsno = purOrderDet.CustomerPurchaseOrderDetailsFsno,
                    };
                    listTrackRegs.Add(tr);

                    var poReg = new PurOrderRegister
                    {
                        PurOrderRegisterOrderNo = purOrderDet.CustomerPurchaseOrderDetailsId.ToString(),
                        PurOrderRegisterRefVoucherNo = purOrderDet.CustomerPurchaseOrderDetailsVoucherNo,
                        PurOrderRegisterMaterialId = (int)purOrderDet.CustomerPurchaseOrderDetailsItemId,
                        PurOrderRegisterTransType = VoucherType.CustomerPurchaseOrder_TYPE,
                        PurOrderRegisterQtyOrder = purOrderDet.CustomerPurchaseOrderDetailsQty,
                        PurOrderRegisterQtyIssued = 0,
                        PurOrderRegisterRate = purOrderDet.CustomerPurchaseOrderDetailsAmount,
                        PurOrderRegisterAmount = purOrderDet.CustomerPurchaseOrderDetailsAmount,
                        PurOrderRegisterFcAmount = purOrderDet.CustomerPurchaseOrderDetailsFcAmount,
                        PurOrderRegisterAssignedDate = DateTime.Now,
                        PurOrderRegisterFsno = purOrderDet.CustomerPurchaseOrderDetailsFsno,//to be alloted,
                        PurOrderRegisterSupplierId = (int)0,
                        PurOrderRegisterStatus = "A"
                    };
                    listTrackPurchOrderReg.Add(poReg);

                    var custOrdReg = new CustomerOrderRegister
                    {
                        CustomerOrderRegisterRefVoucherNo = purOrderDet.CustomerPurchaseOrderDetailsVoucherNo,
                        CustomerOrderRegisterOrderNo = purOrderDet.CustomerPurchaseOrderDetailsId.ToString(),
                        CustomerOrderRegisterMaterialId = (int)purOrderDet.CustomerPurchaseOrderDetailsItemId,
                        CustomerOrderRegisterTransType = VoucherType.CustomerPurchaseOrder_TYPE,
                        CustomerOrderRegisterQtyOrder = (int)purOrderDet.CustomerPurchaseOrderDetailsQty,
                        CustomerOrderRegisterQtyIssued = 0,
                        CustomerOrderRegisterRate = purOrderDet.CustomerPurchaseOrderDetailsAmount,
                        CustomerOrderRegisterAssignedDate = DateTime.Now,
                        CustomerOrderRegisterFsno = purOrderDet.CustomerPurchaseOrderDetailsFsno,//to be alloted,
                        CustomerOrderRegisterCustomerId = (int)0,
                        CustomerOrderRegisterStatus = "A"
                    };
                    listCustOrderRegisterReg.Add(custOrdReg);

                }

                _trackingRegisterRepository.InsertList(listTrackRegs);
                _purOrderRegisterRepository.InsertList(listTrackPurchOrderReg);
                _custOrderRegRepository.InsertList(listCustOrderRegisterReg);
                _customerPurchaseOrderRepository.TransactionCommit();

            }
            catch (Exception ex)
            {
                _customerPurchaseOrderRepository.TransactionRollback();
                throw ex;
            }
            return this.GetSavedCustomerPurchaseOrderDetails((int)customerPurchaseOrder.CustomerPurchaseOrderId);
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
                clsCommonFunctions.Delete_OldEntry_StockRegister(customerPurchaseOrder.CustomerPurchaseOrderVoucherNo, VoucherType.CustomerPurchaseOrder_TYPE, _stockRegisterRepository);
                clsCommonFunctions.Delete_OldEntry_AccountsTransactions(customerPurchaseOrder.CustomerPurchaseOrderVoucherNo, VoucherType.CustomerPurchaseOrder_TYPE, _accountTransactionRepository);
                //=================================
                customerPurchaseOrder.CustomerPurchaseOrderDelStatus = true;

                customerPurchaseOrder.CustomerPurchaseOrderDetails = customerPurchaseOrder.CustomerPurchaseOrderDetails.Select((k) =>
                {
                    k.CustomerPurchaseOrderDetailsDelStatus = true;
                    return k;
                }).OrderBy(x => x.CustomerPurchaseOrderDetailsVoucherNo).AsEnumerable().ToList();

                accountsTransactions = accountsTransactions.Select((k) =>
                {
                    k.AccountstransactionsDelStatus = true;
                    return k;
                }).OrderBy(x => x.AccountsTransactionsVoucherNo).AsEnumerable().ToList();
                _accountTransactionRepository.UpdateList(accountsTransactions);

                stockRegister = stockRegister.Select((k) =>
                {
                    k.StockRegisterDelStatus = true;
                    return k;
                }).OrderBy(x => x.StockRegisterRefVoucherNo).AsEnumerable().ToList();
                _stockRegisterRepository.UpdateList(stockRegister);
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
            , List<StockRegister> stockRegister)
        {
            try
            {
                _customerPurchaseOrderRepository.BeginTransaction();
                string customerPurchaseOrderNumber = this.GenerateVoucherNo(customerPurchaseOrder.CustomerPurchaseOrderDate).VouchersNumbersVNo;
                customerPurchaseOrder.CustomerPurchaseOrderVoucherNo = customerPurchaseOrderNumber;
                customerPurchaseOrder.CustomerPurchaseOrderDate = DateTime.Now;

                int maxcount = 0;
                maxcount = Convert.ToInt32(
                    _customerPurchaseOrderRepository.GetAsQueryable()
                    .DefaultIfEmpty().Max(o => o == null ? 0 : o.CustomerPurchaseOrderId) + 1);
                customerPurchaseOrder.CustomerPurchaseOrderId = maxcount;

                customerPurchaseOrder.CustomerPurchaseOrderDetails = customerPurchaseOrder.CustomerPurchaseOrderDetails.Select((x) =>
                {
                    x.CustomerPurchaseOrderId = maxcount;
                    x.CustomerPurchaseOrderDetailsVoucherNo = customerPurchaseOrderNumber;
                    return x;
                }).OrderBy(x => x.CustomerPurchaseOrderDetailsVoucherNo).AsEnumerable().ToList();

                accountsTransactions = accountsTransactions.Select((k) =>
                {
                    k.AccountsTransactionsVoucherNo = customerPurchaseOrderNumber;
                    k.AccountsTransactionsVoucherType = VoucherType.CustomerPurchaseOrder_TYPE;
                    k.AccountsTransactionsStatus = AccountStatus.Approved;
                    return k;
                }).OrderBy(x => x.AccountsTransactionsVoucherNo).AsEnumerable().ToList();
                _accountTransactionRepository.InsertList(accountsTransactions);
                _customerPurchaseOrderRepository.Insert(customerPurchaseOrder);

                //insert tracking register
                var listTrackRegs = new List<TrackingRegister>();
                var listTrackPurchOrderReg = new List<PurOrderRegister>();
                var listCustOrderRegisterReg = new List<CustomerOrderRegister>();
                foreach (var purOrderDet in customerPurchaseOrder.CustomerPurchaseOrderDetails)
                {
                    var tr = new TrackingRegister
                    {
                        TrackingRegisterDetailsId = (int)purOrderDet.CustomerPurchaseOrderDetailsId,
                        TrackingRegisterVoucherNo = purOrderDet.CustomerPurchaseOrderDetailsVoucherNo,
                        TrackingRegisterMaterialId = (int)purOrderDet.CustomerPurchaseOrderDetailsItemId,
                        TrackingRegisterVoucherType = VoucherType.CustomerPurchaseOrder_TYPE,
                        TrackingRegisterQty = (int)purOrderDet.CustomerPurchaseOrderDetailsQty,
                        TrackingRegisterQtyin = (int)purOrderDet.CustomerPurchaseOrderDetailsQty,
                        TrackingRegisterQtyout = 0,
                        TrackingRegisterRate = purOrderDet.CustomerPurchaseOrderDetailsAmount,
                        TrackingRegisterTrackDate = customerPurchaseOrder.CustomerPurchaseOrderDate,
                        TrackingRegisterFsno = purOrderDet.CustomerPurchaseOrderDetailsFsno,
                    };
                    listTrackRegs.Add(tr);

                    var poReg = new PurOrderRegister
                    {
                        PurOrderRegisterOrderNo = purOrderDet.CustomerPurchaseOrderDetailsId.ToString(),
                        PurOrderRegisterRefVoucherNo = purOrderDet.CustomerPurchaseOrderDetailsVoucherNo,
                        PurOrderRegisterMaterialId = (int)purOrderDet.CustomerPurchaseOrderDetailsItemId,
                        PurOrderRegisterTransType = VoucherType.CustomerPurchaseOrder_TYPE,
                        PurOrderRegisterQtyOrder = purOrderDet.CustomerPurchaseOrderDetailsQty,
                        PurOrderRegisterQtyIssued = 0,
                        PurOrderRegisterRate = purOrderDet.CustomerPurchaseOrderDetailsAmount,
                        PurOrderRegisterAmount = purOrderDet.CustomerPurchaseOrderDetailsAmount,
                        PurOrderRegisterFcAmount = purOrderDet.CustomerPurchaseOrderDetailsFcAmount,
                        PurOrderRegisterAssignedDate = DateTime.Now,
                        PurOrderRegisterFsno = purOrderDet.CustomerPurchaseOrderDetailsFsno,//to be alloted,
                        PurOrderRegisterSupplierId = (int)0,
                        PurOrderRegisterStatus = "A"
                    };
                    listTrackPurchOrderReg.Add(poReg);

                    var custOrdReg = new CustomerOrderRegister
                    {
                        CustomerOrderRegisterRefVoucherNo = purOrderDet.CustomerPurchaseOrderDetailsVoucherNo,
                        CustomerOrderRegisterOrderNo = purOrderDet.CustomerPurchaseOrderDetailsId.ToString(),
                        CustomerOrderRegisterMaterialId = (int)purOrderDet.CustomerPurchaseOrderDetailsItemId,
                        CustomerOrderRegisterTransType = VoucherType.CustomerPurchaseOrder_TYPE,
                        CustomerOrderRegisterQtyOrder = (int)purOrderDet.CustomerPurchaseOrderDetailsQty,
                        CustomerOrderRegisterQtyIssued = 0,
                        CustomerOrderRegisterRate = purOrderDet.CustomerPurchaseOrderDetailsAmount,
                        CustomerOrderRegisterAssignedDate = DateTime.Now,
                        CustomerOrderRegisterFsno = purOrderDet.CustomerPurchaseOrderDetailsFsno,//to be alloted,
                        CustomerOrderRegisterCustomerId = (int)0,
                        CustomerOrderRegisterStatus = "A"
                    };
                    listCustOrderRegisterReg.Add(custOrdReg);
                }
                _trackingRegisterRepository.InsertList(listTrackRegs);
                _purOrderRegisterRepository.InsertList(listTrackPurchOrderReg);
                _custOrderRegRepository.InsertList(listCustOrderRegisterReg);
                _customerPurchaseOrderRepository.TransactionCommit();
                return this.GetSavedCustomerPurchaseOrderDetails((int)customerPurchaseOrder.CustomerPurchaseOrderId);
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
        public CustomerPurchaseOrderModel GetSavedCustomerPurchaseOrderDetails(int id)
        {
            CustomerPurchaseOrderModel customerPurchaseOrderModel = new CustomerPurchaseOrderModel();
            customerPurchaseOrderModel.customerPurchaseOrder = _customerPurchaseOrderRepository.GetAsQueryable().Where(k => k.CustomerPurchaseOrderId == id).SingleOrDefault();
            customerPurchaseOrderModel.accountsTransactions = _accountTransactionRepository.GetAsQueryable().Where(c => c.AccountsTransactionsVoucherNo == customerPurchaseOrderModel.customerPurchaseOrder.CustomerPurchaseOrderVoucherNo && c.AccountsTransactionsVoucherType == VoucherType.CustomerPurchaseOrder_TYPE && (c.AccountstransactionsDelStatus == false || c.AccountstransactionsDelStatus == null)).OrderBy(x => x.AccountsTransactionsVoucherNo).AsEnumerable().ToList();
            customerPurchaseOrderModel.customerPurchaseOrderDetails = _customerPurchaseOrderDetailsRepository.GetAsQueryable().Where(x => x.CustomerPurchaseOrderDetailsVoucherNo == customerPurchaseOrderModel.customerPurchaseOrder.CustomerPurchaseOrderVoucherNo && (x.CustomerPurchaseOrderDetailsDelStatus == false || x.CustomerPurchaseOrderDetailsDelStatus == null)).OrderBy(x => x.CustomerPurchaseOrderDetailsVoucherNo).AsEnumerable().ToList();
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
        public async Task<Response<List<GetCustomerPurchaseOrderTrackingResponse>>> GetCustomerPurchaseOrderTracking(CustPurchOrderFilterModel model)
        {
            try
            {
                List<GetCustomerPurchaseOrderTrackingResponse> response = new List<GetCustomerPurchaseOrderTrackingResponse>();

                var subqueryDRQD = (from s in _Context.CustomerDeliveryNoteDetails
                                    join d in _Context.CustomerDeliveryNote on s.CustomerDeliveryNoteDetailsDeliveryNo equals d.CustomerDeliveryNoteDeliveryID
                                    where d.CustomerDeliveryNoteDelStatus != true
                                    select new
                                    {
                                        s.CustomerDeliveryNoteDetailsPodId,
                                        s.CustomerDeliveryNoteDetailsQty,
                                        d.CustomerDeliveryNoteDeliveryDate,
                                        s.CustomerDeliveryNoteDetailsDeliveryDetailId,
                                        d.CustomerDeliveryNoteDeliveryID,
                                    }).OrderBy(x => x.CustomerDeliveryNoteDeliveryDate).AsEnumerable().ToList();

                var purchaseorder = (from pod in _Context.CustomerPurchaseOrderDetails
                                     join po in _Context.CustomerPurchaseOrder
                                     on pod.CustomerPurchaseOrderId equals po.CustomerPurchaseOrderId
                                     where po.CustomerPurchaseOrderDelStatus != true
                                     where po.CustomerPurchaseOrderCustomerId != null
                                     && (model.CustomerId == null || model.CustomerId == 0 || po.CustomerPurchaseOrderCustomerId == model.CustomerId)
                                     && (model.LocacationId == null || model.LocacationId == 0 || po.CustomerPurchaseOrderLocationId == model.LocacationId)
                                     select new
                                     {
                                         po.CustomerPurchaseOrderId,
                                         po.CustomerPurchaseOrderDate,
                                         po.CustomerPurchaseOrderLpoNo,
                                         po.CustomerPurchaseOrderVoucherNo,
                                         pod.CustomerPurchaseOrderDetailsId,
                                         pod.CustomerPurchaseOrderDetailsItemId,
                                         pod.CustomerPurchaseOrderDetailsDescription,
                                         pod.CustomerPurchaseOrderDetailsQty,
                                         pod.CustomerPurchaseOrderDetailsUnitPrice,
                                         pod.CustomerPurchaseOrderDetailsUnitId,
                                         po.CustomerPurchaseOrderCustomerId,
                                         po.CustomerPurchaseOrderLocationId
                                     }).OrderBy(x => x.CustomerPurchaseOrderVoucherNo).AsEnumerable().ToList();
                if (model.isDataChecked)
                {
                    purchaseorder = purchaseorder.Where(a => a.CustomerPurchaseOrderDate.Value.Date >= model.DateFrom.Value.Date && a.CustomerPurchaseOrderDate.Value.Date <= model.DateTo.Value.Date).OrderBy(x => x.CustomerPurchaseOrderVoucherNo).AsEnumerable().ToList();
                }

                var results = (from cpod in purchaseorder
                               join cm in _Context.CustomerMaster
                               on cpod.CustomerPurchaseOrderCustomerId equals cm.CustomerMasterCustomerNo into gum
                               from cmg in gum.DefaultIfEmpty()

                               join im in _Context.ItemMaster
                               on cpod.CustomerPurchaseOrderDetailsItemId equals im.ItemMasterItemId into iminto
                               from img in iminto.DefaultIfEmpty()

                               join um in _Context.UnitMaster
                               on cpod.CustomerPurchaseOrderDetailsUnitId equals um.UnitMasterUnitId into uminto
                               from umg in uminto.DefaultIfEmpty()

                               join srq in subqueryDRQD
                               on cpod.CustomerPurchaseOrderDetailsId equals srq.CustomerDeliveryNoteDetailsPodId into srqGroup
                               from srqdr in srqGroup.DefaultIfEmpty()

                               select new GetCustomerPurchaseOrderTrackingResponse
                               {
                                   CPOId = cpod.CustomerPurchaseOrderId,
                                   CPODId = cpod.CustomerPurchaseOrderDetailsId,
                                   CPODate = cpod.CustomerPurchaseOrderDate,
                                   VoucherNo = cpod.CustomerPurchaseOrderLpoNo ?? "",
                                   CustomerName = cmg.CustomerMasterCustomerName,
                                   PartNo = img.ItemMasterPartNo ?? "",
                                   MatDes = cpod.CustomerPurchaseOrderDetailsDescription ?? img.ItemMasterItemName,
                                   UnitName = umg.UnitMasterUnitShortName.Trim(),
                                   UnitId = umg.UnitMasterUnitId,
                                   Quantity = cpod.CustomerPurchaseOrderDetailsQty,
                                   UnitPrice = cpod.CustomerPurchaseOrderDetailsUnitPrice,
                                   DeliveredQuantity = srqdr?.CustomerDeliveryNoteDetailsQty ?? 0,
                                   BalancedQuantity = (cpod.CustomerPurchaseOrderDetailsQty - (srqdr?.CustomerDeliveryNoteDetailsQty ?? 0)),
                                   DeliveryDate = srqdr?.CustomerDeliveryNoteDeliveryDate,
                                   DeliveredId = srqdr?.CustomerDeliveryNoteDeliveryID,
                                   ItemId = (int)cpod.CustomerPurchaseOrderDetailsItemId,
                                   LocationId = (int)cpod.CustomerPurchaseOrderLocationId
                               }).OrderBy(x => x.VoucherNo).AsEnumerable().ToList();
                return Response<List<GetCustomerPurchaseOrderTrackingResponse>>.Success(results, "Data found");
            }
            catch (Exception ex)
            {
                return Response<List<GetCustomerPurchaseOrderTrackingResponse>>.Fail(new List<GetCustomerPurchaseOrderTrackingResponse>(), ex.Message);
            }
        }


        public async Task<Response<List<GetCustomerPurchaseOrderTrackingResponse>>> GetCustomerPOStatuses(CustPurchOrderFilterModel model)
        {
            try
            {
                List<GetCustomerPurchaseOrderTrackingResponse> response = new List<GetCustomerPurchaseOrderTrackingResponse>();

                var subqueryDRQD = (from s in _Context.CustomerDeliveryNoteDetails
                                    group s by new { s.CustomerDeliveryNoteDetailsPodId } into g
                                    select new
                                    {
                                        CustomerDeliveryNoteDetailsPodId = g.Key.CustomerDeliveryNoteDetailsPodId,
                                        CustomerDeliveryNoteDetailsQty = g.Sum(s => s.CustomerDeliveryNoteDetailsQty)
                                    });

                var results = await (from cpo in _Context.CustomerPurchaseOrder
                                     join cpod in _Context.CustomerPurchaseOrderDetails on cpo.CustomerPurchaseOrderId equals cpod.CustomerPurchaseOrderId
                                     join im in _Context.ItemMaster on cpod.CustomerPurchaseOrderDetailsItemId equals im.ItemMasterItemId
                                     join um in _Context.UnitMaster on cpod.CustomerPurchaseOrderDetailsUnitId equals um.UnitMasterUnitId
                                     join umd in _Context.UnitDetails on new { unitId = um.UnitMasterUnitId, itemId = im.ItemMasterItemId } equals new { unitId = umd.UnitDetailsUnitId, itemId = umd.UnitDetailsItemId }
                                     join drq in subqueryDRQD on cpod.CustomerPurchaseOrderDetailsPodId equals drq.CustomerDeliveryNoteDetailsPodId into gdrq
                                     from q in gdrq.DefaultIfEmpty()
                                     where (model.CustomerId == 0 || model.CustomerId == null || cpo.CustomerPurchaseOrderCustomerId == model.CustomerId)
                                     && (model.ItemId == 0 || model.ItemId == null || im.ItemMasterItemId == model.ItemId)

                                     && ((model.DateFrom == null || model.DateTo == null) || (cpo.CustomerPurchaseOrderDate.Value.Date >= model.DateFrom && cpo.CustomerPurchaseOrderDate.Value.Date <= model.DateTo))

                                     select new GetCustomerPurchaseOrderTrackingResponse
                                     {

                                         CPOId = cpo.CustomerPurchaseOrderId,
                                         CPODate = cpo.CustomerPurchaseOrderDate,
                                         LPONo = cpo.CustomerPurchaseOrderLpoNo == null ? "" : cpo.CustomerPurchaseOrderLpoNo,
                                         CustomerName = cpo.CustomerPurchaseOrderCustomerName,
                                         PartNo = im.ItemMasterPartNo == null ? "" : im.ItemMasterPartNo,
                                         MatDes = cpod.CustomerPurchaseOrderDetailsDescription == null ? im.ItemMasterItemName : cpod.CustomerPurchaseOrderDetailsDescription,
                                         UnitDes = um.UnitMasterUnitDescription,
                                         CPODId = cpod.CustomerPurchaseOrderDetailsId,
                                         Amount = cpod.CustomerPurchaseOrderDetailsAmount,
                                         Quantity = cpod.CustomerPurchaseOrderDetailsQty,
                                         DeliveredQuantity = q.CustomerDeliveryNoteDetailsQty ?? 0,
                                         BalancedQuantity = cpod.CustomerPurchaseOrderDetailsQty - q.CustomerDeliveryNoteDetailsQty ?? 0,
                                         UnitPrice = cpod.CustomerPurchaseOrderDetailsUnitPrice,
                                         StockQuantity = _Context.StockRegister.Sum(y => y.StockRegisterSIN ?? 0 - y.StockRegisterSout ?? 0) / (decimal)umd.UnitDetailsConversionType
                                     }).ToListAsync();
                return Response<List<GetCustomerPurchaseOrderTrackingResponse>>.Success(results, "Data found");
            }
            catch (Exception ex)
            {
                return Response<List<GetCustomerPurchaseOrderTrackingResponse>>.Fail(new List<GetCustomerPurchaseOrderTrackingResponse>(), ex.Message);
            }
        }

        public async Task<Response<List<CustomerSalesQuotation>>> GetCustomerQuotationStatuses(CustQuotationFilterModel model)
        {
            try
            {
                List<CustomerSalesQuotation> response = new List<CustomerSalesQuotation>();

                var results = await (from cpo in _Context.CustomerQuotation
                                     join cpods in _Context.CustomerQuotationDetails on cpo.CustomerQuotationQid equals cpods.CustomerQuotationDetailsQuotationId
                                     join cl in _Context.LocationMaster on cpo.CustomerQuotationLocationId equals cl.LocationMasterLocationId
                                     join cm in _Context.CustomerMaster on cpo.CustomerQuotationCustomerId equals cm.CustomerMasterCustomerNo
                                     join itm in _Context.ItemMaster on cpods.CustomerQuotationDetailsItemId equals itm.ItemMasterItemId

                                   into gdrq
                                     from q in gdrq.DefaultIfEmpty()
                                     where (model.CustomerId == 0 || model.CustomerId == null || cpo.CustomerQuotationCustomerId == model.CustomerId)
                                     && (model.LocacationId == 0 || model.LocacationId == null || cpo.CustomerQuotationLocationId == model.LocacationId)


                                     && (model.ItemId == 0 || model.ItemId == null || cpods.CustomerQuotationDetailsItemId == model.ItemId)
                                   && ((model.DateFrom == null || model.DateTo == null) || (cpo.CustomerQuotationQuotationDate.Value.Date >= model.DateFrom.Value.Date && cpo.CustomerQuotationQuotationDate.Value.Date <= model.DateTo.Value.Date))
                                     select new CustomerSalesQuotation
                                     {
                                         CustomerQuotationQid = cpo.CustomerQuotationQid,
                                         CustomerQuotationVoucherNo = cpo.CustomerQuotationVoucherNo,
                                         CustomerQuotationDetailsCashCustomerName = cpo.CustomerQuotationCustomerId,
                                         customerQuotationQuotationdate = cpo.CustomerQuotationQuotationDate,
                                         CustomerQuotationDetailsQty = cpods.CustomerQuotationDetailsQty,
                                         CustomerQuotationDetailsUnitPrice = cpods.CustomerQuotationDetailsUnitPrice,
                                         CustomerQuotationDetailsItemId = cpods.CustomerQuotationDetailsItemId,
                                         CustomerQuotationDetailsDesription = cpods.CustomerQuotationDetailsDescription,
                                         CustomerQuotationDetailsAmount = cpods.CustomerQuotationDetailsNetAmount,
                                         CustomerQuotationDetailsUnits = cpods.CustomerQuotationDetailsUnitId,
                                         CustomerQuotationDetailsRemarks = cpo.CustomerQuotationRemarks,
                                         CustomerQuotationVatPercentage = cpo.CustomerQuotationVatPercentage,
                                         CustomerQuotationDiscountPercentage = cpo.CustomerQuotationDiscountPercentage,
                                         CustomerQuotationVatAmount = cpo.CustomerQuotationVatAmount,
                                         CustomerQuotationDiscountAmountTotal = cpo.CustomerQuotationDiscountAmountTotal

                                     }).ToListAsync();
                return Response<List<CustomerSalesQuotation>>.Success(results, "Data found");
            }
            catch (Exception ex)
            {
                return Response<List<CustomerSalesQuotation>>.Fail(new List<CustomerSalesQuotation>(), ex.Message);
            }
        }

        public async Task<Response<List<GetCustomerPurchaseOrderTrackingResponse>>> GetGRSOs(CustPurchOrderFilterModel model)
        {
            try
            {
                List<GetCustomerPurchaseOrderTrackingResponse> response = new List<GetCustomerPurchaseOrderTrackingResponse>();

                var results = (from cpo in _Context.CustomerPurchaseOrder
                               join cpod in _Context.CustomerPurchaseOrderDetails on cpo.CustomerPurchaseOrderId equals cpod.CustomerPurchaseOrderId
                               join im in _Context.ItemMaster on cpod.CustomerPurchaseOrderDetailsItemId equals im.ItemMasterItemId
                               join um in _Context.UnitMaster on cpod.CustomerPurchaseOrderDetailsUnitId equals um.UnitMasterUnitId
                               join cm in _Context.CustomerMaster on cpo.CustomerPurchaseOrderCustomerId equals cm.CustomerMasterCustomerNo
                               join umd in _Context.UnitDetails on new { unitId = um.UnitMasterUnitId, itemId = im.ItemMasterItemId } equals new { unitId = umd.UnitDetailsUnitId, itemId = umd.UnitDetailsItemId } into gdrq
                               from q in gdrq.DefaultIfEmpty()
                               where (model.CustomerId == 0 || model.CustomerId == null || cpo.CustomerPurchaseOrderCustomerId == model.CustomerId)
                               && (model.ItemId == 0 || model.ItemId == null || im.ItemMasterItemId == model.ItemId)
                               && (model.LocacationId == 0 || model.LocacationId == null || cpo.CustomerPurchaseOrderLocationId == model.LocacationId)
                               && ((model.DateFrom == null || model.DateTo == null) || (cpo.CustomerPurchaseOrderDate.Value.Date >= model.DateFrom && cpo.CustomerPurchaseOrderDate.Value.Date <= model.DateTo))
                               select new GetCustomerPurchaseOrderTrackingResponse
                               {
                                   CPOId = cpo.CustomerPurchaseOrderId,
                                   CPODate = cpo.CustomerPurchaseOrderDate,
                                   VoucherNo = cpo.CustomerPurchaseOrderVoucherNo,
                                   LPONo = cpo.CustomerPurchaseOrderLpoNo == null ? "" : cpo.CustomerPurchaseOrderLpoNo,
                                   CustomerName = cm.CustomerMasterCustomerName,
                                   CustomerId = cpo.CustomerPurchaseOrderCustomerId,
                                   LocationId = (int?)cpo.CustomerPurchaseOrderLocationId,
                                   PartNo = im.ItemMasterPartNo == null ? "" : im.ItemMasterPartNo,
                                   MatDes = cpod.CustomerPurchaseOrderDetailsDescription == null ? im.ItemMasterItemName : cpod.CustomerPurchaseOrderDetailsDescription,
                                   ItemId = (int)cpod.CustomerPurchaseOrderDetailsItemId,
                                   UnitDes = um.UnitMasterUnitDescription.Trim(),
                                   UnitId = cpod.CustomerPurchaseOrderDetailsUnitId,
                                   UnitName = um.UnitMasterUnitFullName.Trim(),
                                   CPODId = cpod.CustomerPurchaseOrderDetailsId,
                                   Amount = cpod.CustomerPurchaseOrderDetailsAmount,
                                   Quantity = cpod.CustomerPurchaseOrderDetailsQty,
                                   UnitPrice = cpod.CustomerPurchaseOrderDetailsUnitPrice,
                                   //BalancedQuantity = (int)cpod.CustomerPurchaseOrderDetailsQty - (_Context.SalesVoucherDetails.AsQueryable().Where(pr =>
                                   //    (pr.SalesVoucherDetailsSodId == cpod.CustomerPurchaseOrderDetailsId
                                   //       && pr.SalesVoucherDetailsMatId == cpod.CustomerPurchaseOrderDetailsItemId))
                                   //     .Sum(prdq => (int)((prdq.SalesVoucherDetailsQuantity) ?? 0))),

                                   DeliveredQuantity = (_Context.SalesVoucherDetails.AsQueryable().Where(pr =>
                                           (pr.SalesVoucherDetailsSodId == cpod.CustomerPurchaseOrderDetailsId
                                            && pr.SalesVoucherDetailsMatId == cpod.CustomerPurchaseOrderDetailsItemId))
                                        .Sum(prdq => (int)((prdq.SalesVoucherDetailsQuantity) ?? 0)))
                               })
                                .OrderBy(x => x.CPODate).ThenBy(x => x.VoucherNo)
                                .ToList();
                return Response<List<GetCustomerPurchaseOrderTrackingResponse>>.Success(results, "Data found");
            }
            catch (Exception ex)
            {
                return Response<List<GetCustomerPurchaseOrderTrackingResponse>>.Fail(new List<GetCustomerPurchaseOrderTrackingResponse>(), ex.Message);
            }
        }
    }
}
