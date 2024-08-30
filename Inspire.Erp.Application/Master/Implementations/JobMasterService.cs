using Inspire.Erp.Domain.DTO.Job_Master;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Enums;
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
        private IRepository<JobDocuments> jobDocumentsRepository;
        private IRepository<ProgramSettings> programsettingsRepository;
        private IRepository<VouchersNumbers> voucherNumbersRepository;

        private IRepository<AccountsTransactions> accountsTransactionsRepository;
        private readonly InspireErpDBContext context;
        public JobMasterService(IRepository<JobMaster> _jobrepository,
            IRepository<JobMasterBudgetDetails> _jobbudgetRepository,
            IRepository<JobMasterJobDetails> _jobdetailsRepository,
            IRepository<JobMasterJobWiseBankGuarantees> _jobBankGuaruRepository,
            IRepository<AccountsTransactions> _accountsTransactionsRepository,
            InspireErpDBContext _context,IRepository<ProgramSettings> _programsettingsRepository,
            IRepository<VouchersNumbers> _voucherNumbersRepository)
        {
            jobrepository = _jobrepository;
            jobbudgetRepository = _jobbudgetRepository;
            jobdetailsRepository = _jobdetailsRepository;
            jobBankGuaruRepository = _jobBankGuaruRepository;
            accountsTransactionsRepository = _accountsTransactionsRepository;
            context = _context;
            programsettingsRepository = _programsettingsRepository;
            voucherNumbersRepository = _voucherNumbersRepository;
            
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
                jobrepository.BeginTransaction();
                int jobId = (int)jobrepository.GetAsQueryable().Where(k => k.JobMasterJobId != null).Select(x => x.JobMasterJobId).Max();
                if (jobId == null) { jobId = 1; } else { jobId = jobId + 1; }
                jobMaster.JobMasterJobId = jobId;


                string vNumber = this.GenerateVoucherNo(jobMaster.JobMasterJobDate).VouchersNumbersVNo;
                jobMaster.JobMasterJobNo = vNumber;

                foreach (var item in jobMaster.JobMasterBudgetDetails)
                {
                    var groupedResults =
                        from at in context.AccountsTransactions
                        join mat in context.MasterAccountsTable on at.AccountsTransactionsAccNo equals mat.MaAccNo
                        join cos in context.CostCenterMaster on at.AccountsTransactionsCostCenterId equals cos.CostCenterMasterCostCenterId
                        where (mat.MaMainHead == "Expenses" && (at.AccountstransactionsDelStatus == false || at.AccountstransactionsDelStatus == null)  && at.AccountsTransactionsCostCenterId == item.JobMasterBudgetDetailsBudId
                        && at.AccountsTransactionsDebit>0)
                        group at by cos.CostCenterMasterCostCenterName into g
                        select new
                        {
                            CostName = g.Key,
                            TotalDrAmt = g.Sum(x => x.AccountsTransactionsDebit)
                        };

                    // Now you can iterate over the grouped results if needed
                    foreach (var result in groupedResults)
                    {
                        // Example: print out the CostName and TotalDrAmt
                        //Console.WriteLine($"Cost Center: {result.CostName}, Total DrAmt: {result.TotalDrAmt}");

                        // Or assign the result to a property of the current item in jobMasterBudgetDetails
                        // item.TotalDrAmtByCostCenter = result.TotalDrAmt; // Assuming you have such a property
                        item.JobMasterBudgetDetailsActual = result.TotalDrAmt;
                        item.JobMasterBudgetDetailsVariance = -result.TotalDrAmt;

                    }


                }


                jobMaster.JobMasterBudgetDetails.Select((x) =>
                {
                    x.JobMasterBudgetDetailsJobId = jobId;
                    x.JobMasterNo = vNumber;
                    return x;
                }).ToList();
                //jobbudgetRepository.InsertList(jobMasterBudgetDetails);

                jobMaster.JobMasterJobDetails.Select((x) =>
                {
                    x.JobMasterJobDetailsJobId = jobId;
                    x.JobMasterNo = vNumber;
                    return x;
                }).ToList();
                //jobdetailsRepository.InsertList(jobMasterJobDetails);

                jobMaster.JobMasterJobWiseBankGuarantees.Select((x) =>
                {
                    x.JobMasterJobWiseBankGuaranteesJobId = jobId;
                    x.JobMasterNo = vNumber;
                    return x;
                }).ToList();

                // jobBankGuaruRepository.InsertList(jobMasterJobWiseBankGuarantees);

                jobrepository.Insert(jobMaster);
                jobrepository.TransactionCommit();

                return this.GetSavedJobMasterDetails(jobMaster.JobMasterJobNo);
            }
            catch (Exception ex)
            {
                jobrepository.TransactionRollback();
                throw;
            }
        }

   
        public JobMaster UpdateJobMaster(JobMaster jobMaster, List<JobMasterBudgetDetails> jobMasterBudgetDetails, List<JobMasterJobDetails> jobMasterJobDetails, List<JobMasterJobWiseBankGuarantees> jobMasterJobWiseBankGuarantees)
        {
            try
            {
                jobrepository.Update(jobMaster);
                
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
                jobMasterData.JobMasterBudgetDetails = jobbudgetRepository.GetAsQueryable().Where(x => x.JobMasterNo == jbno && x.JobMasterBudgetDetailsDelStatus == false).ToList();
                jobMasterData.JobMasterJobDetails = jobdetailsRepository.GetAsQueryable().Where(x => x.JobMasterNo == jbno && x.JobMasterJobDetailsDelStatus == false).ToList();
                jobMasterData.JobMasterJobWiseBankGuarantees = jobBankGuaruRepository.GetAsQueryable().Where(x => x.JobMasterNo == jbno && x.JobMasterJobWiseBankGuaranteesDelStatus == false).ToList();
                jobMasterData.AccountsTransactions = accountsTransactionsRepository.GetAsQueryable().Where(x => x.AccountsTransactionsJobNo == jobMasterData.JobMasterJobId).ToList();
                
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

        public void SaveJobDocument(JobDocuments jobDocumnet)
        {
            jobDocumentsRepository.Insert(jobDocumnet);
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

    }
}
