using Inspire.Erp.Application.Account.Implementations;
using Inspire.Erp.Application.Common;
using Inspire.Erp.Application.Email.Interfaces;
using Inspire.Erp.Application.General;
using Inspire.Erp.Application.Helpers;
using Inspire.Erp.Application.MODULE;
using Inspire.Erp.Application.NewFolder.Interfaces;
using Inspire.Erp.Application.Procurement.Interfaces;
using Inspire.Erp.Application.Settings.Interfaces;
using Inspire.Erp.Application.User;
using Inspire.Erp.Domain.Constants;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Enums;
using Inspire.Erp.Domain.Modals;
using Inspire.Erp.Domain.Models.Approvals;
using Inspire.Erp.Infrastructure;
using Inspire.Erp.Infrastructure.Database;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

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
        private readonly IRepository<GeneralPurchaseOrder> _generalPurchaseOrder;
        private IPurchaseRequisitionService _purchaseRequisitionService;
        private ApplicationSettings applicationSettings;
        DownloadAndSaveVoucherReport downloadAndSaveVoucherReport;
        private readonly ICurrentUserService _currentUserService;
        IGeneralSelectQueries _generalSelectQueries;
        private readonly PurchaseOrderRequisitionHelper _purchaseOrderRequisitionHelper;
        public PurchaseOrderService(
            IConfiguration config,
            IRepository<UserFile> userFileRepository, IRepository<PermissionApproval> permissionApprovalRepo,
            IRepository<Approval> approvalRepo, IRepository<ApprovalDetail> approvalDetailRepo, InspireErpDBContext context,
            IRepository<ItemMaster> itemMasterRepository, IRepository<UnitMaster> unitMasterRepository, IDocsAttachmentService docsAttachmentService,
            IRepository<SuppliersMaster> suppliersMasterRepository, IRepository<LocationMaster> locationMasterRepository,
            IRepository<AccountsTransactions> accountTransactionRepository, IRepository<StockRegister> stockRegisterRepository, IRepository<ProgramSettings> programsettingsRepository,
            IRepository<VouchersNumbers> voucherNumbers, ILogger<PaymentVoucherService> logger,
            IRepository<PurchaseOrder> purchaseOrderRepository, IRepository<PurchaseOrderDetails> purchaseOrderDetailsRepository,
            IRepository<PurchaseReturn> purchaseReturnRepository, IRepository<PurchaseReturnDetails> purchaseReturnDetailsRepository,
            IRepository<TrackingRegister> trackingRegisterRepository, IRepository<PurOrderRegister> purOrderRegisterRepository,
            IRepository<UnitDetails> unitDetailsRepository,
            IRepository<PurchaseVoucherDetails> purchaseVoucherDetailsRepo,
            IRepository<PermissionApprovalDetail> permissionApprovalDetailRepo, IRepository<PurchaseRequisitionDetails> purchaseRequisitionDetailsRepository, IRepository<Permissionapproval> permissionapproval, IRepository<ApproverLevelSettings> approverLevelSettings, IOrderEmailNotificationService orderEmailNotificationService, IRepository<PurchaseRequisition> purchaseRequisitionRepository, IRepository<OrderApprovalHierarchy> orderApprovalHierarchy, IPurchaseRequisitionService purchaseRequisitionService, IRepository<CustomerQuotation> customerQuotation, IRepository<JobMaster> jobMaster, IGeneralSettings generalSettings, IRepository<GeneralPurchaseOrder> generalPurchaseOrder, ICurrentUserService currentUserService, IGeneralSelectQueries generalSelectQueries)
        {
            applicationSettings = config.GetSection("ApplicationSettings").Get<ApplicationSettings>();
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
            _orderApprovalHierarchy = orderApprovalHierarchy;
            _purchaseRequisitionService = purchaseRequisitionService;
            _customerQuotation = customerQuotation;
            _jobMaster = jobMaster;
            _generalSettings = generalSettings;
            _generalPurchaseOrder = generalPurchaseOrder;
            downloadAndSaveVoucherReport = new DownloadAndSaveVoucherReport(docsAttachmentService);
            _currentUserService = currentUserService;
            _generalSelectQueries = generalSelectQueries;
            _purchaseOrderRequisitionHelper = new PurchaseOrderRequisitionHelper(userFileRepository, approverLevelSettings); ;
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
                await downloadAndSaveVoucherReport.DownloadedFileAsync(applicationSettings.DownloadFileBaseUrl, "PurchaseOrder", purchaseOrder.PurchaseOrderNo, (int)purchaseOrder.PurchaseOrderId, purchaseOrder.PurchaseOrderNo, true);
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

                var isFromPurchaseRequisition = purchaseOrder.PurchaseOrderDetails.All(x => x.PurchaseRequisitionId != null && x.PurchaseRequisitionId > 0);

                var settingValue = await _generalSettings.GetApproverStatusAsync(purchaseOrder.PurchaseOrderJobId.ToString(), isFromPurchaseRequisition ? nameof(EnumOrderForms.PurchaseRequisition) : nameof(EnumOrderForms.PurchaseOrder));

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


                var filePath = await downloadAndSaveVoucherReport.DownloadedFileAsync(applicationSettings.DownloadFileBaseUrl, "PurchaseOrder", purchaseOrder.PurchaseOrderNo, (int)purchaseOrder.PurchaseOrderId, purchaseOrder.PurchaseOrderNo);

                var jobId = purchaseOrder?.PurchaseOrderJobId ?? -1;

                if (!isFromPurchaseRequisition)
                {
                    #region EmailNotification
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
                                                                                   _currentUserService.Username ?? "",
                                                                                   settingValue.GeneralSettingValue,
                                                                                   new List<string>() { filePath }
                                                                                  );
                    #endregion
                }
                else
                {
                    var approvers = await _purchaseOrderRequisitionHelper.GetOrderApproverNameAndEmail(EnumOrderForms.PurchaseRequisition, (int)jobId);
                    var prIds = purchaseOrder.PurchaseOrderDetails.Select(x => x.PurchaseRequisitionId).ToList();
                    var lastApprovredPR = await _orderApprovalHierarchy.GetAsQueryable().AsNoTracking().Where(x => prIds.Contains((int)x.OrderId) && x.OrderType == EnumOrderForms.PurchaseRequisition).ToListAsync();
                    var orderApprovers = approvers
                        .Select(x => new OrderApprovalHierarchy
                        {
                            ApproverId = x.ApproverId,
                            OrderId = (int)purchaseOrder.PurchaseOrderId,
                            Status = EnumApprovalStatus.Approved,
                            OrderType = EnumOrderForms.PurchaseOrder,
                            Remarks = "",
                            ApproverLevel = x.ApproverLevel,
                            Sno = x.Sno,
                            ApprovedBy = lastApprovredPR.FirstOrDefault(lap=>lap.ApproverId == x.ApproverId).ApprovedBy,
                            ApprovedDate = lastApprovredPR.FirstOrDefault(lap=>lap.ApproverId == x.ApproverId).ApprovedDate,
                            FilePath = string.Join(',', filePath)
                        })
                        .ToList();

                    if (orderApprovers.Any())
                    {
                        _orderApprovalHierarchy.InsertList(orderApprovers);
                    }
                }
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

                //var approvals = _permissionApprovalDetailRepo.GetAsQueryable().AsNoTracking().Where(a => a.VoucherId == purchaseOrderModel.purchaseOrder.PurchaseOrderId).Select(x => new { x.Status, x.ApprovedBy }).ToList();
                //if (approvals.Count > 0)
                //{
                //    if (approvals.Any(a => a.Status == "Approved"))
                //    {
                //        purchaseOrderModel.purchaseOrder.PurchaseOrderApproveUserId = approvals[0].ApprovedBy;
                //        purchaseOrderModel.purchaseOrder.PurchaseOrderApproveEnable = true;
                //    }
                //    else
                //    {
                //        purchaseOrderModel.purchaseOrder.PurchaseOrderApproveUserId = approvals[0].ApprovedBy;
                //        purchaseOrderModel.purchaseOrder.PurchaseOrderApproveEnable = false;
                //    }
                //}
                var settingValue = await _generalSettings.GetApproverStatusAsync(id.ToString(), nameof(EnumOrderForms.PurchaseRequisition));
                bool isNoApproval = settingValue.GeneralSettingValue == InspireConstants.ApprovalAsNoApproval;

                var approvalStatuses = await _orderApprovalHierarchy.GetAsQueryable()
                    .AsNoTracking()
                    .Where(x => x.OrderId == id && x.OrderType == EnumOrderForms.PurchaseOrder)
                    .Select(x => x.Status)
                    .ToListAsync();

                purchaseOrderModel.purchaseOrder.IsPrintEnabled =
                    isNoApproval || approvalStatuses.All(x => x == EnumApprovalStatus.Approved);

                purchaseOrderModel.purchaseOrder.IsEditEnabled =
                    isNoApproval || approvalStatuses.All(x => x == EnumApprovalStatus.Pending);

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



        public List<VoucherApprivalDto> GetPurchaseOrderForApproval(ApprovalFilterModel model)
        {
            var fromDate = model.fromDate?.Date;
            var toDate = model.toDate?.Date;
            bool filterByDate = model.IsDate && fromDate.HasValue && toDate.HasValue;
            bool filterByJob = model.JobId >= 0;
            bool isUserRequired = model.IsUserRequired;

            long userId = Convert.ToInt64(_currentUserService.UserId);
            var groupId = _currentUserService.GroupId;
            bool isAdmin = _generalSelectQueries.GetBooleanValue(groupId).Result;

            var poQuery = _purchaseOrderRepository.GetAsQueryable()
                .Where(po =>
                    (!filterByDate || (po.PurchaseOrderDate.Date >= fromDate && po.PurchaseOrderDate.Date <= toDate)) &&
                    (!filterByJob || po.PurchaseOrderJobId == model.JobId))
                .Include(po => po.OrderApprovalHierarchies)
                .AsNoTracking()
                .ToList();

            poQuery.ForEach(po =>
            {
                po.OrderApprovalHierarchies = po.OrderApprovalHierarchies
                    .Where(h => h.OrderType == EnumOrderForms.PurchaseOrder)
                    .ToList();
            });

            var locations = _locationMasterRepository.GetAsQueryable().ToList();
            var jobs = _jobMaster.GetAsQueryable().ToList();

            var poData = poQuery.Select(po =>
            {
                var location = locations.FirstOrDefault(l => l.LocationMasterLocationId == po.PurchaseOrderLocationId);
                var job = jobs.FirstOrDefault(j => j.JobMasterJobId == po.PurchaseOrderJobId);

                var orderedApprovers = po.OrderApprovalHierarchies
                    .Where(oh => oh.OrderId == po.PurchaseOrderId)
                    .OrderBy(x => x.ApproverLevel)
                    .ThenBy(x => x.Sno)
                    .ToList();

                var currentUserApprovers = orderedApprovers
                    .Where(oh => oh.ApproverId == userId)
                    .ToList();

                // Sequential approval for non-admins
                if (isUserRequired && !isAdmin)
                {
                    bool hasPendingPriorApprover = currentUserApprovers.Any(ua =>
                        orderedApprovers.Any(oh =>
                            (
                                string.Compare(oh.ApproverLevel.ToString(), ua.ApproverLevel.ToString()) < 0 ||
                                (oh.ApproverLevel == ua.ApproverLevel && oh.Sno < ua.Sno)
                            ) &&
                            oh.Status == EnumApprovalStatus.Pending));

                    if (hasPendingPriorApprover)
                        return null;
                }

                var approvers = po.OrderApprovalHierarchies
                    .Where(oh =>
                        !isUserRequired || // report mode
                        (isAdmin && oh.Status == EnumApprovalStatus.Pending) || // admin sees pending
                        (!isAdmin && oh.ApproverId == userId)) // user sees their own
                    .OrderBy(x => x.ApproverLevel)
                    .ThenBy(x => x.Sno)
                    .Select(oh => new VoucherApprover
                    {
                        ApproverId = (int)oh.ApproverId,
                        ApproverLevel = oh.ApproverLevel,
                        Status = oh.Status,
                        Remarks = oh.Remarks
                    })
                    .ToList();

                if (!approvers.Any())
                    return null;

                return new VoucherApprivalDto
                {
                    VoucherId = (int)po.PurchaseOrderId,
                    VoucherType = EnumOrderForms.PurchaseOrder,
                    VoucherNo = po.PurchaseOrderNo,
                    VoucherDate = po.PurchaseOrderDate,
                    LocationId = po.PurchaseOrderLocationId,
                    LocationName = location?.LocationMasterLocationName ?? string.Empty,
                    JobId = po.PurchaseOrderJobId,
                    JobName = job?.JobMasterJobName ?? string.Empty,
                    Amount = po.PurchaseOrderNetAmount ?? 0,
                    Status = po.PurchaseOrderApproveStatus,
                    Remarks = string.Join("\n", po.PurchaseOrderRemarks1, po.PurchaseOrderRemarks2, po.PurchaseOrderRemarks3).Trim(),
                    VoucherApprovers = approvers
                };
            })
            .Where(x => x != null)
            .ToList();

            poData = poData
                .Where(dto =>
                    model.Status != EnumApprovalStatus.None
                        ? dto.VoucherApprovers.Any(v => v.Status == model.Status)
                        : (!isUserRequired || isAdmin)
                            ? true
                            : dto.VoucherApprovers.Any(v => v.ApproverId == userId && v.Status == EnumApprovalStatus.Pending))
                .ToList();

            return poData
                .Where(x => x.VoucherType != EnumOrderForms.None)
                .ToList();
        }


        public async Task<Response<List<VoucherApprivalDto>>> GetPOPRForApproval(ApprovalFilterModel model)
        {
            try
            {
                if (!model.Type.Any())
                    return Response<List<VoucherApprivalDto>>.Fail(new List<VoucherApprivalDto>(), "No Form Type were found");

                var modelTypes = model.Type.Contains(EnumOrderForms.None) ? GetModelTypes() : model.Type;

                var validTypes = new[] {
                    EnumOrderForms.PurchaseOrder,
                    EnumOrderForms.PurchaseRequisition,
                    EnumOrderForms.GeneralPurchaseOrder,
                };
                var CombinedData = modelTypes
                    .Where(type => validTypes.Contains(type))
                    .SelectMany(type => type switch
                    {
                        EnumOrderForms.PurchaseOrder => GetPurchaseOrderForApproval(model),
                        EnumOrderForms.PurchaseRequisition => GetPurchaseRequisitionForApproval(model),
                        EnumOrderForms.GeneralPurchaseOrder => GetGeneralPurchaseOrderForApproval(model),
                        _ => throw new ArgumentOutOfRangeException(nameof(type), $"No form configured for order type: {type}")
                    })
                    .ToList();


                return Response<List<VoucherApprivalDto>>.Success(CombinedData, "Data found");

            }
            catch (Exception ex)
            {

                throw;
            }
        }


        private List<VoucherApprivalDto> GetGeneralPurchaseOrderForApproval(ApprovalFilterModel model)
        {
            var fromDate = model.fromDate?.Date;
            var toDate = model.toDate?.Date;
            bool filterByDate = model.IsDate && fromDate.HasValue && toDate.HasValue;
            bool filterByJob = model.JobId >= 0;
            bool isUserRequired = model.IsUserRequired;

            long userId = Convert.ToInt64(_currentUserService.UserId);
            var groupId = _currentUserService.GroupId;
            bool isAdmin = _generalSelectQueries.GetBooleanValue(groupId).Result;

            var gpoQuery = _generalPurchaseOrder.GetAsQueryable()
                .Where(gpo =>
                    (!filterByDate || (gpo.GeneralPurchaseOrderPoDate.Date >= fromDate && gpo.GeneralPurchaseOrderPoDate.Date <= toDate)) &&
                    (!filterByJob || gpo.GeneralPurchaseOrderJobId == model.JobId))
                .Include(gpo => gpo.OrderApprovalHierarchies)
                .AsNoTracking()
                .ToList();

            gpoQuery.ForEach(gpo =>
            {
                gpo.OrderApprovalHierarchies = gpo.OrderApprovalHierarchies
                    .Where(h => h.OrderType == EnumOrderForms.GeneralPurchaseOrder)
                    .ToList();
            });

            var locations = _locationMasterRepository.GetAsQueryable().ToList();
            var jobs = _jobMaster.GetAsQueryable().ToList();

            var gpoData = gpoQuery.Select(gpo =>
            {
                var location = locations.FirstOrDefault(l => l.LocationMasterLocationId == gpo.GeneralPurchaseOrderLocationId);
                var job = jobs.FirstOrDefault(j => j.JobMasterJobId == gpo.GeneralPurchaseOrderJobId);

                var orderedApprovers = gpo.OrderApprovalHierarchies
                    .Where(oh => oh.OrderId == gpo.GeneralPurchaseOrderId)
                    .OrderBy(x => x.ApproverLevel)
                    .ThenBy(x => x.Sno)
                    .ToList();

                var currentUserApprovers = orderedApprovers
                    .Where(oh => oh.ApproverId == userId)
                    .ToList();

                if (isUserRequired && !isAdmin)
                {
                    bool hasPendingPriorApprover = currentUserApprovers.Any(ua =>
                        orderedApprovers.Any(oh =>
                            (
                                string.Compare(oh.ApproverLevel.ToString(), ua.ApproverLevel.ToString()) < 0 ||
                                (oh.ApproverLevel == ua.ApproverLevel && oh.Sno < ua.Sno)
                            ) &&
                            oh.Status == EnumApprovalStatus.Pending));

                    if (hasPendingPriorApprover)
                        return null;
                }

                var approvers = gpo.OrderApprovalHierarchies
                    .Where(oh =>
                        !isUserRequired || // Report mode
                        (isAdmin && oh.Status == EnumApprovalStatus.Pending) || // Admin sees all pending
                        (!isAdmin && oh.ApproverId == userId)) // Normal user sees only theirs
                    .OrderBy(x => x.ApproverLevel)
                    .ThenBy(x => x.Sno)
                    .Select(oh => new VoucherApprover
                    {
                        ApproverId = (int)oh.ApproverId,
                        ApproverLevel = oh.ApproverLevel,
                        Status = oh.Status,
                        Remarks = oh.Remarks
                    })
                    .ToList();

                if (!approvers.Any())
                    return null;

                return new VoucherApprivalDto
                {
                    VoucherId = (int)gpo.GeneralPurchaseOrderId,
                    VoucherType = EnumOrderForms.GeneralPurchaseOrder,
                    VoucherNo = gpo.GeneralPurchaseOrderPono,
                    VoucherDate = gpo.GeneralPurchaseOrderPoDate,
                    LocationId = gpo.GeneralPurchaseOrderLocationId,
                    LocationName = location?.LocationMasterLocationName ?? string.Empty,
                    JobId = gpo.GeneralPurchaseOrderJobId ?? -1,
                    JobName = job?.JobMasterJobName ?? string.Empty,
                    Amount = (decimal)(gpo.GeneralPurchaseOrderNetAmount ?? 0),
                    Status = gpo.GeneralPurchaseOrderStatus,
                    Remarks = string.Empty,
                    VoucherApprovers = approvers
                };
            })
            .Where(x => x != null)
            .ToList();

            gpoData = gpoData
                .Where(dto =>
                    model.Status != EnumApprovalStatus.None
                        ? dto.VoucherApprovers.Any(v => v.Status == model.Status)
                        : (!isUserRequired || isAdmin)
                            ? true
                            : dto.VoucherApprovers.Any(v => v.ApproverId == userId && v.Status == EnumApprovalStatus.Pending))
                .ToList();

            return gpoData
                .Where(x => x.VoucherType != EnumOrderForms.None)
                .ToList();
        }


        private List<EnumOrderForms> GetModelTypes()
        {
            return new List<EnumOrderForms> {
                EnumOrderForms.PurchaseOrder,
                EnumOrderForms.PurchaseRequisition,
                EnumOrderForms.GeneralPurchaseOrder,
            };
        }

        public List<VoucherApprivalDto> GetPurchaseRequisitionForApproval(ApprovalFilterModel model)
        {
            var fromDate = model.fromDate?.Date;
            var toDate = model.toDate?.Date;
            bool filterByDate = model.IsDate && fromDate.HasValue && toDate.HasValue;
            bool filterByJob = model.JobId >= 0;
            bool isUserRequired = model.IsUserRequired;

            long userId = Convert.ToInt64(_currentUserService.UserId);
            var groupId = _currentUserService.GroupId;
            bool isAdmin = _generalSelectQueries.GetBooleanValue(groupId).Result;

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

                var orderedApprovers = pr.OrderApprovalHierarchies
                    .Where(oh => oh.OrderId == pr.PurchaseRequisitionId)
                    .OrderBy(x => x.ApproverLevel)
                    .ThenBy(x => x.Sno)
                    .ToList();

                var currentUserApprovers = orderedApprovers
                    .Where(oh => oh.ApproverId == userId)
                    .ToList();

                // Enforce sequential approval for non-admins
                if (isUserRequired && !isAdmin)
                {
                    bool hasPendingPriorApprover = currentUserApprovers.Any(ua =>
                        orderedApprovers.Any(oh =>
                            (
                                string.Compare(oh.ApproverLevel.ToString(), ua.ApproverLevel.ToString()) < 0 ||
                                (oh.ApproverLevel == ua.ApproverLevel && oh.Sno < ua.Sno)
                            ) &&
                            oh.Status == EnumApprovalStatus.Pending));

                    if (hasPendingPriorApprover)
                        return null;
                }

                var approvers = pr.OrderApprovalHierarchies
                    .Where(oh =>
                        !isUserRequired || // report mode
                        (isAdmin && oh.Status == EnumApprovalStatus.Pending) || // admin sees only pending
                        (!isAdmin && oh.ApproverId == userId)) // user sees their own
                    .OrderBy(x => x.ApproverLevel)
                    .ThenBy(x => x.Sno)
                    .Select(oh => new VoucherApprover
                    {
                        ApproverId = (int)oh.ApproverId,
                        ApproverLevel = oh.ApproverLevel,
                        Status = oh.Status,
                        Remarks = oh.Remarks
                    })
                    .ToList();

                if (!approvers.Any())
                    return null;

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
            })
            .Where(x => x != null)
            .ToList();

            prData = prData
                .Where(dto =>
                    model.Status != EnumApprovalStatus.None
                        ? dto.VoucherApprovers.Any(v => v.Status == model.Status)
                        : (!isUserRequired || isAdmin)
                            ? true
                            : dto.VoucherApprovers.Any(v => v.ApproverId == userId && v.Status == EnumApprovalStatus.Pending))
                .ToList();

            return prData
                .Where(x => x.VoucherType != EnumOrderForms.None)
                .ToList();
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
                long UserId = approvalDto.UserId ?? Convert.ToInt64(_currentUserService.UserId);
                var Username = _currentUserService.Username;
                var groupId = _currentUserService.GroupId;
                bool isAdmin = _generalSelectQueries.GetBooleanValue(groupId).Result;


                var currentApproval = _orderApprovalHierarchy.GetAsQueryable()
                    .FirstOrDefault(a =>
                        a.ApproverId == UserId &&
                        a.OrderId == approvalDto.OrderId &&
                        a.OrderType == approvalDto.OrderType &&
                        a.Status == EnumApprovalStatus.Pending);

                if (currentApproval == null)
                    return Response<List<VoucherApprivalDto>>.Fail(new List<VoucherApprivalDto>(), "Approval record not found for current user");

                var currentApproverLevel = currentApproval.ApproverLevel;

                // Prevent skipping levels: check if lower level approvals are still pending
                var orderedLevels = _orderApprovalHierarchy.GetAsQueryable()
                    .Where(x => x.OrderId == approvalDto.OrderId && x.OrderType == approvalDto.OrderType)
                    .Select(x => x.ApproverLevel)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();

                var lowerLevels = orderedLevels
                    .TakeWhile(l => l != currentApproverLevel)
                    .ToList();

                var pendingInLowerLevels = _orderApprovalHierarchy.GetAsQueryable()
                    .Where(x =>
                        x.OrderId == approvalDto.OrderId &&
                        x.OrderType == approvalDto.OrderType &&
                        lowerLevels.Contains(x.ApproverLevel) &&
                        x.Status == EnumApprovalStatus.Pending)
                    .Any();




                if (pendingInLowerLevels)
                {
                    return Response<List<VoucherApprivalDto>>.Fail(
                        new List<VoucherApprivalDto>(),
                        $"Cannot approve. Pending approvals exist in lower levels before level {currentApproverLevel}."
                    );
                }

                // Approve current user
                currentApproval.Status = approvalDto.ApprovalStatus;
                currentApproval.Remarks = string.IsNullOrEmpty(approvalDto.Remarks) ? "" : approvalDto.Remarks;
                currentApproval.ApprovedBy = (int)UserId;
                currentApproval.ApprovedDate = DateTime.Now;

                _orderApprovalHierarchy.Update(currentApproval);
                _orderApprovalHierarchy.TransactionCommit();

                if (approvalDto.ApprovalStatus == EnumApprovalStatus.Rejected)
                    return await GetDefaultPOPR(approvalDto.OrderId, approvalDto.OrderType);

                // Check for next pending user at same level
                var pendingUsersAtCurrentLevel = _orderApprovalHierarchy.GetAsQueryable()
                    .Where(x =>
                        x.OrderId == approvalDto.OrderId &&
                        x.OrderType == approvalDto.OrderType &&
                        x.ApproverLevel == currentApproverLevel &&
                        x.Status == EnumApprovalStatus.Pending)
                    .OrderBy(x => x.ApproverId)
                    .ToList();

                if (pendingUsersAtCurrentLevel.Any())
                {
                    var nextUser = pendingUsersAtCurrentLevel.First();

                    await SendApprovalEmailToUser(approvalDto.OrderId, approvalDto.OrderType, currentApproverLevel, settingValue, (int)nextUser.ApproverId, nextUser.FilePath);
                }
                else
                {
                    // Move to next level
                    var nextLevel = orderedLevels
                        .SkipWhile(l => l != currentApproverLevel)
                        .Skip(1)
                        .FirstOrDefault();

                    if (nextLevel != EnumApproverLevels.None)
                    {
                        var nextLevelUser = _orderApprovalHierarchy.GetAsQueryable()
                            .Where(x =>
                                x.OrderId == approvalDto.OrderId &&
                                x.OrderType == approvalDto.OrderType &&
                                x.ApproverLevel == nextLevel &&
                                x.Status == EnumApprovalStatus.Pending)
                            .OrderBy(x => x.ApproverId)
                            .FirstOrDefault();

                        if (nextLevelUser != null)
                        {
                            await SendApprovalEmailToUser(approvalDto.OrderId, approvalDto.OrderType, nextLevel, settingValue, (int)nextLevelUser.ApproverId, nextLevelUser.FilePath);
                        }
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
        private async Task SendApprovalEmailToUser(int orderId, EnumOrderForms orderType, EnumApproverLevels level, string settingValue, int approverId, string filePath)
        {
            switch (orderType)
            {
                case EnumOrderForms.PurchaseOrder:
                    await SendEmailPurchaseOrder(orderId, level, settingValue, approverId, filePath);
                    break;

                case EnumOrderForms.PurchaseRequisition:
                    await SendEmailPurchaseRequisition(orderId, level, settingValue, approverId, filePath);
                    break;
                case EnumOrderForms.GeneralPurchaseOrder:
                    break;

                default:
                    throw new NotSupportedException($"Unsupported order type: {orderType}");
            }
        }


        private async Task<Response<List<VoucherApprivalDto>>> SendEmailPurchaseOrder(int orderId, EnumApproverLevels nextLevelToSendEamil, string settingValue, int approverId, string filePath)
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
                                                                          settingValue,
                                                                         !string.IsNullOrEmpty(filePath) ? new List<string>() { filePath } : new List<string>(),
                                                                          approverId);
            return await GetDefaultPOPR(orderId, EnumOrderForms.PurchaseRequisition);
        }

        private async Task<Response<List<VoucherApprivalDto>>> SendEmailPurchaseRequisition(int orderId, EnumApproverLevels nextLevelToSendEamil, string settingValue, int approverId, string filePath)
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
                                                                          false, "", settingValue, !string.IsNullOrEmpty(filePath) ? new List<string>() { filePath } : new List<string>(), approverId);

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
                long UserId = Convert.ToInt64(_currentUserService.UserId);


                IQueryable<LocationMaster> locQuery = _locationMasterRepository.GetAsQueryable();
                IQueryable<JobMaster> jobQuery = _jobMaster.GetAsQueryable();
                var approvers = _approverLevelSettings.GetAsQueryable()
                    .Where(x => x.FormType == orderType && x.ApproverId == UserId)
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
                                          .Where(ah => ah.ApproverId == UserId)
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
                                          .Where(ah => ah.ApproverId == UserId)
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

        public async Task<Response<string>> IsPOPRAlreadyApprovedOrRejected(POPRApprovalDto approvalDto)
        {
            try
            {
                long UserId = approvalDto.UserId ?? Convert.ToInt64(_currentUserService.UserId);
                var selectedPoPr = await _orderApprovalHierarchy.GetAsQueryable()
                    .FirstOrDefaultAsync(a =>
                        a.ApproverId == UserId &&
                        a.OrderId == approvalDto.OrderId &&
                        a.OrderType == approvalDto.OrderType &&
                        a.ApproverLevel == approvalDto.ApproverLevel);

                return selectedPoPr.Status switch
                {
                    EnumApprovalStatus.Approved => Response<string>.Success("", "This is been already approved"),
                    EnumApprovalStatus.Rejected => Response<string>.Success("", "This is been already rejected"),
                    _ => Response<string>.Fail("", "")
                };

            }
            catch (Exception)
            {
                return Response<string>.Fail("", "");
            }
        }
    }
}


