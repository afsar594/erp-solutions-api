using Dapper;
using Inspire.Erp.Application.Common;
using Inspire.Erp.Application.Common.Email;
using Inspire.Erp.Application.Email.Interfaces;
using Inspire.Erp.Application.MODULE;
using Inspire.Erp.Application.Sales.Interfaces;
using Inspire.Erp.Application.Settings.Interfaces;
using Inspire.Erp.Application.User;
using Inspire.Erp.Domain.Constants;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Entities.POS;
using Inspire.Erp.Domain.Enums;
using Inspire.Erp.Domain.Modals;
using Inspire.Erp.Domain.Modals.Common;
using Inspire.Erp.Domain.Modals.Sales;
using Inspire.Erp.Domain.Modals.Stock;
using Inspire.Erp.Domain.Models;
using Inspire.Erp.Domain.Models.Sales.DeliveryNoteReport;
using Inspire.Erp.Infrastructure;
using Inspire.Erp.Infrastructure.Database;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using Microsoft.AspNetCore.DataProtection.XmlEncryption;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Inspire.Erp.Application.Sales.Implementations
{
    public class BillofQtyService : IBillofQtyService
    {
        private const int @false = 0;
        private IRepository<BillofQtyMaster> _BillofQtyMasterRepository;
        private IRepository<BillOfQTyDetails> _BillOfQTyDetailsRepository;
        private IRepository<VouchersNumbers> _voucherNumbersRepository;
        private IRepository<Domain.Entities.ProgramSettings> _programsettingsRepository;
        private IRepository<UserTracking> _userTrackingRepository;
        private readonly ILogger<CustomerQuotationService> _logger;
        public readonly InspireErpDBContext _Context;
        private readonly IRepository<PermissionApproval> _permissionApprovalRepo;
        private readonly IRepository<Approval> _approvalRepo;
        private readonly IRepository<ApprovalDetail> _approvalDetailRepo;
        private IRepository<ItemMaster> _itemMasterRepository;
        private IRepository<EstimationCostCenter> _EstimationCostCenterRepository;
        private IRepository<JobMasterBudgetDetails> jobbudgetRepository;
        private IRepository<JobMaster> JobMasterRepository;
        private IRepository<JobMasterJobDetails> _jobMasterJobDetailsRepo;
        private readonly IRepository<BillofQtyTracking> _billofQtyTracking;
        private readonly IOrderEmailNotificationService _orderEmailNotificationService;
        private readonly IRepository<CostCenterMaster> _costCenterMastr;
        private readonly IRepository<CustomerMaster> _customerMaster;
        private IRepository<JobMaster> jobrepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IGeneralSettings _generalSettings;
        private readonly IDbConnection _dbConnection;

        public BillofQtyService(ILogger<CustomerQuotationService> logger, IRepository<PermissionApproval> permissionApprovalRepo,
            IRepository<Approval> approvalRepo, IRepository<ApprovalDetail> approvalDetailRepo,
            IRepository<BillofQtyMaster> BillofQtyMasterRepository, IRepository<VouchersNumbers> voucherNumbersRepository, IRepository<Domain.Entities.ProgramSettings> programsettingsRepository,
            IRepository<BillOfQTyDetails> BillOfQTyDetailsRepository,
            IRepository<UserTracking> userTrackingRepository,
            IRepository<JobMasterBudgetDetails> _jobbudgetRepository,
             IOptions<ApplicationSettings> applicationSettings,
            InspireErpDBContext Context, IRepository<EstimationCostCenter> estimationCostCenterRepository, IRepository<JobMaster> jobMasterRepository, IRepository<BillofQtyTracking> billofQtyTracking, IOrderEmailNotificationService orderEmailNotificationService, IRepository<JobMasterJobDetails> jobMasterJobDetailsRepo, IRepository<CostCenterMaster> costCenterMastr, IRepository<ItemMaster> itemMasterRepository, IGeneralSettings generalSettings, IRepository<CustomerMaster> customerMaster, IRepository<JobMaster> jobrepository, ICurrentUserService currentUserService)
        {

            _BillofQtyMasterRepository = BillofQtyMasterRepository;
            _BillOfQTyDetailsRepository = BillOfQTyDetailsRepository;
            this._voucherNumbersRepository = voucherNumbersRepository;
            this._programsettingsRepository = programsettingsRepository;
            _logger = logger;
            this._userTrackingRepository = userTrackingRepository;
            _approvalDetailRepo = approvalDetailRepo;
            _approvalRepo = approvalRepo;
            _permissionApprovalRepo = permissionApprovalRepo;


            _Context = Context;
            _EstimationCostCenterRepository = estimationCostCenterRepository;
            jobbudgetRepository = _jobbudgetRepository;
            JobMasterRepository = jobMasterRepository;
            _billofQtyTracking = billofQtyTracking;
            _orderEmailNotificationService = orderEmailNotificationService;
            _jobMasterJobDetailsRepo = jobMasterJobDetailsRepo;
            _costCenterMastr = costCenterMastr;
            _itemMasterRepository = itemMasterRepository;
            _generalSettings = generalSettings;
            _customerMaster = customerMaster;
            this.jobrepository = jobrepository;
            _currentUserService = currentUserService;
            _dbConnection = new SqlConnection(applicationSettings.Value.ConnectionString);
        }

        public async Task<Response<BillofQtyMasterModel>> UpdateBillofQty(BillofQtyMaster BillofQty, List<BillOfQTyDetails> BillOfQTyDetails, List<EstimationCostCenter> EstimationCostCenter)
        {

            try
            {
                _BillofQtyMasterRepository.BeginTransaction();
                //  BillofQty.BillOfQTyDetails.Clear();

                var boqDetailsTODelete = this._BillOfQTyDetailsRepository.GetAsQueryable().Where(x => x.BillQtyId == BillofQty.Id).ToList();
                this._BillOfQTyDetailsRepository.DeleteList(boqDetailsTODelete);

                _BillofQtyMasterRepository.Update(BillofQty);

                decimal maxcount1 = 0;
                maxcount1 = _BillOfQTyDetailsRepository.GetAsQueryable()
                    .DefaultIfEmpty()
                    .Max(o => o == null ? 0 : o.Id) + 1;


                foreach (var x in BillOfQTyDetails)
                {
                    x.BillQtyId = BillofQty.Id;


                    x.Id = maxcount1;
                    x.BillQtyId = BillofQty.Id;
                    _BillOfQTyDetailsRepository.Insert(x);
                    maxcount1++;

                }
                foreach (var xx in EstimationCostCenter)
                {
                    xx.BillQtyId = (int)BillofQty.Id;

                    if (xx.Id == 0)
                    {
                        //xx.Id = (int)maxcount1;
                        xx.BillQtyId = (int)BillofQty.Id;
                        _EstimationCostCenterRepository.Insert(xx);
                        maxcount1++;
                    }
                    else
                    {
                        _EstimationCostCenterRepository.Update(xx);
                    }
                }

                // 5/17  disable as per instruction of ali ,no need add job budget form 
                //InsertUpdateJobBudgetDetails(BillofQty, EstimationCostCenter);

                _BillofQtyMasterRepository.TransactionCommit();
            }
            catch (Exception ex)
            {
                _BillofQtyMasterRepository.TransactionRollback();
                throw ex;
            }

            return await this.GetSavedBillofQty((long)BillofQty.Id);
        }

        private void InsertUpdateJobBudgetDetails(BillofQtyMaster BillofQty, List<EstimationCostCenter> EstimationCostCenter)
        {
            var jobNum = JobMasterRepository.GetAsQueryable()
                .Where(e => e.JobMasterJobId == BillofQty.CustomerEnqId)
                .FirstOrDefault()?.JobMasterJobNo;

            var jobMasterBudgetDetailsExisting = jobbudgetRepository
                .GetAsQueryable()
                .Where(x => x.RefVoucherNo == BillofQty.BillofqtyNo)
                .ToList();

            // **Insert: Add new records that don't exist**
            var jobMasterBudgetDetailsToAdd = EstimationCostCenter
                .Where(estimation => estimation.Amount > 0 &&
                                     !jobMasterBudgetDetailsExisting
                                         .Any(existing => existing.JobMasterBudgetDetailsBudId == estimation.costcenter))
                .Select(estimation => new JobMasterBudgetDetails
                {
                    JobMasterBudgetDetailsJobId = (int)BillofQty.CustomerEnqId,
                    JobMasterBudgetDetailsBudId = estimation.costcenter,
                    JobMasterBudgetDetailsBudAmount = estimation.Amount ?? 0,
                    JobMasterBudgetDetailsDelStatus = false,
                    JobMasterNo = jobNum ?? null,
                    RefVoucherNo = BillofQty.BillofqtyNo
                })
                .ToList();

            // **Update: Modify existing records instead of creating new ones**
            var jobMasterBudgetDetailsToUpdate = jobMasterBudgetDetailsExisting
                .Where(existing => EstimationCostCenter
                    .Any(estimation => estimation.costcenter == existing.JobMasterBudgetDetailsBudId))
                .ToList();

            jobMasterBudgetDetailsToUpdate.ForEach(x =>
            {
                var matchingEstimation = EstimationCostCenter.First(estimation => estimation.costcenter == x.JobMasterBudgetDetailsBudId);
                x.JobMasterBudgetDetailsBudAmount = matchingEstimation.Amount ?? 0;
                x.JobMasterBudgetDetailsDelStatus = false;
            });

            // **Delete: Remove records that are no longer in EstimationCostCenter**
            var jobMasterBudgetDetailsToDelete = jobMasterBudgetDetailsExisting
                .Where(existing => !EstimationCostCenter
                    .Any(estimation => estimation.costcenter == existing.JobMasterBudgetDetailsBudId))
                .ToList();

            // **Apply changes to the database**
            if (jobMasterBudgetDetailsToAdd.Any())
                jobbudgetRepository.InsertList(jobMasterBudgetDetailsToAdd);

            if (jobMasterBudgetDetailsToUpdate.Any())
                jobbudgetRepository.UpdateList(jobMasterBudgetDetailsToUpdate); // Updates records correctly

            if (jobMasterBudgetDetailsToDelete.Any())
                jobbudgetRepository.DeleteList(jobMasterBudgetDetailsToDelete); // Deletes unused records

        }

        public int DeleteBillofQty(decimal id)
        {
            int i = 0;
            try
            {
                _BillofQtyMasterRepository.BeginTransaction();

                var BillofQty = _BillofQtyMasterRepository.GetAsQueryable().Where(x => x.Id == id).FirstOrDefault();

                var BillofQtydt = _BillOfQTyDetailsRepository.GetAsQueryable().Where(x => x.BillQtyId == id).ToList();
                _BillOfQTyDetailsRepository.DeleteList(BillofQtydt);

                var EstimationQtydt = _EstimationCostCenterRepository.GetAsQueryable().Where(x => x.BillQtyId == id).ToList();
                _EstimationCostCenterRepository.DeleteList(EstimationQtydt);

                var budget = jobbudgetRepository.GetAsQueryable().Where(b => b.RefVoucherNo == BillofQty.BillofqtyNo).ToList();
                jobbudgetRepository.DeleteList(budget);

                _BillofQtyMasterRepository.Delete(BillofQty);
                _BillofQtyMasterRepository.TransactionCommit();
                i = 1;
            }
            catch (Exception ex)
            {
                _BillofQtyMasterRepository.TransactionRollback();
                i = 0;
                throw ex;
            }


            return i;

        }
        public async Task<Response<BillofQtyMasterModel>> InsertBillofQty(BillofQtyMaster BillofQty, List<BillOfQTyDetails> BillOfQTyDetails, List<EstimationCostCenter> EstimationCostCenter)
        {
            try
            {


                var settingValue = await _generalSettings.GetApproverStatusAsync((BillofQty.BOQJobID ?? -1).ToString(), nameof(EnumOrderForms.BillOfQuantity));

                if (!string.IsNullOrEmpty(settingValue.ApproverStatus))
                    return Response<BillofQtyMasterModel>.Fail(null, settingValue.ApproverStatus);


                _BillofQtyMasterRepository.BeginTransaction();
                var jobNum = JobMasterRepository.GetAsQueryable().AsNoTracking()
                .Where(e => e.JobMasterJobId == BillofQty.CustomerEnqId)
                .FirstOrDefault()?.JobMasterJobNumber ?? "GEN";

                string customerQuotationNumber = clsCommonFunctions.GenrateVoucherNo(
                    VoucherType.BOQ, Prefix.BOQ_Prefix, _programsettingsRepository, _voucherNumbersRepository, jobNum);
                BillofQty.BillofqtyNo = customerQuotationNumber;

                BillofQty.BillOfQTyDetails = new List<BillOfQTyDetails>();
                BillofQty.EstimationCostCenter = new List<EstimationCostCenter>();
                decimal maxcount = 0;
                maxcount = (
                    _BillofQtyMasterRepository.GetAsQueryable()
                    .DefaultIfEmpty().Max(o => o == null ? 0 : o.Id) + 1);

                BillofQty.Id = maxcount;

                decimal maxDetailId = 0;
                maxDetailId = _BillOfQTyDetailsRepository.GetAsQueryable()
                     .DefaultIfEmpty()
                     .Max(o => o == null ? 0 : (decimal)o.Id) + 1;

                BillOfQTyDetails = BillOfQTyDetails.Select((k) =>
                {
                    k.Id = maxDetailId++;
                    k.BillQtyId = BillofQty.Id;
                    return k;
                }).OrderBy(x => x.Id).AsEnumerable().ToList();
                _BillOfQTyDetailsRepository.InsertList(BillOfQTyDetails);

                //var jobNum = JobMasterRepository.GetAsQueryable().Where(e => e.JobMasterJobId == BillofQty.CustomerEnqId).FirstOrDefault()?.JobMasterJobNo;

                int maxEstimationId = _EstimationCostCenterRepository.GetAsQueryable()
                    .DefaultIfEmpty().Max(o => o == null ? 0 : o.Id) + 1;


                EstimationCostCenter = EstimationCostCenter.Select((estimation) =>
                {
                    //estimation.Id = maxEstimationId++;
                    estimation.BillQtyId = (int)BillofQty.Id;
                    return estimation;
                }

                ).OrderBy(x => x.Id).AsEnumerable().ToList();


                _EstimationCostCenterRepository.InsertList(EstimationCostCenter);

                //InsertUpdateJobBudgetDetails(BillofQty, EstimationCostCenter);

                _BillofQtyMasterRepository.Insert(BillofQty);

                _BillofQtyMasterRepository.TransactionCommit();

                #region EmailNotification
                //SendEmailOnValueExceedsAsync((int)BillofQty.CustomerEnqId, BillofQty.BillofqtyNo);


                var Username = _currentUserService.Username;
                var jobId = BillofQty?.BOQJobID ?? -1;
                var jobDetail = await jobrepository.GetAsQueryable().Where(x => x.JobMasterJobId == jobId).FirstOrDefaultAsync();
                var customerName = await _customerMaster.FirstOrDefaultAsync(x => x.CustomerMasterCustomerNo == BillofQty.CustomerEnqId, s => s.CustomerMasterCustomerName);
                await _orderEmailNotificationService.SendOrderEmailNotificationAsync(
                                                                EnumOrderForms.BillOfQuantity,
                                                                EnumApproverLevels.None,
                                                                "BillOfQuantity",
                                                                BillofQty.BillofqtyNo,
                                                                (DateTime)BillofQty.Date,
                                                                customerName,
                                                                (decimal)BillofQty.BOQAmount,
                                                                (int)BillofQty.Id,
                                                                (int)jobId,
                                                                jobDetail == null ? "" : jobDetail.JobMasterJobNumber,
                                                                jobDetail == null ? "" : jobDetail.JobMasterJobName,
                                                                true,
                                                               Username,
                                                                settingValue.GeneralSettingValue,
                                                                new List<string> { "" },
                                                                -1,
                                                                null,
                                                                (long)_currentUserService.UserId
                                                                );
                #endregion

                return await this.GetSavedBillofQty((long)BillofQty.Id);
            }
            catch (Exception ex)
            {
                _BillofQtyMasterRepository.TransactionRollback();
                throw;
            }
        }

        private async void SendEmailOnValueExceedsAsync(int jobId, string prNo)
        {
            try
            {
                var jobDetail = GetJobDetail(jobId);
                if (jobDetail == null)
                    return;

                decimal boqValue = GetBoqTotalValue(jobId);
                decimal jobValue = GetJobTotalValue(jobId);

                if (boqValue <= jobValue)
                    return;

                string subject = $"BoQ exceeds Job Value against {jobDetail.JobMasterJobNumber}-{jobDetail.JobMasterJobName}";
                string messageBody = GenerateBoqEmailBody(jobDetail, boqValue, prNo);
                var recipients = GetEmailRecipients();

                await _orderEmailNotificationService.SendTextEmailAsync(recipients, subject, messageBody, new List<string>());
            }
            catch (Exception)
            {
                throw;
            }
        }

        private JobMaster GetJobDetail(int jobId)
        {
            return JobMasterRepository.GetAsQueryable()
                                      .AsNoTracking()
                                      .SingleOrDefault(x => x.JobMasterJobId == jobId);
        }
        private decimal GetJobTotalValue(int jobId)
        {
            return _jobMasterJobDetailsRepo.GetAsQueryable()
                                           .AsNoTracking()
                                           .Where(x => x.JobMasterJobDetailsJobId == jobId)
                                           .Sum(x => x.JobMasterJobDetailsJdAmt ?? 0);
        }
        private decimal GetBoqTotalValue(int jobId)
        {
            return _BillofQtyMasterRepository.GetAsQueryable()
                                             .Where(x => x.CustomerEnqId == jobId)
                                             .AsNoTracking()
                                             .Sum(x => x.TotalAmount ?? 0);
        }

        private string GenerateBoqEmailBody(JobMaster jobDetail, decimal boqValue, string prNo)
        {
            return InspireConstants.BOQEmailTemplate
                                   .Replace("{{jobCode}}", jobDetail.JobMasterJobNumber)
                                   .Replace("{{jobName}}", jobDetail.JobMasterJobName)
                                   .Replace("{{jobValue}}", jobDetail.JobMasterJobValue?.ToString("n2"))
                                   .Replace("{{prNetAmount}}", boqValue.ToString("n2"))
                                   .Replace("{{prNO}}", prNo);
        }

        private List<UserEmail> GetEmailRecipients()
        {
            return new List<UserEmail>
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
                    Email = "jeevan@waterbirdwtc.com",
                    Name = "Jeevan",
                    BccEmails = new List<CCAndBccMailDto>(),
                    CCEmails = new List<CCAndBccMailDto>()
                }
            };
        }

        public IEnumerable<BillofQtyMaster> GetBillofQty()
        {
            IEnumerable<BillofQtyMaster> customerQuotation = _BillofQtyMasterRepository.GetAll();
            return customerQuotation;
        }

        public async Task<Response<BillofQtyMasterModel>> GetSavedBillofQty(long Id)
        {
            var result = await _BillofQtyMasterRepository.GetAsQueryable()
                                      .Where(x => x.Id == Id)
                                      .Select(x => new BillofQtyMasterModel
                                      {
                                          Id = x.Id,
                                          BillofqtyNo = x.BillofqtyNo,
                                          Date = x.Date,
                                          CustomerEnqId = x.CustomerEnqId,
                                          Remarks = x.Remarks ?? string.Empty,
                                          Status = x.Status,
                                          BOQAmount = x.BOQAmount,
                                          CostantsAmount = x.CostantsAmount,
                                          TotalAmount = x.TotalAmount,
                                          ProfitAmount = x.ProfitAmount,
                                          ProftPercentage = x.ProftPercentage,
                                          QuoteAmount = x.QuoteAmount,
                                          BOQJobID = x.BOQJobID,
                                      }).FirstOrDefaultAsync();

            if (result == null)
                return null;

            result.BillOfQTyDetails = await (from d in _BillOfQTyDetailsRepository.GetAsQueryable()
                                             where d.BillQtyId == result.Id
                                             join t in _itemMasterRepository.GetAsQueryable()
                                                 on d.ItemCode equals t.ItemMasterItemId into itemJoin
                                             from t in itemJoin.DefaultIfEmpty()
                                             select new BillOfQTyDetailsModel
                                             {
                                                 Id = d.Id,
                                                 BillQtyId = d.BillQtyId,
                                                 ItemCode = d.ItemCode,
                                                 ItemName = t != null ? t.ItemMasterItemName : string.Empty,
                                                 UnitCode = d.UnitCode,
                                                 Qty = d.Qty,
                                                 Rate = d.Rate,
                                                 Amount = d.Amount,
                                                 Remarks = d.Remarks ?? string.Empty,
                                                 IsEdit = d.IsEdit,
                                                 Freight_Charges = d.Freight_Charges,
                                                 Packing_Charges = d.Packing_Charges,
                                                 Clearance_Charges = d.Clearance_Charges,
                                                 Duty = d.Duty,
                                                 Mofa = d.Mofa,
                                                 Others = d.Others,
                                                 Currency_TypeId = d.Currency_TypeId,
                                                 FinalAmount = d.FinalAmount,
                                                 CurrencyRate = d.CurrencyRate
                                             }).ToListAsync();

            result.EstimationCostCenter = await (from c in _EstimationCostCenterRepository.GetAsQueryable()
                                                 where c.BillQtyId == result.Id
                                                 join cc in _costCenterMastr.GetAsQueryable()
                                                     on c.costcenter equals cc.CostCenterMasterCostCenterId into ccJoin
                                                 from cc in ccJoin.DefaultIfEmpty()
                                                 select new EstimationCostCenterModel
                                                 {
                                                     Id = c.Id,
                                                     Remarks = c.Remarks ?? string.Empty,
                                                     Amount = c.Amount ?? 0,
                                                     BillQtyId = c.BillQtyId,
                                                     costcenter = c.costcenter,
                                                     CostCenterName = cc != null ? cc.CostCenterMasterCostCenterName : string.Empty
                                                 }).ToListAsync();

            if (result is null)
                return Response<BillofQtyMasterModel>.Fail(null, $"No bill of quantity were found against {Id}");
            return Response<BillofQtyMasterModel>.Success(result, $"Bill of quantity were found against {Id}");
        }

        public VouchersNumbers GenerateVoucherNo(DateTime? VoucherGenDate)
        {
            try
            {
                var prefix = this._programsettingsRepository.GetAsQueryable().Where(k => k.ProgramSettingsKeyValue == Prefix.BOQ_Prefix).FirstOrDefault().ProgramSettingsTextValue;
                int vnoMaxVal = Convert.ToInt32(_voucherNumbersRepository.GetAsQueryable()
                                                        .Where(x => x.VouchersNumbersVNoNu > 0 && x.VouchersNumbersVType == VoucherType.BOQ)
                                                        .DefaultIfEmpty()
                                                        .Max(o => o == null ? 0 : o.VouchersNumbersVNoNu)) + 1;
                VouchersNumbers vouchersNumbers = new VouchersNumbers

                {
                    VouchersNumbersVNo = prefix + vnoMaxVal,
                    VouchersNumbersVNoNu = vnoMaxVal,
                    VouchersNumbersVType = VoucherType.BOQ,
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
        public VouchersNumbers GetVouchersNumbers(string Qtno)
        {
            try
            {
                VouchersNumbers vouchersNumbers = _voucherNumbersRepository.GetAsQueryable().Where(k => k.VouchersNumbersVNo == Qtno && k.VouchersNumbersVType == VoucherType.BOQ).SingleOrDefault();
                return vouchersNumbers;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex;
            }

        }

        public Response<List<BillofQtyTracking>> GetBillofQtyTracking(DateTime? fromDate, DateTime? toDate, string? jobNo)
        {
            try
            {
                var result = this._billofQtyTracking.GetAsQueryable().Where(x =>
                                                                                                                            (fromDate == null ? true : (x.BOQDate.Date >= fromDate.Value.Date && x.BOQDate.Date <= toDate.Value.Date)) &&
                                                                                                                            (string.IsNullOrEmpty(jobNo) ? true : x.JobMasterNo.ToUpper() == jobNo.ToUpper())).ToList();
                return Response<List<BillofQtyTracking>>.Success(result, "Data Found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<Response<List<BoqToCustomerQuotationDto>>> GetBoqToCustomerQuotationAsync(string boqNo, DateTime? fromDate, DateTime? toDate)
        {
            try
            {
                var where = $" 1=1 and bd.Id not in (select distinct isnull(BoqDetailsId, -1) from Customer_Quotation_Details)";
                if (!string.IsNullOrEmpty(boqNo))
                    where += $"  and upper(bm.BillofqtyNo) = upper('{boqNo}')  ";

                if (fromDate.HasValue && toDate.HasValue)
                    where += $" and cast(bm.date as Date) between '{fromDate:yyyy-MM-dd}' and '{toDate:yyyy-MM-dd}'  ";

                var sql = $@"select  bd.Id as BoqDetailsId, bm.Id BillofQtyId, bm.BillofqtyNo BillofQtyNo,
                                        im.Item_Master_Item_ID as ItemId,
                                        im.Item_Master_Item_Name as ItemName,
                                        um.Unit_Master_Unit_ID as UnitId,
                                        um.Unit_Master_Unit_Full_Name as UnitName,
                                        bd.Qty,
                                        bd.Rate,
                                        bd.CurrencyRate,
                                        isnull(bd.Freight_Charges,0) as FreightCharges,
                                        isnull(bd.Packing_Charges,0) PackingCharges,
                                        isnull(bd.Clearance_Charges,0) ClearanceCharges,
                                        isnull(bd.Duty,0) Duty,
                                        isnull(bd.Mofa,0) Mofa,
                                        isnull(bd.Others,0) Others,
                                        bd.FinalAmount
                                        from BillofQty_Details bd 
                                        join BillofQty_Master bm on bd.billQtyId = bm.Id
                                        join Item_Master im on bd.ItemCode = im.Item_Master_Item_ID
                                        join Unit_Details ud on im.Item_Master_Item_ID = ud.Unit_Details_Item_ID
                                        join Unit_Master um on ud.Unit_Details_Unit_ID = um.Unit_Master_Unit_ID
                                        where {where} and bm.Id in (
                                        select OrderId
                                        from OrderApprovalHierarchy
                                        group by OrderId
                                        having count(*) = sum(case when Status = 'Approved' then 1 else 0 end)
                                    )";

                var result = (await _dbConnection.QueryAsync<BoqToCustomerQuotationDto>(sql)).ToList();
                if (!result.Any())
                    return Response<List<BoqToCustomerQuotationDto>>.Fail(result, "No BOQ Details found for the given criteria.");

                return Response<List<BoqToCustomerQuotationDto>>.Success(result, "BOQ Details found for the given criteria.");
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}


