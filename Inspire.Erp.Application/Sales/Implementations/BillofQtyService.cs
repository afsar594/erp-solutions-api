using Inspire.Erp.Application.Sales.Interfaces;
using Inspire.Erp.Application.Common;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Enums;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Inspire.Erp.Application.MODULE;
using Inspire.Erp.Domain.Modals.Sales;
using Inspire.Erp.Domain.Modals;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;
using Inspire.Erp.Infrastructure.Database;
using Inspire.Erp.Domain.Modals.Common;
using Inspire.Erp.Domain.Models.Sales.DeliveryNoteReport;
using System.Security.Cryptography;
using Inspire.Erp.Domain.Modals.Stock;
using Microsoft.AspNetCore.Mvc;
using Inspire.Erp.Domain.Models;
using Inspire.Erp.Domain.Entities.POS;
using Microsoft.AspNetCore.DataProtection.XmlEncryption;
using Inspire.Erp.Infrastructure;
using Inspire.Erp.Domain.Constants;
using Inspire.Erp.Application.Email.Interfaces;
using Inspire.Erp.Application.Common.Email;

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
        public BillofQtyService(ILogger<CustomerQuotationService> logger, IRepository<PermissionApproval> permissionApprovalRepo,
            IRepository<Approval> approvalRepo, IRepository<ApprovalDetail> approvalDetailRepo,
            IRepository<BillofQtyMaster> BillofQtyMasterRepository, IRepository<VouchersNumbers> voucherNumbersRepository, IRepository<Domain.Entities.ProgramSettings> programsettingsRepository,
            IRepository<BillOfQTyDetails> BillOfQTyDetailsRepository,
            IRepository<UserTracking> userTrackingRepository,
            IRepository<JobMasterBudgetDetails> _jobbudgetRepository,
            InspireErpDBContext Context, IRepository<EstimationCostCenter> estimationCostCenterRepository, IRepository<JobMaster> jobMasterRepository, IRepository<BillofQtyTracking> billofQtyTracking, IOrderEmailNotificationService orderEmailNotificationService, IRepository<JobMasterJobDetails> jobMasterJobDetailsRepo)
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
        }

        public BillofQtyMaster UpdateBillofQty(BillofQtyMaster BillofQty, List<BillOfQTyDetails> BillOfQTyDetails, List<EstimationCostCenter> EstimationCostCenter)
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

            return this.GetSavedBillofQty(BillofQty.Id);
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
        public BillofQtyMaster InsertBillofQty(BillofQtyMaster BillofQty, List<BillOfQTyDetails> BillOfQTyDetails, List<EstimationCostCenter> EstimationCostCenter)
        {
            try
            {
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

                //#region EmailNotification
                //SendEmailOnValueExceedsAsync((int)BillofQty.CustomerEnqId, BillofQty.BillofqtyNo);
                //#endregion

                return this.GetSavedBillofQty(BillofQty.Id);
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
                                           .Sum(x => x.JobMasterJobDetailsJdAmt ?? 0) ;
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
        public BillofQtyMaster GetSavedBillofQty(decimal id)
        {

            BillofQtyMaster voucherdata = new BillofQtyMaster();
            voucherdata = _BillofQtyMasterRepository.GetAsQueryable().Where(k => k.Id == id).FirstOrDefault();
            if (voucherdata != null)
            {
                voucherdata.BillOfQTyDetails = _BillOfQTyDetailsRepository.GetAsQueryable().Where(qd => qd.BillQtyId == id).ToList();
                voucherdata.EstimationCostCenter = _EstimationCostCenterRepository.GetAsQueryable().Where(qd => qd.BillQtyId == id).ToList();

                return voucherdata;

            }
            else
            {
                return null;
            }

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
    }
}


