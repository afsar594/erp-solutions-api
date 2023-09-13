////using Inspire.Erp.Application.Account.Interfaces;
////using Inspire.Erp.Application.Common;
////using Inspire.Erp.Domain.Entities;
////using Inspire.Erp.Domain.Modals;
////using Inspire.Erp.Infrastructure.Database.Repositoy;
////using System;
////using System.Collections.Generic;
////using System.Linq;
////using System.Text;
////using System.Threading.Tasks;

////namespace Inspire.Erp.Application.Account.Implementations
////{
////    public class JobInvoiceImp : IJobInvoice
////    {
////        private readonly IRepository<JobInvoiceResponse> _JobInvoiceResponseRepo;
////        private readonly IRepository<JobInvoice> _jobInvoiceRepo;
////        private readonly IRepository<JobInvoiceResponse> _jobResponseRepo;
////        private readonly IRepository<JobInvoiceDetails> _jobDetailsRepo;
////        private readonly IRepository<VouchersNumbers> _vouchersNumbersRepo;
////        private readonly IRepository<MasterAccountsTable> _MasterAccountTableRepo;
////        private readonly IRepository<AccountsTransactions> _AccountTableRepo;
////        private readonly IRepository<StockRegister> _stockRegisterRepo;
////        private readonly IRepository<ItemMaster> _itemMasterRepo;
////        private readonly IRepository<CustomerMaster> _cmRepo;
////        private readonly IRepository<LocationMaster> _lmRepo;
////        private IRepository<ProgramSettings> _programsettingsRepository;
////        private static string prefix;
////        public JobInvoiceImp(IRepository<ItemMaster> itemMasterRepo,
////               IRepository<JobInvoiceResponse> JobInvoiceResponseRepo,
////            IRepository<MasterAccountsTable> MasterAccountTableRepo,
////            IRepository<AccountsTransactions> AccountTableRepo,
////            IRepository<JobInvoice> jobInvoiceRepo,
////            IRepository<StockRegister> stockRegisterRepo,
////            IRepository<JobInvoiceResponse> jobResponseRepo,
////            IRepository<JobInvoiceDetails> jobDetailsRepo,
////            IRepository<VouchersNumbers> vouchersNumbersRepo,
////            IRepository<CustomerMaster> cmRepo,
////            IRepository<LocationMaster> lmRepo,
////            IRepository<ProgramSettings> programsettingsRepository)
////        {
////            _lmRepo = lmRepo;
////            _cmRepo = cmRepo;
////            _itemMasterRepo = itemMasterRepo;
////            _AccountTableRepo = AccountTableRepo;
////            _JobInvoiceResponseRepo = JobInvoiceResponseRepo;
////            _MasterAccountTableRepo = MasterAccountTableRepo;
////            _programsettingsRepository = programsettingsRepository;
////            _jobInvoiceRepo = jobInvoiceRepo;
////            _jobResponseRepo = jobResponseRepo;
////            _jobDetailsRepo = jobDetailsRepo;
////            _vouchersNumbersRepo = vouchersNumbersRepo;
////            _programsettingsRepository = programsettingsRepository;
////            prefix = this._programsettingsRepository.GetAsQueryable().Where(k => k.ProgramSettingsKeyValue == "HIDE_JOBINVOICE").FirstOrDefault().ProgramSettingsTextValue;
////        }
////        public async Task<bool> InsertJobInvoice(JobInvoiceRequest obj)
////        {
////            try
////            {
////                _jobInvoiceRepo.InsertList(obj.ji);
////                _vouchersNumbersRepo.InsertList(obj.vn);
////                _AccountTableRepo.InsertList(obj.at);
////                _jobDetailsRepo.InsertList(obj.jid);
////                return true;
////            }
////            catch (Exception ex)
////            {
////                throw ex;
////            }
////        }
////        public async Task<bool> UpdateJobInvoice(JobInvoiceRequest obj)
////        {
////            try
////            {
////                JobInvoice ji = _jobInvoiceRepo.GetAsQueryable().Where(k => k.JobInvoiceVoucherNo == obj.vno).SingleOrDefault();
////                if (ji == null)
////                {
////                    return false;
////                }

////                ji.JobInvoiceDiscount = double.Parse(obj.discount);
////                ji.JobInvoiceGrossAmount = double.Parse(obj.grossAmt);
////                ji.JobInvoiceNetAmount = double.Parse(obj.netAmt);
////                _jobInvoiceRepo.Update(ji);
////                JobInvoiceDetails jid = _jobDetailsRepo.GetAsQueryable().Where(k => k.JobInvoiceDetailsVoucherNo == obj.vno).SingleOrDefault();
////                if (jid == null)
////                {
////                    return false;
////                }
////                jid.JobInvoiceDetailsSoldQty = double.Parse(obj.qty);
////                jid.JobInvoiceDetailsDiscount = double.Parse(obj.discount);
////                jid.JobInvoiceDetailsGrossAmount = double.Parse(obj.grossAmt);
////                jid.JobInvoiceDetailsNetAmount = double.Parse(obj.netAmt);
////                _jobDetailsRepo.Update(jid);
////                return true;
////            }
////            catch (Exception ex)
////            {
////                throw ex;
////            }
////        }
////        public async Task<bool> DeleteJobInvoice(JobInvoiceRequest obj)
////        {
////            try
////            {
////                //int vno = int.Parse(obj.vno??"0");

////                AccountsTransactions at = _AccountTableRepo.GetAsQueryable().Where(k => k.AccountsTransactionsVoucherNo == obj.vno).SingleOrDefault();
////                if (at == null)
////                {
////                    return false;
////                }
////                at.AccountstransactionsDelStatus = true;
////                _AccountTableRepo.Update(at);
////                JobInvoice ji = _jobInvoiceRepo.GetAsQueryable().Where(k => k.JobInvoiceVoucherNo == obj.vno).SingleOrDefault();
////                if (at == null)
////                {
////                    return false;
////                }
////                ji.JobInvoiceDelStatus = true;
////                _jobInvoiceRepo.Update(ji);
////                JobInvoiceDetails jid = _jobDetailsRepo.GetAsQueryable().Where(k => k.JobInvoiceDetailsVoucherNo == obj.vno).SingleOrDefault();
////                if (at == null)
////                {
////                    return false;
////                }
////                jid.JobInvoiceDetailsDelStatus = true;
////                _jobDetailsRepo.Update(jid);
////                //StockRegister sr = _stockRegisterRepo.GetAsQueryable().Where(k => k.StockRegisterRefVoucherNo == obj.vnonu).SingleOrDefault();
////                //if (at == null)
////                //{
////                //    return false;
////                //}
////                //sr.StockRegisterDelStatus = true;
////                //_stockRegisterRepo.Update(sr);
////                VouchersNumbers vn = _vouchersNumbersRepo.GetAsQueryable().Where(k => k.VouchersNumbersVNo == obj.vno).SingleOrDefault();
////                if (at == null)
////                {
////                    return false;
////                }
////                vn.VouhersNumbersDelStatus = true;
////                _vouchersNumbersRepo.Update(vn);
////                return true;
////            }
////            catch (Exception ex)
////            {
////                throw ex;
////            }
////        }
////        public async Task<List<MasterAccountsTable>> GetSalesAccount()
////        {
////            try
////            {
////                var x = _MasterAccountTableRepo.GetAsQueryable().Where(k => k.MaAccType == "A" && k.MaMainHead == "Income").ToList();
////                return x;
////            }
////            catch (Exception ex)
////            {
////                throw ex;
////            }
////        }
////        public async Task<List<JobInvoiceModel>> GetJobInvoice()
////        {
////            try
////            {
////                var data = (from ji in _jobInvoiceRepo.GetAsQueryable()
////                            join jid in _jobDetailsRepo.GetAsQueryable()
////                            on ji.JobInvoiceVoucherNo equals jid.JobInvoiceDetailsVoucherNo
////                            join itm in _itemMasterRepo.GetAsQueryable()
////                            on jid.JobInvoiceDetailsItemId equals itm.ItemMasterItemId
////                            where (ji.JobInvoiceDelStatus ?? false) == false
////                            select new JobInvoiceModel
////                            {
////                                Job_Invoice_Voucher_Date = ji.JobInvoiceVoucherDate,
////                                Job_Invoice_Voucher_No = ji.JobInvoiceVoucherNo,
////                                Job_Invoice_Customer_Name =
////                                (from cm in _cmRepo.GetAsQueryable()
////                                 where (cm.CustomerMasterCustomerUserId == ji.JobInvoiceCustomerId)
////                                 select cm.CustomerMasterCustomerName
////                                 ).ToString(),
////                                location =
////                                (from lm in _lmRepo.GetAsQueryable()
////                                 where (lm.LocationMasterLocationId == ji.JobInvoiceLocationId)
////                                 select lm.LocationMasterLocationName
////                                 ).ToString(),
////                                Job_Invoice_Discount = jid.JobInvoiceDetailsDiscount,
////                                Job_Invoice_Gross_Amount = jid.JobInvoiceDetailsGrossAmount,
////                                Job_Invoice_Net_Amount = jid.JobInvoiceDetailsNetAmount,
////                                Job_Invoice_Details_Unit_ID = jid.JobInvoiceDetailsUnitId,
////                                Job_Invoice_Details_Unit = jid.JobInvoiceDetailsUnit,
////                                Item_Name = itm.ItemMasterItemName,
////                                Job_Invoice_Details_Unit_Price = jid.JobInvoiceDetailsUnitPrice,
////                                Job_Invoice_Details_Sold_Qty = jid.JobInvoiceDetailsSoldQty,
////                                Job_Invoice_Details_Item_ID = jid.JobInvoiceDetailsItemId
////                            }).ToList();


////                //var response = _JobInvoiceResponseRepo.GetBySP($"GetJobInvoice").ToList();
////                return data;
////            }
////            catch (Exception ex)
////            {
////                throw ex;
////            }
////        }
////    }
////}
