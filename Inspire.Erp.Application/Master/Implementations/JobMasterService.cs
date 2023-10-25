using Inspire.Erp.Domain.DTO.Job_Master;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Infrastructure.Database;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Inspire.Erp.Application.Master
{
   public class JobMasterService: IJobMasterService
    {
        private IRepository<JobMaster> jobrepository;
        private IRepository<JobMasterBudgetDetails> jobbudgetRepository;
        private IRepository<JobMasterJobDetails> jobdetailsRepository;
        private IRepository<JobMasterJobWiseBankGuarantees> jobBankGuaruRepository;
        private IRepository<AccountsTransactions> accountsTransactionsRepository;
        private readonly InspireErpDBContext context;
        public JobMasterService(IRepository<JobMaster> _jobrepository,
            IRepository<JobMasterBudgetDetails> _jobbudgetRepository,
            IRepository<JobMasterJobDetails> _jobdetailsRepository,
            IRepository<JobMasterJobWiseBankGuarantees> _jobBankGuaruRepository,
            IRepository<AccountsTransactions> _accountsTransactionsRepository,
            InspireErpDBContext _context)
        {
            jobrepository = _jobrepository;
            jobbudgetRepository = _jobbudgetRepository;
            jobdetailsRepository = _jobdetailsRepository;
            jobBankGuaruRepository = _jobBankGuaruRepository;
            accountsTransactionsRepository = _accountsTransactionsRepository;
            context = _context;
        }
        //public IEnumerable<JobMaster> InsertJob(JobMaster jobMaster)
        //{
        //    bool valid = false;
        //    try
        //    {
        //        valid = true;
        //        int mxc = 0;
        //        mxc = (int)jobrepository.GetAsQueryable().Where(k => k.JobMasterJobId != null).Select(x => x.JobMasterJobId).Max();
        //        if (mxc == null) { mxc = 1; } else { mxc = mxc + 1; }

        //        jobMaster.JobMasterJobId = mxc;
        //        jobrepository.Insert(jobMaster);
        //    }
        //    catch (Exception ex)
        //    {
        //        valid = false;
        //        throw ex;
        //    }
        //    finally
        //    {
        //        //jobrepository.Dispose();
        //    }
        //    return this.GetAllJob();
        //}
        //public IEnumerable<JobMaster> UpdateJob(JobMaster jobMaster)
        //{
        //    bool valid = false;
        //    try
        //    {
        //       jobrepository.Update(jobMaster);
        //        valid = true;

        //    }
        //    catch (Exception ex)
        //    {
        //        valid = false;
        //        throw ex;

        //    }
        //    finally
        //    {
        //        //jobrepository.Dispose();
        //    }
        //    return this.GetAllJob();
        //}
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
                //jobrepository.Dispose();
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
                //cityrepository.Dispose();
            }
            return jobMasters;

        }

        public JobMaster InsertJobMaster(JobMaster jobMaster, List<JobMasterBudgetDetails> jobMasterBudgetDetails, List<JobMasterJobDetails> jobMasterJobDetails, List<JobMasterJobWiseBankGuarantees> jobMasterJobWiseBankGuarantees)
        {
            try
            {
               int jobId = (int)jobrepository.GetAsQueryable().Where(k => k.JobMasterJobId != null).Select(x => x.JobMasterJobId).Max();
                if (jobId == null) { jobId = 1; } else { jobId = jobId + 1; }

                jobMaster.JobMasterJobId = jobId;
              
                jobMasterBudgetDetails.Select((x) =>
                {
                    x.JobMasterBudgetDetailsJobId = jobId;
                    return x;
                }).ToList();
                jobbudgetRepository.InsertList(jobMasterBudgetDetails);

                jobMasterJobDetails.Select((x) =>
                {
                    x.JobMasterJobDetailsJobId = jobId;
                    return x;
                }).ToList();
                jobdetailsRepository.InsertList(jobMasterJobDetails);

                jobMasterJobWiseBankGuarantees.Select((x) =>
                {
                    x.JobMasterJobWiseBankGuaranteesJobId = jobId;
                    return x;
                }).ToList();

                jobBankGuaruRepository.InsertList(jobMasterJobWiseBankGuarantees);
                jobrepository.Insert(jobMaster);

                return this.GetSavedJobMasterDetails(jobMaster.JobMasterJobNo);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

   
        public JobMaster UpdateJobMaster(JobMaster jobMaster, List<JobMasterBudgetDetails> jobMasterBudgetDetails, List<JobMasterJobDetails> jobMasterJobDetails, List<JobMasterJobWiseBankGuarantees> jobMasterJobWiseBankGuarantees)
        {
            try
            {
                jobrepository.Update(jobMaster);
                
                //jobbudgetRepository.UpdateList(jobMasterBudgetDetails);

              
                //jobdetailsRepository.UpdateList(jobMasterJobDetails);

            
               // jobBankGuaruRepository.UpdateList(jobMasterJobWiseBankGuarantees);

                return this.GetSavedJobMasterDetails(jobMaster.JobMasterJobNo);
                
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public JobMaster GetSavedJobMasterDetails(string jbno)
        {
            JobMaster jobMasterData = new JobMaster();
            if (jobrepository.GetAsQueryable().Any(x => x.JobMasterJobNo == jbno))
            {
                jobMasterData = jobrepository.GetAsQueryable().Where(k => k.JobMasterJobNo == jbno).FirstOrDefault();
                jobMasterData.JobMasterBudgetDetails = jobbudgetRepository.GetAll().Where(x => x.JobMasterNo == jbno && x.JobMasterBudgetDetailsDelStatus == false).ToList();
                jobMasterData.JobMasterJobDetails = jobdetailsRepository.GetAll().Where(x => x.JobMasterNo == jbno && x.JobMasterJobDetailsDelStatus == false).ToList();
                jobMasterData.JobMasterJobWiseBankGuarantees = jobBankGuaruRepository.GetAll().Where(x => x.JobMasterNo == jbno && x.JobMasterJobWiseBankGuaranteesDelStatus == false).ToList();
                jobMasterData.AccountsTransactions = accountsTransactionsRepository.GetAll().Where(x => x.AccountsTransactionsJobNo == jobMasterData.JobMasterJobId).ToList();
                
                jobMasterData.JobMasterExpenses =
                                       (from at in context.AccountsTransactions
                                       join mat in context.MasterAccountsTable on at.AccountsTransactionsAccNo equals mat.MaAccNo
                                       join cos in context.CostCenterMaster on at.AccountsTransactionsCostCenterId equals cos.CostCenterMasterCostCenterId
                                       where (mat.MaMainHead == "Expenses" && at.AccountsTransactionsJobNo == jobMasterData.JobMasterJobId && at.AccountstransactionsDelStatus == false)
                                       select new JobMasterExpenseDto {

                                           accNo  = at.AccountsTransactionsAccNo,
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
                                       }).ToList();
                return jobMasterData;
            }
            return null;
        }
    }
}
