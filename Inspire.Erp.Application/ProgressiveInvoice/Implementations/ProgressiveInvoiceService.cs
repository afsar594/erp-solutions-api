using Inspire.Erp.Application.Master.Interfaces;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using System;
using System.Collections.Generic;
using System.Text;
using Inspire.Erp.Infrastructure.Database;
using Inspire.Erp.Application.Common;
using Inspire.Erp.Domain.Enums;
using Inspire.Erp.Application.MODULE;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Inspire.Erp.Domain.Modals;
using Inspire.Erp.Domain.Modals.Common;
using Microsoft.AspNetCore.Server.IIS.Core;
using System.Linq;
using AutoMapper.Configuration;
using Inspire.Erp.Application.Common.Invoices;
using System.Data;
using Inspire.Erp.Infrastructure;
using Microsoft.Extensions.Options;
using Dapper;

namespace Inspire.Erp.Application.Master.Implementations
{
    public class ProgressiveInoiceService : IProgressiveInvoiceService
    {
        private IRepository<ProgressiveInvoice> _progressiveInvoice;
        private IRepository<AccountsTransactions> _accountTransactionRepository;
        private IRepository<ProgramSettings> _programsettingsRepository;
        private IRepository<VouchersNumbers> _voucherNumbersRepository;
        private IRepository<ProgressiveInvoiceDetails> _progressiveInvoiceDetails;
        private readonly IDbConnection _dbConnection;
        public ProgressiveInoiceService(Microsoft.Extensions.Configuration.IConfiguration config, IOptions<ApplicationSettings> applicationSettings, IRepository<ProgressiveInvoice> progressiveInvoice, IRepository<AccountsTransactions> accountTransactionRepository, IRepository<ProgramSettings> programsettingsRepository, IRepository<VouchersNumbers> voucherNumbersRepository)
        {
            _progressiveInvoice = progressiveInvoice;
            _accountTransactionRepository = accountTransactionRepository;
            _programsettingsRepository = programsettingsRepository;
            _voucherNumbersRepository = voucherNumbersRepository;
            var settings = config.GetSection("ApplicationSettings").Get<ApplicationSettings>();
            _dbConnection = new SqlConnection(settings.ConnectionString);
        }
        public ProgressiveInvoice InsertProgressInvoice(ProgressiveInvoice progressiveInvoice, List<AccountsTransactions> accountsTransactions, List<ProgressiveInvoiceDetails> progressiveInvoiceDetails)
        {
            try
            {
                _progressiveInvoice.BeginTransaction();
                string puchaseVoucherNumber = this.GenerateVoucherNo(progressiveInvoice.ProgressiveInvoiceInvDate).VouchersNumbersVNo;
                progressiveInvoice.ProgressiveInvoiceInvNo = puchaseVoucherNumber;


                int maxcount = 0;
                maxcount = Convert.ToInt32(
                    _progressiveInvoice.GetAsQueryable()
                    .DefaultIfEmpty().Max(o => o == null ? 0 : o.ProgressiveInvoiceId) + 1);

                //progressiveInvoiceDetails = progressiveInvoiceDetails.Select((x) =>
                //{
                //    x.ProgressiveInvoiceDetailsPrDetId = maxcount;
                //    return x;

                //}).ToList();

                //_progressiveInvoiceDetails.InsertList(progressiveInvoiceDetails);

                accountsTransactions = accountsTransactions.Select((k) =>
                {
                    k.AccountsTransactionsTransDate = progressiveInvoice.ProgressiveInvoiceInvDate;
                    k.AccountsTransactionsVoucherNo = puchaseVoucherNumber;
                    k.AccountsTransactionsVoucherType = VoucherType.ProgressiveInvoice_TYPE;
                    k.AccountsTransactionsStatus = AccountStatus.Approved;
                    k.AccountsTransactionsAllocDebit = 0;
                    k.AccountsTransactionsAllocCredit = (decimal)progressiveInvoice.ProgressiveInvoiceTotAmount;
                    k.AccountsTransactionsFcDebit = 0;
                    k.AccountsTransactionsStatus = "A";
                    return k;
                }).OrderBy(x => x.AccountsTransactionsVoucherNo).AsEnumerable().ToList();
                _accountTransactionRepository.InsertList(accountsTransactions);


                _progressiveInvoice.Insert(progressiveInvoice);

                _progressiveInvoice.TransactionCommit();
                return this.GetSavedProgressiveInoices(progressiveInvoice.ProgressiveInvoiceInvNo);

            }
            catch (Exception ex)
            {
                _progressiveInvoice.TransactionRollback();
                throw ex;
            }
        }
        public ProgressiveInvoice UpdateProgressInvoice(ProgressiveInvoice progressiveInvoice, List<AccountsTransactions> accountsTransactions, List<ProgressiveInvoiceDetails> progressiveInvoiceDetails)
        {
            try
            {

                _progressiveInvoice.BeginTransaction();
                clsCommonFunctions.Delete_OldEntry_AccountsTransactions(progressiveInvoice.ProgressiveInvoiceInvNo, progressiveInvoice.ProgressiveInvoiceInvoiceType, _accountTransactionRepository);
                _progressiveInvoice.Update(progressiveInvoice);
                //_progressiveInvoiceDetails.UpdateList(progressiveInvoiceDetails);
                accountsTransactions = accountsTransactions.Select((k) =>
                {
                    if (k.AccountsTransactionsTransSno == 0)
                    {
                        k.AccountsTransactionsTransDate = progressiveInvoice.ProgressiveInvoiceInvDate;
                        k.AccountsTransactionsVoucherNo = progressiveInvoice.ProgressiveInvoiceInvNo;
                        k.AccountsTransactionsVoucherType = VoucherType.ProgressiveInvoice_TYPE;
                        k.AccountsTransactionsStatus = AccountStatus.Approved;
                    }
                    return k;
                }).OrderBy(x => x.AccountsTransactionsVoucherNo).AsEnumerable().ToList();
                _accountTransactionRepository.InsertList(accountsTransactions);
                _progressiveInvoice.TransactionCommit();

            }
            catch (Exception ex)
            {
                _progressiveInvoice.TransactionRollback();
                throw ex;
            }

            return this.GetSavedProgressiveInoices(progressiveInvoice.ProgressiveInvoiceInvNo);
        }
        public int DeleteProgressInvoice(ProgressiveInvoice progressiveInvoice, List<AccountsTransactions> accountsTransactions, List<ProgressiveInvoiceDetails> progressiveInvoiceDetails)
        {
            int i = 0;
            try
            {
                _progressiveInvoice.BeginTransaction();

                clsCommonFunctions.Delete_OldEntry_AccountsTransactions(progressiveInvoice.ProgressiveInvoiceInvNo, VoucherType.ProgressiveInvoice_TYPE, _accountTransactionRepository);

                progressiveInvoice.ProgressiveInvoiceDelStatus = true;
                accountsTransactions = accountsTransactions.Select((k) =>
                {
                    k.AccountstransactionsDelStatus = true;
                    return k;
                }).ToList();

                progressiveInvoiceDetails = progressiveInvoiceDetails.Select((x) =>
                {
                    x.ProgressiveInvoiceDetailsDelStatus = true;
                    return x;
                }).ToList();
                _accountTransactionRepository.UpdateList(accountsTransactions);
                _progressiveInvoice.Update(progressiveInvoice);
                var vchrnumer = _voucherNumbersRepository.GetAsQueryable().Where(k => k.VouchersNumbersVNo == progressiveInvoice.ProgressiveInvoiceInvNo).FirstOrDefault();
                _voucherNumbersRepository.Update(vchrnumer);
                _progressiveInvoice.TransactionCommit();
                i = 1;
            }
            catch (Exception ex)
            {
                _progressiveInvoice.TransactionRollback();
                i = 0;
                throw ex;
            }

            return i;
        }
        public ProgressiveInvoice GetSavedProgressiveInoices(string pvno)
        {
            ProgressiveInvoice voucherModel = new ProgressiveInvoice();
            voucherModel = _progressiveInvoice.GetAsQueryable().Where(k => k.ProgressiveInvoiceInvNo == pvno).SingleOrDefault();

            //voucherModel.progressiveInvoiceDetails = _progressiveInvoiceDetails.GetAsQueryable().Where(x => x.)
            return voucherModel;
        }
        public IQueryable GetProgressInvoiceReport()
        {
            try
            {
                var detailData = _progressiveInvoice.GetAsQueryable().Where(a => a.ProgressiveInvoiceDelStatus != true);
                return detailData;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
        public VouchersNumbers GenerateVoucherNo(DateTime? VoucherGenDate)
        {
            try
            {

                var prefix = this._programsettingsRepository.GetAsQueryable().Where(k => k.ProgramSettingsKeyValue == Prefix.ProgressiveInvoice_Prefix).FirstOrDefault().ProgramSettingsTextValue;
                int vnoMaxVal = Convert.ToInt32(_voucherNumbersRepository.GetAsQueryable()
                                                        .Where(x => x.VouchersNumbersVNoNu > 0 && x.VouchersNumbersVType == VoucherType.ProgressiveInvoice_TYPE)
                                                        .DefaultIfEmpty()
                                                        .Max(o => o == null ? 0 : o.VouchersNumbersVNoNu)) + 1;
                //var prefix = "CN";
                //int vnoMaxVal = 1;

                VouchersNumbers vouchersNumbers = new VouchersNumbers
                {
                    VouchersNumbersVNo = prefix + vnoMaxVal,
                    VouchersNumbersVNoNu = vnoMaxVal,
                    VouchersNumbersVType = VoucherType.ProgressiveInvoice_TYPE,
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

        public async Task<Response<ProgressiveInvoiceDto>> GetById(int Id)
        {
            var sql = $@"SELECT CM.Customer_Master_Customer_Name AS CustomerName,
		                        CM.Customer_Master_Customer_No As CustomerNo,JM.Job_Master_Job_Name as JobName, 
		                        JM.Job_Master_Job_id as JobId,PI.Progressive_Invoice_ID AS ProgressiveInvoiceId,
                                PI.Progressive_Invoice_InvNo AS ProgressiveInvoiceInvNo,PI.Progressive_Invoice_Cust_ID AS ProgressiveInvoiceCustId,
                               PI.Progressive_Invoice_Job_ID AS ProgressiveInvoiceJobId,PI.Progressive_Invoice_InvDate AS ProgressiveInvoiceInvDate,
                               PI.Progressive_Invoice_IsVariation AS ProgressiveInvoiceIsVariation,PI.Progressive_Invoice_Invoice_Type AS ProgressiveInvoiceInvoiceType,
                               PI.Progressive_Invoice_Reference AS ProgressiveInvoiceReference,PI.Progressive_Invoice_TotAmount AS ProgressiveInvoiceTotAmount,
                               PI.Progressive_Invoice_Status AS ProgressiveInvoiceStatus,PI.Progressive_Invoice_Disc_Percentage AS ProgressiveInvoiceDiscPercentage,
                               PI.Progressive_Invoice_Discount AS ProgressiveInvoiceDiscount,PI.Progressive_Invoice_PrintInv AS ProgressiveInvoicePrintInv,
                               PI.Progressive_Invoice_Location_ID AS ProgressiveInvoiceLocationId,PI.Progressive_Invoice_PayAppNo AS ProgressiveInvoicePayAppNo,
                               PI.Progressive_Invoice_Description AS ProgressiveInvoiceDescription,PI.Progressive_Invoice_Progress AS ProgressiveInvoiceProgress,
                               PI.Progressive_Invoice_Cur_Compl_Val AS ProgressiveInvoiceCurComplVal,PI.Progressive_Invoice_Cur_Compl_Per AS ProgressiveInvoiceCurComplPer,
                               PI.Progressive_Invoice_Prev_Compl_Val AS ProgressiveInvoicePrevComplVal,PI.Progressive_Invoice_Prev_Compl_Per AS ProgressiveInvoicePrevComplPer,
                               PI.Progressive_Invoice_Cumilative_Val AS ProgressiveInvoiceCumilativeVal,PI.Progressive_Invoice_Cumilative_Per AS ProgressiveInvoiceCumilativePer,
                               PI.Progressive_Invoice_AmountDue AS ProgressiveInvoiceAmountDue,PI.Progressive_Invoice_Total AS ProgressiveInvoiceTotal,
                               PI.Progressive_Invoice_FSNO AS ProgressiveInvoiceFsno,PI.Progressive_Invoice_User_ID AS ProgressiveInvoiceUserId,
                               PI.Progressive_Invoice_Description_ID AS ProgressiveInvoiceDescriptionId,PI.Progressive_Invoice_Pl_Retention AS ProgressiveInvoicePlRetention,
                               PI.Progressive_Invoice_Pl_Total_Amount AS ProgressiveInvoicePlTotalAmount,PI.Progressive_Invoice_DelStatus AS ProgressiveInvoiceDelStatus
                              FROM Progressive_Invoice PI
                              left join Customer_Master CM on CM.Customer_Master_Customer_No = PI.Progressive_Invoice_Cust_ID
                              left join Job_Master JM on JM.Job_Master_Job_id = PI.Progressive_Invoice_Job_id   Where PI.Progressive_Invoice_ID = {Id}";
            var progressiveInvoiceResult = await _dbConnection.QueryFirstOrDefaultAsync<ProgressiveInvoiceDto>(sql);
            progressiveInvoiceResult.VariaAmount = await _dbConnection.QueryFirstOrDefaultAsync<VariationAmount>($"Select ISNULL(Job_Master_job_value,0) as JobAmount,ISNULL(Job_Master_Job_Advance_Amount,0) AS AdvAmount,ISNULL(Job_Master_Job_AdvAmountOnVar,0) AS AdvAmountOnVar From Job_Master Where Job_Master_Job_id ={progressiveInvoiceResult.JobId}");

            var dateFilter = progressiveInvoiceResult.ProgressiveInvoiceInvDate.HasValue ? $" and Progressive_Invoice_InvDate < '{progressiveInvoiceResult.ProgressiveInvoiceInvDate.Value.ToString("dd-MMM-yyyy")}'" : "";
            var jobSummaryWhere = progressiveInvoiceResult.ProgressiveInvoiceIsVariation ?? false  ? $" and ISNULL(Progressive_Invoice_IsVariation,0) = 1  {dateFilter}" : $" and ISNULL(Progressive_Invoice_IsVariation,0) = 0 {dateFilter}";
            
            progressiveInvoiceResult.PreviousSummary = await _dbConnection.QueryFirstOrDefaultAsync<JobSummary>($"Select ISNULL(sum(Progressive_Invoice_Cur_Compl_Per),0) as TotalCompletedPer,ISNULL(Sum(Progressive_Invoice_Cur_Compl_Val),0) as TotalCompletedValue,ISNULL(SUM(Progressive_Invoice_RetAmount),0) AS TotalRetentionAmount,ISNULL(SUM(Progressive_Invoice_VariationAmount),0) AS TotalVariationAmount,ISNULL(SUM(Progressive_Invoice_AdvanceDeduction),0) AS TotalAdvanceAmount From  Progressive_Invoice Where Progressive_Invoice_Job_id = {progressiveInvoiceResult.JobId} {jobSummaryWhere}");
            progressiveInvoiceResult.TotalCompletedValue = await _dbConnection.QueryFirstOrDefaultAsync<double>($"Select ISNULL(Sum(Progressive_Invoice_Cur_Compl_Val),0) as TotalCompletedValue From  Progressive_Invoice Where Progressive_Invoice_Job_id = {progressiveInvoiceResult.JobId} {dateFilter} and ISNULL(Progressive_Invoice_IsVariation,0) = 0"); //'2025-04-18'
            progressiveInvoiceResult.PrgInvoiceDetails = await _dbConnection.QueryAsync<PrgInvoiceDetail>($"SELECT ISNULL(Progressive_Invoice_InvNo,0) AS ProgressiveInvoiceInvNo,ISNULL(Progressive_Invoice_Progress,0) AS ProgressiveInvoiceProgress,ISNULL(Progressive_Invoice_Cur_Compl_Val,0) AS ProgressiveInvoiceCurComplVal,ISNULL(Progressive_Invoice_Cur_Compl_Per,0) AS ProgressiveInvoiceCurComplPer,ISNULL(Progressive_Invoice_Prev_Compl_Val,0) AS ProgressiveInvoicePrevComplVal,ISNULL(Progressive_Invoice_Prev_Compl_Per,0) AS ProgressiveInvoicePrevComplPer,ISNULL(Progressive_Invoice_Cumilative_Val,0) AS ProgressiveInvoiceCumilativeVal,ISNULL(Progressive_Invoice_Cumilative_Per,0) AS ProgressiveInvoiceCumilativePer,ISNULL(Progressive_Invoice_VariationAmount,0) AS ProgressiveInvoiceVariationAmount,ISNULL(Progressive_Invoice_RetAmount,0) AS ProgressiveInvoiceRetAmount,CASE WHEN ISNULL(Progressive_Invoice_IsVariation,0) = 1 then 'Yes' else 'NO' end AS VariationInvoice FROM Progressive_Invoice  Where Progressive_Invoice_Job_id = {progressiveInvoiceResult.JobId} ");
            return Response<ProgressiveInvoiceDto>.Success(progressiveInvoiceResult, "Data found");
        }
    }
}
