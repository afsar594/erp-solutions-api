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

namespace Inspire.Erp.Application.Master
{
    public class JobMasterService : IJobMasterService
    {
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
            IRepository<JobDocuments> jobDocumentsRepository, IRepository<JobfilesStatus> jobfilesStatus)
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
        public JobMaster InsertJobMaster(JobMaster jobMaster, List<JobMasterBudgetDetails> jobMasterBudgetDetails, List<JobMasterJobDetails> jobMasterJobDetails, List<JobMasterJobWiseBankGuarantees> jobMasterJobWiseBankGuarantees, List<JobStaff> jobStaff, List<JobEquipment> jobEquipment, List<JobExcutionDetails> jobExcutionDetails, List<JobTimesheetDesc> JobTimesheetDesc, List<JobTimeSheetEquip> JobTimeSheetEquip, List<JobTimesheetMaster> JobTimesheetMaster, List<JobTimeSheetStaff> JobTimeSheetStaff, List<JobfilesStatus> JobfilesStatus)
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
                // job file 
                jobMaster.JobfilesStatus.Select((x) =>
                {
                    //x.job_id = jobMaster.JobfilesStatus[0].job_id;
                    x.JobMasterJobId = jobId;
                    x.job_filename = jobMaster.JobfilesStatus[0].job_filename;
                    x.job_fileStatus = jobMaster.JobfilesStatus[0].job_fileStatus;
                    x.job_IsDeleted = false;
                    return  x;
                });
              //  _JobfilesStatus.InsertList(jobMaster.JobfilesStatus);
                // end
                jobrepository.Insert(jobMaster);
                jobrepository.TransactionCommit();

                return this.GetSavedJobMasterDetails(jobMaster.JobMasterJobNo);
                
            }
            catch (Exception ex)
            {
                jobrepository.TransactionRollback();
                throw ex;
            }
        }
        //public JobMaster UpdateJobMaster(JobMaster jobMaster, List<JobMasterBudgetDetails> jobMasterBudgetDetails, List<JobMasterJobDetails> jobMasterJobDetails, List<JobMasterJobWiseBankGuarantees> jobMasterJobWiseBankGuarantees)
        //{
        //    try
        //    {

        //        jobrepository.Update(jobMaster);


        //        return this.GetSavedJobMasterDetails(jobMaster.JobMasterJobNo);

        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}
        public JobMaster UpdateJobMaster(JobMaster jobMaster, List<JobMasterBudgetDetails> jobMasterBudgetDetails, List<JobMasterJobDetails> jobMasterJobDetails, List<JobMasterJobWiseBankGuarantees> jobMasterJobWiseBankGuarantees, List<JobStaff> jobStaff, List<JobEquipment> jobEquipment, List<JobExcutionDetails> jobExcutionDetails, List<JobTimesheetDesc> JobTimesheetDesc, List<JobTimeSheetEquip> JobTimeSheetEquip, List<JobTimesheetMaster> JobTimesheetMaster, List<JobTimeSheetStaff> JobTimeSheetStaff, List<JobfilesStatus> JobfilesStatus)
        {
            try
            {
                int jobId = 0;
                jobrepository.BeginTransaction();
                jobId = jobMaster.JobMasterJobId;

                string vNumber = jobMaster.JobMasterJobNo;

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

                // job file 
                //jobMaster.JobfilesStatus.Select((x) =>
                //{
                //    x.JobMasterJobId = jobId;
                //    x.job_filename = jobMaster.JobfilesStatus[0].job_filename;
                //    x.job_fileStatus = jobMaster.JobfilesStatus[0].job_fileStatus;
                //    x.job_IsDeleted = false;
                //    return x;
                //});
                //_JobfilesStatus.GetAsQueryable().Where(e => e.JobMasterJobId == jobId).ToList();
                //jobMaster.JobfilesStatus.Select((x) =>
                //{
                //    x.JobMasterJobId = jobId;
                //    x.job_filename = jobMaster.JobfilesStatus[0].job_filename;
                //    x.job_fileStatus = jobMaster.JobfilesStatus[0].job_fileStatus;
                //    x.job_IsDeleted = false;
                //    return x;
                //});
                
                jobrepository.Update(jobMaster);
                jobrepository.TransactionCommit();

                return this.GetSavedJobMasterDetails(jobMaster.JobMasterJobNo);

            }
            catch (Exception ex)
            {
                jobrepository.TransactionRollback();
                throw ex;
            }
        }

        public JobMaster GetSavedJobMasterDetails(string jbno)
        {
            JobMaster jobMasterData = new JobMaster();
            if (jobrepository.GetAsQueryable().Any(x => x.JobMasterJobNo == jbno))
            {
                jobMasterData = jobrepository.GetAsQueryable().Where(k => k.JobMasterJobNo == jbno).FirstOrDefault();
                jobMasterData.JobMasterBudgetDetails = jobbudgetRepository.GetAsQueryable().Where(x => x.JobMasterNo == jbno && x.JobMasterBudgetDetailsDelStatus == false).ToList();
                foreach (var item in jobMasterData.JobMasterBudgetDetails)
                {
                    var groupedResults =
                        from at in context.AccountsTransactions
                        join cos in context.CostCenterMaster on
                        at.AccountsTransactionsCostCenterId equals cos.CostCenterMasterCostCenterId
                        where at.AccountstransactionsDelStatus == false
                        && at.AccountsTransactionsCostCenterId == item.JobMasterBudgetDetailsBudId
                        && at.AccountsTransactionsJobNo == (long)item.JobMasterBudgetDetailsJobId
                        group at by cos.CostCenterMasterCostCenterName into g
                        select new
                        {
                            CostName = g.Key,
                            TotalDrAmt = g.Sum(x => x.AccountsTransactionsDebit)
                        };

                    foreach (var result in groupedResults)
                    {

                        item.JobMasterBudgetDetailsActual = result.TotalDrAmt;
                        item.JobMasterBudgetDetailsVariance = -result.TotalDrAmt;
                    }
                }
                jobMasterData.JobMasterJobDetails = jobdetailsRepository.GetAsQueryable().Where(x => x.JobMasterNo == jbno && x.JobMasterJobDetailsDelStatus == false).ToList();
                jobMasterData.JobMasterJobWiseBankGuarantees = jobBankGuaruRepository.GetAsQueryable().Where(x => x.JobMasterNo == jbno && x.JobMasterJobWiseBankGuaranteesDelStatus == false).ToList();
                jobMasterData.AccountsTransactions = accountsTransactionsRepository.GetAsQueryable().Where(x => x.AccountsTransactionsJobNo == jobMasterData.JobMasterJobId).ToList();
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
                                        }).OrderBy(x => x.accName).AsEnumerable().ToList();
                jobMasterData.JobTimeSheetStaff = _jobTimeSheetStaff.GetAsQueryable().AsNoTracking().Where(a => a.JobId == jobMasterData.JobMasterJobId).ToList();
                jobMasterData.JobTimesheetMaster = _jobTimesheetMaster.GetAsQueryable().AsNoTracking().Where(a => a.JobId == jobMasterData.JobMasterJobId).ToList();
                jobMasterData.JobTimeSheetEquip = _jobTimeSheetEquip.GetAsQueryable().AsNoTracking().Where(a => a.JobId == jobMasterData.JobMasterJobId).OrderBy(x => x.JobMasterJobJob).ToList();
                jobMasterData.JobTimesheetDesc = _jobTimesheetDesc.GetAsQueryable().AsNoTracking().Where(a => a.JobId == jobMasterData.JobMasterJobId).ToList();
                jobMasterData.JobfilesStatus = _JobfilesStatus.GetAsQueryable().AsNoTracking().Where(a => a.JobMasterJobId == jobMasterData.JobMasterJobId).ToList();
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
        public IEnumerable<JobDocuments> GetAllJobDocumentById(int id)
        {
            try
            {
                var Jobdoc = _jobDocumentsRepository.GetAsQueryable().Where(k => k.JobDocumentJobId == Convert.ToInt32(id) && k.JobDocumentDelStatus==false).ToList();
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
    }
}
