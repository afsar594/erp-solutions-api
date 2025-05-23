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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Inspire.Erp.Application.Settings.Interfaces;

namespace Inspire.Erp.Application.Procurement.Implementation
{
    public class PurchaseOrderService : IPurchaseOrderService
    {
        private readonly IGeneralSettings _generalSettings;
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
        private readonly IRepository<CustomerQuotation> _customerQuotation;
        private readonly IRepository<JobMaster> _jobMaster;
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
            IRepository<PermissionApprovalDetail> permissionApprovalDetailRepo, IRepository<PurchaseRequisitionDetails> purchaseRequisitionDetailsRepository, IRepository<Permissionapproval> permissionapproval, IRepository<ApproverLevelSettings> approverLevelSettings, IOrderEmailNotificationService orderEmailNotificationService, IRepository<PurchaseRequisition> purchaseRequisitionRepository, IRepository<OrderApprovalHierarchy> orderApprovalHierarchy, IPurchaseRequisitionService purchaseRequisitionService, IRepository<CustomerQuotation> customerQuotation, IRepository<JobMaster> jobMaster, IGeneralSettings generalSettings)
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
            _customerQuotation = customerQuotation;
            _jobMaster = jobMaster;
            _generalSettings = generalSettings;
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

        public async Task<Response<PurchaseOrderModel>> UpdatePurchaseOrder(PurchaseOrder purchaseOrder, List<AccountsTransactions> accountsTransactions,
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

            return await this.GetSavedPurchaseOrderDetails((int)purchaseOrder.PurchaseOrderId);
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
        public async Task<Response<int>> DeletePurchaseOrder(PurchaseOrder purchaseOrder, List<AccountsTransactions> accountsTransactions,
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

            return Response<int>.Success(i, "Deledted");
        }
        public async Task<Response<PurchaseOrderModel>> InsertPurchaseOrder(PurchaseOrder purchaseOrder,
                                                                                        List<AccountsTransactions> accountsTransactions,
                                                                                        List<PurchaseOrderDetails> purchaseOrderDetails)
        {
            try
            {
                var settingValue = await _generalSettings.GetApproverStatusAsync(purchaseOrder.PurchaseOrderJobId.ToString(), nameof(EnumOrderForms.PurchaseOrder));

                if (!string.IsNullOrEmpty(settingValue.ApproverStatus))
                    return Response<PurchaseOrderModel>.Fail(null, settingValue.ApproverStatus);

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
                //List<PermissionApprovalDetail> permissionApprovalDetails = new List<PermissionApprovalDetail>();
                //var rights = _context.tbl_approvalforms.Where(a => a.Voucher_Type.Contains("Purchase Order") && a.is_active == true).AsNoTracking().ToList();
                //if (rights.Count > 0)
                //{
                //    permissionApprovalDetails.Clear();
                //    foreach (var item in rights)
                //    {
                //        for (int level = 1; level <= item.NoOfLevel; level++)
                //        {
                //            PermissionApprovalDetail POApproversToAdd = new PermissionApprovalDetail();
                //            POApproversToAdd.VoucherType = "Purchase Order";
                //            POApproversToAdd.VoucherNo = purchaseOrder.PurchaseOrderNo;
                //            POApproversToAdd.VoucherId = (int)purchaseOrder.PurchaseOrderId;
                //            POApproversToAdd.VoucherDate = purchaseOrder.PurchaseOrderDate;
                //            POApproversToAdd.Amount = (double)purchaseOrder.PurchaseOrderNetAmount;
                //            POApproversToAdd.CreatedBy = (int?)purchaseOrder.PurchaseOrderUserId ?? 1;
                //            POApproversToAdd.LevelId = level;
                //            POApproversToAdd.Status = "Pending";
                //            POApproversToAdd.JobId = (int)purchaseOrder.PurchaseOrderJobId;
                //            POApproversToAdd.Remarks = purchaseOrder.PurchaseOrderDescription;
                //            POApproversToAdd.LocationName = _context.LocationMaster
                //                        .FirstOrDefault(l => l.LocationMasterLocationId == purchaseOrder.PurchaseOrderLocationId)?.LocationMasterLocationName;
                //            permissionApprovalDetails.Add(POApproversToAdd);
                //        }
                //    }
                //    if (permissionApprovalDetails.Any())
                //        _permissionApprovalDetailRepo.InsertList(permissionApprovalDetails);
                //}


                #region EmailNotification
                var user = _cacheMemoryService.GetUserObject(InspireConstants.UserCacheKey) as UserFile;
                var jobId = purchaseOrder?.PurchaseOrderJobId ?? -1;
                var jobDetail = _jobMaster.GetAsQueryable().Where(x => x.JobMasterJobId == jobId).FirstOrDefault();
                await _orderEmailNotificationService.SendOrderEmailNotificationAsync(
                                                                              EnumOrderForms.PurchaseOrder,
                                                                              EnumApproverLevels.Level1,
                                                                              "Order",
                                                                              purchaseOrder.PurchaseOrderNo,
                                                                              purchaseOrder.PurchaseOrderDate,
                                                                              purchaseOrder.PurchaseOrderPartyName,
                                                                              (decimal)purchaseOrder.PurchaseOrderNetAmount,
                                                                              (int)purchaseOrder.PurchaseOrderId,
                                                                              (int)jobId,
                                                                               jobDetail == null ? "" : jobDetail.JobMasterJobNumber,
                                                                               jobDetail == null ? "" : jobDetail.JobMasterJobName,
                                                                               true,
                                                                               user.UserName ?? "",
                                                                               settingValue.GeneralSettingValue
                                                                              );
                #endregion

                _purchaseOrderRepository.TransactionCommit();

                var res = await this.GetSavedPurchaseOrderDetails((int)purchaseOrder.PurchaseOrderId);
                return Response<PurchaseOrderModel>.Success(res.Result, "Purchase Order Save Successfully.");
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
        public async Task<Response<PurchaseOrderModel>> GetSavedPurchaseOrderDetails(int id)
        {
            try
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
                var settingValue = await _generalSettings.GetApproverStatusAsync(id.ToString(), nameof(EnumOrderForms.PurchaseRequisition));
                purchaseOrderModel.purchaseOrder.IsPrintEnabled = settingValue.GeneralSettingValue == InspireConstants.ApprovalAsNoApproval ? true : await _orderApprovalHierarchy.GetAsQueryable().AsNoTracking().Where(x => x.OrderId == id).AllAsync(x => x.Status == EnumApprovalStatus.Approved);
                return Response<PurchaseOrderModel>.Success(purchaseOrderModel, "Data found");
            }
            catch (Exception)
            {

                throw;
            }
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

        public async Task<Response<List<PODetailsViewModel>>> GetGRMPOs(int? supplierId)
        {
            try
            {
                bool hasApprovals = _approverLevelSettings.GetAsQueryable()
                    .AsNoTracking()
                    .Any(a => a.FormType == EnumOrderForms.PurchaseOrder);

                var poQuery = _purchaseOrderRepository.GetAsQueryable();
                var podQuery = _purchaseOrderDetailsRepository.GetAsQueryable();
                var smQuery = _suppliersMasterRepository.GetAsQueryable();
                var imQuery = _itemMasterRepository.GetAsQueryable();
                var pvdQuery = _purchaseVoucherDetailsRepo.GetAsQueryable();
                var unitQuery = _unitMasterRepository.GetAsQueryable();
                var oahQuery = _orderApprovalHierarchy.GetAsQueryable();

                var result = await (
                    from po in poQuery
                    join pod in podQuery on po.PurchaseOrderId equals pod.PurchaseOrderId
                    join sm in smQuery on po.PurchaseOrderPartyId equals sm.SuppliersMasterSupplierId
                    join im in imQuery on (long)pod.PurchaseOrderDetailsMatId equals im.ItemMasterItemId
                    where supplierId == null || supplierId == 0 || sm.SuppliersMasterSupplierId == supplierId
                    let isApproved = hasApprovals
                        ? oahQuery
                            .Where(o => o.OrderId == po.PurchaseOrderId)
                            .All(o => o.Status == EnumApprovalStatus.Approved)
                        : true
                    where isApproved
                    select new PODetailsViewModel
                    {
                        ItemId = (int)pod.PurchaseOrderDetailsMatId,
                        ItemName = im.ItemMasterItemName,
                        POId = (int)po.PurchaseOrderId,
                        PODId = (int)pod.PurchaseOrderDetailsId,
                        PONo = po.PurchaseOrderNo,
                        PODate = po.PurchaseOrderDate,
                        SupplierName = sm.SuppliersMasterSupplierName,
                        Rate = pod.PurchaseOrderDetailsRate,
                        Quantity = (int)pod.PurchaseOrderDetailsQuantity,
                        UnitId = pod.PurchaseOrderDetailsUnitId,
                        UnitName = unitQuery
                            .Where(u => u.UnitMasterUnitId == pod.PurchaseOrderDetailsUnitId)
                            .Select(u => u.UnitMasterUnitFullName.Trim())
                            .FirstOrDefault(),
                        DeliveredQuantity = pvdQuery
                            .Where(pvd => pvd.PurchaseVoucherDetailsPodId == pod.PurchaseOrderDetailsId &&
                                          pvd.PurchaseVoucherDetailsMaterialId == pod.PurchaseOrderDetailsMatId)
                            .Sum(pvd => (int?)pvd.PurchaseVoucherDetailsQuantity) ?? 0,
                        BalanceQuantity = (int)pod.PurchaseOrderDetailsQuantity -
                                          (pvdQuery
                                            .Where(pvd => pvd.PurchaseVoucherDetailsPodId == pod.PurchaseOrderDetailsId &&
                                                          pvd.PurchaseVoucherDetailsMaterialId == pod.PurchaseOrderDetailsMatId)
                                            .Sum(pvd => (int?)pvd.PurchaseVoucherDetailsQuantity) ?? 0)
                    })
                    .OrderBy(po => po.PODate)
                    .ThenBy(po => po.PONo)
                    .ToListAsync();

                return Response<List<PODetailsViewModel>>.Success(result, "Data found");
            }
            catch (Exception ex)
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

        public List<VoucherApprivalDto> GetPurchaseOrderForApproval(ApprovalFilterModel model, UserFile user)
        {
            var fromDate = model.fromDate?.Date ?? DateTime.MinValue;
            var toDate = model.toDate?.Date ?? DateTime.MaxValue;
            var jobIdFilter = model.JobId < 0;



            var purchaseOrders = _purchaseOrderRepository.GetAsQueryable()
                .Where(po =>
                    (!model.IsDate || (po.PurchaseOrderDate.Date >= fromDate && po.PurchaseOrderDate.Date <= toDate)) &&
                    (jobIdFilter || po.PurchaseOrderJobId == model.JobId))
                .Include(po => po.OrderApprovalHierarchies)
                .AsNoTracking()
                .ToList();

            foreach (var po in purchaseOrders)
            {
                po.OrderApprovalHierarchies = po.OrderApprovalHierarchies
                    .Where(h => h.OrderType == EnumOrderForms.PurchaseOrder)
                    .ToList();
            }

            var locationDict = _locationMasterRepository.GetAsQueryable().Where(x => purchaseOrders.Select(x => x.PurchaseOrderLocationId).ToList().Contains(x.LocationMasterLocationId))
                .ToDictionary(l => l.LocationMasterLocationId, l => l.LocationMasterLocationName);

            var jobDict = _jobMaster.GetAsQueryable().Where(j => (jobIdFilter || j.JobMasterJobId == model.JobId))
                .ToDictionary(j => j.JobMasterJobId, j => j.JobMasterJobName);

            var approvalData = purchaseOrders.Select(po => new VoucherApprivalDto
            {
                VoucherId = (int)po.PurchaseOrderId,
                VoucherType = EnumOrderForms.PurchaseOrder,
                VoucherNo = po.PurchaseOrderNo,
                VoucherDate = po.PurchaseOrderDate,
                LocationId = po.PurchaseOrderLocationId,
                LocationName = locationDict.TryGetValue(po.PurchaseOrderLocationId, out var locName) ? locName ?? "" : "",
                JobId = po.PurchaseOrderJobId,
                JobName = jobDict.TryGetValue((int)po.PurchaseOrderJobId, out var jobName) ? jobName ?? "" : "",
                Amount = po.PurchaseOrderNetAmount ?? 0,
                Status = po.PurchaseOrderApproveStatus,
                Remarks = string.Join("\n", po.PurchaseOrderRemarks1, po.PurchaseOrderRemarks2, po.PurchaseOrderRemarks3).Trim(),

                VoucherApprovers = po.OrderApprovalHierarchies
                    .Where(oh =>
                        !model.IsUserRequired ||
                        (user.WorkGroupId == 1 || oh.ApproverId == user.UserId) &&
                        oh.OrderId == po.PurchaseOrderId &&
                        oh.Status != EnumApprovalStatus.Approved)
                    .Select(oh => new VoucherApprover
                    {
                        ApproverId = (int)oh.ApproverId,
                        ApproverLevel = oh.ApproverLevel,
                        Status = oh.Status,
                        Remarks = oh.Remarks
                    })
                    .OrderBy(va => va.ApproverLevel)
                    .ToList()
            }).ToList();

            if (model.Status != EnumApprovalStatus.None)
            {
                approvalData = approvalData
                    .Where(x => x.VoucherApprovers.Any(v => v.Status == model.Status) &&
                                (jobIdFilter || x.JobId == model.JobId))
                    .ToList();
            }

            return approvalData;
        }

        public async Task<Response<List<VoucherApprivalDto>>> GetPOPRForApproval(ApprovalFilterModel model)
        {
            try
            {
                if (!model.Type.Any())
                    return Response<List<VoucherApprivalDto>>.Fail(new List<VoucherApprivalDto>(), "No Form Type were found");

                var user = _cacheMemoryService.GetUserObject(InspireConstants.UserCacheKey) as UserFile;
                var modelTypes = model.Type.Contains(EnumOrderForms.None) ? GetModelTypes() : model.Type;



                var validTypes = new[] {
                    EnumOrderForms.PurchaseOrder,
                    EnumOrderForms.PurchaseRequisition
                };
                var CombinedData = modelTypes
                    .Where(type => validTypes.Contains(type))
                    .SelectMany(type => type switch
                    {
                        EnumOrderForms.PurchaseOrder => GetPurchaseOrderForApproval(model, user),
                        EnumOrderForms.PurchaseRequisition => GetPurchaseRequisitionForApproval(model, user),
                        _ => throw new ArgumentOutOfRangeException(nameof(type), $"No form configured for order type: {type}")
                    })
                    .ToList();

                //var CombinedData = modelTypes.SelectMany(type =>
                //{
                //    return type switch
                //    {
                //        EnumOrderForms.PurchaseOrder => GetPurchaseOrderForApproval(model, approvers, user),
                //        EnumOrderForms.PurchaseRequisition => GetPurchaseRequisitionForApproval(model, approvers, user),
                //        EnumOrderForms.CustomerQuotation => GetCustomerQuotationForApproval(model, approvers, user),
                //        _ => throw new ArgumentOutOfRangeException(nameof(type), $"No form configured for order type: {type}")
                //    };
                //}).ToList();

                return Response<List<VoucherApprivalDto>>.Success(CombinedData, "Data found");

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private List<EnumOrderForms> GetModelTypes()
        {
            return new List<EnumOrderForms> { EnumOrderForms.PurchaseOrder, EnumOrderForms.PurchaseRequisition, EnumOrderForms.CustomerQuotation };
        }

        //public List<VoucherApprivalDto> GetPurchaseOrderForApproval(ApprovalFilterModel model, List<ApproverLevelSettings> approvers, UserFile user)
        //{
        //    var poQuery = _purchaseOrderRepository.GetAsQueryable()
        //                             .Where(x => !model.IsDate ? true : x.PurchaseOrderDate.Date >= model.fromDate.Value.Date && x.PurchaseOrderDate.Date <= model.toDate.Value.Date
        //                             && (model.JobId < 0 || x.PurchaseOrderJobId == model.JobId))
        //                            .Include(inc => inc.OrderApprovalHierarchies)
        //                            .AsNoTracking()
        //                            .ToList();

        //    poQuery.ForEach(f => f.OrderApprovalHierarchies = f.OrderApprovalHierarchies.Where(x => x.OrderType == EnumOrderForms.PurchaseOrder).ToList());

        //    IQueryable<LocationMaster> locQuery = _locationMasterRepository.GetAsQueryable();
        //    IQueryable<JobMaster> jobQuery = _jobMaster.GetAsQueryable();


        //    var poList = (from po in poQuery
        //                  join loc in locQuery on po.PurchaseOrderLocationId equals loc.LocationMasterLocationId into locJoin
        //                  from loc in locJoin.DefaultIfEmpty()

        //                  join job in jobQuery on po.PurchaseOrderJobId equals job.JobMasterJobId into jobJoin
        //                  from job in jobJoin.DefaultIfEmpty()

        //                  select new
        //                  {
        //                      po,
        //                      loc,
        //                      job
        //                  }).ToList();

        //    var poData = poList.Select(x => new VoucherApprivalDto
        //    {
        //        VoucherId = (int)x.po.PurchaseOrderId,
        //        VoucherType = EnumOrderForms.PurchaseOrder,
        //        VoucherNo = x.po.PurchaseOrderNo,
        //        VoucherDate = x.po.PurchaseOrderDate,
        //        LocationId = x.po.PurchaseOrderLocationId,
        //        LocationName = x.loc != null ? x.loc.LocationMasterLocationName ?? "" : "",
        //        JobId = x.po.PurchaseOrderJobId,
        //        JobName = x.job != null ? x.job.JobMasterJobName ?? "" : "",
        //        Amount = (decimal)x.po.PurchaseOrderNetAmount,
        //        Status = x.po.PurchaseOrderApproveStatus,
        //        Remarks = $"{x.po.PurchaseOrderRemarks1}\n{x.po.PurchaseOrderRemarks2}\n{x.po.PurchaseOrderRemarks3}",
        //        VoucherApprovers = x.po.OrderApprovalHierarchies
        //                                            .Where(oh => !model.IsUserRequired ? true : (user.WorkGroupId == 1 || oh.ApproverId == user.UserId) && oh.OrderId == x.po.PurchaseOrderId && oh.Status != EnumApprovalStatus.Approved)
        //                                            .Select(oh => new VoucherApprover
        //                                            {
        //                                                ApproverId = (int)oh.ApproverId,
        //                                                ApproverLevel = !approvers.Any(a => a.ApproverId == oh.ApproverId) ? EnumApproverLevels.None : approvers.FirstOrDefault(a => a.ApproverId == oh.ApproverId)?.ApproverLevel ?? 0,
        //                                                Status = oh.Status,
        //                                                Remarks = oh.Remarks
        //                                            }).OrderBy(x => x.ApproverId).ToList()
        //    }).ToList();

        //    if (model.Status != EnumApprovalStatus.None)
        //        poData = poData.Where(x => x.VoucherApprovers.Any(v => v.Status == model.Status) && (model.JobId < 0 || x.JobId == model.JobId)).ToList();

        //    return poData;
        //}


        public List<VoucherApprivalDto> GetPurchaseRequisitionForApproval(ApprovalFilterModel model, UserFile user)
        {
            var fromDate = model.fromDate?.Date;
            var toDate = model.toDate?.Date;
            bool filterByDate = model.IsDate && fromDate.HasValue && toDate.HasValue;
            bool filterByJob = model.JobId >= 0;
            bool isUserRequired = model.IsUserRequired;
            bool isAdmin = user.WorkGroupId == 1;

            var prQuery = _purchaseRequisitionRepository.GetAsQueryable()
                .Where(pr =>
                    (!filterByDate || (pr.PurchaseRequisitionDate.Date >= fromDate && pr.PurchaseRequisitionDate.Date <= toDate)) &&
                    (!filterByJob || pr.PurchaseRequisitionJobId == model.JobId))
                .Include(pr => pr.OrderApprovalHierarchies)
                .AsNoTracking()
                .ToList();

            prQuery.ForEach(pr =>
            {
                pr.OrderApprovalHierarchies = pr.OrderApprovalHierarchies
                    .Where(h => h.OrderType == EnumOrderForms.PurchaseRequisition)
                    .ToList();
            });

            var locations = _locationMasterRepository.GetAsQueryable().ToList();
            var jobs = _jobMaster.GetAsQueryable().ToList();

            var prData = prQuery.Select(pr =>
            {
                var location = locations.FirstOrDefault(l => l.LocationMasterLocationId == pr.PurchaseRequisitionLocationId);
                var job = jobs.FirstOrDefault(j => j.JobMasterJobId == pr.PurchaseRequisitionJobId);

                var approvers = pr.OrderApprovalHierarchies
                    .Where(oh =>
                        (!isUserRequired || isAdmin || oh.ApproverId == user.UserId) &&
                        oh.OrderId == pr.PurchaseRequisitionId)
                    .Select(oh => new VoucherApprover
                    {
                        ApproverId = (int)oh.ApproverId,
                        ApproverLevel = oh.ApproverLevel,
                        Status = oh.Status,
                        Remarks = oh.Remarks
                    })
                    .OrderBy(a => a.ApproverId)
                    .ToList();

                return new VoucherApprivalDto
                {
                    VoucherId = (int)pr.PurchaseRequisitionId,
                    VoucherType = EnumOrderForms.PurchaseRequisition,
                    VoucherNo = pr.PurchaseRequisitionNo,
                    VoucherDate = pr.PurchaseRequisitionDate,
                    LocationId = pr.PurchaseRequisitionLocationId,
                    LocationName = location?.LocationMasterLocationName ?? string.Empty,
                    JobId = pr.PurchaseRequisitionJobId,
                    JobName = job?.JobMasterJobName ?? string.Empty,
                    Amount = (decimal)pr.PurchaseRequisitionNetAmount,
                    Status = pr.PurchaseRequisitionStatus,
                    Remarks = pr.PurchaseRequisitionRemarks,
                    VoucherApprovers = approvers
                };
            }).ToList();

            if (model.Status != EnumApprovalStatus.None)
            {
                prData = prData
                    .Where(dto => dto.VoucherApprovers.Any(v =>
                        v.Status == model.Status &&
                        (!filterByJob || dto.JobId == model.JobId)))
                    .ToList();
            }

            return prData;
        }

        public List<VoucherApprivalDto> GetCustomerQuotationForApproval(ApprovalFilterModel model, List<ApproverLevelSettings> approvers, UserFile user)
        {
            var fromDate = model.fromDate?.Date;
            var toDate = model.toDate?.Date;
            bool filterByDate = model.IsDate && fromDate.HasValue && toDate.HasValue;
            bool filterByJob = model.JobId > 0;
            bool filterByStatus = model.Status != EnumApprovalStatus.None;

            var quotations = _customerQuotation.GetAsQueryable()
                .Where(q =>
                    (!filterByDate || (q.CustomerQuotationQuotationDate >= fromDate && q.CustomerQuotationQuotationDate <= toDate)) &&
                    (!filterByJob || q.CustomerQuotationJobId == model.JobId))
                .AsNoTracking()
                .ToList();

            var quotationIds = quotations.Select(q => (decimal)q.CustomerQuotationQid).ToList();

            var approvals = _orderApprovalHierarchy.GetAsQueryable()
                .Where(a => quotationIds.Contains(a.OrderId) && a.OrderType == EnumOrderForms.CustomerQuotation)
                .ToList();

            var locationLookup = _locationMasterRepository.GetAsQueryable().ToDictionary(l => l.LocationMasterLocationId);
            var jobLookup = _jobMaster.GetAsQueryable().ToDictionary(j => j.JobMasterJobId);

            var result = quotations.Select(q =>
            {
                locationLookup.TryGetValue((int)q.CustomerQuotationLocationId, out var location);
                jobLookup.TryGetValue((int)q.CustomerQuotationJobId, out var job);

                var approversForVoucher = approvals
                    .Where(a => a.OrderId == q.CustomerQuotationQid)
                    .Select(a =>
                    {
                        var levelSetting = approvers.FirstOrDefault(s => s.ApproverId == a.ApproverId);
                        return new VoucherApprover
                        {
                            ApproverId = (int)a.ApproverId,
                            ApproverLevel = levelSetting?.ApproverLevel ?? EnumApproverLevels.None,
                            Status = a.Status,
                            Remarks = a.Remarks
                        };
                    })
                    .OrderBy(a => a.ApproverId)
                    .ToList();

                return new VoucherApprivalDto
                {
                    VoucherId = (int)q.CustomerQuotationQid,
                    VoucherType = EnumOrderForms.CustomerQuotation,
                    VoucherNo = q.CustomerQuotationVoucherNo,
                    VoucherDate = q.CustomerQuotationQuotationDate ?? DateTime.MinValue,
                    LocationId = (int)q.CustomerQuotationLocationId,
                    LocationName = location?.LocationMasterLocationName ?? string.Empty,
                    JobId = q.CustomerQuotationJobId,
                    JobName = job?.JobMasterJobName ?? string.Empty,
                    Amount = q.CustomerQuotationNetTotal ?? 0,
                    Status = "N/A",
                    Remarks = q.CustomerQuotationRemarks,
                    VoucherApprovers = approversForVoucher
                };
            }).ToList();

            if (filterByStatus)
            {
                result = result
                    .Where(r => r.VoucherApprovers.All(a => a.Status == model.Status) &&
                                (!filterByJob || r.JobId == model.JobId))
                    .ToList();
            }

            return result;
        }




        public async Task<Response<List<VoucherApprivalDto>>> ApprovePOPR(POPRApprovalDto approvalDto)
        {
            try
            {
                var settingValue = await _generalSettings.GetValueByKey(InspireConstants.DefaultApprovalType);

                _orderApprovalHierarchy.BeginTransaction();
                var user = _cacheMemoryService.GetUserObject(InspireConstants.UserCacheKey) as UserFile;
                var approvers = _approverLevelSettings.GetAsQueryable().AsNoTracking()
                 .Where(x => x.FormType == approvalDto.OrderType && x.JobId == approvalDto.JobId)
                  .ToList();


                EnumApproverLevels currentApproverLevel = _orderApprovalHierarchy.GetAsQueryable().AsNoTracking().Where(a => a.ApproverId == user.UserId && a.OrderId == approvalDto.OrderId && a.Status == EnumApprovalStatus.Pending).FirstOrDefault()?.ApproverLevel ?? EnumApproverLevels.None;

                var poprToApprove = _orderApprovalHierarchy.GetAsQueryable()
                    .Where(x => x.OrderId == approvalDto.OrderId
                        && x.OrderType == approvalDto.OrderType
                        && x.ApproverLevel == currentApproverLevel
                        && x.Status == EnumApprovalStatus.Pending)
                    .ToList();

                if (!poprToApprove.Any())
                    return Response<List<VoucherApprivalDto>>.Fail(new List<VoucherApprivalDto>(), "data not found");

                poprToApprove.ForEach(x =>
                    {
                        x.Status = approvalDto.ApprovalStatus;
                        x.Remarks = approvalDto.Remarks;
                        x.ApprovedBy = (int)user.UserId;
                        x.ApprovedDate = DateTime.Now;
                    });

                _orderApprovalHierarchy.UpdateList(poprToApprove);
                _orderApprovalHierarchy.TransactionCommit();

                var approverLevels = approvers.GroupBy(x => x.ApproverLevel).Select(g => g.Key).ToList();
                var nextLevelToSendEmail = approverLevels
                                            .SkipWhile(a => a != currentApproverLevel)
                                            .Skip(1)
                                            .DefaultIfEmpty(EnumApproverLevels.None)
                                            .First();



                // EnumApproverLevels nextLevelToSendEamil = GetNextApproverLevel(approvers, (EnumApproverLevels)currentApproverLevel);
                if (nextLevelToSendEmail != EnumApproverLevels.None)
                {
                    switch (approvalDto.OrderType)
                    {
                        case EnumOrderForms.PurchaseOrder:
                            await SendEmailPurchaseOrder(approvalDto.OrderId, nextLevelToSendEmail, settingValue);
                            break;

                        case EnumOrderForms.PurchaseRequisition:
                            await SendEmailPurchaseRequisition(approvalDto.OrderId, nextLevelToSendEmail, settingValue);
                            break;

                        default:
                            // Optional: log or throw if unexpected type
                            throw new NotSupportedException($"Unsupported order type: {approvalDto.OrderType}");
                    }
                }


                return await GetDefaultPOPR(approvalDto.OrderId, approvalDto.OrderType);
            }
            catch (Exception)
            {
                _orderApprovalHierarchy.TransactionRollback();
                throw;
            }
        }

        private async Task<Response<List<VoucherApprivalDto>>> SendEmailPurchaseOrder(int orderId, EnumApproverLevels nextLevelToSendEamil, string settingValue)
        {
            List<object> rptData = new List<object>();
            var po = await GetSavedPurchaseOrderDetails(orderId);
            var jobId = po.Result.purchaseOrder?.PurchaseOrderJobId ?? -1;
            var jobDetail = _jobMaster.GetAsQueryable().Where(x => x.JobMasterJobId == jobId).FirstOrDefault();
            await _orderEmailNotificationService.SendOrderEmailNotificationAsync(
                                                                             EnumOrderForms.PurchaseOrder,
                                                                             nextLevelToSendEamil,
                                                                             "Order",
                                                                          po.Result.purchaseOrder.PurchaseOrderNo,
                                                                          po.Result.purchaseOrder.PurchaseOrderDate,
                                                                          po.Result.purchaseOrder.PurchaseOrderPartyName ?? "",
                                                                          (decimal)po.Result.purchaseOrder.PurchaseOrderNetAmount,
                                                                          (int)po.Result.purchaseOrder.PurchaseOrderId,
                                                                          (int)jobId,
                                                                          jobDetail == null ? "" : jobDetail.JobMasterJobNumber,
                                                                          jobDetail == null ? "" : jobDetail.JobMasterJobName,
                                                                          false, "",
                                                                          settingValue);
            return await GetDefaultPOPR(orderId, EnumOrderForms.PurchaseRequisition);
        }

        private async Task<Response<List<VoucherApprivalDto>>> SendEmailPurchaseRequisition(int orderId, EnumApproverLevels nextLevelToSendEamil, string settingValue)
        {
            List<object> rptData = new List<object>();
            var pr = await _purchaseRequisitionService.GetSavedPurchaseRequisitionDetails(orderId);
            var jobId = pr.Result.purchaseRequisition?.PurchaseRequisitionJobId ?? -1;
            var jobDetail = _jobMaster.GetAsQueryable().Where(x => x.JobMasterJobId == jobId).FirstOrDefault();
            await _orderEmailNotificationService.SendOrderEmailNotificationAsync(
                                                                         EnumOrderForms.PurchaseRequisition,
                                                                         nextLevelToSendEamil,
                                                                         "Requisition",
                                                                         pr.Result.purchaseRequisition.PurchaseRequisitionNo,
                                                                         pr.Result.purchaseRequisition.PurchaseRequisitionDate,
                                                                         pr.Result.purchaseRequisition.PurchaseRequisitionPartyName ?? "",
                                                                         (decimal)pr.Result.purchaseRequisition.PurchaseRequisitionNetAmount,
                                                                         (int)pr.Result.purchaseRequisition.PurchaseRequisitionId,
                                                                          (int)jobId,
                                                                          jobDetail == null ? "" : jobDetail.JobMasterJobNumber,
                                                                          jobDetail == null ? "" : jobDetail.JobMasterJobName,
                                                                          false, "", settingValue);

            return await GetDefaultPOPR(orderId, EnumOrderForms.PurchaseRequisition);
        }




        private EnumApproverLevels GetNextApproverLevel(List<EnumApproverLevels> approverLevels, EnumApproverLevels currentUserLevel)
        {
            var nextLevel = approverLevels
                .SkipWhile(a => a != currentUserLevel)
                .Skip(1)
                .FirstOrDefault();

            return nextLevel;
        }

        public async Task<Response<List<VoucherApprivalDto>>> GetDefaultPOPR(int orderId, EnumOrderForms orderType)
        {
            try
            {
                var user = _cacheMemoryService.GetUserObject(InspireConstants.UserCacheKey) as UserFile;
                IQueryable<LocationMaster> locQuery = _locationMasterRepository.GetAsQueryable();
                IQueryable<JobMaster> jobQuery = _jobMaster.GetAsQueryable();
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

                    var poQuery = _purchaseOrderRepository.GetWithInclude(x => true, inc => inc.OrderApprovalHierarchies)
                                .AsNoTracking()
                                .Where(x => x.PurchaseOrderId == orderId)
                                .ToList(); // Move to memory for further processing

                    var poData = (from po in poQuery
                                  join loc in locQuery on po.PurchaseOrderLocationId equals loc.LocationMasterLocationId into locJoin
                                  from loc in locJoin.DefaultIfEmpty()

                                  join job in jobQuery on po.PurchaseOrderJobId equals job.JobMasterJobId into jobJoin
                                  from job in jobJoin.DefaultIfEmpty()

                                  select new VoucherApprivalDto
                                  {
                                      VoucherId = (int)po.PurchaseOrderId,
                                      VoucherType = EnumOrderForms.PurchaseOrder,
                                      VoucherNo = po.PurchaseOrderNo,
                                      VoucherDate = po.PurchaseOrderDate,
                                      LocationName = loc != null ? loc.LocationMasterLocationName ?? "" : "",
                                      JobId = po.PurchaseOrderJobId ?? 0,
                                      JobName = job != null ? job.JobMasterJobName ?? "" : "",
                                      Amount = (decimal)po.PurchaseOrderNetAmount,
                                      Status = po.PurchaseOrderApproveStatus,
                                      Remarks = $"{po.PurchaseOrderRemarks1}\n{po.PurchaseOrderRemarks2}\n{po.PurchaseOrderRemarks3}",
                                      VoucherApprovers = po.OrderApprovalHierarchies
                                          .Where(ah => ah.ApproverId == user.UserId)
                                          .Select(ah =>
                                          {
                                              var matchedApprover = approvers.FirstOrDefault(a => a.ApproverId == ah.ApproverId);
                                              return new VoucherApprover
                                              {
                                                  ApproverId = (int)ah.ApproverId,
                                                  ApproverLevel = matchedApprover?.ApproverLevel ?? EnumApproverLevels.None,
                                                  Status = ah.Status,
                                                  Remarks = ah.Remarks
                                              };
                                          }).OrderBy(x => x.ApproverId).ToList()
                                  }).ToList();

                    return Response<List<VoucherApprivalDto>>.Success(poData, "Data found");
                }
                else if (orderType == EnumOrderForms.PurchaseRequisition)
                {
                    var prQuery = _purchaseRequisitionRepository
                        .GetWithInclude(x => true, inc => inc.OrderApprovalHierarchies)
                        .AsNoTracking()
                        .Where(x => x.PurchaseRequisitionId == orderId)
                        .ToList();

                    var prData = (from pr in prQuery
                                  join loc in locQuery on pr.PurchaseRequisitionLocationId equals loc.LocationMasterLocationId into locJoin
                                  from loc in locJoin.DefaultIfEmpty()

                                  join job in jobQuery on pr.PurchaseRequisitionJobId equals job.JobMasterJobId into jobJoin
                                  from job in jobJoin.DefaultIfEmpty()

                                  select new VoucherApprivalDto
                                  {
                                      VoucherId = (int)pr.PurchaseRequisitionId,
                                      VoucherType = EnumOrderForms.PurchaseRequisition,
                                      VoucherNo = pr.PurchaseRequisitionNo,
                                      VoucherDate = pr.PurchaseRequisitionDate,
                                      LocationName = loc != null ? loc.LocationMasterLocationName ?? "" : "",
                                      JobId = pr.PurchaseRequisitionJobId ?? 0,
                                      JobName = job != null ? job.JobMasterJobName ?? "" : "",
                                      Amount = (decimal)pr.PurchaseRequisitionNetAmount,
                                      Status = pr.PurchaseRequisitionStatus,
                                      Remarks = pr.PurchaseRequisitionRemarks,
                                      VoucherApprovers = pr.OrderApprovalHierarchies
                                          .Where(ah => ah.ApproverId == user.UserId)
                                          .Select(ah =>
                                          {
                                              var matchedApprover = approvers.FirstOrDefault(a => a.ApproverId == ah.ApproverId);
                                              return new VoucherApprover
                                              {
                                                  ApproverId = (int)ah.ApproverId,
                                                  ApproverLevel = matchedApprover?.ApproverLevel ?? EnumApproverLevels.None,
                                                  Status = ah.Status,
                                                  Remarks = ah.Remarks
                                              };
                                          }).ToList()
                                  }).ToList();

                    return Response<List<VoucherApprivalDto>>.Success(prData, "Data found");
                }


                return Response<List<VoucherApprivalDto>>.Success(new List<VoucherApprivalDto>(), "No Order Type were found");

            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}


