using Inspire.Erp.Application.Account.Interface;
using Inspire.Erp.Application.Common;
using Inspire.Erp.Application.MODULE;
using Inspire.Erp.Application.RFQ.Interface;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Entities.documents;
using Inspire.Erp.Domain.Enums;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inspire.Erp.Application.RFQ.Implementation
{
    public class RrqService : IRfqService
    {
        private readonly IRepository<RfqEntry> _rfqEntry;
        private readonly IRepository<RfqEntryDetails> _rfqEntryDetails;
        private readonly IRepository<ProgramSettings> _programsettingsrepository;
        private IRepository<VouchersNumbers> _voucherNumbersRepository;
        public RrqService(IRepository<RfqEntry> rfqEntry, IRepository<RfqEntryDetails> rfqEntryDetails, IRepository<ProgramSettings> programsettingsrepository,
            IRepository<VouchersNumbers> voucherNumbersRepository)
        {
            _rfqEntry = rfqEntry;
            _rfqEntryDetails = rfqEntryDetails;
            _programsettingsrepository = programsettingsrepository;
            _voucherNumbersRepository = voucherNumbersRepository;
        }
        public IEnumerable<RfqEntry> GetRfq()
        {
            try
            {
                var data = _rfqEntry.GetAll().Where(a => a.RfqEntryStatus != true).OrderBy(c => c.RfqEntryId).ToList();
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public RfqEntry GetRfqById(int? id)
        {
            try
            {
                var RfqEntry = new RfqEntry();
                RfqEntry = _rfqEntry.GetAsQueryable().FirstOrDefault(x => x.RfqEntryId == Convert.ToInt32(id));
                RfqEntry.RfqEntryDetails = _rfqEntryDetails.GetAsQueryable().Where(x => x.RfqEntryDetailsDetId == Convert.ToInt32(id) && x.RfqEntryDetailsDelStatus != true).ToList();
                return RfqEntry;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<RfqEntry> InsertRfq(RfqEntry RfqEntry)
        {
            try
            {

                string RFQVoucherNumber = this.GenerateVoucherNo(RfqEntry.RfqEntryRfqDate.Value).VouchersNumbersVNo;
                RfqEntry.RFQ_EntryvouchaseNo = RFQVoucherNumber;

                _rfqEntry.BeginTransaction();
                int maxcount = 0;
                maxcount = Convert.ToInt32(
                    _rfqEntry.GetAsQueryable()
                    .DefaultIfEmpty().Max(o => o == null ? 0 : o.RfqEntryRfqEnqId) + 1);
                RfqEntry.RfqEntryRfqEnqId = maxcount;
                _rfqEntry.Insert(RfqEntry);


                RfqEntry.RfqEntryDetails.ForEach(elemt =>
                {
                    elemt.RfqEntryDetailsDetId = maxcount;
                    elemt.RFQ_EntryvouchaseNo = RFQVoucherNumber;
                    maxcount++;
                }); 
                _rfqEntryDetails.InsertList(RfqEntry.RfqEntryDetails);
                _rfqEntry.TransactionCommit();

                return this.GetRfqEntrybyvoucher(RFQVoucherNumber);
            }
            catch (Exception ex)
            {
                _rfqEntry.TransactionRollback();
                throw ex;
            }
        }

        public async Task<RfqEntry> UpdatetRfq(RfqEntry RfqEntry)
        {
            try
            {

                string RFQVoucherNumber = this.GenerateVoucherNo(RfqEntry.RfqEntryRfqDate.Value).VouchersNumbersVNo;
                RfqEntry.RFQ_EntryvouchaseNo = RFQVoucherNumber;

                _rfqEntry.BeginTransaction();
                int maxcount = 0;
                maxcount = Convert.ToInt32(
                    _rfqEntry.GetAsQueryable()
                    .DefaultIfEmpty().Max(o => o == null ? 0 : o.RfqEntryRfqEnqId) + 1);
                RfqEntry.RfqEntryRfqEnqId = maxcount;
                _rfqEntry.Update(RfqEntry);


                RfqEntry.RfqEntryDetails.ForEach(elemt =>
                {
                    elemt.RfqEntryDetailsDetId = maxcount;
                    elemt.RFQ_EntryvouchaseNo = RFQVoucherNumber;
                    maxcount++;
                });
                _rfqEntryDetails.UpdateList(RfqEntry.RfqEntryDetails);
                _rfqEntry.TransactionCommit();

                return this.GetRfqEntrybyvoucher(RFQVoucherNumber);
            }
            catch (Exception ex)
            {
                _rfqEntry.TransactionRollback();
                throw ex;
            }
        }

        public IEnumerable<RfqEntry> DeleteRfq(string Id)
        {
            try
            {
                var dataObj = _rfqEntry.GetAsQueryable().Where(o => o.RFQ_EntryvouchaseNo == Id).FirstOrDefault();
                dataObj.RfqEntryDelStatus = true;
                var detailData = _rfqEntryDetails.GetAsQueryable().Where(o => o.RFQ_EntryvouchaseNo == Id).ToList();
                detailData.ForEach(elemt =>
                {
                    elemt.RfqEntryDetailsDelStatus = true;
                });
                _rfqEntry.Update(dataObj);
                _rfqEntryDetails.UpdateList(detailData);
                return _rfqEntry.GetAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public RfqEntry GetDocumentMasterbyvoucher(string VNo)
        {
            try
            {
                RfqEntry RfqEntry = new RfqEntry();
                RfqEntry = _rfqEntry.GetAsQueryable().Where(k => k.RFQ_EntryvouchaseNo == VNo).SingleOrDefault();
                return RfqEntry;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public RfqEntry GetRfqEntrybyvoucher(string VNo)
        {
            try
            {

                RfqEntry RfqEntry = new RfqEntry();
                RfqEntry = _rfqEntry.GetAsQueryable().Where(k => k.RFQ_EntryvouchaseNo == VNo).FirstOrDefault();
                RfqEntry.RfqEntryDetails  = _rfqEntryDetails.GetAsQueryable().Where(x => x.RFQ_EntryvouchaseNo == VNo && (x.RfqEntryDetailsDelStatus == false || x.RfqEntryDetailsDelStatus == null)).ToList();
                return RfqEntry;
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
                var prefix = this._programsettingsrepository.GetAsQueryable().Where(k => k.ProgramSettingsKeyValue == Prefix.RFQ_Prefix).FirstOrDefault().ProgramSettingsTextValue;
                int vnoMaxVal = Convert.ToInt32(_voucherNumbersRepository.GetAsQueryable()
                                                        .Where(x => x.VouchersNumbersVNoNu > 0 && x.VouchersNumbersVType == VoucherType.RFQ_TYPE)
                                                        .DefaultIfEmpty()
                                                        .Max(o => o == null ? 0 : o.VouchersNumbersVNoNu)) + 1;


                VouchersNumbers vouchersNumbers = new VouchersNumbers
                {
                    VouchersNumbersVNo = prefix + vnoMaxVal,
                    VouchersNumbersVNoNu = vnoMaxVal,
                    VouchersNumbersVType = VoucherType.RFQ_TYPE,
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
