using Inspire.Erp.Application.Account.Implementations;
using Inspire.Erp.Application.Common;
using Inspire.Erp.Application.Common.Email;
using Inspire.Erp.Application.Email.Interfaces;
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
using Inspire.Erp.Domain.Modals.Common;
using Inspire.Erp.Infrastructure;
using Inspire.Erp.Infrastructure.Database;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using Inspire.Erp.NotificationBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Inspire.Erp.Application.Procurement.Implementation
{
    public class PurchaseRequisitionService : IPurchaseRequisitionService
    {
        private readonly IGeneralSettings _generalSettings;
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
        private IRepository<JobMasterJobDetails> _jobMasterJobDetailsRepo;
        private readonly IOrderEmailNotificationService _orderEmailNotificationService;
        private IRepository<LocationMaster> locationrepository;
        private IRepository<BillOfQTyDetails> _BillOfQTyDetailsRepository;
        private IRepository<TrackingRegister> _trackingRegisterRepository;
        private IRepository<ItemMaster> _itemMasterRepository;
        private IRepository<SalesVoucherDetails> _salesVocuherDetailsRepository;
        private IRepository<UnitMaster> _unitMasterRepository;
        private IRepository<SuppliersMaster> _suppliersMasterRepository;
        private IRepository<LocationMaster> _locationMasterRepository;
        private IRepository<JobMasterControl> _jobMasterControls;
        private readonly IRepository<PurchaseOrderDetails> _purchaseOrderDetailsRepository;
        private readonly IRepository<ApproverLevelSettings> _approverLevelSettings;
        private readonly IRepository<OrderApprovalHierarchy> _orderApprovalHierarchy;
        private ApplicationSettings applicationSettings;
        DownloadAndSaveVoucherReport downloadAndSaveVoucherReport;
        private readonly ICurrentUserService _currentUserService;
        public PurchaseRequisitionService(IRepository<ItemMaster> itemMasterRepository, IRepository<UnitMaster> unitMasterRepository, IRepository<SalesVoucherDetails> salesVocuherDetailsRepository,
            IRepository<SuppliersMaster> suppliersMasterRepository, IConfiguration config, IDocsAttachmentService docsAttachmentService,
            IRepository<LocationMaster> _locationrepository, IRepository<LocationMaster> locationMasterRepository,
            IRepository<PermissionApproval> permissionApprovalRepo, IRepository<Approval> approvalRepo, IRepository<ApprovalDetail> approvalDetailRepo,
            IRepository<AccountsTransactions> accountTransactionRepository, IRepository<StockRegister> stockRegisterRepository, IRepository<ProgramSettings> programsettingsRepository,
            IRepository<VouchersNumbers> voucherNumbers, ILogger<PaymentVoucherService> logger, IRepository<JobMaster> _jobrepository, IRepository<BillOfQTyDetails> BillOfQTyDetailsRepository,
             IRepository<TrackingRegister> trackingRegisterRepository, IRepository<PermissionApprovalDetail> permissionApprovalDetailRepo,
        IRepository<PurchaseRequisition> purchaseRequisitionRepository, InspireErpDBContext context, IRepository<PurchaseRequisitionDetails> purchaseRequisitionDetailsRepository, IRepository<JobMasterBudgetDetails> budgetDetailsrepository, IRepository<PurchaseOrderDetails> purchaseOrderDetailsRepository,
        IOrderEmailNotificationService orderEmailNotificationService, IRepository<ApproverLevelSettings> approverLevelSettings, IRepository<OrderApprovalHierarchy> orderApprovalHierarchy, IRepository<JobMasterJobDetails> jobMasterJobDetailsRepo, IGeneralSettings generalSettings, IRepository<JobMasterControl> jobMasterControls, ICurrentUserService currentUserService)
        {
            applicationSettings = config.GetSection("ApplicationSettings").Get<ApplicationSettings>();
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
            _jobMasterJobDetailsRepo = jobMasterJobDetailsRepo;
            _salesVocuherDetailsRepository = salesVocuherDetailsRepository;
            _generalSettings = generalSettings;
            downloadAndSaveVoucherReport = new DownloadAndSaveVoucherReport(docsAttachmentService);
            _jobMasterControls = jobMasterControls;
            _currentUserService = currentUserService;
        }
        public async Task<Response<PurchaseRequisitionModel>> UpdatePurchaseRequisition(PurchaseRequisition purchaseRequisition, List<AccountsTransactions> accountsTransactions, List<PurchaseRequisitionDetails> purchaseRequisitionDetails)
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


                await downloadAndSaveVoucherReport.DownloadedFileAsync(applicationSettings.DownloadFileBaseUrl, "PurchaseRequisition", purchaseRequisition.PurchaseRequisitionId.ToString(), (int)purchaseRequisition.PurchaseRequisitionId, purchaseRequisition.PurchaseRequisitionNo, true);

            }
            catch (Exception ex)
            {
                _purchaseRequisitionRepository.TransactionRollback();
                throw ex;
            }

            return await this.GetSavedPurchaseRequisitionDetails((int)purchaseRequisition.PurchaseRequisitionId);
        }

        public async Task<Response<int>> DeletePurchaseRequisition(PurchaseRequisition purchaseRequisition, List<AccountsTransactions> accountsTransactions,
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

            return Response<int>.Success(i, "Deleted");

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
        public async Task<Response<PurchaseRequisitionModel>> InsertPurchaseRequisition(PurchaseRequisition purchaseRequisition, List<AccountsTransactions> accountsTransactions,
            List<PurchaseRequisitionDetails> purchaseRequisitionDetails
            )
        {
            try
            {
                var settingValue = await _generalSettings.GetApproverStatusAsync(purchaseRequisition.PurchaseRequisitionJobId.ToString(), nameof(EnumOrderForms.PurchaseRequisition));

                if (!string.IsNullOrEmpty(settingValue.ApproverStatus))
                    return Response<PurchaseRequisitionModel>.Fail(null, settingValue.ApproverStatus);

                //var settingValue = await _generalSettings.GetValueByKey(InspireConstants.DefaultApprovalType);

                //if (settingValue == InspireConstants.ApprovalAsJobWise)
                //{
                //    var jobWiseStatus = await _generalSettings.GetApproverStatusAsync(purchaseRequisition.PurchaseRequisitionJobId.ToString(), nameof(EnumOrderForms.PurchaseRequisition),true);
                //    if (!string.IsNullOrEmpty(jobWiseStatus))
                //        return Response<PurchaseRequisitionModel>.Fail(null, jobWiseStatus);
                //}

                //if (settingValue == InspireConstants.ApprovalAsVoucherWise)
                //{
                //    var jobWiseStatus = await _generalSettings.GetApproverStatusAsync("-1", nameof(EnumOrderForms.PurchaseRequisition),false);
                //    if (!string.IsNullOrEmpty(jobWiseStatus))
                //        return Response<PurchaseRequisitionModel>.Fail(null, jobWiseStatus);
                //}



                _purchaseRequisitionRepository.BeginTransaction();

                var jobNum = jobrepository.GetAsQueryable().AsNoTracking()
                .Where(e => e.JobMasterJobId == purchaseRequisition.PurchaseRequisitionJobId)
                .FirstOrDefault()?.JobMasterJobNumber ?? "GEN";

                string purchaseRequisitionNumber = clsCommonFunctions.GenrateVoucherNo(VoucherType.PurchaseRequisition_TYPE, Prefix.PurchaseRequisition_Prefix,
                    _programsettingsRepository, _voucherNumbersRepository, jobNum);
                purchaseRequisition.PurchaseRequisitionNo = purchaseRequisitionNumber;

                int maxcount = 0;
                maxcount = Convert.ToInt32(
                    _purchaseRequisitionRepository.GetAsQueryable()
                    .DefaultIfEmpty().Max(o => o == null ? 0 : o.PurchaseRequisitionId) + 1);

                purchaseRequisition.PurchaseRequisitionId = maxcount;


                purchaseRequisition.PurchaseRequisitionDetails.ToList().ForEach(x =>
                {
                    x.PurchaseRequisitionDetailsId = null;
                    x.PurchaseRequisitionId = maxcount;
                    x.PurchaseRequisitionDetailsNo = purchaseRequisitionNumber;

                });


                purchaseRequisition.PurchaseRequisitionUserId = _currentUserService.UserId;
                _purchaseRequisitionRepository.Insert(purchaseRequisition);

                
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
                var filePath = await downloadAndSaveVoucherReport.DownloadedFileAsync(applicationSettings.DownloadFileBaseUrl, "PurchaseRequisition", purchaseRequisition.PurchaseRequisitionId.ToString(), (int)purchaseRequisition.PurchaseRequisitionId, purchaseRequisition.PurchaseRequisitionNo);

                #region EmailNotification
                var Username = _currentUserService.Username;
                var jobId = purchaseRequisition?.PurchaseRequisitionJobId ?? -1;
                var jobDetail = jobrepository.GetAsQueryable().Where(x => x.JobMasterJobId == jobId).FirstOrDefault();
                await _orderEmailNotificationService.SendOrderEmailNotificationAsync(
                                                                                EnumOrderForms.PurchaseRequisition,
                                                                                EnumApproverLevels.None,
                                                                                "Requisition",
                                                                                purchaseRequisition.PurchaseRequisitionNo,
                                                                                purchaseRequisition.PurchaseRequisitionDate,
                                                                                purchaseRequisition.PurchaseRequisitionPartyName,
                                                                                (decimal)purchaseRequisition.PurchaseRequisitionNetAmount,
                                                                                (int)purchaseRequisition.PurchaseRequisitionId,
                                                                                (int)jobId,
                                                                                jobDetail == null ? "" : jobDetail.JobMasterJobNumber,
                                                                                jobDetail == null ? "" : jobDetail.JobMasterJobName,
                                                                                true,
                                                                               Username,
                                                                                settingValue.GeneralSettingValue,
                                                                                new List<string> { filePath },
                                                                                -1,
                                                                                null,
                                                                                (long)purchaseRequisition.PurchaseRequisitionUserId
                                                                                );
                //await SendEmailOnValueExceedsAsync((int)purchaseRequisition.PurchaseRequisitionJobId, purchaseRequisition.PurchaseRequisitionNo);
                #endregion
                _purchaseRequisitionRepository.TransactionCommit();


                var res = await this.GetSavedPurchaseRequisitionDetails((int)purchaseRequisition.PurchaseRequisitionId);
                return Response<PurchaseRequisitionModel>.Success(res.Result, "Purchase Requisition Save Successfully.");

            }
            catch (Exception ex)
            {
                _purchaseRequisitionRepository.TransactionRollback();
                throw ex;
            }



        }

        private async Task SendEmailOnValueExceedsAsync(int jobId, string prNo)
        {
            try
            {
                var jobDetail = GetJobDetail(jobId);
                if (jobDetail == null)
                    return;

                decimal jobValue = GetJobTotalValue(jobId);
                decimal prValue = GetTotalPrValue(jobId);
                if ((jobValue <= 0) || (prValue <= jobValue))
                    return;

                string subject = $"PR Amount Exceeds Budget Amount against {jobDetail.JobMasterJobNumber}-{jobDetail.JobMasterJobName}";
                string messageBody = GenerateEmailBody(jobDetail, prValue, prNo);

                var recipients = await GetEmailRecipients(jobId);

                await _orderEmailNotificationService.SendTextEmailAsync(recipients, subject, messageBody, new List<string>());
            }
            catch (Exception)
            {
                throw;
            }
        }

        private JobMaster GetJobDetail(int jobId)
        {
            return jobrepository.GetAsQueryable()
                                .AsNoTracking()
                                .SingleOrDefault(x => x.JobMasterJobId == jobId);
        }

        private decimal GetJobTotalValue(int jobId)
        {
            return _jobMasterControls.GetAsQueryable()
                                           .AsNoTracking()
                                           .Where(x => x.JobMasterControlJobID == jobId)
                                           .Sum(x => x.JobMasterControlAmount ?? 0);
        }

        private decimal GetTotalPrValue(int jobId)
        {
            return _purchaseRequisitionRepository.GetAsQueryable()
                                                 .AsNoTracking()
                                                 .Where(x => x.PurchaseRequisitionJobId == jobId)
                                                 .Sum(x => x.PurchaseRequisitionNetAmount ?? 0);
        }

        private string GenerateEmailBody(JobMaster jobDetail, decimal prValue, string prNo)
        {
            return InspireConstants.PurchaseRequisitionEmailTemplate
                                   .Replace("{{Recipient}}", "Ali Ansari")
                                   .Replace("{{jobCode}}", jobDetail.JobMasterJobNumber)
                                   .Replace("{{jobName}}", jobDetail.JobMasterJobName)
                                   .Replace("{{jobValue}}", jobDetail.JobMasterJobValue?.ToString("n2"))
                                   .Replace("{{prNetAmount}}", prValue.ToString("n2"))
                                   .Replace("{{prNO}}", prNo);
        }

        public async Task<List<UserEmail>> GetEmailRecipients(int jobId)
        {
            try
            {
                var result = await _approverLevelSettings.GetAsQueryable().AsNoTracking()
                              .Where(x => (int)x.JobId == jobId &&
                                          x.OptEmail &&
                                          x.ApproverLevel == EnumApproverLevels.Level1 &&
                                          x.FormType == EnumOrderForms.PurchaseRequisition)
                              .Select(a => new EmailMetaDataDto()
                              {
                                  JobId = a.JobId,
                                  NotificationDays = a.NotificationDays,
                                  MinAmount = a.MinAmount,
                                  MaxAmount = a.MaxAmount,
                                  MetaData = JsonConvert.DeserializeObject<List<UserEmail>>(a.MetaData),
                              }).ToListAsync();



                var toEmails = result.SelectMany(e => e.MetaData).Distinct().ToList();
                toEmails.AddRange(new List<UserEmail>
                {
                    new UserEmail
                    {
                        Email = "Shawn@waterbirdwtc.com",
                        Name = "Shawn",
                        BccEmails = new List<CCAndBccMailDto>(),
                        CCEmails = new List<CCAndBccMailDto>()
                    },
                    new UserEmail
                    {
                        Email = "jijit@waterbirdwtc.com",
                        Name = "Jijit",
                        BccEmails = new List<CCAndBccMailDto>(),
                        CCEmails = new List<CCAndBccMailDto>()
                    },
                    new UserEmail
                    {
                        Email = "ambrish@waterbirdwtc.com",
                        Name = "Ambrish",
                        BccEmails = new List<CCAndBccMailDto>(),
                        CCEmails = new List<CCAndBccMailDto>()
                    }
                });
                return toEmails;
            }
            catch (Exception)
            {
                throw;
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
        public async Task<Response<PurchaseRequisitionModel>> GetSavedPurchaseRequisitionDetails(int pvno)
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
            var settingValue = await _generalSettings.GetApproverStatusAsync(pvno.ToString(), nameof(EnumOrderForms.PurchaseRequisition));

            bool isNoApproval = settingValue.GeneralSettingValue == InspireConstants.ApprovalAsNoApproval;

            var approvalStatuses = await _orderApprovalHierarchy.GetAsQueryable()
                .AsNoTracking()
                .Where(x => x.OrderId == pvno && x.OrderType == EnumOrderForms.PurchaseRequisition)
                .Select(x => x.Status)
                .ToListAsync();

            purchaseRequisitionModel.purchaseRequisition.IsPrintEnabled =
                isNoApproval || approvalStatuses.All(x => x == EnumApprovalStatus.Approved);

            purchaseRequisitionModel.purchaseRequisition.IsEditEnabled =
                isNoApproval || approvalStatuses.All(x => x == EnumApprovalStatus.Pending);

            return Response<PurchaseRequisitionModel>.Success(purchaseRequisitionModel, "Data found");

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
                var settingValue = await _generalSettings.GetValueByKey(InspireConstants.DefaultApprovalType);

                var query = _purchaseRequisitionRepository.GetAsQueryable().AsNoTracking()
                    .Where(s => s.PurchaseRequisitionDelStatus != true);

                if (model.JobId > 0)
                    query = query.Where(a => a.PurchaseRequisitionJobId == model.JobId);

                if (model.locationId > 0)
                    query = query.Where(a => a.PurchaseRequisitionLocationId == model.locationId);

                //if (model.IsDateChecked && model.fromDate.HasValue && model.toDate.HasValue)
                //    query = query.Where(a => a.PurchaseRequisitionDate.Date >= model.fromDate.Value && a.PurchaseRequisitionDate.Date <= model.toDate.Value);
                if (model.IsDateChecked && model.fromDate.HasValue && model.toDate.HasValue)
                {
                    DateTime fromDate = model.fromDate.Value.Date;
                    DateTime toDate = model.toDate.Value.Date;

                    query = query.Where(a => a.PurchaseRequisitionDate.Date >= fromDate &&
                                             a.PurchaseRequisitionDate.Date <= toDate);
                }

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

                List<PurChaseReqFields> finalData;
                bool hasApproval = false;

                if (settingValue != InspireConstants.ApprovalAsNoApproval)
                {
                    hasApproval = _approverLevelSettings.GetAsQueryable()
                                     .AsNoTracking()
                                     .Any(a => a.FormType == EnumOrderForms.PurchaseRequisition);
                }

                if (hasApproval)
                {
                    var approvedOrderIds = await _orderApprovalHierarchy.GetAsQueryable().AsNoTracking()
                                                .Where(x => x.OrderType == EnumOrderForms.PurchaseRequisition)
                                                .Select(x => new { OrderId = x.OrderId, Status = x.Status })
                                                .ToListAsync();

                    var orderIdsApprovedByAll = approvedOrderIds
                        .GroupBy(x => x.OrderId)
                        .Where(g => g.All(a => a.Status == EnumApprovalStatus.Approved))
                        .Select(g => g.Key)
                        .ToList();

                    var rawData = await (
                        from sm in query
                        join sd in _purchaseRequisitionDetailsRepository.GetAsQueryable().AsNoTracking()
                            on sm.PurchaseRequisitionId equals sd.PurchaseRequisitionId
                        join im in _itemMasterRepository.GetAsQueryable().AsNoTracking()
                            on sd.PurchaseRequisitionDetailsMatId equals im.ItemMasterItemId
                        where orderIdsApprovedByAll.Contains((decimal)sm.PurchaseRequisitionId)
                        select new
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
                            sm.PurchaseRequisitionJobId,
                            sd.PurchaseRequisitionDetailsSupplierId,
                            sm.PurchaseRequisitionCurrencyId,
                            sm.deliveryatsite,
                            sm.confirmationemaildetails,
                            sm.paymentTerms,
                            sm.deliveryterms,
                            sm.PurchaseRequisitionDescription
                        }).ToListAsync();

                    finalData = rawData
                        .GroupBy(g => new
                        {
                            g.PurchaseRequisitionDetailsId,
                            g.PurchaseRequisitionDetailsMatId,
                            g.ItemMasterItemName,
                            g.PurchaseRequisitionId,
                            g.PurchaseRequisitionNo,
                            g.PurchaseRequisitionDate,
                            g.PurchaseRequisitionDetailsRate,
                            g.PurchaseRequisitionDetailsQuantity,
                            g.PurchaseRequisitionDetailsUnitId,
                            g.PurchaseRequisitionJobId,
                            g.PurchaseRequisitionDetailsSupplierId,
                            g.PurchaseRequisitionCurrencyId
                        })
                        .Select(g =>
                        {
                            var first = g.First();

                            return new PurChaseReqFields
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
                                PRSupplierId = g.Key.PurchaseRequisitionDetailsSupplierId,
                                PRCurrId = g.Key.PurchaseRequisitionCurrencyId,
                                Unit = _unitMasterRepository.GetAsQueryable()
                                    .Where(x => x.UnitMasterUnitId == g.Key.PurchaseRequisitionDetailsUnitId)
                                    .Select(x => x.UnitMasterUnitFullName.Trim())
                                    .FirstOrDefault(),
                                DeliverySite = first.deliveryatsite ?? "",
                                ConfirmationEmailDetails = first.confirmationemaildetails ?? "",
                                PaymentTerms = first.paymentTerms ?? "",
                                DeliveryTerms = first.deliveryterms ?? "",
                                Notes = first.PurchaseRequisitionDescription ?? ""
                            };
                        })
                        .OrderBy(x => x.Date)
                        .ThenBy(x => x.SRNo)
                        .ToList();

                    finalData = finalData.Select(x =>
                    {
                        var receivedQty = purOrderQuantities
                            .FirstOrDefault(pr => pr.PurchaseRequisitionId == x.SRID && pr.PurchaseOrderDetailsMatId == x.matId)?.RecQty ?? 0;

                        x.BalQty = x.SRQ - receivedQty;
                        x.DelQty = receivedQty;
                        return x;
                    }).ToList();
                }
                else
                {
                    finalData = await (
                        from sm in query
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
                            SRID = Convert.ToInt32(sm.PurchaseRequisitionId),
                            SRDID = Convert.ToInt32(sd.PurchaseRequisitionDetailsId),
                            SRNo = sm.PurchaseRequisitionNo ?? "",
                            Date = sm.PurchaseRequisitionDate,
                            Rate = sd.PurchaseRequisitionDetailsRate ?? 0,
                            SRQ = sd.PurchaseRequisitionDetailsQuantity ?? 0,
                            BalQty = (sd.PurchaseRequisitionDetailsQuantity ?? 0) - (srq.IssueVoucherDetailsQuantity ?? 0),
                            matId = sd.PurchaseRequisitionDetailsMatId,
                            unitid = sd.PurchaseRequisitionDetailsUnitId,
                            jobid = sm.PurchaseRequisitionJobId,
                            Stock = srq.IssueVoucherDetailsCurrentStockQty ?? 0,
                            PRSupplierId = sd.PurchaseRequisitionDetailsSupplierId,
                            PRCurrId = sm.PurchaseRequisitionCurrencyId,
                            DeliverySite = sm.deliveryatsite ?? "",
                            ConfirmationEmailDetails = sm.confirmationemaildetails ?? "",
                            PaymentTerms = sm.paymentTerms ?? "",
                            DeliveryTerms = sm.deliveryterms ?? "",
                            Notes = sm.PurchaseRequisitionDescription ?? ""
                        })
                        .OrderBy(x => x.SRDID)
                        .ToListAsync();
                }

                finalData = finalData.Where(a => a.BalQty > 0).DistinctBy(x=>x.SRID).ToList();

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

        public async Task<Response<List<PurChaseReqFields>>> GetPurchaseReqSale(int prId)
        {
            try
            {
                //var settingValue = await _generalSettings.GetValueByKey(InspireConstants.DefaultApprovalType);

                var query = _purchaseRequisitionRepository.GetAsQueryable().AsNoTracking().Where(s => s.PurchaseRequisitionDelStatus != true && s.PurchaseRequisitionId == prId);


                var purReqIdList = query.Select(x => x.PurchaseRequisitionId).ToList();

                var salesQuantities = _salesVocuherDetailsRepository.GetAsQueryable()
                    .Where(x => purReqIdList.Contains(x.PurchaseRequisitionId))
                    .AsNoTracking()
                    .GroupBy(p => new { p.PurchaseRequisitionId, p.SalesVoucherDetailsMatId })
                    .Select(g => new
                    {
                        g.Key.PurchaseRequisitionId,
                        g.Key.SalesVoucherDetailsMatId,
                        RecQty = g.Sum(x => x.SalesVoucherDetailsQuantity)
                    }).ToList();


                List<PurChaseReqFields> finalData;
                var data = await (from sm in query
                                  join sd in _purchaseRequisitionDetailsRepository.GetAsQueryable().AsNoTracking() on sm.PurchaseRequisitionId equals sd.PurchaseRequisitionId
                                  join im in _itemMasterRepository.GetAsQueryable().AsNoTracking() on sd.PurchaseRequisitionDetailsMatId equals im.ItemMasterItemId
                                  // where orderIdsApprovedByAll.Contains((decimal)sm.PurchaseRequisitionId)
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
                                      sm.PurchaseRequisitionJobId,
                                      sd.PurchaseRequisitionDetailsSupplierId,
                                      sm.PurchaseRequisitionCurrencyId,
                                  } into g
                                  select new PurChaseReqFields
                                  {
                                      matId = (int)g.Key.PurchaseRequisitionDetailsMatId,
                                      Item_Name = g.Key.ItemMasterItemName,
                                      SRID = (int)(int)g.Key.PurchaseRequisitionId,
                                      SRDID = (int)g.Key.PurchaseRequisitionDetailsId,
                                      SRNo = g.Key.PurchaseRequisitionNo,
                                      Date = g.Key.PurchaseRequisitionDate,
                                      Rate = g.Key.PurchaseRequisitionDetailsRate,
                                      SRQ = (int)g.Key.PurchaseRequisitionDetailsQuantity,
                                      unitid = g.Key.PurchaseRequisitionDetailsUnitId,
                                      jobid = g.Key.PurchaseRequisitionJobId,
                                      PRSupplierId = g.Key.PurchaseRequisitionDetailsSupplierId,
                                      PRCurrId = g.Key.PurchaseRequisitionCurrencyId,
                                      PRNO= g.Key.PurchaseRequisitionNo
                                     ,

                                      Unit = _unitMasterRepository.GetAsQueryable()
                                               .Where(x => x.UnitMasterUnitId == g.Key.PurchaseRequisitionDetailsUnitId)
                                               .Select(x => x.UnitMasterUnitShortName.Trim())
                                               .FirstOrDefault()
                                  })
                    .OrderBy(x => x.Date)
                    .ThenBy(x => x.SRNo)
                    .ToListAsync();

                finalData = data.Select(x =>
                {
                    var receivedQty = salesQuantities.FirstOrDefault(pr => pr.PurchaseRequisitionId == x.SRID && pr.SalesVoucherDetailsMatId == x.matId)?.RecQty ?? 0;

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
                        DelQty = receivedQty,
                        PRSupplierId = x.PRSupplierId,
                        PRCurrId = x.PRCurrId,
                       
                        PRNO = x.PRNO

                    };
                }).OrderBy(x => x.Date).ThenBy(x => x.SRNo).ToList();


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
        public ResponseInfo GetAllPReq(DateTime fromDate, DateTime toDate, bool isDate)
        {
            var obj = new ResponseInfo();

            try
            {
                var locations = locationrepository.GetAsQueryable()
                    .AsNoTracking()
                    .ToDictionary(x => x.LocationMasterLocationId, x => x.LocationMasterLocationName);

                var preqs = _purchaseRequisitionRepository.GetAsQueryable()
                    .AsNoTracking()
                    .Where(pr => pr.PurchaseRequisitionDelStatus != true);

                if (isDate)
                {
                    preqs = preqs.Where(pr => pr.PurchaseRequisitionDate >= fromDate && pr.PurchaseRequisitionDate <= toDate);
                }
                var preqDetails = _purchaseRequisitionDetailsRepository.GetAsQueryable().AsNoTracking();
                var salesDetails = _salesVocuherDetailsRepository.GetAsQueryable().AsNoTracking();

                var result = preqs
                    .ToList()
                    .Select(pr =>
                    {
                        var prQty = preqDetails
                            .Where(d => d.PurchaseRequisitionId == pr.PurchaseRequisitionId)
                            .Sum(d => (decimal?)d.PurchaseRequisitionDetailsQuantity) ?? 0;

                        var recQty = salesDetails
                            .Where(d => d.PurchaseRequisitionId == pr.PurchaseRequisitionId)
                            .Sum(d => (decimal?)d.SalesVoucherDetailsQuantity) ?? 0;

                        return new
                        {
                            pr.PurchaseRequisitionId,
                            loc = locations.TryGetValue(pr.PurchaseRequisitionLocationId, out var name) ? name : "",
                            pr.PurchaseRequisitionDate,
                            pr.PurchaseRequisitionNo,
                            PRQty = prQty,
                            RecQty = recQty
                        };
                    })
                    .Where(x => x.PRQty > x.RecQty)
                    .ToList();

                obj.ResultSet = result;
                obj.IsSuccess = true;
            }
            catch (Exception ex)
            {
                obj.IsSuccess = false;
            }
            return obj;
        }
    }
}
