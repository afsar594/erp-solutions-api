using Inspire.Erp.Application.Procurement.Interfaces;
using Inspire.Erp.Application.Common;
using Inspire.Erp.Application.Account.Implementations;
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
using Microsoft.Extensions.Logging;
using Inspire.Erp.Domain.Modals.Common;
using Inspire.Erp.Domain.Modals;
using Inspire.Erp.Domain.Models.Procurement.PurchaseOrderTracking;
using System.Threading.Tasks;
using Inspire.Erp.Infrastructure.Database;
using SendGrid.Helpers.Mail;
using System.Linq.Dynamic.Core;
using Inspire.Erp.Domain.Entities.POS;
using Inspire.Erp.Application.MODULE;
using System.Threading;
using Inspire.Erp.Application.Email.Interfaces;
using Inspire.Erp.Domain.Models.Common;
using Microsoft.Extensions.Options;
using Inspire.Erp.Application.Helpers;
using Inspire.Erp.Domain.Models.Approvals;

namespace Inspire.Erp.Application.Procurement.Implementation
{
    public class PurchaseRequisitionService : IPurchaseRequisitionService
    {
        private InspireErpDBContext _context;
        private IRepository<StockRegister> _stockRegisterRepository;
        private IRepository<AccountsTransactions> _accountTransactionRepository;
        private IRepository<PurchaseRequisition> _purchaseRequisitionRepository;
        private IRepository<PurchaseRequisitionDetails> _purchaseRequisitionDetailsRepository;
        private IRepository<ProgramSettings> _programsettingsRepository;
        private IRepository<VouchersNumbers> _voucherNumbersRepository;
        private readonly ILogger<PaymentVoucherService> _logger;
        private readonly IRepository<PermissionApproval> _permissionApprovalRepo;
        private readonly IRepository<Approval> _approvalRepo;
        private readonly IRepository<ApprovalDetail> _approvalDetailRepo;
        public readonly InspireErpDBContext dbContext;
        private IRepository<IPurchaseOrderService> _purchaseOrderService;
        private IRepository<AvIssueVoucherDetails> _AvIssueVoucherDetails;
        private IRepository<AvIssueVoucher> _AvIssueVoucher;
        private IRepository<PermissionApprovalDetail> _permissionApprovalDetailRepo;
        private IRepository<JobMaster> jobrepository;
        private IRepository<JobMasterBudgetDetails> _budgetDetailsrepository;
        private readonly IOrderEmailNotificationService _orderEmailNotificationService;
        private IRepository<LocationMaster> locationrepository;
        private IRepository<BillOfQTyDetails> _BillOfQTyDetailsRepository;
        private IRepository<TrackingRegister> _trackingRegisterRepository;
        private IRepository<ItemMaster> _itemMasterRepository;
        private IRepository<UnitMaster> _unitMasterRepository;
        private IRepository<SuppliersMaster> _suppliersMasterRepository;
        private IRepository<LocationMaster> _locationMasterRepository;
        private readonly IRepository<PurchaseOrderDetails> _purchaseOrderDetailsRepository;
        private readonly IRepository<ApproverLevelSettings> _approverLevelSettings;
        private readonly IRepository<OrderApprovalHierarchy> _orderApprovalHierarchy;
        public PurchaseRequisitionService(IRepository<ItemMaster> itemMasterRepository, IRepository<UnitMaster> unitMasterRepository,
            IRepository<SuppliersMaster> suppliersMasterRepository, IRepository<LocationMaster> _locationrepository, IRepository<LocationMaster> locationMasterRepository,
            IRepository<PermissionApproval> permissionApprovalRepo, IRepository<Approval> approvalRepo, IRepository<ApprovalDetail> approvalDetailRepo,
            IRepository<AccountsTransactions> accountTransactionRepository, IRepository<StockRegister> stockRegisterRepository, IRepository<ProgramSettings> programsettingsRepository,
            IRepository<VouchersNumbers> voucherNumbers, ILogger<PaymentVoucherService> logger, IRepository<JobMaster> _jobrepository, IRepository<BillOfQTyDetails> BillOfQTyDetailsRepository,
             IRepository<TrackingRegister> trackingRegisterRepository, IRepository<PermissionApprovalDetail> permissionApprovalDetailRepo,
        IRepository<PurchaseRequisition> purchaseRequisitionRepository, InspireErpDBContext context, IRepository<PurchaseRequisitionDetails> purchaseRequisitionDetailsRepository, IRepository<JobMasterBudgetDetails> budgetDetailsrepository, IRepository<PurchaseOrderDetails> purchaseOrderDetailsRepository,
        IOrderEmailNotificationService orderEmailNotificationService, IRepository<ApproverLevelSettings> approverLevelSettings, IRepository<OrderApprovalHierarchy> orderApprovalHierarchy)
        {
            this._accountTransactionRepository = accountTransactionRepository;
            this._stockRegisterRepository = stockRegisterRepository; _context = context;
            this._purchaseRequisitionRepository = purchaseRequisitionRepository; _permissionApprovalDetailRepo = permissionApprovalDetailRepo;
            this._purchaseRequisitionDetailsRepository = purchaseRequisitionDetailsRepository; _BillOfQTyDetailsRepository = BillOfQTyDetailsRepository;
            _programsettingsRepository = programsettingsRepository; _voucherNumbersRepository = voucherNumbers; _approvalDetailRepo = approvalDetailRepo; _permissionApprovalRepo = permissionApprovalRepo; _approvalRepo = approvalRepo; locationrepository = _locationrepository;
            jobrepository = _jobrepository; _itemMasterRepository = itemMasterRepository; _unitMasterRepository = unitMasterRepository; _suppliersMasterRepository = suppliersMasterRepository; _locationMasterRepository = locationMasterRepository; _trackingRegisterRepository = trackingRegisterRepository;
            _budgetDetailsrepository = budgetDetailsrepository;
            _purchaseOrderDetailsRepository = purchaseOrderDetailsRepository;
            _orderEmailNotificationService = orderEmailNotificationService;
            _approverLevelSettings = approverLevelSettings;
            _orderApprovalHierarchy = orderApprovalHierarchy;
        }
        public PurchaseRequisitionModel UpdatePurchaseRequisition(PurchaseRequisition purchaseRequisition, List<AccountsTransactions> accountsTransactions,List<PurchaseRequisitionDetails> purchaseRequisitionDetails)
        {

            try
            {
                _purchaseRequisitionRepository.BeginTransaction();

                this.deleteAccountTractionByVoucherId(purchaseRequisition.PurchaseRequisitionNo, VoucherType.PurchaseRequisition_TYPE);
                purchaseRequisition.PurchaseRequisitionDetails = purchaseRequisitionDetails.Select((k) =>
                {
                    if (k.PurchaseRequisitionDetailsId == 0)
                    {
                        k.PurchaseRequisitionId = purchaseRequisition.PurchaseRequisitionId;
                        k.PurchaseRequisitionDetailsNo = purchaseRequisition.PurchaseRequisitionNo;
                        k.PurchaseRequisitionDetailsId = null;
                    }
                    else
                    {
                        k.PurchaseRequisitionId = purchaseRequisition.PurchaseRequisitionId;
                        k.PurchaseRequisitionDetailsNo = purchaseRequisition.PurchaseRequisitionNo;

                    }
                    return k;
                }).OrderBy(x => x.PurchaseRequisitionDetailsItemName).AsEnumerable().ToList();

                _purchaseRequisitionRepository.Update(purchaseRequisition);

                accountsTransactions = accountsTransactions.Select((k) =>
                {
                    if (k.AccountsTransactionsTransSno == 0)
                    {
                        k.AccountsTransactionsTransDate = purchaseRequisition.PurchaseRequisitionDate;
                        k.AccountsTransactionsVoucherNo = purchaseRequisition.PurchaseRequisitionNo;
                        k.AccountsTransactionsVoucherType = VoucherType.PurchaseRequisition_TYPE;
                        k.AccountsTransactionsStatus = AccountStatus.Approved;
                    }

                    return k;
                }).OrderBy(x => x.AccountsTransactionsVoucherNo).AsEnumerable().ToList();
                _accountTransactionRepository.UpdateList(accountsTransactions);

                //PermissionApprovalDetail by zaid              

                var rights = _context.tbl_approvalforms.Where(a => a.Voucher_Type.Contains("Purchase Requisition") && a.is_active == true).AsNoTracking().ToList();
                if (rights.Count > 0)
                {
                    var existdata = _permissionApprovalDetailRepo.GetAsQueryable().Where(a => a.VoucherId == purchaseRequisition.PurchaseRequisitionId && a.VoucherType == "Purchase Requisition").ToList();

                    if (existdata.Count > 0)
                    {
                        _permissionApprovalDetailRepo.DeleteList(existdata);
                    }
                    foreach (var item in rights)
                    {
                        for (int level = 1; level <= item.NoOfLevel; level++)
                        {
                            PermissionApprovalDetail POList = new PermissionApprovalDetail();
                            POList.VoucherType = "Purchase Requisition";
                            POList.VoucherNo = purchaseRequisition.PurchaseRequisitionNo;
                            POList.VoucherId = (int)purchaseRequisition.PurchaseRequisitionId;
                            POList.VoucherDate = purchaseRequisition.PurchaseRequisitionDate;
                            POList.Amount = (double)purchaseRequisition.PurchaseRequisitionNetAmount;
                            POList.CreatedBy = (int?)(purchaseRequisition.PurchaseRequisitionUserId ?? 1);
                            POList.LevelId = level;
                            POList.Status = "Pending";
                            POList.JobId = (int)purchaseRequisition.PurchaseRequisitionJobId;
                            POList.Remarks = purchaseRequisition.PurchaseRequisitionDescription;
                            _permissionApprovalDetailRepo.Insert(POList);
                        }
                    }

                }
                // Item Master SupplierId is updated.
                foreach (var item in purchaseRequisitionDetails)
                {
                    if (item.PurchaseRequisitionDetailsSupplierId > 0)
                    {
                        if (item.PurchaseRequisitionDetailsMatId > 0)
                        {
                            var entity = _itemMasterRepository.GetAsQueryable().FirstOrDefault(x => x.ItemMasterItemId == item.PurchaseRequisitionDetailsMatId);

                            if (entity != null)
                            {
                                entity.ItemMasterSupplierId = (int)item.PurchaseRequisitionDetailsSupplierId;
                                _itemMasterRepository.Update(entity);
                            }
                        }
                    }
                }
                //End

                foreach (var item in purchaseRequisitionDetails)
                {
                    if (item.boqdId > 0)
                    {
                        var data = _BillOfQTyDetailsRepository.GetAsQueryable().AsNoTracking().Where(a => a.Id == item.boqdId).FirstOrDefault();
                        if (data != null)
                        {
                            data.IsEdit = false;
                            _BillOfQTyDetailsRepository.Update(data);
                        }
                    }
                }
                Thread.Sleep(2000);
                _purchaseRequisitionRepository.TransactionCommit();

            }
            catch (Exception ex)
            {
                _purchaseRequisitionRepository.TransactionRollback();
                throw ex;
            }

            return this.GetSavedPurchaseRequisitionDetails((int)purchaseRequisition.PurchaseRequisitionId);
        }

        public int DeletePurchaseRequisition(PurchaseRequisition purchaseRequisition, List<AccountsTransactions> accountsTransactions,
            List<PurchaseRequisitionDetails> purchaseRequisitionDetails
            )
        {
            int i = 0;
            try
            {
                _purchaseRequisitionRepository.BeginTransaction();

                purchaseRequisition.PurchaseRequisitionDelStatus = true;

                purchaseRequisitionDetails = purchaseRequisitionDetails.Select((k) =>
                {
                    k.PurchaseRequisitionDetailsDelStatus = true;
                    return k;
                }).OrderBy(x => x.PurchaseRequisitionDetailsItemName).AsEnumerable().ToList();

                //_purchaseRequisitionDetailsRepository.UpdateList(purchaseRequisitionDetails);

                accountsTransactions = accountsTransactions.Select((k) =>
                {
                    k.AccountstransactionsDelStatus = true;
                    return k;
                }).OrderBy(x => x.AccountsTransactionsVoucherNo).AsEnumerable().ToList();
                _accountTransactionRepository.UpdateList(accountsTransactions);




                //stockRegister = stockRegister.Select((k) =>
                //{
                //    k.StockRegisterDelStatus = true;
                //    return k;
                //}).ToList();
                //_stockRegisterRepository.UpdateList(stockRegister);




                purchaseRequisition.PurchaseRequisitionDetails = purchaseRequisitionDetails;

                _purchaseRequisitionRepository.Update(purchaseRequisition);

                var vchrnumer = _voucherNumbersRepository.GetAsQueryable().Where(k => k.VouchersNumbersVNo == purchaseRequisition.PurchaseRequisitionNo).FirstOrDefault();

                _voucherNumbersRepository.Update(vchrnumer);

                var detailsToDelete = _permissionApprovalDetailRepo
               .GetAsQueryable().Where(a => a.VoucherNo == purchaseRequisition.PurchaseRequisitionNo).ToList();
                foreach (var detail in detailsToDelete)
                {
                    detail.IsDeleted = true;
                    _permissionApprovalDetailRepo.Update(detail);
                }

                _purchaseRequisitionRepository.TransactionCommit();
                i = 1;
            }
            catch (Exception ex)
            {
                _purchaseRequisitionRepository.TransactionRollback();
                i = 0;
                throw ex;
            }

            return i;

        }
        public void deleteAccountTractionByVoucherId(string AccountsTransactionsVoucherNo, string AccountsTransactionsVoucherType)
        {
            List<AccountsTransactions> accttrans = this._accountTransactionRepository.GetAsQueryable()
                 .Where(k => k.AccountsTransactionsVoucherNo == AccountsTransactionsVoucherNo && k.AccountsTransactionsVoucherType == AccountsTransactionsVoucherType && k.AccountstransactionsDelStatus != true).Select((l) => new AccountsTransactions
                 {
                     AccountsTransactionsTransSno = l.AccountsTransactionsTransSno,
                     AccountstransactionsDelStatus = true,

                     AccountsTransactionsAccNo = l.AccountsTransactionsAccNo,
                     AccountsTransactionsTransDate = l.AccountsTransactionsTransDate,
                     AccountsTransactionsParticulars = l.AccountsTransactionsParticulars,

                     AccountsTransactionsDebit = l.AccountsTransactionsDebit,


                     AccountsTransactionsCredit = l.AccountsTransactionsCredit,
                     AccountsTransactionsFcDebit = l.AccountsTransactionsFcDebit,
                     AccountsTransactionsFcCredit = l.AccountsTransactionsFcCredit,

                     AccountsTransactionsVoucherType = l.AccountsTransactionsVoucherType,







                     AccountsTransactionsVoucherNo = l.AccountsTransactionsVoucherNo,
                     AccountsTransactionsDescription = l.AccountsTransactionsDescription,
                     AccountsTransactionsUserId = l.AccountsTransactionsUserId,

                     AccountsTransactionsStatus = l.AccountsTransactionsStatus,







                     AccountsTransactionsTstamp = l.AccountsTransactionsTstamp,
                     RefNo = l.RefNo,
                     AccountsTransactionsFsno = l.AccountsTransactionsFsno,

                     AccountsTransactionsAllocDebit = l.AccountsTransactionsAllocDebit,



                     AccountsTransactionsAllocCredit = l.AccountsTransactionsAllocCredit,
                     AccountsTransactionsAllocBalance = l.AccountsTransactionsAllocBalance,
                     AccountsTransactionsFcAllocDebit = l.AccountsTransactionsFcAllocDebit,

                     AccountsTransactionsFcAllocCredit = l.AccountsTransactionsFcAllocCredit,




                     AccountsTransactionsFcAllocBalance = l.AccountsTransactionsFcAllocBalance,
                     AccountsTransactionsLocation = l.AccountsTransactionsLocation,
                     AccountsTransactionsJobNo = l.AccountsTransactionsJobNo,

                     AccountsTransactionsCostCenterId = l.AccountsTransactionsCostCenterId,






                     AccountsTransactionsApprovalDt = l.AccountsTransactionsApprovalDt,
                     AccountsTransactionsDepartment = l.AccountsTransactionsDepartment,
                     AccountsTransactionsFcRate = l.AccountsTransactionsFcRate,

                     AccountsTransactionsCompanyId = l.AccountsTransactionsCompanyId,

                     AccountsTransactionsCurrencyId = l.AccountsTransactionsCurrencyId,
                     AccountsTransactionsDrGram = l.AccountsTransactionsDrGram,
                     AccountsTransactionsCrGram = l.AccountsTransactionsCrGram,

                     AccountsTransactionsCheqNo = l.AccountsTransactionsCheqNo,


                     AccountsTransactionsLpoNo = l.AccountsTransactionsLpoNo,
                     AccountsTransactionsCheqDate = l.AccountsTransactionsCheqDate,
                     AccountsTransactionsOpposEntryDesc = l.AccountsTransactionsOpposEntryDesc,

                     AccountsTransactionsAllocUpdateBal = l.AccountsTransactionsAllocUpdateBal,






                     AccountsTransactionsDeptId = l.AccountsTransactionsDeptId,
                     AccountsTransactionsVatno = l.AccountsTransactionsVatno,
                     AccountsTransactionsVatableAmount = l.AccountsTransactionsVatableAmount,








                 }).OrderBy(x => x.AccountsTransactionsVoucherNo).AsEnumerable().ToList();

            this._accountTransactionRepository.UpdateList(accttrans);
        }
        public PurchaseRequisitionModel InsertPurchaseRequisition(PurchaseRequisition purchaseRequisition, List<AccountsTransactions> accountsTransactions,
            List<PurchaseRequisitionDetails> purchaseRequisitionDetails
            )
        {
            try
            {
                _purchaseRequisitionRepository.BeginTransaction();
                string purchaseRequisitionNumber = clsCommonFunctions.GenrateVoucherNo(VoucherType.PurchaseRequisition_TYPE, Prefix.PurchaseRequisition_Prefix,
                    _programsettingsRepository, _voucherNumbersRepository);
                purchaseRequisition.PurchaseRequisitionNo = purchaseRequisitionNumber;

                int maxcount = 0;
                maxcount = Convert.ToInt32(
                    _purchaseRequisitionRepository.GetAsQueryable()
                    .DefaultIfEmpty().Max(o => o == null ? 0 : o.PurchaseRequisitionId) + 1);

                purchaseRequisition.PurchaseRequisitionId = maxcount;

                purchaseRequisition.PurchaseRequisitionDetails = purchaseRequisition.PurchaseRequisitionDetails.Select((x) =>
                {
                    x.PurchaseRequisitionDetailsId = null;
                    x.PurchaseRequisitionId = maxcount;
                    x.PurchaseRequisitionDetailsNo = purchaseRequisitionNumber;
                    return x;
                }).OrderBy(x => x.PurchaseRequisitionDetailsId).AsEnumerable().ToList();

                accountsTransactions = accountsTransactions.Select((k) =>
                {
                    k.AccountsTransactionsVoucherNo = purchaseRequisitionNumber;
                    k.AccountsTransactionsVoucherType = VoucherType.PurchaseRequisition_TYPE;
                    k.AccountsTransactionsStatus = AccountStatus.Approved;
                    return k;
                }).OrderBy(x => x.AccountsTransactionsVoucherNo).AsEnumerable().ToList();
                _accountTransactionRepository.InsertList(accountsTransactions);

                _purchaseRequisitionRepository.Insert(purchaseRequisition);




                //var permsionApproval = _permissionApprovalRepo.GetAsQueryable().AsNoTracking().Where(x => x.VoucherType == "Purchase Requisition").OrderBy(x => x.LevelOrder).ToList();
                //var approvals = new Approval()
                //{
                //    CreatedBy = 0,
                //    LocationId = null,
                //    Status = false,
                //    VoucherType = "Purchase Requisition"
                //};
                //_approvalRepo.Insert(approvals);
                //_approvalDetailRepo.InsertList(permsionApproval.Select(x => new ApprovalDetail()
                //{
                //    ApprovalId = approvals.Id,
                //    Status = false,
                //    UserId = x.UserId,
                //    LevelOrder = x.LevelOrder,
                //    ApprovedAt = DateTime.Now,
                //}).ToList());

                // PermissionApprovalDetail by zaid
                List<PermissionApprovalDetail> permissionApprovalDetails = new List<PermissionApprovalDetail>();

                var rights = _context.tbl_approvalforms.Where(a => a.Voucher_Type.Contains("Purchase Requisition") && a.is_active == true).AsNoTracking().ToList();
                if (rights.Count > 0)
                {
                    foreach (var item in rights)
                    {
                        for (int level = 1; level <= item.NoOfLevel; level++)
                        {
                            PermissionApprovalDetail POList = new PermissionApprovalDetail();
                            POList.VoucherType = "Purchase Requisition";
                            POList.VoucherNo = purchaseRequisition.PurchaseRequisitionNo;
                            POList.VoucherId = (int)purchaseRequisition.PurchaseRequisitionId;
                            POList.VoucherDate = purchaseRequisition.PurchaseRequisitionDate;
                            POList.Amount = (double)purchaseRequisition.PurchaseRequisitionNetAmount;
                            POList.CreatedBy = (int?)(purchaseRequisition.PurchaseRequisitionUserId ?? 1);
                            POList.LevelId = level;
                            POList.Status = "Pending";
                            POList.JobId = (int)purchaseRequisition.PurchaseRequisitionJobId;
                            POList.Remarks = purchaseRequisition.PurchaseRequisitionDescription;
                            POList.LocationName = _context.LocationMaster
                                                .FirstOrDefault(l => l.LocationMasterLocationId == purchaseRequisition.PurchaseRequisitionLocationId)?.LocationMasterLocationName;
                            permissionApprovalDetails.Add(POList);
                        }
                    }
                    if (permissionApprovalDetails.Any())
                        _permissionApprovalDetailRepo.InsertList(permissionApprovalDetails);
                }
                // Item Master SupplierId is updated.
                foreach (var item in purchaseRequisitionDetails)
                {
                    if (item.PurchaseRequisitionDetailsSupplierId > 0)
                    {
                        if (item.PurchaseRequisitionDetailsMatId > 0)
                        {
                            var entity = _itemMasterRepository.GetAsQueryable().FirstOrDefault(x => x.ItemMasterItemId == item.PurchaseRequisitionDetailsMatId);

                            if (entity != null)
                            {
                                entity.ItemMasterSupplierId = (int)item.PurchaseRequisitionDetailsSupplierId;
                                _itemMasterRepository.Update(entity);
                            }
                        }
                    }
                }
                //End


                foreach (var item in purchaseRequisitionDetails)
                {
                    if (item.boqdId > 0)
                    {
                        var data = _BillOfQTyDetailsRepository.GetAsQueryable().AsNoTracking().Where(a => a.Id == item.boqdId).FirstOrDefault();
                        if (data != null)
                        {
                            data.IsEdit = false;
                            _BillOfQTyDetailsRepository.Update(data);
                        }
                    }
                }
                Thread.Sleep(2000);
                _purchaseRequisitionRepository.TransactionCommit();

                #region EmailNotification
                _orderEmailNotificationService.SendOrderEmailNotificationAsync(
                                                                               EnumOrderForms.PurchaseRequisition,
                                                                               EnumApproverLevels.Level1,
                                                                               "Requisition",
                                                                               purchaseRequisition.PurchaseRequisitionNo,
                                                                               purchaseRequisition.PurchaseRequisitionDate,
                                                                               purchaseRequisition.PurchaseRequisitionPartyName,
                                                                               (decimal)purchaseRequisition.PurchaseRequisitionNetAmount,
                                                                               (int)purchaseRequisition.PurchaseRequisitionId).ConfigureAwait(false);
                #endregion

                return this.GetSavedPurchaseRequisitionDetails((int)purchaseRequisition.PurchaseRequisitionId);

            }
            catch (Exception ex)
            {
                _purchaseRequisitionRepository.TransactionRollback();
                throw ex;
            }



        }
        public IEnumerable<AccountsTransactions> GetAllTransaction()
        {
            return _accountTransactionRepository.GetAll();
        }
        public IEnumerable<PurchaseRequisition> GetPurchaseRequisition()
        {
            IEnumerable<PurchaseRequisition> purchaseRequisition_ALL = _purchaseRequisitionRepository.GetAll().Where(k => k.PurchaseRequisitionDelStatus == false || k.PurchaseRequisitionDelStatus == null);
            return purchaseRequisition_ALL;
        }
        public PurchaseRequisitionModel GetSavedPurchaseRequisitionDetails(int pvno)
        {
            PurchaseRequisitionModel purchaseRequisitionModel = new PurchaseRequisitionModel();

            purchaseRequisitionModel.purchaseRequisition = _purchaseRequisitionRepository.GetAsQueryable().Where(k => k.PurchaseRequisitionId == pvno).FirstOrDefault();

            purchaseRequisitionModel.accountsTransactions = _accountTransactionRepository.GetAsQueryable()
             .Where(c => purchaseRequisitionModel.purchaseRequisition != null && c.AccountsTransactionsVoucherNo == purchaseRequisitionModel.purchaseRequisition.PurchaseRequisitionNo
             && c.AccountsTransactionsVoucherType == VoucherType.PurchaseRequisition_TYPE && c.AccountstransactionsDelStatus != true).ToList();

            purchaseRequisitionModel.purchaseRequisitionDetails = _purchaseRequisitionDetailsRepository.GetAsQueryable()
                .Where(x => purchaseRequisitionModel.purchaseRequisition != null && x.PurchaseRequisitionDetailsNo == purchaseRequisitionModel.purchaseRequisition.PurchaseRequisitionNo && (x.PurchaseRequisitionDetailsDelStatus != true)).ToList();

            var approvals = _permissionApprovalDetailRepo.GetAsQueryable().AsNoTracking().Where(a => purchaseRequisitionModel.purchaseRequisition != null
            && a.VoucherId == (int)purchaseRequisitionModel.purchaseRequisition.PurchaseRequisitionId).ToList();
            if (approvals.Count > 0)
            {
                if (approvals.Any(a => a.Status.Contains("Approved")))
                {
                    purchaseRequisitionModel.purchaseRequisition.PurchaseRequisitionApproveEnable = true;
                    purchaseRequisitionModel.purchaseRequisition.PurchaseRequisitionApprovedBy = approvals[0].ApprovedBy;
                }
                else
                {
                    purchaseRequisitionModel.purchaseRequisition.PurchaseRequisitionApproveEnable = false;
                    purchaseRequisitionModel.purchaseRequisition.PurchaseRequisitionApprovedBy = approvals[0].ApprovedBy ?? 1;
                }
            }
            return purchaseRequisitionModel;

        }
        public VouchersNumbers GenerateVoucherNo(DateTime? VoucherGenDate)
        {
            try
            {
                var prefix = this._programsettingsRepository.GetAsQueryable().Where(k => k.ProgramSettingsKeyValue == Prefix.PurchaseRequisition_Prefix).FirstOrDefault().ProgramSettingsTextValue;
                int vnoMaxVal = Convert.ToInt32(_voucherNumbersRepository.GetAsQueryable()
                                                        .Where(x => x.VouchersNumbersVNoNu > 0 && x.VouchersNumbersVType == VoucherType.PurchaseRequisition_TYPE)
                                                        .DefaultIfEmpty()
                                                        .Max(o => o == null ? 0 : o.VouchersNumbersVNoNu)) + 1;

                VouchersNumbers vouchersNumbers = new VouchersNumbers
                {
                    VouchersNumbersVNo = prefix + vnoMaxVal,
                    VouchersNumbersVNoNu = vnoMaxVal,
                    VouchersNumbersVType = VoucherType.PurchaseRequisition_TYPE,
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
        public async Task<Response<List<PurChaseRequisitionStatus>>> GetPurchaseRequisitionStatus(PurChaseRequisitionStatusFilterReport model)
        {
            List<PurChaseRequisitionStatus> data = new List<PurChaseRequisitionStatus>();
            try
            {
                string filteredValue = string.Empty;
                if (model.JobId > 0)
                {
                    filteredValue += $" and pd.PurchaseRequisitionJobId = {model.JobId} ";
                }
                if (model.ItemId > 0)
                {
                    filteredValue += $" and pd.PurchaseRequisitionDetailsItemID = {model.ItemId} ";
                }
                if (!string.IsNullOrEmpty(model.PRNO) && model.PRNO.ToUpper() != "ALL".ToUpper())
                {
                    filteredValue += $" and po.PurchaseRequisitionRefNo = {model.PRNO} ";
                }
                if (!string.IsNullOrEmpty(model.POStatus))
                {
                    filteredValue += $" and po.PurchaseRequisitionStatus = {model.POStatus} ";
                }
                if (!string.IsNullOrEmpty(model.fromDate) && !string.IsNullOrEmpty(model.toDate))
                {
                    filteredValue += $" and (pm.PurchaseRequisitionDate >= {model.fromDate} and  pm.PurchaseRequisitionDate <= {model.toDate} )";
                }

                var subqueryRDI = (from pod in dbContext.PurchaseOrderDetails
                                   join po in dbContext.GeneralPurchaseOrder on pod.PurchaseOrderDetailsPoId equals (int?)po.GeneralPurchaseOrderId
                                   group new { pod, po } by new
                                   {
                                       pod.PurchaseOrderDetailsId,
                                       pod.PurchaseOrderDetailsPrdId,
                                       po.GeneralPurchaseOrderStatus
                                   } into grp
                                   select new
                                   {
                                       grp.Key.PurchaseOrderDetailsId,
                                       grp.Key.PurchaseOrderDetailsPrdId,
                                       grp.Key.GeneralPurchaseOrderStatus,
                                       Quantity = grp.Sum(x => x.pod.PurchaseOrderDetailsQuantity)
                                   });
                var subqueryIMT = (from d in dbContext.AvIssueVoucherDetails
                                   join i in dbContext.AvIssueVoucher on d.AvIssueVoucherDetailsAvSvNo equals i.AvIssueVoucherAccNo
                                   group d by d.AvIssueVoucherDetailsMaterialId into grp
                                   select new
                                   {
                                       grp.Key,
                                       issueQty = grp.Sum(x => x.AvIssueVoucherDetailsSoldQuantity),
                                       stock = grp.Sum(x => x.AvIssueVoucherDetailsStock),
                                       BalToIssue = grp.Sum(x => x.AvIssueVoucherDetailsStock) - grp.Sum(x => x.AvIssueVoucherDetailsSoldAmount)
                                   });
                var subquerySEI = (from d in dbContext.AvIssueVoucherDetails
                                   join i in dbContext.AvIssueVoucher on d.AvIssueVoucherDetailsAvSvNo equals i.AvIssueVoucherAccNo
                                   join s in dbContext.StockRegister on d.AvIssueVoucherDetailsMaterialId equals s.StockRegisterMaterialID
                                   group new { d, s } by d.AvIssueVoucherDetailsMaterialId into grp
                                   select new
                                   {
                                       grp.Key,
                                       issueQty = grp.Sum(x => x.d.AvIssueVoucherDetailsSoldQuantity),
                                       stock = grp.Sum(x => x.s.StockRegisterSIN - x.s.StockRegisterSout),
                                       BalToIssue = grp.Sum(x => (x.s.StockRegisterSIN - x.s.StockRegisterSout) - Convert.ToDecimal(x.d.AvIssueVoucherDetailsSoldQuantity))
                                   });
                var results = (from pd in _purchaseRequisitionDetailsRepository.GetAsQueryable().Where($"1 == 1  {filteredValue}")

                               join pm in dbContext.PurchaseRequisition on (decimal?)pd.PurchaseRequisitionId equals pm.PurchaseRequisitionId
                               join im in dbContext.ItemMaster on (long)pd.PurchaseRequisitionDetailsMatId equals im.ItemMasterItemId into imJoin
                               from im in imJoin.DefaultIfEmpty()

                               join um in dbContext.UnitMaster on pd.PurchaseRequisitionDetailsUnitId equals um.UnitMasterUnitId into umJoin
                               from um in umJoin.DefaultIfEmpty()

                               join pods in subqueryRDI on pd.PurchaseRequisitionDetailsMatId equals pods.PurchaseOrderDetailsPrdId into podJoin
                               from pod in podJoin.DefaultIfEmpty()

                               join vd in subqueryIMT on im.ItemMasterItemId equals Convert.ToInt64(vd.Key) into vdJoin
                               from vd in vdJoin.DefaultIfEmpty()
                               join vda in subquerySEI on im.ItemMasterItemId equals Convert.ToInt64(vda.Key) into vdaJoin
                               from vda in vdaJoin.DefaultIfEmpty()

                                   // where pd.PurchaseRequisitionDetailsReqStatus == "OPEN" && "$ {filteredValue}"

                               select new PurChaseRequisitionStatus
                               {
                                   issueQty = Convert.ToInt32(vd.issueQty),
                                   BalToIssue = Convert.ToInt32(vd.BalToIssue),
                                   stock = Convert.ToInt32(vd.stock),
                                   Unit = um.UnitMasterUnitShortName + " :: " + um.UnitMasterUnitId.ToString(),
                                   PartNo = Convert.ToString(im.ItemMasterPartNo),
                                   DelQty = Convert.ToInt32(pod.Quantity),
                                   PoStatus = pod.GeneralPurchaseOrderStatus,
                                   RequisitionDate = Convert.ToDateTime(pm.PurchaseRequisitionDate)

                               }).ToListAsync();
                return new Response<List<PurChaseRequisitionStatus>>()
                {
                    Valid = true,
                    Message = "Data Found",
                    Result = data
                };
            }
            catch (Exception ex)
            {
                return Response<List<PurChaseRequisitionStatus>>.Fail(new List<PurChaseRequisitionStatus>(), ex.Message);
            }
        }

        public async Task<Response<List<PurChaseReqFields>>> GetPurchaseReqJobId(PurChaseRequisitionFilterReport model)
        {
            try
            {
                var query = _purchaseRequisitionRepository.GetAsQueryable().AsNoTracking().Where(s => s.PurchaseRequisitionDelStatus != true);
                IQueryable<OrderApprovalHierarchy> oahQuery = _orderApprovalHierarchy.GetAsQueryable();

                if (model.JobId > 0)
                    query = query.Where(a => a.PurchaseRequisitionJobId == model.JobId);

                if (model.locationId > 0)
                    query = query.Where(a => a.PurchaseRequisitionLocationId == model.locationId);

                if (model.IsDateChecked && model.fromDate.HasValue && model.toDate.HasValue)
                    query = query.Where(a => a.PurchaseRequisitionDate.Date >= model.fromDate.Value && a.PurchaseRequisitionDate.Date <= model.toDate.Value);

                var purReqIdList = query.Select(x => x.PurchaseRequisitionId).ToList();

                var purOrderQuantities = _purchaseOrderDetailsRepository.GetAsQueryable()
                    .Where(x => purReqIdList.Contains(x.PurchaseRequisitionId))
                    .AsNoTracking()
                    .GroupBy(p => new { p.PurchaseRequisitionId, p.PurchaseOrderDetailsMatId })
                    .Select(g => new
                    {
                        g.Key.PurchaseRequisitionId,
                        g.Key.PurchaseOrderDetailsMatId,
                        RecQty = g.Sum(x => x.PurchaseOrderDetailsQuantity)
                    }).ToList();

                var hasApproval = _approverLevelSettings.GetAsQueryable()
                                                                                          .AsNoTracking()
                                                                                          .Any(a => a.FormType == EnumOrderForms.PurchaseOrder);

                List<PurChaseReqFields> finalData;

                if (hasApproval)
                {
                    var data = await (from sm in query
                                      join per in oahQuery on sm.PurchaseRequisitionId equals per.OrderId
                                      join sd in _purchaseRequisitionDetailsRepository.GetAsQueryable().AsNoTracking() on sm.PurchaseRequisitionId equals sd.PurchaseRequisitionId
                                      join im in _itemMasterRepository.GetAsQueryable().AsNoTracking() on sd.PurchaseRequisitionDetailsMatId equals im.ItemMasterItemId
                                      where per.Status == EnumApprovalStatus.Approved && per.Status != EnumApprovalStatus.Rejected
                                      group new { sm, sd, im } by new
                                      {
                                          sd.PurchaseRequisitionDetailsId,
                                          sd.PurchaseRequisitionDetailsMatId,
                                          im.ItemMasterItemName,
                                          sm.PurchaseRequisitionId,
                                          sm.PurchaseRequisitionNo,
                                          sm.PurchaseRequisitionDate,
                                          sd.PurchaseRequisitionDetailsRate,
                                          sd.PurchaseRequisitionDetailsQuantity,
                                          sd.PurchaseRequisitionDetailsUnitId,
                                          sm.PurchaseRequisitionJobId
                                      } into g
                                      select new PurChaseReqFields
                                      {
                                          matId = (int)g.Key.PurchaseRequisitionDetailsMatId,
                                          Item_Name = g.Key.ItemMasterItemName,
                                          SRID = (int)g.Key.PurchaseRequisitionId,
                                          SRDID = (int)g.Key.PurchaseRequisitionDetailsId,
                                          SRNo = g.Key.PurchaseRequisitionNo,
                                          Date = g.Key.PurchaseRequisitionDate,
                                          Rate = g.Key.PurchaseRequisitionDetailsRate,
                                          SRQ = (int)g.Key.PurchaseRequisitionDetailsQuantity,
                                          unitid = g.Key.PurchaseRequisitionDetailsUnitId,
                                          jobid = g.Key.PurchaseRequisitionJobId,
                                          Unit = _unitMasterRepository.GetAsQueryable()
                                                   .Where(x => x.UnitMasterUnitId == g.Key.PurchaseRequisitionDetailsUnitId)
                                                   .Select(x => x.UnitMasterUnitFullName.Trim())
                                                   .FirstOrDefault()
                                      })
                        .OrderBy(x => x.Date)
                        .ThenBy(x => x.SRNo)
                        .ToListAsync();

                    finalData = data.Select(x =>
                    {
                        var receivedQty = purOrderQuantities.FirstOrDefault(pr => pr.PurchaseRequisitionId == x.SRID && pr.PurchaseOrderDetailsMatId == x.matId)?.RecQty ?? 0;

                        return new PurChaseReqFields
                        {
                            matId = x.matId,
                            Item_Name = x.Item_Name,
                            SRID = x.SRID,
                            SRDID = x.SRDID,
                            SRNo = x.SRNo,
                            Date = x.Date,
                            Rate = x.Rate,
                            SRQ = x.SRQ,
                            unitid = x.unitid,
                            jobid = x.jobid,
                            Unit = x.Unit,
                            BalQty = x.SRQ - receivedQty,
                            DelQty = receivedQty
                        };
                    }).OrderBy(x => x.Date).ThenBy(x => x.SRNo).ToList();
                }
                else
                {
                    finalData = await (from sm in query
                                       join sd in _purchaseRequisitionDetailsRepository.GetAsQueryable().AsNoTracking()
                                           on sm.PurchaseRequisitionId equals sd.PurchaseRequisitionId
                                       join im in _itemMasterRepository.GetAsQueryable().AsNoTracking()
                                           on sd.PurchaseRequisitionDetailsMatId equals im.ItemMasterItemId into imGroup
                                       from im in imGroup.DefaultIfEmpty()
                                       join um in _unitMasterRepository.GetAsQueryable().AsNoTracking()
                                           on sd.PurchaseRequisitionDetailsUnitId equals um.UnitMasterUnitId into umGroup
                                       from um in umGroup.DefaultIfEmpty()
                                       join iv in _context.IssueVoucherDetails.AsNoTracking()
                                           on sd.PurchaseRequisitionDetailsId equals iv.IssueVoucherDetailsReqDId into ivGroup
                                       from srq in ivGroup.DefaultIfEmpty()
                                       select new PurChaseReqFields
                                       {
                                           PurchaseRequisitionPartyName = sm.PurchaseRequisitionPartyName ?? "",
                                           purchaseRequisitionType = sm.PurchaseRequisitionType ?? "",
                                           Item_Name = im.ItemMasterItemName ?? "",
                                           Unit = um.UnitMasterUnitShortName ?? "",
                                           DelQty = srq.IssueVoucherDetailsQuantity ?? 0,
                                           SRID = sm.PurchaseRequisitionId,
                                           SRDID = sd.PurchaseRequisitionDetailsId,
                                           SRNo = sm.PurchaseRequisitionNo ?? "",
                                           Date = sm.PurchaseRequisitionDate,
                                           Rate = sd.PurchaseRequisitionDetailsRate ?? 0,
                                           SRQ = sd.PurchaseRequisitionDetailsQuantity ?? 0,
                                           BalQty = (sd.PurchaseRequisitionDetailsQuantity ?? 0) - (srq.IssueVoucherDetailsQuantity ?? 0),
                                           matId = sd.PurchaseRequisitionDetailsMatId,
                                           unitid = sd.PurchaseRequisitionDetailsUnitId,
                                           jobid = sm.PurchaseRequisitionJobId,
                                           Stock = srq.IssueVoucherDetailsCurrentStockQty ?? 0
                                       })
                        .OrderBy(x => x.SRDID)
                        .ToListAsync();
                }

                finalData = finalData.Where(a => a.BalQty > 0).ToList();

                return new Response<List<PurChaseReqFields>>
                {
                    Valid = true,
                    Message = "Data Found",
                    Result = finalData
                };
            }
            catch (Exception ex)
            {
                return Response<List<PurChaseReqFields>>.Fail(new List<PurChaseReqFields>(), ex.Message);
            }
        }




        public async Task<Response<List<PurChaseReqFields>>> GetPurchaseReqJobId11223(PurChaseRequisitionFilterReport model)
        {
            List<PurChaseReqFields> finalData = new List<PurChaseReqFields>();
            try
            {
                string filteredValue = string.Empty;
                var query = _purchaseRequisitionRepository.GetAsQueryable().AsNoTracking().Where(s => s.PurchaseRequisitionDelStatus != true);
                if (model.JobId > 0)
                {
                    query = query.Where(a => a.PurchaseRequisitionJobId == model.JobId);
                }
                if (model.locationId > 0)
                {
                    query = query.Where(a => a.PurchaseRequisitionLocationId == model.locationId);
                }
                if (model.IsDateChecked)
                {
                    query = query.Where(a => a.PurchaseRequisitionDate.Date >= model.fromDate.Value && a.PurchaseRequisitionDate.Date <= model.toDate.Value);
                }

                var check = _context.tbl_approvalforms.AsNoTracking().Where(a => a.Voucher_Type.Trim() == "Purchase Requisition" && a.is_active == true).ToList();

                var purReqId = query.Select(x => x.PurchaseRequisitionId).ToList();
                var purOrderQuantities = _purchaseOrderDetailsRepository.GetAsQueryable().Where(x => purReqId.Contains(x.PurchaseRequisitionId))
                    .AsNoTracking()
                    .GroupBy(p => new { p.PurchaseRequisitionId, p.PurchaseOrderDetailsMatId })
                    .Select(g => new
                    {
                        PurReqId = (decimal)g.Key.PurchaseRequisitionId,
                        ItemCode = (decimal)g.Key.PurchaseOrderDetailsMatId,
                        RecQty = g.Sum(x => x.PurchaseOrderDetailsQuantity)
                    }).ToList();

                if (check.Count > 0)
                {
                    var data = await (from sm in query
                                      join per in _permissionApprovalDetailRepo.GetAsQueryable() on sm.PurchaseRequisitionId equals per.VoucherId
                                      join sd in _purchaseRequisitionDetailsRepository.GetAsQueryable().AsNoTracking()
                                          on (decimal?)sm.PurchaseRequisitionId equals sd.PurchaseRequisitionId
                                      join im in _itemMasterRepository.GetAsQueryable().AsNoTracking()
                                          on (long?)sd.PurchaseRequisitionDetailsMatId equals im.ItemMasterItemId
                                      join um in _unitMasterRepository.GetAsQueryable().AsNoTracking()
                                          on sd.PurchaseRequisitionDetailsUnitId equals um.UnitMasterUnitId
                                      join iv in _context.IssueVoucherDetails.AsNoTracking()
                                          on sd.PurchaseRequisitionDetailsId equals iv.IssueVoucherDetailsReqDId into ivGroup
                                      from srq in ivGroup.DefaultIfEmpty()
                                      where (_permissionApprovalDetailRepo.GetAsQueryable().Where(p => p.VoucherNo == sm.PurchaseRequisitionNo && (p.IsDeleted == null || (bool)!p.IsDeleted))
                                      .Any(p => p.Status == "Approved" && p.Status != "Rejected"))
                                      group new { sm, sd, im, um, srq } by new
                                      {
                                          sd.PurchaseRequisitionDetailsId,
                                          sd.PurchaseRequisitionDetailsMatId,
                                          im.ItemMasterItemName,
                                          sm.PurchaseRequisitionId,
                                          sm.PurchaseRequisitionNo,
                                          sm.PurchaseRequisitionDate,
                                          sd.PurchaseRequisitionDetailsRate,
                                          sd.PurchaseRequisitionDetailsQuantity,
                                          sd.PurchaseRequisitionDetailsUnitId,
                                          sm.PurchaseRequisitionJobId
                                      } into g
                                      select new PurChaseReqFields
                                      {
                                          matId = (int)g.Key.PurchaseRequisitionDetailsMatId,
                                          Item_Name = g.Key.ItemMasterItemName,
                                          SRID = (int)g.Key.PurchaseRequisitionId,
                                          SRDID = (int)g.Key.PurchaseRequisitionDetailsId,
                                          SRNo = g.Key.PurchaseRequisitionNo,
                                          Date = g.Key.PurchaseRequisitionDate,
                                          Rate = g.Key.PurchaseRequisitionDetailsRate,
                                          SRQ = (int)g.Key.PurchaseRequisitionDetailsQuantity,
                                          unitid = g.Key.PurchaseRequisitionDetailsUnitId,
                                          jobid = g.Key.PurchaseRequisitionJobId,

                                          Unit = _unitMasterRepository.GetAsQueryable()
                                                        .Where(x => x.UnitMasterUnitId == g.Key.PurchaseRequisitionDetailsUnitId)
                                                        .Select(x => x.UnitMasterUnitFullName.Trim())
                                                        .FirstOrDefault(),
                                          //if both the quantities were need then uncomment the code
                                          //BalQty =  _context.IssueVoucherDetails.AsNoTracking() .Where(iv => iv.IssueVoucherDetailsReqDId == g.Key.PurchaseRequisitionDetailsId) .Sum(iv => (int?)iv.IssueVoucherDetailsQuantity) ?? 0,
                                          //DelQty =  _context.IssueVoucherDetails.AsNoTracking().Where(iv => iv.IssueVoucherDetailsReqDId == g.Key.PurchaseRequisitionDetailsId).Sum(iv => (int?)iv.IssueVoucherDetailsQuantity) ?? 0,
                                      })
                   .OrderBy(x => x.Date)
                   .ThenBy(x => x.SRNo)
                   .ToListAsync();

                    finalData = data.Select(x =>
                    {
                        var receivedQty = purOrderQuantities.FirstOrDefault(pr => pr.PurReqId == x.SRID && pr.ItemCode == x.matId)?.RecQty ?? 0;

                        return new PurChaseReqFields
                        {
                            matId = x.matId,
                            Item_Name = x.Item_Name,
                            SRID = x.SRID,
                            SRDID = x.SRDID,
                            SRNo = x.SRNo,
                            Date = x.Date,
                            Rate = x.Rate,
                            SRQ = x.SRQ,
                            unitid = x.unitid,
                            jobid = x.jobid,
                            Unit = _unitMasterRepository.GetAsQueryable()
                                       .Where(u => u.UnitMasterUnitId == x.unitid)
                                       .Select(u => u.UnitMasterUnitFullName.Trim())
                                       .FirstOrDefault(),
                            BalQty = x.SRQ - receivedQty,
                            DelQty = receivedQty
                        };
                    }).OrderBy(x => x.Date)
                      .ThenBy(x => x.SRNo)
                      .ToList();

                }
                else
                {
                    finalData = await (from sm in query
                                       join sd in _purchaseRequisitionDetailsRepository.GetAsQueryable().AsNoTracking()
                                           on (decimal?)sm.PurchaseRequisitionId equals sd.PurchaseRequisitionId
                                       join im in _itemMasterRepository.GetAsQueryable().AsNoTracking()
                                           on (long?)sd.PurchaseRequisitionDetailsMatId equals im.ItemMasterItemId into imGroup
                                       from im in imGroup.DefaultIfEmpty()
                                       join um in _unitMasterRepository.GetAsQueryable().AsNoTracking()
                                           on sd.PurchaseRequisitionDetailsUnitId equals um.UnitMasterUnitId into umGroup
                                       from um in umGroup.DefaultIfEmpty()
                                       join jm in jobrepository.GetAsQueryable().AsNoTracking()
                                           on sm.PurchaseRequisitionJobId equals jm.JobMasterJobId into jmGroup
                                       from jm in jmGroup.DefaultIfEmpty()
                                       join lm in locationrepository.GetAsQueryable().AsNoTracking()
                                           on sm.PurchaseRequisitionLocationId equals lm.LocationMasterLocationId into lmGroup
                                       from lm in lmGroup.DefaultIfEmpty()
                                       join iv in _context.IssueVoucherDetails.AsNoTracking()
                                           on sd.PurchaseRequisitionDetailsId equals iv.IssueVoucherDetailsReqDId into ivGroup
                                       from srq in ivGroup.DefaultIfEmpty()
                                       select new PurChaseReqFields
                                       {
                                           PurchaseRequisitionPartyName = sm.PurchaseRequisitionPartyName ?? "",
                                           purchaseRequisitionType = sm.PurchaseRequisitionType ?? "",
                                           Item_Name = im != null ? im.ItemMasterItemName : "",
                                           Unit = um != null ? um.UnitMasterUnitShortName : "",
                                           DelQty = srq.IssueVoucherDetailsQuantity ?? 0m,
                                           SRID = sm.PurchaseRequisitionId,
                                           SRDID = sd.PurchaseRequisitionDetailsId,
                                           SRNo = sm.PurchaseRequisitionNo ?? "",
                                           Date = sm.PurchaseRequisitionDate,
                                           Rate = sd.PurchaseRequisitionDetailsRate ?? 0,
                                           SRQ = sd.PurchaseRequisitionDetailsQuantity ?? 0,
                                           BalQty = (sd.PurchaseRequisitionDetailsQuantity - (srq.IssueVoucherDetailsQuantity ?? 0m)) ?? 0,
                                           matId = sd.PurchaseRequisitionDetailsMatId,
                                           unitid = sd.PurchaseRequisitionDetailsUnitId,
                                           jobid = sm.PurchaseRequisitionJobId,
                                           Stock = srq.IssueVoucherDetailsCurrentStockQty ?? 0m
                                       })
                         .OrderBy(x => x.SRDID)
                         .ToListAsync();

                }
                finalData = finalData.Where(a => a.BalQty > 0).ToList();
                return new Response<List<PurChaseReqFields>>()
                {
                    Valid = true,
                    Message = "Data Found",
                    Result = finalData
                };
            }
            catch (Exception ex)
            {
                return Response<List<PurChaseReqFields>>.Fail(new List<PurChaseReqFields>(), ex.Message);
            }
        }

        public ResponseInfo GetBalanceAmount(int JobId)
        {
            var obj = new ResponseInfo();
            try
            {
                var budgetQuery = from b in _budgetDetailsrepository.GetAsQueryable()
                                  where b.JobMasterBudgetDetailsJobId == JobId
                                  group b by b.JobMasterBudgetDetailsJobId into g
                                  select new
                                  {
                                      JobId = g.Key,
                                      TotalBudget = g.Sum(x => x.JobMasterBudgetDetailsBudAmount)
                                  };

                var requisitionQuery = from r in _purchaseRequisitionRepository.GetAsQueryable()
                                       where r.PurchaseRequisitionJobId == JobId
                                       group r by r.PurchaseRequisitionJobId into g
                                       select new
                                       {
                                           JobId = g.Key,
                                           TotalPurchaseRequisition = g.Sum(x => x.PurchaseRequisitionNetAmount)
                                       };

                var result = from b in budgetQuery
                             join r in requisitionQuery on b.JobId equals r.JobId into br
                             from r in br.DefaultIfEmpty()
                             select new
                             {
                                 BalanceAmount = (b.TotalBudget ?? 0) - (r.TotalPurchaseRequisition ?? 0)
                             };

                var balanceAmount = result.FirstOrDefault();


                obj.ResultSet = balanceAmount;
                obj.IsSuccess = true;
            }
            catch (Exception ex)
            {
                obj.IsSuccess = false;
            }
            return obj;
        }

        public bool IsPurchaseRequisitionQtyExcededFromBOQ(int BoqId, List<PurchaseReqItesm> reqItems)
        {
            try
            {
                var itemIds = reqItems.Select(x => x.ItemCode).ToList();

                var totalBoqQty = this._BillOfQTyDetailsRepository.GetAsQueryable()
                    .AsNoTracking()
                    .Where(x => x.BillQtyId == BoqId && itemIds.Contains((int)x.ItemCode))
                    .GroupBy(x => new { x.BillQtyId, x.ItemCode })
                    .Select(x => new
                    {
                        ItemCode = x.Key.ItemCode,
                        ItemQty = x.Sum(x => x.Qty)
                    }).ToList();

                var totalPrQty = this._purchaseRequisitionDetailsRepository.GetAsQueryable()
                    .Where(x => x.boqdId == BoqId && itemIds.Contains((int)x.PurchaseRequisitionDetailsMatId))
                    .GroupBy(x => new { x.boqdId, x.PurchaseRequisitionDetailsMatId })
                    .Select(x => new
                    {
                        ItemCode = x.Key.PurchaseRequisitionDetailsMatId,
                        ItemQty = x.Sum(x => x.PurchaseRequisitionDetailsQuantity)
                    }).ToList();

                var result = totalPrQty.Join(reqItems,
                    pr => pr.ItemCode,
                    rq => rq.ItemCode,
                    (pr, rq) => new
                    {
                        ItemCode = pr.ItemCode,
                        ItemQty = pr.ItemQty + rq.ItemQty
                    }).ToList();

                if (!totalPrQty.Any())
                    return totalBoqQty.Any(boq => reqItems.Any(r => r.ItemCode == boq.ItemCode && r.ItemQty > boq.ItemQty));
                if (totalBoqQty.Any(boq => result.Any(r => r.ItemCode == boq.ItemCode && r.ItemQty > boq.ItemQty)))
                    return true;
                if (totalBoqQty.Any(boq => totalPrQty.Any(r => r.ItemCode == boq.ItemCode && r.ItemQty == boq.ItemQty)))
                    return false;
                return totalBoqQty.Any(boq => result.Any(r => r.ItemCode == boq.ItemCode && r.ItemQty > boq.ItemQty));
            }
            catch (Exception ex)
            {
                throw;
            }

        }


    }
}
