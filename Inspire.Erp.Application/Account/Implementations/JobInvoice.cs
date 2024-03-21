using Inspire.Erp.Application.Common;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inspire.Erp.Application.Account.Interfaces;
using System.IO.Enumeration;
using Microsoft.EntityFrameworkCore;
using Inspire.Erp.Application.Master;
using Inspire.Erp.Domain.Enums;
using Inspire.Erp.Application.Sales.Interface;
using Inspire.Erp.Application.Account.Interface;
using Inspire.Erp.Domain.Modals;

namespace Inspire.Erp.Application.Account.implementations
{
    public class jobinvoiceimp: IJobInvoice
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
                IRepository<CostCenterMaster> customerMaster,
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
            _costCenterMaster = customerMaster;
            _costCenterMaster = costCenterMaster;
            _programsettingsRepository = programsettingsRepository;
            _voucherNumbersRepository = voucherNumbers;
        }

       public JobInvoice InsertJobInvoice(JobInvoice JobInvoice)
        {
            try
            {
                

                string openingstockVoucherNumber = this.GenerateVoucherNo(JobInvoice.JobInvoiceVoucherDate.Value).VouchersNumbersVNo;
                JobInvoice.JobInvoiceVoucherNo = openingstockVoucherNumber;
              
                _jobinvoicerepo.BeginTransaction();
                int maxcount = 0;
                maxcount = Convert.ToInt32(
                    _jobinvoicerepo.GetAsQueryable()
                    .DefaultIfEmpty().Max(o => o == null ? 0 : o.JobInvoiceId) + 1);
                JobInvoice.JobInvoiceId = maxcount;

                _jobinvoicerepo.Insert(JobInvoice);
                JobInvoice.JobInvoiceDetails.ForEach(elemt =>
                {
                    elemt.JobInvoiceDetailsDetailsId = maxcount;
                    elemt.JobInvoiceDetailsVoucherNo = JobInvoice.JobInvoiceVoucherNo;
                });
                _jobdetailsrepo.InsertList(JobInvoice.JobInvoiceDetails);               
                _jobinvoicerepo.TransactionCommit();
                return this.GetJobInvoiceById(JobInvoice.JobInvoiceId);
            }
            catch (Exception ex)
            {
                _jobinvoicerepo.TransactionRollback();
                throw ex;
            }
        }
        public JobInvoice UpdateJobInvoice(JobInvoice JobInvoice)
        {
            try
            {
                _jobinvoicerepo.BeginTransaction();
                _jobinvoicerepo.Update(JobInvoice);
                _jobdetailsrepo.UpdateList(JobInvoice.JobInvoiceDetails);
                _jobinvoicerepo.TransactionCommit();
                return this.GetJobInvoiceById(JobInvoice.JobInvoiceId);
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
                var detailList = _jobdetailsrepo.GetAll().ToList();
                var data = _jobinvoicerepo.GetAll().Select(o => new
                {
                    JobInvoice = o,
                    JobInvoiceDetails = detailList.Where(a => Convert.ToInt32(a.JobInvoiceDetailsDetailsId) == Convert.ToInt32(o.JobInvoiceId)).ToList()
                }).ToList();
                return _jobinvoicerepo.GetAll();

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
                JobInvoice.JobInvoiceDetails =_jobdetailsrepo.GetAsQueryable().Where(x => x.JobInvoiceDetailsDetailsId == Convert.ToInt32(id)).ToList();
                return JobInvoice;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public JobInvoice GetJobInvoiceVoucherNo(string VNo)
        {
            try
            {
             
                JobInvoice JobInvoice = new JobInvoice();
                JobInvoice = _jobinvoicerepo.GetAsQueryable().Where(k => k.JobInvoiceVoucherNo == VNo).SingleOrDefault();
                JobInvoice.JobInvoiceDetails = _jobdetailsrepo.GetAsQueryable().Where(x => x.JobInvoiceDetailsVoucherNo == VNo && (x.JobInvoiceDetailsDelStatus == false || x.JobInvoiceDetailsDelStatus == null)).ToList();
                return JobInvoice;
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


                //var prefix = "CN";
                //int vnoMaxVal = 1;


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
