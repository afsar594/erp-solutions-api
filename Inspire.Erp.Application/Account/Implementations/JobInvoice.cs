using Inspire.Erp.Application.Account.Interface;
using Inspire.Erp.Application.Common;
using Inspire.Erp.Application.MODULE;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Enums;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inspire.Erp.Application.Account.implementations
{
    public class jobinvoiceimp : IJobInvoice
    {
        private readonly IRepository<JobInvoiceRequest> _jobinvoiceresponserepo;
        private readonly IRepository<JobInvoice> _jobinvoicerepo;
        private readonly IRepository<JobInvoiceRequest> _jobresponserepo;
        private readonly IRepository<JobInvoiceDetails> _jobdetailsrepo;
        private readonly IRepository<VouchersNumbers> _vouchersnumbersrepo;
        private readonly IRepository<MasterAccountsTable> _masteraccounttablerepo;
        private readonly IRepository<AccountsTransactions> _accounttablerepo;
        private readonly IRepository<ItemMaster> _itemmasterrepo;
        private readonly IRepository<CustomerMaster> _cmrepo;
        private readonly IRepository<LocationMaster> _lmrepo;
        private readonly IRepository<ProgramSettings> _programsettingsrepository;
        private readonly IRepository<CostCenterMaster> _costCenterMaster;
        private readonly IRepository<CustomerMaster> _customerMaster;
        private readonly IRepository<LocationMaster> _locationMaster;
        private IRepository<ProgramSettings> _programsettingsRepository;
        private IRepository<VouchersNumbers> _voucherNumbersRepository;
        private IRepository<AccountsTransactions> _accountTransactionRepository;
        private static string prefix;
        public jobinvoiceimp(IRepository<ItemMaster> itemmasterrepo,
               IRepository<JobInvoiceRequest> jobinvoiceresponserepo,
                IRepository<MasterAccountsTable> masteraccounttablerepo,
                IRepository<AccountsTransactions> accounttablerepo,
                IRepository<JobInvoice> jobinvoicerepo,
                IRepository<StockRegister> stockregisterrepo,
                IRepository<JobInvoiceRequest> jobresponserepo,
                IRepository<JobInvoiceDetails> jobdetailsrepo,
                IRepository<VouchersNumbers> vouchersnumbersrepo,
                IRepository<CustomerMaster> cmrepo,
                IRepository<LocationMaster> lmrepo,
                IRepository<ProgramSettings> programsettingsrepository,
                IRepository<CostCenterMaster> costCenterMaster,
                IRepository<CostCenterMaster> customerMaster, IRepository<AccountsTransactions> accountTransactionRepository,
                IRepository<ProgramSettings> programsettingsRepository,
                IRepository<VouchersNumbers> voucherNumbers
            )
        {
            _lmrepo = lmrepo;
            _cmrepo = cmrepo;
            _itemmasterrepo = itemmasterrepo;
            _accounttablerepo = accounttablerepo;
            _jobinvoiceresponserepo = jobinvoiceresponserepo;
            _masteraccounttablerepo = masteraccounttablerepo;
            _programsettingsrepository = programsettingsrepository;
            _jobinvoicerepo = jobinvoicerepo;
            _jobresponserepo = jobresponserepo;
            _jobdetailsrepo = jobdetailsrepo;
            _vouchersnumbersrepo = vouchersnumbersrepo;
            _programsettingsrepository = programsettingsrepository;
            _costCenterMaster = costCenterMaster;
            _costCenterMaster = customerMaster; _accountTransactionRepository = accountTransactionRepository;
            _costCenterMaster = costCenterMaster;
            _programsettingsRepository = programsettingsRepository;
            _voucherNumbersRepository = voucherNumbers;
        }

        public async Task<JobInvoiceModel> InsertJobInvoice(JobInvoice JobInvoice, List<JobInvoiceDetails> jobInvoiceDetails, List<AccountsTransactions> accountsTransactions)
        {
            try
            {
                string openingstockVoucherNumber = clsCommonFunctions.GenrateVoucherNo(VoucherType.JobInoviceVoucher_TYPE, Prefix.JobInoviceVoucher_Prefix, _programsettingsRepository, _voucherNumbersRepository);


                //string openingstockVoucherNumber = this.GenerateVoucherNo(JobInvoice.JobInvoiceVoucherDate.Value).VouchersNumbersVNo;
                JobInvoice.JobInvoiceVoucherNo = openingstockVoucherNumber;

                _jobinvoicerepo.BeginTransaction();
                int maxcount = 0;
                maxcount = Convert.ToInt32(
                    _jobinvoicerepo.GetAsQueryable()
                    .DefaultIfEmpty().Max(o => o == null ? 0 : o.JobInvoiceId) + 1);
                JobInvoice.JobInvoiceId = maxcount;

                _jobinvoicerepo.Insert(JobInvoice);

                int maxcount1 = 0;
                maxcount1 = Convert.ToInt32(
                    _jobdetailsrepo.GetAsQueryable()
                    .DefaultIfEmpty().Max(o => o == null ? 0 : o.JobInvoiceDetailsDetailsId) + 1);
                JobInvoice.JobInvoiceDetails.ForEach(elemt =>
                {
                    elemt.JobInvoiceDetailsDetailsId = maxcount1;
                    elemt.JobInvoiceDetailsVoucherNo = JobInvoice.JobInvoiceVoucherNo;
                    maxcount1++;
                });
                _jobdetailsrepo.InsertList(JobInvoice.JobInvoiceDetails);

                accountsTransactions = accountsTransactions.Select((k) =>
                {
                    k.AccountsTransactionsVoucherNo = openingstockVoucherNumber;
                    k.AccountsTransactionsAccNo = k.AccountsTransactionsAccNo ?? string.Empty;
                    k.AccountsTransactionsVoucherType = VoucherType.JobInoviceVoucher_TYPE;
                    k.AccountsTransactionsStatus = AccountStatus.Approved;
                    k.AccountsTransactionsTransDate = JobInvoice.JobInvoiceVoucherDate;
                    k.AccountsTransactionsLocation = JobInvoice.JobInvoiceLocationId ?? 0;
                    return k;
                }).OrderBy(x => x.AccountsTransactionsVoucherNo).AsEnumerable().ToList();
                _accountTransactionRepository.InsertList(accountsTransactions);




                _jobinvoicerepo.TransactionCommit();
                return this.GetJobInvoiceVoucherNo((int)JobInvoice.JobInvoiceId);
            }
            catch (Exception ex)
            {
                _jobinvoicerepo.TransactionRollback();
                throw ex;
            }
        }
        public async Task<JobInvoiceModel>  UpdateJobInvoice(JobInvoice JobInvoice, List<JobInvoiceDetails> jobInvoiceDetails, List<AccountsTransactions> accountsTransactions)
        {
            try
            {
                _jobinvoicerepo.BeginTransaction();
                //=================================
                clsCommonFunctions.Delete_OldEntry_AccountsTransactions(JobInvoice.JobInvoiceVoucherNo, VoucherType.JobInoviceVoucher_TYPE, _accountTransactionRepository);
                //=================================                

                int maxcount1 = 0;
                maxcount1 = Convert.ToInt32(
                    _jobdetailsrepo.GetAsQueryable()
                    .DefaultIfEmpty().Max(o => o == null ? 0 : o.JobInvoiceDetailsDetailsId) + 1);

                foreach (var item in JobInvoice.JobInvoiceDetails)
                    if (item.JobInvoiceDetailsDetailsId == 0)
                    {
                        item.JobInvoiceDetailsVoucherNo = JobInvoice.JobInvoiceVoucherNo;
                        item.JobInvoiceDetailsDetailsId = maxcount1;
                        _jobdetailsrepo.Insert(item);
                    }
                    else
                    {
                        _jobdetailsrepo.Update(item);
                    }
                _jobinvoicerepo.Update(JobInvoice);


                accountsTransactions = accountsTransactions.Select((k) =>
                {
                    k.AccountsTransactionsVoucherNo = JobInvoice.JobInvoiceVoucherNo;
                    k.AccountsTransactionsAccNo = k.AccountsTransactionsAccNo ?? string.Empty;
                    k.AccountsTransactionsVoucherType = VoucherType.JobInoviceVoucher_TYPE;
                    k.AccountsTransactionsStatus = AccountStatus.Approved;
                    k.AccountsTransactionsTransDate = JobInvoice.JobInvoiceVoucherDate;
                    k.AccountsTransactionsLocation = JobInvoice.JobInvoiceLocationId ?? 0;
                    return k;
                }).OrderBy(x => x.AccountsTransactionsVoucherNo).AsEnumerable().ToList();
                _accountTransactionRepository.InsertList(accountsTransactions);
                // _jobdetailsrepo.UpdateList(JobInvoice.JobInvoiceDetails);
                _jobinvoicerepo.TransactionCommit();
                return this.GetJobInvoiceVoucherNo((int)JobInvoice.JobInvoiceId);
            }
            catch (Exception ex)
            {
                _jobinvoicerepo.TransactionRollback();
                throw ex;
            }
        }
        public IEnumerable<JobInvoice> DeleteJobInvoice(string Id)
        {
            try
            {
                var dataObj = _jobinvoicerepo.GetAsQueryable().Where(o => o.JobInvoiceVoucherNo == Id).FirstOrDefault();
                dataObj.JobInvoiceDelStatus = true;
                var detailData = _jobdetailsrepo.GetAsQueryable().Where(o => o.JobInvoiceDetailsVoucherNo == Id).ToList();
                detailData.ForEach(elemt =>
                {
                    elemt.JobInvoiceDetailsDelStatus = true;
                });
                _jobinvoicerepo.Update(dataObj);
                _jobdetailsrepo.UpdateList(detailData);
                return _jobinvoicerepo.GetAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<JobInvoice> Getjobinvoice()
        {
            try
            {
                var data = _jobinvoicerepo.GetAll().Where(a => a.JobInvoiceDelStatus != true).OrderBy(c => c.JobInvoiceId).AsEnumerable().ToList();
                return data;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public JobInvoice GetJobInvoiceById(int? id)
        {
            try
            {
                var JobInvoice = new JobInvoice();
                JobInvoice = _jobinvoicerepo.GetAsQueryable().FirstOrDefault(x => x.JobInvoiceId == Convert.ToInt32(id));
                JobInvoice.JobInvoiceDetails = _jobdetailsrepo.GetAsQueryable().Where(x => x.JobInvoiceDetailsDetailsId == Convert.ToInt32(id) && x.JobInvoiceDetailsDelStatus != true).ToList();
                return JobInvoice;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public JobInvoiceModel GetJobInvoiceVoucherNo(int VNo)
        {
            try
            {

                JobInvoiceModel xs = new JobInvoiceModel();
                xs.JobInvoice = _jobinvoicerepo.GetAsQueryable().Where(k => k.JobInvoiceId == VNo).FirstOrDefault();
                xs.JobInvoiceDetails = _jobdetailsrepo.GetAsQueryable().Where(x => x.JobInvoiceDetailsVoucherNo == xs.JobInvoice.JobInvoiceVoucherNo && (x.JobInvoiceDetailsDelStatus == false || x.JobInvoiceDetailsDelStatus == null)).ToList();
                xs.accountsTransactions = _accountTransactionRepository.GetAsQueryable().Where(x => x.AccountsTransactionsVoucherNo == xs.JobInvoice.JobInvoiceVoucherNo && x.AccountstransactionsDelStatus != true).ToList();
                return xs;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public VouchersNumbers GenerateVoucherNo(DateTime? VoucherGenDate)
        {
            try
            {
                var prefix = this._programsettingsRepository.GetAsQueryable().Where(k => k.ProgramSettingsKeyValue == Prefix.JobInoviceVoucher_Prefix).FirstOrDefault().ProgramSettingsTextValue;
                int vnoMaxVal = Convert.ToInt32(_voucherNumbersRepository.GetAsQueryable()
                                                        .Where(x => x.VouchersNumbersVNoNu > 0 && x.VouchersNumbersVType == VoucherType.JobInoviceVoucher_TYPE)
                                                        .DefaultIfEmpty()
                                                        .Max(o => o == null ? 0 : o.VouchersNumbersVNoNu)) + 1;

                VouchersNumbers vouchersNumbers = new VouchersNumbers
                {
                    VouchersNumbersVNo = prefix + vnoMaxVal,
                    VouchersNumbersVNoNu = vnoMaxVal,
                    VouchersNumbersVType = VoucherType.JobInoviceVoucher_TYPE,
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

    }
}
