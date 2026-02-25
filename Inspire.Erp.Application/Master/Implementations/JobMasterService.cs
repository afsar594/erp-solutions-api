using Inspire.Erp.Domain.DTO.Job_Master;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Enums;
using Inspire.Erp.Domain.Modals.File;
using Inspire.Erp.Domain.Modals;
using Inspire.Erp.Infrastructure.Database;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DinkToPdf.Contracts;
using System.IO;
using Inspire.Erp.Application.Helpers;
using System.Data;
using Microsoft.Data.SqlClient;
using Inspire.Erp.Infrastructure;
using Microsoft.Extensions.Configuration;
using Dapper;

namespace Inspire.Erp.Application.Master
{

    public class JobMasterService : IJobMasterService
    {
        private readonly IDbConnection _dbConnection;
        private IRepository<JobMaster> jobrepository;
        private IRepository<JobMasterBudgetDetails> jobbudgetRepository;
        private IRepository<JobMasterJobDetails> jobdetailsRepository;
        private IRepository<JobMasterJobWiseBankGuarantees> jobBankGuaruRepository;
        private IRepository<JobDocuments> _jobDocumentsRepository;
        private IRepository<ProgramSettings> programsettingsRepository;
        private IRepository<VouchersNumbers> voucherNumbersRepository;
        private IRepository<JobStaff> jobstaffrepository;
        private IRepository<JobEquipment> jobEquipmentrepository;
        private IRepository<JobExcutionDetails> jobexecrepository;
        private IRepository<JobTimesheetDesc> _jobTimesheetDesc;
        private IRepository<JobTimeSheetEquip> _jobTimeSheetEquip;
        private IRepository<JobTimesheetMaster> _jobTimesheetMaster;
        private IRepository<JobTimeSheetStaff> _jobTimeSheetStaff;
        private IRepository<JobfilesStatus> _JobfilesStatus;
        private IRepository<CostCenterMaster> _costCenterMaster;
        private IRepository<JobMasterControl> _jobMasterControl;
        private IRepository<ApproverLevelSettings> _approverLevelSettings;
        private IRepository<EstimationCostCenter> _EstimationCostCenterRepository;

        private IRepository<AccountsTransactions> accountsTransactionsRepository;
        private readonly InspireErpDBContext context;
        public JobMasterService(IRepository<JobMaster> _jobrepository,
            IRepository<JobMasterBudgetDetails> _jobbudgetRepository,
            IRepository<JobMasterJobDetails> _jobdetailsRepository,
            IRepository<JobMasterJobWiseBankGuarantees> _jobBankGuaruRepository,
            IRepository<AccountsTransactions> _accountsTransactionsRepository,
            InspireErpDBContext _context, IRepository<ProgramSettings> _programsettingsRepository,
            IRepository<VouchersNumbers> _voucherNumbersRepository, IRepository<JobStaff> _jobstaffrepository,
            IRepository<JobEquipment> _jobEquipmentrepository, IRepository<JobExcutionDetails> _jobexecrepository,
            IRepository<JobTimeSheetEquip> jobTimeSheetEquip,
            IRepository<JobTimesheetMaster> jobTimesheetMaster,
            IRepository<JobTimeSheetStaff> jobTimeSheetStaff, IRepository<JobTimesheetDesc> jobTimesheetDesc,
            IRepository<JobDocuments> jobDocumentsRepository,
            Microsoft.Extensions.Configuration.IConfiguration config,
             IRepository<EstimationCostCenter> estimationCostCenterRepository,
            IRepository<JobfilesStatus> jobfilesStatus, IRepository<CostCenterMaster> costCenterMaster, IRepository<JobMasterControl> jobMasterControl, IRepository<ApproverLevelSettings> approverLevelSettings)
        {
            jobrepository = _jobrepository;
            jobbudgetRepository = _jobbudgetRepository;
            jobdetailsRepository = _jobdetailsRepository;
            jobBankGuaruRepository = _jobBankGuaruRepository;
            accountsTransactionsRepository = _accountsTransactionsRepository;
            context = _context;
            programsettingsRepository = _programsettingsRepository;
            voucherNumbersRepository = _voucherNumbersRepository;
            jobstaffrepository = _jobstaffrepository;
            jobEquipmentrepository = _jobEquipmentrepository;
            jobexecrepository = _jobexecrepository;
            _jobTimesheetDesc = jobTimesheetDesc; _jobTimesheetMaster = jobTimesheetMaster;
            _jobTimeSheetEquip = jobTimeSheetEquip; _jobTimeSheetStaff = jobTimeSheetStaff;
            _jobDocumentsRepository = jobDocumentsRepository;
            _JobfilesStatus = jobfilesStatus;
            _costCenterMaster = costCenterMaster;
            _jobMasterControl = jobMasterControl;
            _approverLevelSettings = approverLevelSettings;
            _EstimationCostCenterRepository = estimationCostCenterRepository;
            var settings = config.GetSection("ApplicationSettings").Get<ApplicationSettings>();
            _dbConnection = new SqlConnection(settings.ConnectionString);
        }
        public IEnumerable<JobMaster> DeleteJob(JobMaster jobMaster)
        {
            bool valid = false;
            try
            {
                jobrepository.Delete(jobMaster);
                valid = true;
            }
            catch (Exception ex)
            {
                valid = false;
                throw ex;
            }
            finally
            {
            }
            return this.GetAllJob();
        }
        public IEnumerable<JobMaster> GetAllJob()
        {
            IEnumerable<JobMaster> jobMasters;
            try
            {
                jobMasters = jobrepository.GetAll();

            }
            catch (Exception ex)
            {
                throw ex;

            }
            finally
            {
                //jobrepository.Dispose();
            }
            jobMasters = jobMasters.OrderBy(sm => sm.JobMasterJobName).ToList();
            return jobMasters;
        }
        public IEnumerable<JobMaster> GetAllJobById(int id)
        {
            IEnumerable<JobMaster> jobMasters;
            try
            {
                jobMasters = jobrepository.GetAsQueryable().Where(k => k.JobMasterJobId == id).Select(k => k);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
            }
            return jobMasters;
        }
        public async Task<JobMaster> InsertJobMaster(JobMaster jobMaster, List<JobMasterBudgetDetails> jobMasterBudgetDetails, List<JobMasterJobDetails> jobMasterJobDetails, List<JobMasterJobWiseBankGuarantees> jobMasterJobWiseBankGuarantees, List<JobStaff> jobStaff, List<JobEquipment> jobEquipment, List<JobExcutionDetails> jobExcutionDetails, List<JobTimesheetDesc> JobTimesheetDesc, List<JobTimeSheetEquip> JobTimeSheetEquip, List<JobTimesheetMaster> JobTimesheetMaster, List<JobTimeSheetStaff> JobTimeSheetStaff, List<JobfilesStatus> JobfilesStatus, List<JobMasterControl> JobMasterControl)
        {
            try
            {

                jobrepository.BeginTransaction();
                int jobId = 0;
                jobId =
                    jobrepository.GetAsQueryable()
                    .DefaultIfEmpty().Max(o => o == null ? 0 : o.JobMasterJobId) + 1;
                jobMaster.JobMasterJobId = jobId;

                string vNumber = this.GenerateVoucherNo(jobMaster.JobMasterJobDate).VouchersNumbersVNo;
                jobMaster.JobMasterJobNo = vNumber;

                jobMaster.JobMasterBudgetDetails.Select((x) =>
                {
                    x.JobMasterBudgetDetailsJobId = jobId;
                    x.JobMasterNo = vNumber;
                    return x;
                }).OrderBy(x => x.JobMasterNo).AsEnumerable().ToList();

                jobMaster.JobMasterJobDetails.Select((x) =>
                {
                    x.JobMasterJobDetailsJobId = jobId;
                    x.JobMasterNo = vNumber;
                    return x;
                }).OrderBy(x => x.JobMasterNo).AsEnumerable().ToList();

                jobMaster.JobMasterJobWiseBankGuarantees.Select((x) =>
                {
                    x.JobMasterJobWiseBankGuaranteesJobId = jobId;
                    x.JobMasterNo = vNumber;
                    return x;
                }).OrderBy(x => x.JobMasterNo).AsEnumerable().ToList();

                // job staff , equipment and execution related updation
                jobMaster.JobStaff.Select((x) =>
                {
                    x.JobMasterJobId = jobId;
                    return x;
                }).OrderBy(x => x.JobMasterJobId).AsEnumerable().ToList();
                jobMaster.JobEquipment.Select((x) =>
                {
                    x.Id = 0;
                    x.JobMasterJobId = jobId;
                    return x;
                }).OrderBy(x => x.JobMasterJobId).AsEnumerable().ToList();
                jobMaster.JobExcutionDetails.Select((x) =>
                {
                    x.JobMasterJobId = jobId;
                    return x;
                }).OrderBy(x => x.JobMasterJobId).AsEnumerable().ToList();
                /// end 
                /// 
                jobMaster.JobTimesheetDesc.Select((x) =>
                {
                    x.JobId = jobId;
                    return x;
                }).OrderBy(x => x.JobId).AsEnumerable().ToList();

                jobMaster.JobTimeSheetEquip.Select((x) =>
                {
                    x.JobId = jobId;
                    return x;
                }).OrderBy(x => x.JobId).AsEnumerable().ToList();
                jobMaster.JobTimesheetMaster.Select((x) =>
                {
                    x.JobId = jobId;
                    return x;
                }).OrderBy(x => x.JobId).AsEnumerable().ToList();
                jobMaster.JobTimeSheetStaff.Select((x) =>
                {
                    x.JobId = jobId;
                    return x;
                }).OrderBy(x => x.JobId).AsEnumerable().ToList();


                jobMaster.JobfilesStatus.Select((x) =>
                {

                    x.JobMasterJobId = jobId;
                    x.job_filename = jobMaster.JobfilesStatus[0].job_filename;
                    x.job_fileStatus = jobMaster.JobfilesStatus[0].job_fileStatus;
                    x.job_IsDeleted = false;
                    return x;
                });

                jobMaster.JobMasterControl.ToList().ForEach(x =>
                {
                    x.JobMasterControlDtlId = 0;
                    x.JobMasterNo = vNumber;
                    x.JobMasterControlJobID = jobId;
                    x.VoucherType = x.VoucherType ?? "";
                });

                // end
                jobrepository.Insert(jobMaster);
                //_jobMasterControl.InsertList(jobMaster.JobMasterControls);
                jobrepository.TransactionCommit();

                return await this.GetSavedJobMasterDetails(jobMaster.JobMasterJobNo);

            }
            catch (Exception ex)
            {
                jobrepository.TransactionRollback();
                throw ex;
            }
        }

        public async Task<JobMaster> UpdateJobMaster(JobMaster jobMaster, List<JobMasterBudgetDetails> jobMasterBudgetDetails, List<JobMasterJobDetails> jobMasterJobDetails, List<JobMasterJobWiseBankGuarantees> jobMasterJobWiseBankGuarantees, List<JobStaff> jobStaff, List<JobEquipment> jobEquipment, List<JobExcutionDetails> jobExcutionDetails, List<JobTimesheetDesc> JobTimesheetDesc, List<JobTimeSheetEquip> JobTimeSheetEquip, List<JobTimesheetMaster> JobTimesheetMaster, List<JobTimeSheetStaff> JobTimeSheetStaff, List<JobfilesStatus> JobfilesStatus, List<JobMasterControl> JobMasterControl)
        {
            try
            {
                int jobId = 0;
                jobrepository.BeginTransaction();
                jobId = jobMaster.JobMasterJobId;
                var existingJobDetails = this.jobdetailsRepository.GetAsQueryable().AsNoTracking().Where(x => x.JobMasterJobDetailsJobId == jobId).ToList();
                //var existingJobControls = this._jobMasterControl.GetAsQueryable().AsNoTracking().Where(x => x.JobMasterControlJobID == jobId).ToList();
                var detailsToDelete = existingJobDetails.Where(x => jobMaster.JobMasterJobDetails.All(d => x.JobMasterJobDetailsJobDetId != d.JobMasterJobDetailsJobDetId)).ToList();
                //var jobControlsToDelete = existingJobControls.Where(x => jobMaster.JobMasterControl.All(d => x.JobMasterControlJobID == d.JobMasterControlJobID)).ToList();

                string vNumber = jobMaster.JobMasterJobNo;

                jobMaster.JobMasterBudgetDetails.Select((x) =>
                {
                    x.JobMasterBudgetDetailsActual = 0;
                    x.JobMasterBudgetDetailsJobId = jobId;
                    x.JobMasterNo = vNumber;
                    return x;
                }).OrderBy(x => x.JobMasterNo).AsEnumerable().ToList();

                jobMaster.JobMasterJobDetails.Select((x) =>
                {
                    x.JobMasterJobDetailsJobId = jobId;
                    x.JobMasterNo = vNumber;
                    return x;
                }).OrderBy(x => x.JobMasterNo).AsEnumerable().ToList();

                jobMaster.JobMasterJobWiseBankGuarantees.Select((x) =>
                {
                    x.JobMasterJobWiseBankGuaranteesJobId = jobId;
                    x.JobMasterNo = vNumber;
                    return x;
                }).OrderBy(x => x.JobMasterNo).AsEnumerable().ToList();

                // job staff , equipment and execution related updation
                jobMaster.JobStaff.Select((x) =>
                {
                    x.JobMasterJobId = jobId;
                    return x;
                }).OrderBy(x => x.JobMasterJobId).AsEnumerable().ToList();

                jobMaster.JobEquipment.Select((x) =>
                {
                    x.JobMasterJobId = jobId;
                    return x;
                }).OrderBy(x => x.JobMasterJobId).AsEnumerable().ToList();
                jobMaster.JobExcutionDetails.Select((x) =>
                {
                    x.JobMasterJobId = jobId;
                    return x;
                }).OrderBy(x => x.JobMasterJobId).AsEnumerable().ToList();
                /// end 
                /// 
                jobMaster.JobTimesheetDesc.Select((x) =>
                {
                    x.JobId = jobId;
                    return x;
                }).OrderBy(x => x.JobId).AsEnumerable().ToList();

                jobMaster.JobTimeSheetEquip.Select((x) =>
                {
                    x.JobId = jobId;
                    return x;
                }).OrderBy(x => x.JobId).AsEnumerable().ToList();
                jobMaster.JobTimesheetMaster.Select((x) =>
                {
                    x.JobId = jobId;
                    return x;
                }).OrderBy(x => x.JobId).AsEnumerable().ToList();
                jobMaster.JobTimeSheetStaff.Select((x) =>
                {
                    x.JobId = jobId;
                    return x;
                }).OrderBy(x => x.JobId).AsEnumerable().ToList();

                jobMaster.JobfilesStatus.Select((x) =>
                {
                    x.JobMasterJobId = jobId;
                    return x;
                }).OrderBy(x => x.JobMasterJobId).AsEnumerable().ToList();


                if (detailsToDelete.Any())
                    this.jobdetailsRepository.DeleteList(detailsToDelete);

                //if (jobControlsToDelete.Any())
                //    this._jobMasterControl.DeleteList(jobControlsToDelete);


                //jobMaster.JobMasterControl.Select((x) =>
                //{
                //    // x.JobMasterControlDtlId = 0;
                //    x.JobMasterControlJobID = jobId;
                //    x.JobMasterNo = vNumber;
                //    return x;
                //}).OrderBy(x => x.JobMasterControlJobID).AsEnumerable().ToList();
                jobrepository.Update(jobMaster);
                //this._jobMasterControl.InsertList(jobMaster.JobMasterControl);

                jobrepository.TransactionCommit();

                return await this.GetSavedJobMasterDetails(jobMaster.JobMasterJobNo);

            }
            catch (Exception ex)
            {
                jobrepository.TransactionRollback();
                throw ex;
            }
        }

        public async Task<JobMaster> GetSavedJobMasterDetails(string jbno)
        {
            JobMaster jobMasterData = new JobMaster();
            if (jobrepository.GetAsQueryable().Any(x => x.JobMasterJobNo == jbno))
            {
                jobMasterData = jobrepository.GetAsQueryable().AsNoTracking().Where(k => k.JobMasterJobNo == jbno).FirstOrDefault();

                var jobBudgetDetailsData = this.jobbudgetRepository.GetAsQueryable().AsNoTracking()
                                                 .Where(x => x.JobMasterNo == jbno && x.JobMasterBudgetDetailsDelStatus == false)
                                                 .Join(this._costCenterMaster.GetAsQueryable().AsNoTracking(),
                                                     j => j.JobMasterBudgetDetailsBudId,
                                                     c => c.CostCenterMasterCostCenterId,
                                                     (j, c) => new
                                                     {
                                                         jdDetails = j,
                                                         ccMaster = c
                                                     })
                                                 .AsNoTracking()
                                                .OrderBy(x => x.ccMaster.CostCenterMasterCostCenterId)
                                                .ToList();

                var groupedData = jobBudgetDetailsData
                .GroupBy(x => new
                {
                    x.ccMaster.CostCenterMasterCostCenterId,
                    x.ccMaster.CostCenterMasterCostCenterName
                })
                .Select(x =>
                {
                    var first = x.FirstOrDefault()?.jdDetails;
                    return new JobMasterBudgetDetails
                    {
                        JobMasterBudgetDetailsCostCenterId = x.Key.CostCenterMasterCostCenterId,
                        JobMasterBudgetDetailsDtlId = first?.JobMasterBudgetDetailsDtlId ?? 0,
                        JobMasterBudgetDetailsJobId = first?.JobMasterBudgetDetailsJobId,
                        JobMasterNo = first?.JobMasterNo,
                        JobMasterBudgetDetailsSno = first?.JobMasterBudgetDetailsSno,
                        JobMasterBudgetDetailsBudId = first?.JobMasterBudgetDetailsBudId,
                        JobMasterBudgetDetailsBudAmount = x.Sum(y => y.jdDetails.JobMasterBudgetDetailsBudAmount ?? 0),
                        JobMasterBudgetDetailsDelStatus = first?.JobMasterBudgetDetailsDelStatus,
                        JobMasterBudgetDetailsActual = x.Sum(y => y.jdDetails.JobMasterBudgetDetailsActual ?? 0),
                        JobMasterBudgetDetailsVariance = x.Sum(y => y.jdDetails.JobMasterBudgetDetailsBudAmount ?? 0) -
                                                         x.Sum(y => y.jdDetails.JobMasterBudgetDetailsActual ?? 0),
                        RefVoucherNo = first?.RefVoucherNo
                    };
                })
                .ToList();

                var jobMasterControls = _jobMasterControl.GetAsQueryable()
                                                        .AsNoTracking()
                                                        .Where(x => x.JobMasterControlJobID == jobMasterData.JobMasterJobId
                                                                                && x.JobMasterControlCostCenerID > 0)
                                                        .ToList();

                var summaries = await GetEstimationCostCenterSummaries(jobMasterData.JobMasterJobId);

                jobMasterData.JobMasterControl = jobMasterControls
                                                                           .GroupBy(x => x.JobMasterControlCostCenerID)
                                                                           .Select(g =>
                                                                           {
                                                                               var prAmount = g.Sum(x =>
                                                                                   (x.VoucherType ?? "") == "PR"
                                                                                       ? (x.JobMasterControlAmount ?? 0m)
                                                                                       : 0m);

                                                                               var boqAmount = g.Sum(x =>
                                                                                   (x.VoucherType ?? "") == "BOQ"
                                                                                       ? (x.JobMasterControlAmount ?? 0m)
                                                                                       : 0m);

                                                                               var actual = g.Sum(x => x.JobMasterControlActual ?? 0m);

                                                                               return new JobMasterControl
                                                                               {
                                                                                   JobMasterControlCostCenerID = g.Key,
                                                                                   PrAmount = prAmount,
                                                                                   BoqAmount = boqAmount,
                                                                                   JobMasterControlActual = actual,
                                                                                   JobMasterControlVariance = prAmount - actual,
                                                                                   JobMasterControlJobID = jobMasterData.JobMasterJobId
                                                                               };
                                                                           })
                                                                           .ToList();

                // Convert summaries to dictionary for fast lookup
                var summaryDict = summaries
                    .GroupBy(s => s.CostcenterId)
                    .ToDictionary(g => g.Key, g => g.Sum(x => x.Amount));
            
                jobMasterData.JobMasterBudgetDetails = groupedData;

                jobMasterData.JobMasterJobDetails = jobdetailsRepository.GetAsQueryable().AsNoTracking().Where(x => x.JobMasterNo == jbno && x.JobMasterJobDetailsDelStatus == false).ToList();
                jobMasterData.JobMasterJobWiseBankGuarantees = jobBankGuaruRepository.GetAsQueryable().AsNoTracking().Where(x => x.JobMasterNo == jbno && x.JobMasterJobWiseBankGuaranteesDelStatus == false).ToList();
                jobMasterData.AccountsTransactions = accountsTransactionsRepository.GetAsQueryable().AsNoTracking().Where(x => x.AccountsTransactionsJobNo == jobMasterData.JobMasterJobId && x.AccountstransactionsDelStatus == false && (x.AccountsTransactionsVoucherType == "RECEIPT" || x.AccountsTransactionsVoucherType == "BANK RECEIPT")).ToList();
                jobMasterData.JobStaff = jobstaffrepository.GetAsQueryable().AsNoTracking().Where(x => x.JobMasterJobId == jobMasterData.JobMasterJobId).ToList();
                jobMasterData.JobEquipment = jobEquipmentrepository.GetAsQueryable().AsNoTracking().Where(x => x.JobMasterJobId == jobMasterData.JobMasterJobId).ToList();
                jobMasterData.JobExcutionDetails = jobexecrepository.GetAsQueryable().AsNoTracking().Where(x => x.JobMasterJobId == jobMasterData.JobMasterJobId).ToList();
                jobMasterData.JobMasterExpenses =
                                       (from at in context.AccountsTransactions
                                        join mat in context.MasterAccountsTable on at.AccountsTransactionsAccNo equals mat.MaAccNo
                                        //join cos in context.CostCenterMaster on at.AccountsTransactionsCostCenterId equals cos.CostCenterMasterCostCenterId
                                        join cos in context.CostCenterMaster on at.AccountsTransactionsCostCenterId equals cos.CostCenterMasterCostCenterId into cosGroup
                                        from cos in cosGroup.DefaultIfEmpty()
                                        where (mat.MaMainHead == "Expenses" && at.AccountsTransactionsJobNo == jobMasterData.JobMasterJobId && at.AccountstransactionsDelStatus != true)
                                        select new JobMasterExpenseDto
                                        {
                                            accNo = at.AccountsTransactionsAccNo,
                                            accName = mat.MaAccName,
                                            Description = at.AccountsTransactionsDescription,
                                            VoucherNo = at.AccountsTransactionsVoucherNo,
                                            VoucherType = at.AccountsTransactionsVoucherType,
                                            transDate = at.AccountsTransactionsTransDate,
                                            Chq_Date = at.AccountsTransactionsCheqDate,
                                            Chq_No = at.AccountsTransactionsCheqNo,
                                            CrAmt = at.AccountsTransactionsCredit,
                                            DrAmt = at.AccountsTransactionsDebit,
                                            CostName = cos.CostCenterMasterCostCenterName
                                        }).OrderBy(x => x.accName).AsNoTracking().AsEnumerable().ToList();
                jobMasterData.PaymentTransactions = (from at in context.AccountsTransactions
                                                     join mat in context.MasterAccountsTable
                                                         on at.AccountsTransactionsAccNo equals mat.MaAccNo
                                                     where
                                                         mat.MaMainHead != "Expenses"
                                                         && (at.AccountsTransactionsVoucherType == "PAYMENT"
                                                             || at.AccountsTransactionsVoucherType == "BANK PAYMENT")
                                                         && at.AccountsTransactionsJobNo == jobMasterData.JobMasterJobId
                                                         && at.AccountstransactionsDelStatus != true
                                                     select at   // 👈 sirf entity select
                                                    )
                                                    .AsNoTracking()
                                                    .ToList();

                jobMasterData.JobTimeSheetStaff = _jobTimeSheetStaff.GetAsQueryable().AsNoTracking().Where(a => a.JobId == jobMasterData.JobMasterJobId).ToList();
                jobMasterData.JobTimesheetMaster = _jobTimesheetMaster.GetAsQueryable().AsNoTracking().Where(a => a.JobId == jobMasterData.JobMasterJobId).ToList();
                jobMasterData.JobTimeSheetEquip = _jobTimeSheetEquip.GetAsQueryable().AsNoTracking().Where(a => a.JobId == jobMasterData.JobMasterJobId).OrderBy(x => x.JobMasterJobJob).ToList();
                jobMasterData.JobTimesheetDesc = _jobTimesheetDesc.GetAsQueryable().AsNoTracking().Where(a => a.JobId == jobMasterData.JobMasterJobId).ToList();
                jobMasterData.JobfilesStatus = _JobfilesStatus.GetAsQueryable().AsNoTracking().Where(a => a.JobMasterJobId == jobMasterData.JobMasterJobId).ToList();
                jobMasterData.JobEquipment = jobEquipmentrepository.GetAsQueryable().AsNoTracking().Where(x => x.JobMasterJobJob.JobMasterJobId == jobMasterData.JobMasterJobId).Select(x => new JobEquipment() { Id = x.Id, JobMasterEquipId = x.JobMasterEquipId, JobMasterJobId = x.JobMasterJobId, JobMasterJobJob = x.JobMasterJobJob, JobMasterEquipName = x.JobMasterEquipName }).ToList();

                return jobMasterData;


            }
            return null;
        }

        public IEnumerable<JobDocuments> GetJobDocumentstById(int jobDocumentJobId, int jobDocumentId)
        {
            var Jobdoc = _jobDocumentsRepository.GetAsQueryable().AsNoTracking().Where(a => a.JobDocumentJobId == jobDocumentJobId && a.JobDocumentId == jobDocumentId).ToList();
            return Jobdoc;
        }
        public void SaveJobDocument(JobDocuments jobDocumnet)
        {
            _jobDocumentsRepository.Insert(jobDocumnet);
        }
        public void JobfilesStatus(JobfilesStatus JobfilesStatus)
        {

            JobfilesStatus.job_filename = JobfilesStatus.job_filename;
            JobfilesStatus.job_fileStatus = false;
            JobfilesStatus.JobMasterJobId = JobfilesStatus.JobMasterJobId;
            _JobfilesStatus.Insert(JobfilesStatus);
        }
        public VouchersNumbers GenerateVoucherNo(DateTime? VoucherGenDate)
        {
            try
            {
                //var prefix = this.programsettingsRepository.GetAsQueryable().Where(k => k.ProgramSettingsKeyValue == Prefix.ContraVoucher_Prefix).FirstOrDefault().ProgramSettingsTextValue;
                int vnoMaxVal = Convert.ToInt32(voucherNumbersRepository.GetAsQueryable()
                                                        .Where(x => x.VouchersNumbersVNoNu > 0 && x.VouchersNumbersVType == VoucherType.JobMasterVoucher_TYPE)
                                                        .DefaultIfEmpty()
                                                        .Max(o => o == null ? 0 : o.VouchersNumbersVNoNu)) + 1;
                VouchersNumbers vouchersNumbers = new VouchersNumbers
                {
                    VouchersNumbersVNo = "JM" + vnoMaxVal,
                    VouchersNumbersVNoNu = vnoMaxVal,
                    VouchersNumbersVType = VoucherType.JobMasterVoucher_TYPE,
                    VouchersNumbersFsno = 1,
                    VouchersNumbersStatus = AccountStatus.Pending,
                    VouchersNumbersVoucherDate = VoucherGenDate

                };
                voucherNumbersRepository.Insert(vouchersNumbers);
                return vouchersNumbers;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Task<string> GetJobHTML(int id)
        {
            throw new NotImplementedException();

        }
        private readonly string _pdfDirectory = @"Upload//Pdfs"; // Change this to your actual PDF directory
        public async Task<FileResultDto> GetJobPdfById(int id)
        {
            if (!Directory.Exists(_pdfDirectory))
            {
                return null; // Directory does not exist
            }

            // Allowed file types
            string[] allowedExtensions = { ".pdf", ".docx", ".xlsx" };

            // Search for the first file that contains the ID and has a valid extension
            var filePath = Directory.EnumerateFiles(_pdfDirectory)
                .FirstOrDefault(f => allowedExtensions.Contains(Path.GetExtension(f).ToLower()) &&
                                     Path.GetFileName(f).Contains(id.ToString()));

            if (filePath == null)
            {
                return null; // No matching file found
            }

            var fileBytes = await File.ReadAllBytesAsync(filePath);

            return new FileResultDto
            {
                FileName = Path.GetFileName(filePath),
                FileBytes = fileBytes,
                ContentType = GetMimeType(filePath) // Get the correct MIME type
            };
        }

        // Helper method to get the correct MIME type
        private string GetMimeType(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLower();

            return extension switch
            {
                ".pdf" => "application/pdf",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                _ => "application/octet-stream" // Default binary file type
            };
        }
        //public async Task<FileResultDto> GetJobPdfById(int id)
        //{

        //    // Search for the PDF file by ID (assuming the naming convention is "Job123.pdf")
        //    var filePath = Directory.EnumerateFiles(_pdfDirectory, "*.pdf")
        //        .FirstOrDefault(f => Path.GetFileNameWithoutExtension(f).StartsWith($"{id}"));

        //    if (filePath == null)
        //    {
        //        return null;
        //    }

        //    var fileBytes = await File.ReadAllBytesAsync(filePath);

        //    return new FileResultDto
        //    {
        //        FileName = Path.GetFileName(filePath),
        //        FileBytes = fileBytes
        //    };
        //}
        public IEnumerable<JobDocuments> GetAllJobDocumentById(int id)
        {
            try
            {
                var Jobdoc = _jobDocumentsRepository.GetAsQueryable().Where(k => k.JobDocumentJobId == Convert.ToInt32(id) && k.JobDocumentDelStatus == false).ToList();
                return Jobdoc;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IEnumerable<JobDocuments> DeleteJobDocument(int Id)
        {
            try
            {
                var dataObj = _jobDocumentsRepository.GetAsQueryable().Where(o => o.JobDocumentId == Id).FirstOrDefault();
                dataObj.JobDocumentDelStatus = true;
                _jobDocumentsRepository.Update(dataObj);
                return _jobDocumentsRepository.GetAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<Response<string>> GetUserLevelForJob(int userId, int joId)
        {
            try
            {
                var formTypes = Enum.GetValues(typeof(EnumOrderForms))
                    .Cast<EnumOrderForms>()
                    .Where(x => x == EnumOrderForms.PurchaseOrder || x == EnumOrderForms.PurchaseRequisition)
                    .ToList();

                string userLevel = await _approverLevelSettings
                    .GetAsQueryable()
                    .AsNoTracking()
                    .Where(x => x.ApproverId == userId && x.JobId == joId && formTypes.Contains(x.FormType))
                    .Select(x => x.ApproverLevel.ToString())
                    .FirstOrDefaultAsync();

                if (string.IsNullOrEmpty(userLevel))
                    return Response<string>.Fail(string.Empty, "User level not found.");

                return Response<string>.Success(userLevel, $"User level: {userLevel} found.");
            }
            catch (Exception)
            {

                throw;
            }
        }



        private async Task<List<EstimationCostCenterSummary>> GetEstimationCostCenterSummaries(int jobId)
        {
            try
            {
                string sql = @$"select a.cost_center as CostcenterId,SUM(isnull(amount,0)) as Amount from
                                         Estimation_CostCenter a
                                         join BillofQty_Master b on a.BillQtyId = b.id
                                         where b.CustomerEnqId = {jobId}
                                         group by a.cost_center";

                var summaries = (await _dbConnection.QueryAsync<EstimationCostCenterSummary>(sql)).ToList();

                if (!summaries.Any())
                    return new List<EstimationCostCenterSummary>();

                return summaries;
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
    public class EstimationCostCenterSummary
    {
        public int CostcenterId { get; set; }
        public decimal Amount { get; set; }
    }

}
