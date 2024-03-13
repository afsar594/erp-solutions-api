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
        private IRepository<ProgramSettings> _programsettingsrepository;
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
            IRepository<ProgramSettings> programsettingsrepository)
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
            //prefix = this._programsettingsrepository.GetAsQueryable().Where(k => k.ProgramSettingsKeyValue == "hide_jobinvoice").FirstOrDefault().ProgramSettingsKeyValue;
        }
        
        public async Task<bool> Insertjobinvoice(JobInvoiceRequest obj)
        {
            try
            {
                _jobinvoicerepo.InsertList(obj.ji);
                _vouchersnumbersrepo.InsertList(obj.vn);
                _accounttablerepo.InsertList(obj.at);
                _jobdetailsrepo.InsertList(obj.jid);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<bool> Updatejobinvoice(JobInvoiceRequest obj)
        {
            try
            {
                JobInvoice ji = _jobinvoicerepo.GetAsQueryable().Where(k => k.JobInvoiceVoucherNo == obj.vno).SingleOrDefault();
                if (ji == null)
                {
                    return false;
                }

                ji.JobInvoiceDiscount = double.Parse(obj.discount);
                ji.JobInvoiceGrossAmount = double.Parse(obj.grossAmt);
                ji.JobInvoiceNetAmount = double.Parse(obj.netAmt);
                _jobinvoicerepo.Update(ji);
                JobInvoiceDetails jid = _jobdetailsrepo.GetAsQueryable().Where(k => k.JobInvoiceDetailsVoucherNo == obj.vno).SingleOrDefault();
                if (jid == null)
                {
                    return false;
                }
                jid.JobInvoiceDetailsSoldQty = double.Parse(obj.qty);
                jid.JobInvoiceDetailsDiscount = double.Parse(obj.discount);
                jid.JobInvoiceDetailsGrossAmount = double.Parse(obj.grossAmt);
                jid.JobInvoiceDetailsNetAmount = double.Parse(obj.netAmt);
                _jobdetailsrepo.Update(jid);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<bool> DeleteJobInvoice(JobInvoiceRequest obj)
        {
            try
            {
                //int vno = int.parse(obj.vno??"0");

                AccountsTransactions at = _accounttablerepo.GetAsQueryable().Where(k => k.AccountsTransactionsVoucherNo == obj.vno).SingleOrDefault();
                if (at == null)
                {
                    return false;
                }
                at.AccountstransactionsDelStatus = true;
                _accounttablerepo.Update(at);
                JobInvoice ji = _jobinvoicerepo.GetAsQueryable().Where(k => k.JobInvoiceVoucherNo == obj.vno).SingleOrDefault();
                if (at == null)
                {
                    return false;
                }
                ji.JobInvoiceDelStatus = true;
                _jobinvoicerepo.Update(ji);
                JobInvoiceDetails jid = _jobdetailsrepo.GetAsQueryable().Where(k => k.JobInvoiceDetailsVoucherNo == obj.vno).SingleOrDefault();
                if (at == null)
                {
                    return false;
                }
                jid.JobInvoiceDetailsDelStatus = true;
                _jobdetailsrepo.Update(jid);
                //stockregister sr = _stockregisterrepo.getasqueryable().where(k => k.stockregisterrefvoucherno == obj.vnonu).singleordefault();
                //if (at == null)
                //{
                //    return false;
                //}
                //sr.stockregisterdelstatus = true;
                //_stockregisterrepo.update(sr);
                VouchersNumbers vn = _vouchersnumbersrepo.GetAsQueryable().Where(k => k.VouchersNumbersVNo == obj.vno).SingleOrDefault();
                if (at == null)
                {
                    return false;
                }
                vn.VouhersNumbersDelStatus = true;
                _vouchersnumbersrepo.Update(vn);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<List<MasterAccountsTable>> Getsalesaccount()
        {
            try
            {
                var x = _masteraccounttablerepo.GetAsQueryable().Where(k => k.MaAccType == "a" && k.MaMainHead == "income").ToList();
                return x;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<List<JobInvoiceModel>> Getjobinvoice()
        {
            try
            {
                var data = (from ji in _jobinvoicerepo.GetAsQueryable()
                            join jid in _jobdetailsrepo.GetAsQueryable()
                            on ji.JobInvoiceVoucherNo equals jid.JobInvoiceDetailsVoucherNo
                            join itm in _itemmasterrepo.GetAsQueryable()
                            on jid.JobInvoiceDetailsItemId equals Convert.ToInt32(itm.ItemMasterItemId)

                            where (ji.JobInvoiceDelStatus ?? false) == false
                            select new JobInvoiceModel
                            {
                                Job_Invoice_Voucher_Date = ji.JobInvoiceVoucherDate,
                                Job_Invoice_Voucher_No = ji.JobInvoiceVoucherNo,
                                Job_Invoice_Customer_Name =
                                (from cm in _cmrepo.GetAsQueryable()
                                 where (cm.CustomerMasterCustomerUserId == ji.JobInvoiceCustomerId)
                                 select cm.CustomerMasterCustomerName
                                 ).ToString(),
                                location =
                                (from lm in _lmrepo.GetAsQueryable()
                                 where (lm.LocationMasterLocationId == ji.JobInvoiceLocationId)
                                 select lm.LocationMasterLocationName
                                 ).ToString(),
                                Job_Invoice_Discount = jid.JobInvoiceDetailsDiscount,
                                Job_Invoice_Gross_Amount = jid.JobInvoiceDetailsGrossAmount,
                                Job_Invoice_Net_Amount = jid.JobInvoiceDetailsNetAmount,
                                Job_Invoice_Details_Unit_ID = jid.JobInvoiceDetailsUnitId,
                                Job_Invoice_Details_Unit = jid.JobInvoiceDetailsUnit,
                                Item_Name = itm.ItemMasterItemName,
                                Job_Invoice_Details_Unit_Price = jid.JobInvoiceDetailsUnitPrice,
                                Job_Invoice_Details_Sold_Qty = jid.JobInvoiceDetailsSoldQty,
                                Job_Invoice_Details_Item_ID = jid.JobInvoiceDetailsItemId
                            }).ToList();

               // var response = _jobinvoiceresponserepo.GetBySP($"getjobinvoice").ToList();
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Task<List<MasterAccountsTable>> GetSalesAccount()
        {
            throw new NotImplementedException();
        }

        public Task<List<JobInvoiceModel>> GetJobInvoice()
        {
            throw new NotImplementedException();
        }

        public Task<bool> InsertJobInvoice(JobInvoiceRequest obj)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateJobInvoice(JobInvoiceRequest obj)
        {
            throw new NotImplementedException();
        }
    }
}
