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
using Inspire.Erp.Domain.Entities.POS;
using Inspire.Erp.Domain.Modals;
using System.Threading.Tasks;
using Inspire.Erp.Infrastructure.Database;
using Inspire.Erp.Application.MODULE;
using Inspire.Erp.Application.Email.Interfaces;
using Inspire.Erp.Domain.Models.Common;
using Microsoft.Extensions.Options;
using SendGrid.Helpers.Mail;
using System.IO;
using Inspire.Erp.Application.Helpers;
using Inspire.Erp.Domain.Models.Approvals;
using System.Linq.Dynamic.Core;
using Microsoft.Extensions.Caching.Memory;
using Inspire.Erp.Domain.Constants;

namespace Inspire.Erp.Application.Procurement.Implementation
{
    public class PurchaseOrderService : IPurchaseOrderService
    {
        private InspireErpDBContext _context;
        private IRepository<StockRegister> _stockRegisterRepository;
        private IRepository<AccountsTransactions> _accountTransactionRepository;
        private IRepository<PurchaseOrder> _purchaseOrderRepository;
        private IRepository<PurchaseReturn> _purchaseReturnRepository;
        private IRepository<PurchaseReturnDetails> _purchaseReturnDetailsRepository;
        private IRepository<PurchaseOrderDetails> _purchaseOrderDetailsRepository;
        private IRepository<ProgramSettings> _programsettingsRepository;
        private IRepository<VouchersNumbers> _voucherNumbersRepository;
        private readonly ILogger<PaymentVoucherService> _logger;
        private readonly IRepository<TrackingRegister> _trackingRegisterRepository;
        private readonly IRepository<PurOrderRegister> _purOrderRegisterRepository;
        private readonly IRepository<PermissionApproval> _permissionApprovalRepo;
        private readonly IRepository<Approval> _approvalRepo;
        private readonly IRepository<ApprovalDetail> _approvalDetailRepo;
        private readonly IRepository<PurchaseVoucherDetails> _purchaseVoucherDetailsRepo;
        private IRepository<ItemMaster> _itemMasterRepository;
        private IRepository<UnitMaster> _unitMasterRepository;
        private IRepository<SuppliersMaster> _suppliersMasterRepository;
        private IRepository<LocationMaster> _locationMasterRepository;
        private IRepository<UserFile> _userFileRepository;
        private IRepository<UnitDetails> _UnitDetailsRepository;
        private IRepository<PermissionApprovalDetail> _permissionApprovalDetailRepo;
        private readonly IOrderEmailNotificationService _orderEmailNotificationService;
        // private readonly InspireErpDBContext _dbContext;
        private readonly IRepository<PurchaseRequisitionDetails> _purchaseRequisitionDetailsRepository;
        private readonly IRepository<PurchaseRequisition> _purchaseRequisitionRepository;
        private readonly IRepository<ApproverLevelSettings> _approverLevelSettings;
        private readonly IRepository<OrderApprovalHierarchy> _orderApprovalHierarchy;
        private readonly CacheMemoryService _cacheMemoryService;
        private IPurchaseRequisitionService _purchaseRequisitionService;
        public PurchaseOrderService(
            IRepository<UserFile> userFileRepository, IRepository<PermissionApproval> permissionApprovalRepo,
            IRepository<Approval> approvalRepo, IRepository<ApprovalDetail> approvalDetailRepo, InspireErpDBContext context,
            IRepository<ItemMaster> itemMasterRepository, IRepository<UnitMaster> unitMasterRepository,
            IRepository<SuppliersMaster> suppliersMasterRepository, IRepository<LocationMaster> locationMasterRepository,
            IRepository<AccountsTransactions> accountTransactionRepository, IRepository<StockRegister> stockRegisterRepository, IRepository<ProgramSettings> programsettingsRepository,
            IRepository<VouchersNumbers> voucherNumbers, ILogger<PaymentVoucherService> logger,
            IRepository<PurchaseOrder> purchaseOrderRepository, IRepository<PurchaseOrderDetails> purchaseOrderDetailsRepository,
            IRepository<PurchaseReturn> purchaseReturnRepository, IRepository<PurchaseReturnDetails> purchaseReturnDetailsRepository,
            IRepository<TrackingRegister> trackingRegisterRepository, IRepository<PurOrderRegister> purOrderRegisterRepository,
            IRepository<UnitDetails> unitDetailsRepository,
            IRepository<PurchaseVoucherDetails> purchaseVoucherDetailsRepo, IMemoryCache cache,
            IRepository<PermissionApprovalDetail> permissionApprovalDetailRepo, IRepository<PurchaseRequisitionDetails> purchaseRequisitionDetailsRepository, IRepository<Permissionapproval> permissionapproval, IRepository<ApproverLevelSettings> approverLevelSettings, IOrderEmailNotificationService orderEmailNotificationService, IRepository<PurchaseRequisition> purchaseRequisitionRepository, IRepository<OrderApprovalHierarchy> orderApprovalHierarchy, IPurchaseRequisitionService purchaseRequisitionService)
        {
            this._accountTransactionRepository = accountTransactionRepository;
            this._stockRegisterRepository = stockRegisterRepository;
            this._purchaseOrderRepository = purchaseOrderRepository;
            this._purchaseOrderDetailsRepository = purchaseOrderDetailsRepository;
            _programsettingsRepository = programsettingsRepository;
            _voucherNumbersRepository = voucherNumbers;
            _logger = logger; _context = context;
            _trackingRegisterRepository = trackingRegisterRepository;
            _itemMasterRepository = itemMasterRepository;
            _unitMasterRepository = unitMasterRepository;
            _suppliersMasterRepository = suppliersMasterRepository;
            _locationMasterRepository = locationMasterRepository;
            _userFileRepository = userFileRepository;
            _purchaseReturnRepository = purchaseReturnRepository;
            _purchaseReturnDetailsRepository = purchaseReturnDetailsRepository;
            _purOrderRegisterRepository = purOrderRegisterRepository;
            _permissionApprovalRepo = permissionApprovalRepo;
            _approvalRepo = approvalRepo;
            _approvalDetailRepo = approvalDetailRepo;
            _UnitDetailsRepository = unitDetailsRepository;
            _purchaseVoucherDetailsRepo = purchaseVoucherDetailsRepo;
            _permissionApprovalDetailRepo = permissionApprovalDetailRepo;
            _purchaseRequisitionDetailsRepository = purchaseRequisitionDetailsRepository;
            _orderEmailNotificationService = orderEmailNotificationService;
            _purchaseRequisitionRepository = purchaseRequisitionRepository;
            _approverLevelSettings = approverLevelSettings;
            _cacheMemoryService = new CacheMemoryService(cache);
            _orderApprovalHierarchy = orderApprovalHierarchy;
            _purchaseRequisitionService = purchaseRequisitionService;
        }
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
            )
        {

            try
            {
                _purchaseOrderRepository.BeginTransaction();

                var deleteTrackingRegister = _trackingRegisterRepository.GetAsQueryable().Where(x => x.TrackingRegisterVoucherType == purchaseOrder.PurchaseOrderType && x.TrackingRegisterVoucherNo == purchaseOrder.PurchaseOrderNo).FirstOrDefault();
                if (deleteTrackingRegister != null)
                {
                    _trackingRegisterRepository.Delete(deleteTrackingRegister);
                    _trackingRegisterRepository.SaveChangesAsync();
                }

                var deletePurOrderRegister = _purOrderRegisterRepository.GetAsQueryable().Where(x => x.PurOrderRegisterTransType == purchaseOrder.PurchaseOrderType && x.PurOrderRegisterRefVoucherNo == purchaseOrder.PurchaseOrderNo).FirstOrDefault();
                if (deletePurOrderRegister != null)
                {
                    _purOrderRegisterRepository.Delete(deletePurOrderRegister);
                    _purOrderRegisterRepository.SaveChangesAsync();
                }

                purchaseOrder.PurchaseOrderDetails = purchaseOrder.PurchaseOrderDetails.Select((k) =>
                {
                    k.PurchaseOrderId = purchaseOrder.PurchaseOrderId;
                    k.PurchaseOrderDetailsNo = purchaseOrder.PurchaseOrderNo;
                    return k;
                }).OrderBy(x => x.PurchaseOrderDetailsNo).AsEnumerable().ToList();
                _purchaseOrderRepository.Update(purchaseOrder);

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
                }).OrderBy(x => x.AccountsTransactionsVoucherNo).AsEnumerable().ToList();
                _accountTransactionRepository.UpdateList(accountsTransactions);

                //clsCommonFunctions.UpdateAccountsTransactions(purchaseOrder.PurchaseOrderNo, VoucherType.PurchaseOrder_TYPE, _accountTransactionRepository);


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
                        TrackingRegisterRate = purOrderDet.PurchaseOrderDetailsRate,
                        TrackingRegisterTrackDate = purchaseOrder.PurchaseOrderDate,
                        TrackingRegisterFsno = null,

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
                        PurOrderRegisterRate = purOrderDet.PurchaseOrderDetailsRate,
                        PurOrderRegisterAssignedDate = DateTime.Now,
                        PurOrderRegisterFsno = null,
                        PurOrderRegisterSupplierId = (int)purchaseOrder.PurchaseOrderPartyId,
                        PurOrderRegisterStatus = "A"

                    };
                    listTrackPurchOrderReg.Add(poReg);

                }
                _trackingRegisterRepository.InsertList(listTrackRegs);
                _purOrderRegisterRepository.InsertList(listTrackPurchOrderReg);

                //PermissionApprovalDetail by zaid               

                //  var permsionApproval = _context.Permissionapproval.AsNoTracking().Select(x => x.LevelId).Distinct().ToList();
                var rights = _context.tbl_approvalforms.Where(a => a.Voucher_Type.Contains("Purchase Order") && a.is_active == true).AsNoTracking().ToList();
                if (rights.Count > 0)
                {
                    var existdata = _permissionApprovalDetailRepo.GetAsQueryable().Where(a => a.VoucherId == purchaseOrder.PurchaseOrderId && a.VoucherType == "Purchase Order").ToList();

                    if (existdata.Count > 0)
                    {
                        _permissionApprovalDetailRepo.DeleteList(existdata);
                    }
                    foreach (var item in rights)
                    {
                        for (int level = 1; level <= item.NoOfLevel; level++)
                        {
                            PermissionApprovalDetail POList = new PermissionApprovalDetail();
                            POList.VoucherType = "Purchase Order";
                            POList.VoucherNo = purchaseOrder.PurchaseOrderNo;
                            POList.VoucherId = (int)purchaseOrder.PurchaseOrderId;
                            POList.VoucherDate = purchaseOrder.PurchaseOrderDate;
                            POList.Amount = (double)purchaseOrder.PurchaseOrderNetAmount;
                            POList.CreatedBy = (int?)purchaseOrder.PurchaseOrderUserId ?? 1;
                            POList.LevelId = level;
                            POList.Status = "Pending";
                            POList.Remarks = purchaseOrder.PurchaseOrderDescription;
                            POList.JobId = (int)purchaseOrder.PurchaseOrderJobId;
                            _permissionApprovalDetailRepo.Insert(POList);
                        }
                    }

                }
                _purchaseOrderRepository.TransactionCommit();
            }
            catch (Exception ex)
            {
                _purchaseOrderRepository.TransactionRollback();
                throw ex;
            }

            return this.GetSavedPurchaseOrderDetails((int)purchaseOrder.PurchaseOrderId);
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
        public int DeletePurchaseOrder(PurchaseOrder purchaseOrder, List<AccountsTransactions> accountsTransactions,
            List<PurchaseOrderDetails> purchaseOrderDetails
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
                }).OrderBy(x => x.PurchaseOrderDetailsItemName).AsEnumerable().ToList();

                //accountsTransactions = accountsTransactions.Select((k) =>
                //{
                //    k.AccountstransactionsDelStatus = true;
                //    return k;
                //}).OrderBy(x => x.AccountsTransactionsVoucherNo).AsEnumerable().ToList();
                //_accountTransactionRepository.UpdateList(accountsTransactions);

                clsCommonFunctions.Delete_OldEntry_AccountsTransactions(purchaseOrder.PurchaseOrderNo, VoucherType.PurchaseOrder_TYPE, _accountTransactionRepository);
                var detailsToDelete = _permissionApprovalDetailRepo
                .GetAsQueryable().Where(a => a.VoucherNo == purchaseOrder.PurchaseOrderNo).ToList();
                foreach (var detail in detailsToDelete)
                {
                    detail.IsDeleted = true;
                    _permissionApprovalDetailRepo.Update(detail);
                }

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
        public PurchaseOrderModel InsertPurchaseOrder(PurchaseOrder purchaseOrder,
                                                                                        List<AccountsTransactions> accountsTransactions,
                                                                                        List<PurchaseOrderDetails> purchaseOrderDetails)
        {
            try
            {
                _purchaseOrderRepository.BeginTransaction();

                string purchaseOrderNumber = clsCommonFunctions.GenrateVoucherNo(VoucherType.PurchaseOrder_TYPE, Prefix.PurchaseOrder_Prefix,
                    _programsettingsRepository, _voucherNumbersRepository);
                purchaseOrder.PurchaseOrderNo = purchaseOrderNumber;

                int maxcount = 0;
                maxcount = Convert.ToInt32(_purchaseOrderRepository.GetAsQueryable().DefaultIfEmpty().Max(o => o == null ? 0 : o.PurchaseOrderId) + 1);

                purchaseOrder.PurchaseOrderId = maxcount;

                purchaseOrder.PurchaseOrderDetails = purchaseOrder.PurchaseOrderDetails.Select((x) =>
                {
                    x.PurchaseOrderId = maxcount;
                    x.PurchaseOrderDetailsNo = purchaseOrderNumber;
                    return x;
                }).OrderBy(x => x.PurchaseOrderDetailsSno).AsEnumerable().ToList();

                purchaseOrder.PurchaseOrderDetails.Clear();
                foreach (var item in purchaseOrderDetails)
                {
                    item.PurchaseOrderId = maxcount;
                    item.PurchaseOrderDetailsNo = purchaseOrderNumber;
                    purchaseOrder.PurchaseOrderDetails.Add(item);
                }
                accountsTransactions = accountsTransactions.Select((k) =>
                {
                    k.AccountsTransactionsVoucherNo = purchaseOrder.PurchaseOrderNo;
                    k.AccountsTransactionsVoucherType = VoucherType.PurchaseOrder_TYPE;
                    k.AccountsTransactionsStatus = AccountStatus.Approved;
                    return k;
                }).OrderBy(x => x.AccountsTransactionsVoucherType).AsEnumerable().ToList();
                _accountTransactionRepository.InsertList(accountsTransactions);
                _purchaseOrderRepository.Insert(purchaseOrder);

                //insert tracking register
                var listTrackRegs = new List<TrackingRegister>();
                var listTrackPurchOrderReg = new List<PurOrderRegister>();
                purchaseOrder.PurchaseOrderDetails.ToList().ForEach(x =>
                {
                    listTrackRegs.Add(new TrackingRegister
                    {
                        TrackingRegisterDetailsId = (int)x.PurchaseOrderDetailsId,
                        TrackingRegisterVoucherNo = x.PurchaseOrderDetailsNo,
                        TrackingRegisterMaterialId = (int)x.PurchaseOrderDetailsMatId,
                        TrackingRegisterVoucherType = purchaseOrder.PurchaseOrderType,
                        TrackingRegisterQty = (int)x.PurchaseOrderDetailsQuantity,
                        TrackingRegisterQtyin = (int)x.PurchaseOrderDetailsQuantity,
                        TrackingRegisterQtyout = 0,
                        TrackingRegisterRate = x.PurchaseOrderDetailsRate,
                        TrackingRegisterTrackDate = purchaseOrder.PurchaseOrderDate,
                        TrackingRegisterFsno = null,//to be alloted
                    });

                    listTrackPurchOrderReg.Add(new PurOrderRegister
                    {
                        PurOrderRegisterOrderNo = x.PurchaseOrderDetailsId.ToString(),
                        PurOrderRegisterRefVoucherNo = x.PurchaseOrderDetailsNo,
                        PurOrderRegisterMaterialId = (int)x.PurchaseOrderDetailsMatId,
                        PurOrderRegisterTransType = purchaseOrder.PurchaseOrderType,
                        PurOrderRegisterQtyOrder = x.PurchaseOrderDetailsQuantity,
                        PurOrderRegisterQtyIssued = 0,
                        PurOrderRegisterRate = x.PurchaseOrderDetailsRate,
                        PurOrderRegisterAssignedDate = DateTime.Now,
                        PurOrderRegisterFsno = null,//to be alloted,
                        PurOrderRegisterSupplierId = (int)purchaseOrder.PurchaseOrderPartyId,
                        PurOrderRegisterStatus = "A"

                    });
                });

                //foreach (var purOrderDet in purchaseOrder.PurchaseOrderDetails)
                //{
                //    var tr = new TrackingRegister
                //    {
                //        TrackingRegisterDetailsId = (int)purOrderDet.PurchaseOrderDetailsId,
                //        TrackingRegisterVoucherNo = purOrderDet.PurchaseOrderDetailsNo,
                //        TrackingRegisterMaterialId = (int)purOrderDet.PurchaseOrderDetailsMatId,
                //        TrackingRegisterVoucherType = purchaseOrder.PurchaseOrderType,
                //        TrackingRegisterQty = (int)purOrderDet.PurchaseOrderDetailsQuantity,
                //        TrackingRegisterQtyin = (int)purOrderDet.PurchaseOrderDetailsQuantity,
                //        TrackingRegisterQtyout = 0,
                //        TrackingRegisterRate = purOrderDet.PurchaseOrderDetailsRate,
                //        TrackingRegisterTrackDate = purchaseOrder.PurchaseOrderDate,
                //        TrackingRegisterFsno = null,//to be alloted

                //    };
                //    listTrackRegs.Add(tr);

                //    var poReg = new PurOrderRegister
                //    {
                //        PurOrderRegisterOrderNo = purOrderDet.PurchaseOrderDetailsId.ToString(),
                //        PurOrderRegisterRefVoucherNo = purOrderDet.PurchaseOrderDetailsNo,
                //        PurOrderRegisterMaterialId = (int)purOrderDet.PurchaseOrderDetailsMatId,
                //        PurOrderRegisterTransType = purchaseOrder.PurchaseOrderType,
                //        PurOrderRegisterQtyOrder = purOrderDet.PurchaseOrderDetailsQuantity,
                //        PurOrderRegisterQtyIssued = 0,
                //        PurOrderRegisterRate = purOrderDet.PurchaseOrderDetailsRate,
                //        PurOrderRegisterAssignedDate = DateTime.Now,
                //        PurOrderRegisterFsno = null,//to be alloted,
                //        PurOrderRegisterSupplierId = (int)purchaseOrder.PurchaseOrderPartyId,
                //        PurOrderRegisterStatus = "A"

                //    };
                //    listTrackPurchOrderReg.Add(poReg);

                //}

                _trackingRegisterRepository.InsertList(listTrackRegs);
                _purOrderRegisterRepository.InsertList(listTrackPurchOrderReg);
                List<PermissionApprovalDetail> permissionApprovalDetails = new List<PermissionApprovalDetail>();
                var rights = _context.tbl_approvalforms.Where(a => a.Voucher_Type.Contains("Purchase Order") && a.is_active == true).AsNoTracking().ToList();
                if (rights.Count > 0)
                {
                    permissionApprovalDetails.Clear();
                    foreach (var item in rights)
                    {
                        for (int level = 1; level <= item.NoOfLevel; level++)
                        {
                            PermissionApprovalDetail POApproversToAdd = new PermissionApprovalDetail();
                            POApproversToAdd.VoucherType = "Purchase Order";
                            POApproversToAdd.VoucherNo = purchaseOrder.PurchaseOrderNo;
                            POApproversToAdd.VoucherId = (int)purchaseOrder.PurchaseOrderId;
                            POApproversToAdd.VoucherDate = purchaseOrder.PurchaseOrderDate;
                            POApproversToAdd.Amount = (double)purchaseOrder.PurchaseOrderNetAmount;
                            POApproversToAdd.CreatedBy = (int?)purchaseOrder.PurchaseOrderUserId ?? 1;
                            POApproversToAdd.LevelId = level;
                            POApproversToAdd.Status = "Pending";
                            POApproversToAdd.JobId = (int)purchaseOrder.PurchaseOrderJobId;
                            POApproversToAdd.Remarks = purchaseOrder.PurchaseOrderDescription;
                            POApproversToAdd.LocationName = _context.LocationMaster
                                        .FirstOrDefault(l => l.LocationMasterLocationId == purchaseOrder.PurchaseOrderLocationId)?.LocationMasterLocationName;
                            permissionApprovalDetails.Add(POApproversToAdd);
                        }
                    }
                    if (permissionApprovalDetails.Any())
                        _permissionApprovalDetailRepo.InsertList(permissionApprovalDetails);
                }


                #region EmailNotification
                _orderEmailNotificationService.SendOrderEmailNotificationAsync(
                                                                              EnumOrderForms.PurchaseOrder,
                                                                              EnumApproverLevels.Level1,
                                                                              "Order",
                                                                              purchaseOrder.PurchaseOrderNo,
                                                                              purchaseOrder.PurchaseOrderDate,
                                                                              purchaseOrder.PurchaseOrderPartyName,
                                                                              (decimal)purchaseOrder.PurchaseOrderNetAmount,
                                                                              (int)purchaseOrder.PurchaseOrderId).ConfigureAwait(false);
                #endregion

                _purchaseOrderRepository.TransactionCommit();

                return this.GetSavedPurchaseOrderDetails((int)purchaseOrder.PurchaseOrderId);
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
            return purchaseOrder_ALL;
        }
        public PurchaseOrderModel GetSavedPurchaseOrderDetails(int id)
        {
            PurchaseOrderModel purchaseOrderModel = new PurchaseOrderModel();
            purchaseOrderModel.purchaseOrder = _purchaseOrderRepository.GetAsQueryable().Where(k => k.PurchaseOrderId == id).SingleOrDefault();
            try
            {
                purchaseOrderModel.accountsTransactions = _accountTransactionRepository.GetAsQueryable().Where(c => c.AccountsTransactionsVoucherNo == purchaseOrderModel.purchaseOrder.PurchaseOrderNo && c.AccountsTransactionsVoucherType == VoucherType.PurchaseOrder_TYPE && (c.AccountstransactionsDelStatus == false || c.AccountstransactionsDelStatus == null)).ToList();
            }
            catch
            {
            }
            purchaseOrderModel.purchaseOrderDetails = _purchaseOrderDetailsRepository.GetAsQueryable().Where(x => x.PurchaseOrderDetailsNo == purchaseOrderModel.purchaseOrder.PurchaseOrderNo && (x.PurchaseOrderDetailsDelStatus == false || x.PurchaseOrderDetailsDelStatus == null)).ToList();

            foreach (var purchaseOrderDetail in purchaseOrderModel.purchaseOrderDetails)
            {
                if (_stockRegisterRepository.GetAsQueryable().Where(c => c.StockRegisterRefVoucherNo == purchaseOrderDetail.PurchaseOrderDetailsNo).FirstOrDefault() != null)
                {
                    purchaseOrderDetail.PurchaseOrderNetStock = _stockRegisterRepository.GetAsQueryable().Where(c => c.StockRegisterRefVoucherNo == purchaseOrderDetail.PurchaseOrderDetailsNo).FirstOrDefault().StockRegisterSIN;

                }
                purchaseOrderDetail.PurchaseOrderNetStock = 0;
                purchaseOrderDetail.PurchaseOrderTillNowQTY = GetTillNowQTY(purchaseOrderModel.purchaseOrder.PurchaseOrderNo);
            }

            var approvals = _permissionApprovalDetailRepo.GetAsQueryable().AsNoTracking().Where(a => a.VoucherId == purchaseOrderModel.purchaseOrder.PurchaseOrderId).Select(x => new { x.Status, x.ApprovedBy }).ToList();
            if (approvals.Count > 0)
            {
                if (approvals.Any(a => a.Status == "Approved"))
                {
                    purchaseOrderModel.purchaseOrder.PurchaseOrderApproveUserId = approvals[0].ApprovedBy;
                    purchaseOrderModel.purchaseOrder.PurchaseOrderApproveEnable = true;
                }
                else
                {
                    purchaseOrderModel.purchaseOrder.PurchaseOrderApproveUserId = approvals[0].ApprovedBy;
                    purchaseOrderModel.purchaseOrder.PurchaseOrderApproveEnable = false;
                }
            }

            return purchaseOrderModel;

        }
        private decimal? GetTillNowQTY(string pvno)
        {
            try
            {
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
                    }
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
        public IQueryable GetPODetailsForGRN()
        {
            try
            {
                var materialList = _itemMasterRepository.GetAsQueryable().Where(a => a.ItemMasterDelStatus != true).ToList();
                var datalist = _purchaseOrderDetailsRepository.GetAsQueryable().Where(a => a.PurchaseOrderDetailsDelStatus != true).ToList();
                var data = datalist.Select(d => new
                {
                    purchaseOrderDetailsNo = d.PurchaseOrderDetailsNo,
                    itemId = d.PurchaseOrderDetailsMatId,
                    itemObj = materialList.FirstOrDefault(m => m.ItemMasterItemId == d.PurchaseOrderDetailsMatId),
                    unitId = d.PurchaseOrderDetailsUnitId,
                    poQty = d.PurchaseOrderDetailsQuantity,
                }).AsQueryable();
                return data;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
        public List<PurchaseOrderDTO> GetAllPOForGRN(decimal id)
        {
            try
            {
                var materialList = _itemMasterRepository.GetAsQueryable().Where(a => a.ItemMasterDelStatus != true).Select(c => new
                {
                    c.ItemMasterItemName,
                    c.ItemMasterItemId,
                    c.ItemMasterLastPurchasePrice
                });
                var unitList = _unitMasterRepository.GetAsQueryable().Where(a => a.UnitMasterUnitDelStatus != true).Select(c => new
                {
                    c.UnitMasterUnitId,
                    c.UnitMasterUnitShortName,
                    c.UnitMasterUnitFullName
                }).OrderBy(x => x.UnitMasterUnitShortName).AsEnumerable().ToList();
                var detailList = _purchaseOrderDetailsRepository.GetAsQueryable().ToList();
                var details = _purchaseOrderRepository.GetAsQueryable().Where(a => a.PurchaseOrderDelStatus != true && a.PurchaseOrderPartyId == id).ToList();
                var data = details.Select(o => new PurchaseOrderDTO
                {
                    PurchaseOrder = o,
                    PurchaseOrderDetails = detailList.Where(a => a.PurchaseOrderDetailsNo == o.PurchaseOrderNo).Select(d => new PurchaseOrderDetailsDTO
                    {
                        itemId = d.PurchaseOrderDetailsMatId,
                        itemname = materialList.Where(m => m.ItemMasterItemId == (long?)d.PurchaseOrderDetailsMatId).Select(c => c.ItemMasterItemName ?? "").FirstOrDefault() ?? "",
                        unitId = d.PurchaseOrderDetailsUnitId ?? 0,
                        unit = unitList.Where(u => u.UnitMasterUnitId == (int?)d.PurchaseOrderDetailsUnitId).Select(c => c.UnitMasterUnitFullName ?? "").FirstOrDefault() ?? "",
                        price = materialList.Where(m => m.ItemMasterItemId == (long?)d.PurchaseOrderDetailsMatId).Select(c => (decimal?)c.ItemMasterLastPurchasePrice).FirstOrDefault() ?? 0,
                        poQty = d.PurchaseOrderDetailsQuantity ?? 0,
                    }).ToList()
                }).OrderBy(x => x.PurchaseOrder).AsEnumerable().ToList();
                return data;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
        //saleem
        public async Task<Response<List<PODetailsViewModel>>> GetGRMPOs(int? supplierId)
        {
            try
            {
                var check = _context.tbl_approvalforms.AsNoTracking().Where(a => a.Voucher_Type.Trim() == "Purchase Order" && a.is_active == true).ToList();
                //check = null;

                List<PODetailsViewModel> result = null;
                if (check?.Count > 0)
                {
                    result = await (from po in _purchaseOrderRepository.GetAsQueryable()
                                    join per in _permissionApprovalDetailRepo.GetAsQueryable() on po.PurchaseOrderId equals per.VoucherId
                                    join pod in _purchaseOrderDetailsRepository.GetAsQueryable() on po.PurchaseOrderId equals pod.PurchaseOrderId
                                    join sm in _suppliersMasterRepository.GetAsQueryable() on po.PurchaseOrderPartyId equals sm.SuppliersMasterSupplierId
                                    join im in _itemMasterRepository.GetAsQueryable() on (long)pod.PurchaseOrderDetailsMatId equals im.ItemMasterItemId
                                    //where (supplierId == null || supplierId == 0 || sm.SuppliersMasterSupplierId == supplierId) && per.Status == "Approved"
                                    where (supplierId == null || supplierId == 0 || sm.SuppliersMasterSupplierId == supplierId) && _permissionApprovalDetailRepo.GetAsQueryable().Where(p => p.VoucherNo == po.PurchaseOrderNo && (p.IsDeleted == null || (bool)!p.IsDeleted))
                                   .Any(p => p.Status == "Approved" && p.Status != "Rejected")
                                    group new { po, pod, sm, im } by new
                                    {
                                        pod.PurchaseOrderDetailsId,
                                        pod.PurchaseOrderDetailsMatId,
                                        im.ItemMasterItemName,
                                        po.PurchaseOrderId,
                                        po.PurchaseOrderNo,
                                        po.PurchaseOrderDate,
                                        sm.SuppliersMasterSupplierName,
                                        pod.PurchaseOrderDetailsRate,
                                        pod.PurchaseOrderDetailsQuantity,
                                        pod.PurchaseOrderDetailsUnitId
                                    } into g
                                    select new PODetailsViewModel
                                    {
                                        ItemId = (int)g.Key.PurchaseOrderDetailsMatId,
                                        ItemName = g.Key.ItemMasterItemName,
                                        POId = (int)g.Key.PurchaseOrderId,
                                        PODId = (int)g.Key.PurchaseOrderDetailsId,
                                        PONo = g.Key.PurchaseOrderNo,
                                        PODate = g.Key.PurchaseOrderDate,
                                        SupplierName = g.Key.SuppliersMasterSupplierName,
                                        Rate = g.Key.PurchaseOrderDetailsRate,
                                        Quantity = (int)g.Key.PurchaseOrderDetailsQuantity,
                                        UnitId = g.Key.PurchaseOrderDetailsUnitId,
                                        UnitName = _unitMasterRepository.GetAsQueryable()
                                                    .Where(x => x.UnitMasterUnitId == g.Key.PurchaseOrderDetailsUnitId)
                                                    .Select(x => x.UnitMasterUnitFullName.Trim())
                                                    .FirstOrDefault(),
                                        BalanceQuantity = (int)g.Key.PurchaseOrderDetailsQuantity - (
                                            _purchaseVoucherDetailsRepo.GetAsQueryable()
                                                .Where(pr => pr.PurchaseVoucherDetailsPodId == g.Key.PurchaseOrderDetailsId
                                                        && pr.PurchaseVoucherDetailsMaterialId == g.Key.PurchaseOrderDetailsMatId)
                                                .Sum(prdq => (int?)prdq.PurchaseVoucherDetailsQuantity) ?? 0
                                        ),
                                        DeliveredQuantity = _purchaseVoucherDetailsRepo.GetAsQueryable()
                                            .Where(pr => pr.PurchaseVoucherDetailsPodId == g.Key.PurchaseOrderDetailsId
                                                    && pr.PurchaseVoucherDetailsMaterialId == g.Key.PurchaseOrderDetailsMatId)
                                            .Sum(prdq => (int?)prdq.PurchaseVoucherDetailsQuantity) ?? 0,
                                    })
                                    .OrderBy(x => x.PODate)
                                    .ThenBy(x => x.PONo)
                                    .ToListAsync();
                }
                else
                {
                    result = await (from po in _purchaseOrderRepository.GetAsQueryable()
                                    join pod in _purchaseOrderDetailsRepository.GetAsQueryable() on po.PurchaseOrderId equals pod.PurchaseOrderId
                                    join sm in _suppliersMasterRepository.GetAsQueryable() on po.PurchaseOrderPartyId equals sm.SuppliersMasterSupplierId
                                    join im in _itemMasterRepository.GetAsQueryable() on (long)pod.PurchaseOrderDetailsMatId equals im.ItemMasterItemId
                                    where (supplierId == null || supplierId == 0 || sm.SuppliersMasterSupplierId == supplierId)
                                    select new PODetailsViewModel
                                    {
                                        ItemId = (int)pod.PurchaseOrderDetailsMatId,
                                        ItemName = im.ItemMasterItemName,
                                        POId = (int)po.PurchaseOrderId,
                                        PODId = (int)pod.PurchaseOrderDetailsId,
                                        PONo = po.PurchaseOrderNo,
                                        PODate = po.PurchaseOrderDate,
                                        //JobId = (int)po.PurchaseOrderJobId,                                        
                                        SupplierName = sm.SuppliersMasterSupplierName,
                                        Rate = pod.PurchaseOrderDetailsRate,
                                        Quantity = (int)pod.PurchaseOrderDetailsQuantity,
                                        UnitId = pod.PurchaseOrderDetailsUnitId,
                                        UnitName = _unitMasterRepository.GetAsQueryable().Where(x => x.UnitMasterUnitId == pod.PurchaseOrderDetailsUnitId).FirstOrDefault().UnitMasterUnitFullName.Trim(),
                                        BalanceQuantity = (int)pod.PurchaseOrderDetailsQuantity - (_purchaseVoucherDetailsRepo.GetAsQueryable().Where(pr =>
                                        (pr.PurchaseVoucherDetailsPodId == pod.PurchaseOrderDetailsId
                                           && pr.PurchaseVoucherDetailsMaterialId == pod.PurchaseOrderDetailsMatId))
                                        .Sum(prdq => (int)((prdq.PurchaseVoucherDetailsQuantity) ?? 0))),

                                        DeliveredQuantity = (_purchaseVoucherDetailsRepo.GetAsQueryable().Where(pr =>
                                                (pr.PurchaseVoucherDetailsPodId == pod.PurchaseOrderDetailsId
                                                 && pr.PurchaseVoucherDetailsMaterialId == pod.PurchaseOrderDetailsMatId))
                                        .Sum(prdq => (int)((prdq.PurchaseVoucherDetailsQuantity) ?? 0))),

                                    })
                                 .OrderBy(x => x.PODate).ThenBy(x => x.PONo)
                                 .ToListAsync();
                }
                return Response<List<PODetailsViewModel>>.Success(result, "Data found");
            }
            catch (System.Exception ex)
            {
                return Response<List<PODetailsViewModel>>.Fail(new List<PODetailsViewModel>(), ex.Message);
            }
        }

        public bool IsPurchaseOrderQtyExcededFromBOQ(int PurchaseReqId, List<PurchaseReqItesm> reqItems)
        {
            try
            {
                var itemIds = reqItems.Select(x => x.ItemCode).ToList();

                var totalPurReqQty = this._purchaseRequisitionDetailsRepository.GetAsQueryable()
                    .AsNoTracking()
                    .Where(x => x.PurchaseRequisitionId == PurchaseReqId && itemIds.Contains((int)x.PurchaseRequisitionDetailsMatId))
                    .GroupBy(x => new { x.PurchaseRequisitionId, x.PurchaseRequisitionDetailsMatId })
                    .Select(x => new
                    {
                        ItemCode = x.Key.PurchaseRequisitionDetailsMatId,
                        ItemQty = x.Sum(x => x.PurchaseRequisitionDetailsQuantity)
                    }).ToList();

                var totalPurOrdrQty = this._purchaseOrderDetailsRepository.GetAsQueryable()
                    .Where(x => x.PurchaseRequisitionId == PurchaseReqId && itemIds.Contains((int)x.PurchaseOrderDetailsMatId))
                    .GroupBy(x => new { x.PurchaseRequisitionId, x.PurchaseOrderDetailsMatId })
                    .Select(x => new
                    {
                        ItemCode = x.Key.PurchaseOrderDetailsMatId,
                        ItemQty = x.Sum(x => x.PurchaseOrderDetailsQuantity)
                    }).ToList();

                var result = totalPurOrdrQty.Join(reqItems,
                    pr => pr.ItemCode,
                    rq => rq.ItemCode,
                    (pr, rq) => new
                    {
                        ItemCode = pr.ItemCode,
                        ItemQty = pr.ItemQty + rq.ItemQty
                    }).ToList();

                if (!totalPurOrdrQty.Any())
                    return totalPurReqQty.Any(boq => reqItems.Any(r => r.ItemCode == boq.ItemCode && r.ItemQty > boq.ItemQty));
                if (totalPurReqQty.Any(boq => result.Any(r => r.ItemCode == boq.ItemCode && r.ItemQty > boq.ItemQty)))
                    return true;
                if (totalPurReqQty.Any(boq => totalPurOrdrQty.Any(r => r.ItemCode == boq.ItemCode && r.ItemQty == boq.ItemQty)))
                    return false;

                return totalPurReqQty.Any(boq => result.Any(r => r.ItemCode == boq.ItemCode && r.ItemQty > boq.ItemQty));
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public async Task<Response<List<VoucherApprivalDto>>> GetPOPRForApproval(ApprovalFilterModel model)
        {
            try
            {
                var user = _cacheMemoryService.GetUserObject(InspireConstants.UserCacheKey) as UserFile;
                var poQuery = _purchaseOrderRepository
                .GetWithInclude(x => true, inc => inc.OrderApprovalHierarchies)
                .AsNoTracking();

                var prQuery = _purchaseRequisitionRepository
                                .GetWithInclude(x => true, inc => inc.OrderApprovalHierarchies)
                                .AsNoTracking();

                if (model.IsDate)
                {
                    poQuery = poQuery.Where(x => x.PurchaseOrderDate.Date >= model.fromDate.Value.Date
                                              && x.PurchaseOrderDate.Date <= model.toDate.Value.Date);

                    prQuery = prQuery.Where(x => x.PurchaseRequisitionDate.Date >= model.fromDate.Value.Date
                                              && x.PurchaseRequisitionDate.Date <= model.toDate.Value.Date);
                }

                if (model.JobId > 0)
                {
                    poQuery = poQuery.Where(x => x.PurchaseOrderJobId == model.JobId);
                    prQuery = prQuery.Where(x => x.PurchaseRequisitionJobId == model.JobId);
                }

                var approvers = _approverLevelSettings.GetAsQueryable()
                    .Where(x => new[] { EnumOrderForms.PurchaseOrder, EnumOrderForms.PurchaseRequisition }.Contains(x.FormType))
                    .Select(s => new
                    {
                        s.FormType,
                        s.ApproverId,
                        s.ApproverLevel
                    }).ToList();

                var poData = poQuery.ToList().Select(x => new VoucherApprivalDto
                {
                    VoucherId = (int)x.PurchaseOrderId,
                    VoucherType = EnumOrderForms.PurchaseOrder,
                    VoucherNo = x.PurchaseOrderNo,
                    VoucherDate = x.PurchaseOrderDate,
                    LocationName = "",
                    JobId = (int)x.PurchaseOrderJobId,
                    JobName = "",
                    Amount = (decimal)x.PurchaseOrderNetAmount,
                    Status = x.PurchaseOrderApproveStatus,
                    Remarks = $"{x.PurchaseOrderRemarks1}\n{x.PurchaseOrderRemarks2}\n{x.PurchaseOrderRemarks3}",
                    VoucherApprovers = x.OrderApprovalHierarchies.Where(x => user.WorkGroupId == 1 ? true : x.ApproverId == user.UserId).Select(oh => new VoucherApprover
                    {
                        ApproverId = (int)oh.ApproverId,
                        ApproverLevel = approvers.FirstOrDefault(x => x.ApproverId == oh.ApproverId && x.FormType == EnumOrderForms.PurchaseOrder)?.ApproverLevel ?? EnumApproverLevels.None,
                        Status = oh.Status
                    }).ToList()
                }).ToList();

                var prData = prQuery.ToList().Select(x => new VoucherApprivalDto()
                {
                    VoucherId = (int)x.PurchaseRequisitionId,
                    VoucherType = EnumOrderForms.PurchaseRequisition,
                    VoucherNo = x.PurchaseRequisitionNo,
                    VoucherDate = x.PurchaseRequisitionDate,
                    LocationName = "",
                    JobId = (int)x.PurchaseRequisitionJobId,
                    JobName = "",
                    Amount = (decimal)x.PurchaseRequisitionNetAmount,
                    Status = x.PurchaseRequisitionStatus,
                    Remarks = x.PurchaseRequisitionRemarks,
                    VoucherApprovers = x.OrderApprovalHierarchies.Where(x => user.WorkGroupId == 1 ? true : x.ApproverId == user.UserId).Select(oh => new VoucherApprover()
                    {
                        ApproverId = (int)oh.ApproverId,
                        ApproverLevel = approvers.FirstOrDefault(x => x.ApproverId == oh.ApproverId && x.FormType == EnumOrderForms.PurchaseOrder)?.ApproverLevel ?? EnumApproverLevels.None,
                        Status = oh.Status
                    }).ToList()
                }).ToList();

                var CombinedData = poData.Concat(prData).ToList();

                return Response<List<VoucherApprivalDto>>.Success(CombinedData, "Data found");

            }
            catch (Exception)
            {

                throw;
            }
        }

        public Task<Response<VoucherApprivalDto>> ApprovePOPR(int orderId, EnumOrderForms orderType, EnumApprovalStatus approvalStatus)
        {
            try
            {
                _orderApprovalHierarchy.BeginTransaction();
                var user = _cacheMemoryService.GetUserObject(InspireConstants.UserCacheKey) as UserFile;
                var approvers = _approverLevelSettings.GetAsQueryable()
                 .Where(x => x.FormType == orderType)
                  .ToList();

                var poprToApprove = _orderApprovalHierarchy.GetAsQueryable().Where(x => x.OrderId == orderId && x.OrderType == orderType && x.ApproverId == user.UserId).ToList();
                poprToApprove.ForEach(x => x.Status = approvalStatus);
                _orderApprovalHierarchy.UpdateList(poprToApprove);

                EnumApproverLevels nextLevelToSendEamil = GetNextApproverLevel(user.UserId, approvers);
                if (nextLevelToSendEamil != EnumApproverLevels.None)
                {
                    List<object> rptData = new List<object>();
                    var po = orderType == EnumOrderForms.PurchaseOrder ? GetSavedPurchaseOrderDetails(orderId) : null;
                    var pr = orderType == EnumOrderForms.PurchaseRequisition ? _purchaseRequisitionService.GetSavedPurchaseRequisitionDetails(orderId) : null;

                    _orderEmailNotificationService.SendOrderEmailNotificationAsync(
                                                                                po != null ? EnumOrderForms.PurchaseOrder : EnumOrderForms.PurchaseRequisition,
                                                                                nextLevelToSendEamil,
                                                                                 po != null ? "Order" : "Requisition",
                                                                                 po != null ? po.purchaseOrder.PurchaseOrderNo : pr.purchaseRequisition.PurchaseRequisitionNo,
                                                                                 po != null ? po.purchaseOrder.PurchaseOrderDate : pr.purchaseRequisition.PurchaseRequisitionDate,
                                                                                 po != null ? po.purchaseOrder.PurchaseOrderPartyName ?? "" : pr.purchaseRequisition.PurchaseRequisitionPartyName ?? "",
                                                                                 po != null ? (decimal)po.purchaseOrder.PurchaseOrderNetAmount : (decimal)pr.purchaseRequisition.PurchaseRequisitionNetAmount,
                                                                                 po == null ? (int)pr.purchaseRequisition.PurchaseRequisitionId : (int)po.purchaseOrder.PurchaseOrderId,
                                                                                 false).ConfigureAwait(false);
                }
                _orderApprovalHierarchy.TransactionCommit();

                return GetDefaultPOPR(orderId, orderType);
            }
            catch (Exception)
            {
                _orderApprovalHierarchy.TransactionRollback();
                throw;
            }
        }

        private EnumApproverLevels GetNextApproverLevel(long userId, List<ApproverLevelSettings> approvers)
        {
            var currentApprover = approvers.FirstOrDefault(a => a.ApproverId == userId);
            var nextLevel = approvers
                .SkipWhile(a => a.ApproverLevel != currentApprover.ApproverLevel)
                .Skip(1)
                .FirstOrDefault();

            return nextLevel?.ApproverLevel ?? EnumApproverLevels.None;
        }

        public async Task<Response<VoucherApprivalDto>> GetDefaultPOPR(int orderId, EnumOrderForms orderType)
        {
            try
            {
                var user = _cacheMemoryService.GetUserObject(InspireConstants.UserCacheKey) as UserFile;

                var approvers = _approverLevelSettings.GetAsQueryable()
                    .Where(x => x.FormType == orderType && x.ApproverId == user.UserId)
                    .Select(s => new
                    {
                        s.FormType,
                        s.ApproverId,
                        s.ApproverLevel
                    }).ToList();

                if (orderType == EnumOrderForms.PurchaseOrder)
                {
                    var poData = _purchaseOrderRepository.GetWithInclude(x => true, inc => inc.OrderApprovalHierarchies)
                    .AsNoTracking().Where(x => x.PurchaseOrderId == orderId).Select(x => new VoucherApprivalDto
                    {
                        VoucherId = (int)x.PurchaseOrderId,
                        VoucherType = EnumOrderForms.PurchaseOrder,
                        VoucherNo = x.PurchaseOrderNo,
                        VoucherDate = x.PurchaseOrderDate,
                        LocationName = "",
                        JobId = (int)x.PurchaseOrderJobId,
                        JobName = "",
                        Amount = (decimal)x.PurchaseOrderNetAmount,
                        Status = x.PurchaseOrderApproveStatus,
                        Remarks = $"{x.PurchaseOrderRemarks1}\n{x.PurchaseOrderRemarks2}\n{x.PurchaseOrderRemarks3}",
                        VoucherApprovers = x.OrderApprovalHierarchies.Where(ah => ah.ApproverId == user.UserId).Select(x => new VoucherApprover()
                        {
                            ApproverId = (int)x.ApproverId,
                            Status = x.Status
                        }).ToList()
                    }).FirstOrDefault();

                    return Response<VoucherApprivalDto>.Success(poData, "Data found");
                }
                else if (orderType == EnumOrderForms.PurchaseOrder)
                {
                    var prData = _purchaseRequisitionRepository
                                .GetWithInclude(x => true, inc => inc.OrderApprovalHierarchies)
                                .AsNoTracking().Where(x => x.PurchaseRequisitionId == orderId).Select(x => new VoucherApprivalDto()
                                {
                                    VoucherId = (int)x.PurchaseRequisitionId,
                                    VoucherType = EnumOrderForms.PurchaseRequisition,
                                    VoucherNo = x.PurchaseRequisitionNo,
                                    VoucherDate = x.PurchaseRequisitionDate,
                                    LocationName = "",
                                    JobId = (int)x.PurchaseRequisitionJobId,
                                    JobName = "",
                                    Amount = (decimal)x.PurchaseRequisitionNetAmount,
                                    Status = x.PurchaseRequisitionStatus,
                                    Remarks = x.PurchaseRequisitionRemarks,
                                    VoucherApprovers = x.OrderApprovalHierarchies.Where(ah => ah.ApproverId == user.UserId).Select(oh => new VoucherApprover()
                                    {
                                        ApproverId = (int)oh.ApproverId,
                                        Status = oh.Status
                                    }).ToList()
                                }).FirstOrDefault();
                    return Response<VoucherApprivalDto>.Success(prData, "Data found");
                }


                return Response<VoucherApprivalDto>.Success(new VoucherApprivalDto(), "No Order Type were found");

            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}


