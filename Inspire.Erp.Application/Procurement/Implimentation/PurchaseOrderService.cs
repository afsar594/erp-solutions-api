using Inspire.Erp.Application.Procurement.Interfaces;
using Inspire.Erp.Application.Common;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Application.Account.Implementations;
using Inspire.Erp.Domain.Enums;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Inspire.Erp.Application.Procurement.Implementation
{
    public class PurchaseOrderService : IPurchaseOrderService
    {
        private IRepository<StockRegister> _stockRegisterRepository;
        private IRepository<AccountsTransactions> _accountTransactionRepository;
        private IRepository<PurchaseOrder> _purchaseOrderRepository;
        private IRepository<PurchaseReturn> _purchaseReturnRepository;
        private IRepository<PurchaseReturnDetails> _purchaseReturnDetailsRepository;
        private IRepository<PurchaseOrderDetails> _purchaseOrderDetailsRepository;
        private IRepository<ProgramSettings> _programsettingsRepository;
        private IRepository<VouchersNumbers> _voucherNumbersRepository;
        private readonly ILogger<PaymentVoucherService> _logger;

        private IRepository<ItemMaster> _itemMasterRepository;
        private IRepository<UnitMaster> _unitMasterRepository;
        private IRepository<SuppliersMaster> _suppliersMasterRepository;
        private IRepository<LocationMaster> _locationMasterRepository;
        private IRepository<UserFile> _userFileRepository;

        //private IRepository<ReportPurchaseOrder> _reportPurchaseOrderRepository;
        public PurchaseOrderService(
            IRepository<UserFile> userFileRepository,
            //IRepository<ReportPurchaseOrder> reportPurchaseOrderRepository,
            IRepository<ItemMaster> itemMasterRepository, IRepository<UnitMaster> unitMasterRepository,
            IRepository<SuppliersMaster> suppliersMasterRepository, IRepository<LocationMaster> locationMasterRepository,

            IRepository<AccountsTransactions> accountTransactionRepository, IRepository<StockRegister> stockRegisterRepository, IRepository<ProgramSettings> programsettingsRepository,
             IRepository<VouchersNumbers> voucherNumbers, ILogger<PaymentVoucherService> logger,

            IRepository<PurchaseOrder> purchaseOrderRepository, IRepository<PurchaseOrderDetails> purchaseOrderDetailsRepository, IRepository<PurchaseReturn> purchaseReturnRepository, IRepository<PurchaseReturnDetails> purchaseReturnDetailsRepository)
        {
            this._accountTransactionRepository = accountTransactionRepository;
            this._stockRegisterRepository = stockRegisterRepository;
            this._purchaseOrderRepository = purchaseOrderRepository;
            this._purchaseOrderDetailsRepository = purchaseOrderDetailsRepository;
            _programsettingsRepository = programsettingsRepository;
            _voucherNumbersRepository = voucherNumbers;
            _logger = logger;

            _itemMasterRepository = itemMasterRepository;
            _unitMasterRepository = unitMasterRepository;
            _suppliersMasterRepository = suppliersMasterRepository;
            _locationMasterRepository = locationMasterRepository;
            _userFileRepository = userFileRepository;
            _purchaseReturnRepository = purchaseReturnRepository;
            _purchaseReturnDetailsRepository = purchaseReturnDetailsRepository;
            //_reportPurchaseOrderRepository = reportPurchaseOrderRepository;
        }

        //public IEnumerable<ReportPurchaseOrder> PurchaseOrder_GetReportPurchaseOrder()
        //{
        //    return _reportPurchaseOrderRepository.GetAll();
        //}

        public IEnumerable<UserFile> GetAllUserFile()
        {
            return _userFileRepository.GetAll();
        }


        public IEnumerable<LocationMaster> PurchaseOrder_GetAllLocationMaster()
        {
            return _locationMasterRepository.GetAll();
        }


        public IEnumerable<SuppliersMaster> PurchaseOrder_GetAllSuppliersMaster()
        {
            return _suppliersMasterRepository.GetAll();
        }


        public IEnumerable<UnitMaster> PurchaseOrder_GetAllUnitMaster()
        {
            return _unitMasterRepository.GetAll();
        }
        public IEnumerable<ItemMaster> PurchaseOrder_GetAllItemMaster()
        {
            return _itemMasterRepository.GetAll();
        }

        public PurchaseOrderModel UpdatePurchaseOrder(PurchaseOrder purchaseOrder, List<AccountsTransactions> accountsTransactions, 
            List<PurchaseOrderDetails> purchaseOrderDetails
            //, List<StockRegister> stockRegister
            )
        {

            try
            {
                _purchaseOrderRepository.BeginTransaction();

                var deleteTrackingRegister = _trackingRegisterRepository.GetAsQueryable().Where(x => x.TrackingRegisterVoucherType == purchaseOrder.PurchaseOrderType && x.TrackingRegisterVoucherNo == purchaseOrder.PurchaseOrderNo).FirstOrDefault();
                _trackingRegisterRepository.Delete(deleteTrackingRegister);
                _trackingRegisterRepository.SaveChangesAsync();

                var deletePurOrderRegister = _purOrderRegisterRepository.GetAsQueryable().Where(x => x.PurOrderRegisterTransType == purchaseOrder.PurchaseOrderType && x.PurOrderRegisterRefVoucherNo == purchaseOrder.PurchaseOrderNo).FirstOrDefault();
                _purOrderRegisterRepository.Delete(deletePurOrderRegister);
                _purOrderRegisterRepository.SaveChangesAsync();



                purchaseOrder.PurchaseOrderDetails = purchaseOrder.PurchaseOrderDetails.Select((k) =>
                ////purchaseOrder.PurchaseOrderDetailsPurchaseOrder = purchaseOrderDetails.Select((k) =>
                {
                    //if (k.PurchaseOrderDetailsId == 0)
                    //{
                    k.PurchaseOrderId = purchaseOrder.PurchaseOrderId;
                    k.PurchaseOrderDetailsNo = purchaseOrder.PurchaseOrderNo;
                    //k.PurchaseOrderDetailsId = 0;
                    //}

                    return k;
                }).ToList();

                _purchaseOrderRepository.Update(purchaseOrder);





                //_purchaseOrderRepository.Update(purchaseOrder);
                accountsTransactions = accountsTransactions.Select((k) =>
                {
                    if (k.AccountsTransactionsTransSno == 0)
                    {
                        k.AccountsTransactionsTransDate = purchaseOrder.PurchaseOrderDate;
                        k.AccountsTransactionsVoucherNo = purchaseOrder.PurchaseOrderNo;
                        k.AccountsTransactionsVoucherType = VoucherType.PurchaseOrder_TYPE;
                        k.AccountsTransactionsStatus = AccountStatus.Approved;
                    }

                    return k;
                }).ToList();
                _accountTransactionRepository.UpdateList(accountsTransactions);


                //stockRegister = stockRegister.Select((k) =>
                //{
                //    if (k.StockRegisterId == 0)
                //    {
                //        k.StockRegisterVoucherDate = purchaseOrder.PurchaseOrderDate;
                //        k.StockRegisterVoucherNo = purchaseOrder.PurchaseOrderNo;
                //        k.StockRegisterTransType = VoucherType.PurchaseOrder_TYPE;

                //        k.StockRegisterStatus = AccountStatus.Approved;
                //    }

                //    return k;
                //}).ToList();
                //_stockRegisterRepository.UpdateList(stockRegister);


                //insert tracking register
                var listTrackRegs = new List<TrackingRegister>();
                var listTrackPurchOrderReg = new List<PurOrderRegister>();
                foreach (var purOrderDet in purchaseOrder.PurchaseOrderDetails)
                {
                    var tr = new TrackingRegister
                    {
                        TrackingRegisterDetailsId = (int)purOrderDet.PurchaseOrderDetailsId,
                        TrackingRegisterVoucherNo = purOrderDet.PurchaseOrderDetailsNo,
                        TrackingRegisterMaterialId = (int)purOrderDet.PurchaseOrderDetailsMatId,
                        TrackingRegisterVoucherType = purchaseOrder.PurchaseOrderType,
                        TrackingRegisterQty = (int)purOrderDet.PurchaseOrderDetailsQuantity,
                        TrackingRegisterQtyin = (int)purOrderDet.PurchaseOrderDetailsQuantity,
                        TrackingRegisterQtyout = 0,
                        TrackingRegisterRate = (double)purOrderDet.PurchaseOrderDetailsRate,
                        TrackingRegisterTrackDate = purchaseOrder.PurchaseOrderDate,
                        TrackingRegisterFsno = null,//to be alloted

                    };
                    listTrackRegs.Add(tr);

                    var poReg = new PurOrderRegister
                    {
                        PurOrderRegisterOrderNo = purOrderDet.PurchaseOrderDetailsId.ToString(),
                        PurOrderRegisterRefVoucherNo = purOrderDet.PurchaseOrderDetailsNo,
                        PurOrderRegisterMaterialId = (int)purOrderDet.PurchaseOrderDetailsMatId,
                        PurOrderRegisterTransType = purchaseOrder.PurchaseOrderType,
                        PurOrderRegisterQtyOrder = (int)purOrderDet.PurchaseOrderDetailsQuantity,
                        PurOrderRegisterQtyIssued = 0,
                        PurOrderRegisterRate = (double)purOrderDet.PurchaseOrderDetailsRate,
                        PurOrderRegisterAssignedDate = DateTime.Now,
                        PurOrderRegisterFsno = null,//to be alloted,
                        PurOrderRegisterSupplierId = (int)purchaseOrder.PurchaseOrderPartyId,
                        PurOrderRegisterStatus = "A"

                    };
                    listTrackPurchOrderReg.Add(poReg);

                }

                _trackingRegisterRepository.InsertList(listTrackRegs);
                _purOrderRegisterRepository.InsertList(listTrackPurchOrderReg);


                _purchaseOrderRepository.TransactionCommit();

            }
            catch (Exception ex)
            {
                _purchaseOrderRepository.TransactionRollback();
                throw ex;
            }

            return this.GetSavedPurchaseOrderDetails(purchaseOrder.PurchaseOrderNo);
        }

        public int DeletePurchaseOrder(PurchaseOrder purchaseOrder, List<AccountsTransactions> accountsTransactions,
            List<PurchaseOrderDetails> purchaseOrderDetails
            //, List<StockRegister> stockRegister
            )
        {
            int i = 0;
            try
            {
                _purchaseOrderRepository.BeginTransaction();


                purchaseOrder.PurchaseOrderDelStatus = true;

                purchaseOrder.PurchaseOrderDetails = purchaseOrder.PurchaseOrderDetails.Select((k) =>
                {
                    k.PurchaseOrderDetailsDelStatus = true;
                    return k;
                }).ToList();

                //_purchaseOrderDetailsRepository.UpdateList(purchaseOrderDetails);

                accountsTransactions = accountsTransactions.Select((k) =>
                {
                    k.AccountstransactionsDelStatus = true;
                    return k;
                }).ToList();
                _accountTransactionRepository.UpdateList(accountsTransactions);




                //stockRegister = stockRegister.Select((k) =>
                //{
                //    k.StockRegisterDelStatus = true;
                //    return k;
                //}).ToList();
                //_stockRegisterRepository.UpdateList(stockRegister);



              //  purchaseOrder.PurchaseOrderDetails = purchaseOrderDetails;
                ////purchaseOrder.PurchaseOrderDetailsPurchaseOrder = purchaseOrderDetails;


                _purchaseOrderRepository.Update(purchaseOrder);

                var vchrnumer = _voucherNumbersRepository.GetAsQueryable().Where(k => k.VouchersNumbersVNo == purchaseOrder.PurchaseOrderNo).FirstOrDefault();

                _voucherNumbersRepository.Update(vchrnumer);

                _purchaseOrderRepository.TransactionCommit();
                i = 1;
            }
            catch (Exception ex)
            {
                _purchaseOrderRepository.TransactionRollback();
                i = 0;
                throw ex;
            }

            return i;
        }
        public PurchaseOrderModel InsertPurchaseOrder(PurchaseOrder purchaseOrder, List<AccountsTransactions> accountsTransactions,
            List<PurchaseOrderDetails> purchaseOrderDetails
            //, List<StockRegister> stockRegister
            )
        {
            try
            {
                _purchaseOrderRepository.BeginTransaction();
                string purchaseOrderNumber = this.GenerateVoucherNo(purchaseOrder.PurchaseOrderDate.Date).VouchersNumbersVNo;
                purchaseOrder.PurchaseOrderNo = purchaseOrderNumber;


                int maxcount = 0;
                maxcount = Convert.ToInt32(
                    _purchaseOrderRepository.GetAsQueryable()
                    .DefaultIfEmpty().Max(o => o == null ? 0 : o.PurchaseOrderId) + 1);

                purchaseOrder.PurchaseOrderId = maxcount;



                purchaseOrderDetails = purchaseOrderDetails.Select((x) =>
                {
                    x.PurchaseOrderId = maxcount;
                    x.PurchaseOrderDetailsNo = purchaseOrderNumber;
                    return x;
                }).ToList();
                //_purchaseOrderDetailsRepository.InsertList(purchaseOrderDetails);

                accountsTransactions = accountsTransactions.Select((k) =>
                {
                    //k.AccountsTransactionsTransDate = purchaseOrder.PurchaseOrderDate;
                    k.AccountsTransactionsVoucherNo = purchaseOrderNumber;
                    k.AccountsTransactionsVoucherType = VoucherType.PurchaseOrder_TYPE;
                    k.AccountsTransactionsStatus = AccountStatus.Approved;
                    return k;
                }).ToList();
                _accountTransactionRepository.InsertList(accountsTransactions);



                //stockRegister = stockRegister.Select((k) =>
                //{
                //    k.StockRegisterVoucherDate = purchaseOrder.PurchaseOrderDate;
                //    k.StockRegisterVoucherNo = purchaseOrderNumber;
                //    k.StockRegisterTransType = VoucherType.PurchaseOrder_TYPE;
                //    k.StockRegisterStatus = AccountStatus.Approved;
                //    return k;
                //}).ToList();
                //_stockRegisterRepository.InsertList(stockRegister);



                _purchaseOrderRepository.Insert(purchaseOrder);

                //insert tracking register
                var listTrackRegs = new List<TrackingRegister>();
                var listTrackPurchOrderReg = new List<PurOrderRegister>();
                foreach (var purOrderDet in purchaseOrder.PurchaseOrderDetails)
                {
                    var tr = new TrackingRegister
                    {
                        TrackingRegisterDetailsId = (int)purOrderDet.PurchaseOrderDetailsId,
                        TrackingRegisterVoucherNo = purOrderDet.PurchaseOrderDetailsNo,
                        TrackingRegisterMaterialId=(int) purOrderDet.PurchaseOrderDetailsMatId,
                        TrackingRegisterVoucherType=purchaseOrder.PurchaseOrderType,
                        TrackingRegisterQty=(int)purOrderDet.PurchaseOrderDetailsQuantity,
                        TrackingRegisterQtyin= (int)purOrderDet.PurchaseOrderDetailsQuantity,
                        TrackingRegisterQtyout = 0,
                        TrackingRegisterRate=(double)purOrderDet.PurchaseOrderDetailsRate,
                        TrackingRegisterTrackDate=purchaseOrder.PurchaseOrderDate,
                        TrackingRegisterFsno=null,//to be alloted

                    };
                    listTrackRegs.Add(tr);

                    var poReg = new PurOrderRegister
                    {
                        PurOrderRegisterOrderNo = purOrderDet.PurchaseOrderDetailsId.ToString(),
                        PurOrderRegisterRefVoucherNo = purOrderDet.PurchaseOrderDetailsNo,
                        PurOrderRegisterMaterialId = (int)purOrderDet.PurchaseOrderDetailsMatId,
                        PurOrderRegisterTransType = purchaseOrder.PurchaseOrderType,
                        PurOrderRegisterQtyOrder = (int)purOrderDet.PurchaseOrderDetailsQuantity,
                        PurOrderRegisterQtyIssued = 0,
                        PurOrderRegisterRate = (double)purOrderDet.PurchaseOrderDetailsRate,
                        PurOrderRegisterAssignedDate =DateTime.Now,
                        PurOrderRegisterFsno = null,//to be alloted,
                        PurOrderRegisterSupplierId=(int)purchaseOrder.PurchaseOrderPartyId,
                        PurOrderRegisterStatus="A"

                    };
                    listTrackPurchOrderReg.Add(poReg);

                }

                _trackingRegisterRepository.InsertList(listTrackRegs);
                _purOrderRegisterRepository.InsertList(listTrackPurchOrderReg);

                _purchaseOrderRepository.TransactionCommit();
                return this.GetSavedPurchaseOrderDetails(purchaseOrder.PurchaseOrderNo);

            }
            catch (Exception ex)
            {
                _purchaseOrderRepository.TransactionRollback();
                throw ex;
            }

        }
        public IEnumerable<AccountsTransactions> GetAllTransaction()
        {
            return _accountTransactionRepository.GetAll();
        }
        public IEnumerable<PurchaseOrder> GetPurchaseOrder()
        {
            IEnumerable<PurchaseOrder> purchaseOrder_ALL = _purchaseOrderRepository.GetAll().Where(k => k.PurchaseOrderDelStatus == false || k.PurchaseOrderDelStatus == null);
            //foreach (var purchaseOrderDetail in purchaseOrder_ALL)
            //{
            //    purchaseOrderDetail.PurchaseOrderDetails = _purchaseOrderDetailsRepository.GetAll().Where(c => c.PurchaseOrderDetailsNo == purchaseOrderDetail.PurchaseOrderNo).ToList();
            //}
            return purchaseOrder_ALL;
        }
        public PurchaseOrderModel GetSavedPurchaseOrderDetails(string pvno)
        {
            PurchaseOrderModel purchaseOrderModel = new PurchaseOrderModel();
            //purchaseOrderModel.purchaseOrder = _purchaseOrderRepository.GetAsQueryable().Where(k => k.PurchaseOrderNo == pvno && k.PurchaseOrderDelStatus == false).SingleOrDefault();

            purchaseOrderModel.purchaseOrder = _purchaseOrderRepository.GetAsQueryable().Where(k => k.PurchaseOrderNo == pvno).SingleOrDefault();


            try
            {
                purchaseOrderModel.accountsTransactions = _accountTransactionRepository.GetAsQueryable().Where(c => c.AccountsTransactionsVoucherNo == pvno && c.AccountsTransactionsVoucherType == VoucherType.PurchaseOrder_TYPE && (c.AccountstransactionsDelStatus == false || c.AccountstransactionsDelStatus == null)).ToList();
            }
            catch { 
            }
            purchaseOrderModel.purchaseOrderDetails = _purchaseOrderDetailsRepository.GetAsQueryable().Where(x => x.PurchaseOrderDetailsNo == pvno && (x.PurchaseOrderDetailsDelStatus == false || x.PurchaseOrderDetailsDelStatus == null)).ToList();

            foreach (var purchaseOrderDetail in purchaseOrderModel.purchaseOrderDetails)
            {
                if (_stockRegisterRepository.GetAsQueryable().Where(c => c.StockRegisterRefVoucherNo == purchaseOrderDetail.PurchaseOrderDetailsNo).FirstOrDefault() != null)
                {
                    purchaseOrderDetail.PurchaseOrderNetStock = _stockRegisterRepository.GetAsQueryable().Where(c => c.StockRegisterRefVoucherNo == purchaseOrderDetail.PurchaseOrderDetailsNo).FirstOrDefault().StockRegisterSIN;
                   
                }
                purchaseOrderDetail.PurchaseOrderNetStock = 0;
                purchaseOrderDetail.PurchaseOrderTillNowQTY = GetTillNowQTY(pvno);
            }

            return purchaseOrderModel;

        }

        private decimal? GetTillNowQTY(string pvno)
        {
            try
            {
                //var pr = _purchaseReturnRepository.GetAll().Where(c => c.PurchaseReturnGrno == pvno).LastOrDefault();

                //if (pr != null)
                //{
                //    decimal? qty = 0.0M;

                //        var prDetails = _purchaseReturnDetailsRepository.GetAll().Where(c => c.PurchaseReturnDetailsNo == pr.PurchaseReturnNo);
                //        if (prDetails != null)
                //        {
                //        foreach (var item in prDetails)
                //        {
                //            if (item.PurchaseReturnDetailsQuantity !=null)
                //            {
                //                qty += item.PurchaseReturnDetailsQuantity;
                //            }
                //        }
                //            //qty += Convert.ToDecimal(_purchaseReturnDetailsRepository.GetAll().Where(c => c.PurchaseReturnDetailsNo == item.PurchaseReturnNo).FirstOrDefault().PurchaseReturnDetailsQuantity);

                //        }


                //    return qty;
                //}
                //return 0;


                //=====================================//
                var pr = _purchaseReturnRepository.GetAll().Where(c => c.PurchaseReturnGrno == pvno);
                if (pr != null)
                {
                    decimal? qty = 0.0M;
                    foreach (var prdetail in pr)
                    {
                        var rslt = _purchaseReturnDetailsRepository.GetAll().Where(c => c.PurchaseReturnDetailsNo == prdetail.PurchaseReturnNo).FirstOrDefault();
                        if (rslt != null)
                        {
                            qty += rslt.PurchaseReturnDetailsQuantity;
                        }
                            // qty += rslt;
                    }
                    //var prDetails = _purchaseReturnDetailsRepository.GetAll().Where(c => c.PurchaseReturnDetailsNo == pr.PurchaseReturnNo);
                    return qty;
                }
                return 0;
            }
            catch (Exception)
            {
                return 0;
                throw;
            }
        }

        public VouchersNumbers GenerateVoucherNo(DateTime? VoucherGenDate) 
        {
            try
            {

                var prefix = this._programsettingsRepository.GetAsQueryable().Where(k => k.ProgramSettingsKeyValue == Prefix.PurchaseOrder_Prefix).FirstOrDefault().ProgramSettingsTextValue;
                int vnoMaxVal = Convert.ToInt32(_voucherNumbersRepository.GetAsQueryable()
                                                        .Where(x => x.VouchersNumbersVNoNu > 0 && x.VouchersNumbersVType == VoucherType.PurchaseOrder_TYPE)
                                                        .DefaultIfEmpty()
                                                        .Max(o => o == null ? 0 : o.VouchersNumbersVNoNu)) + 1;

                //var prefix = "CN";
                //int vnoMaxVal = 1;

                VouchersNumbers vouchersNumbers = new VouchersNumbers
                { 
                    VouchersNumbersVNo = prefix + vnoMaxVal,
                    VouchersNumbersVNoNu = vnoMaxVal,
                    VouchersNumbersVType = VoucherType.PurchaseOrder_TYPE,
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
        public VouchersNumbers GetVouchersNumbers(string pvno, string vType)
        {
            try
            {
                if (string.IsNullOrEmpty(pvno))
                {

                }
                VouchersNumbers vouchersNumbers = _voucherNumbersRepository.GetAsQueryable().Where(k => k.VouchersNumbersVNo == pvno).SingleOrDefault();
                return vouchersNumbers;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex;
            }

        }



    }
}
