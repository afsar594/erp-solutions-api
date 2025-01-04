using Inspire.Erp.Application.Account.Interface;
using Inspire.Erp.Application.Common;
using Inspire.Erp.Application.MODULE;
using Inspire.Erp.Application.RFQ.Interface;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Entities.documents;
using Inspire.Erp.Domain.Enums;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using Inspire.Erp.Web.ViewModels;
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
                var data = _rfqEntry.GetAll().Where(a => a.RfqEntryStatus != true).OrderBy(c => c.RFQEntryID).ToList();
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public RfqEntry GetRfqById(int id)
        {
            try
            {
                RfqEntry Rfq = new RfqEntry();
                Rfq = _rfqEntry.GetAsQueryable().FirstOrDefault(x => x.RFQEntryID == Convert.ToInt32(id));
                Rfq.RfqEntryDetails = _rfqEntryDetails.GetAsQueryable().Where(x => x.RFQ_Entry_ID == Convert.ToInt32(id) && x.RFQEntryDetailsDelStatus != true).OrderBy(x => x.RFQEntryDetailsDetID).ToList();
                return Rfq;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<RfqEntry> InsertRfq(RfqEntry RfqEntry, List<RfqEntryDetails> RfqEntryDetails)

        {
            try
            {

                string RFQVoucherNumber = clsCommonFunctions.GenrateVoucherNo(VoucherType.RFQ_TYPE, Prefix.RFQ_Prefix,
                    _programsettingsrepository, _voucherNumbersRepository);
                RfqEntry.RFQ_EntryvouchaseNo = RFQVoucherNumber;

                _rfqEntry.BeginTransaction();
                int maxcount = 0;
                maxcount = Convert.ToInt32(
                    _rfqEntry.GetAsQueryable()
                    .DefaultIfEmpty().Max(o => o == null ? 0 : o.RFQEntryID) + 1);
                RfqEntry.RFQEntryID = maxcount;
                _rfqEntry.Insert(RfqEntry);

                int maxcount1 = 0;
                maxcount1 = Convert.ToInt32(
                   _rfqEntryDetails.GetAsQueryable()
                   .DefaultIfEmpty().Max(o => o == null ? 0 : o.RFQEntryDetailsDetID) + 1);
                RfqEntryDetails.ForEach(elemt =>
                {
                    elemt.RFQEntryDetailsDetID = maxcount1;
                    elemt.RFQ_EntryvouchaseNo = RFQVoucherNumber;
                    elemt.RFQ_Entry_ID = maxcount;
                    maxcount1++;
                });
                _rfqEntryDetails.InsertList(RfqEntryDetails);
                _rfqEntry.TransactionCommit();

                return this.GetRfqById(RfqEntry.RFQEntryID);
            }
            catch (Exception ex)
            {
                _rfqEntry.TransactionRollback();
                throw ex;
            }
        }

        public async Task<RfqEntry> UpdatetRfq(RfqEntry RfqEntry, List<RfqEntryDetails> RfqEntryDetails)
        {
            try
            {

                // string RFQVoucherNumber = this.GenerateVoucherNo(RfqEntry.RfqEntryRfqDate.Value).VouchersNumbersVNo;
                // RfqEntry.RFQ_EntryvouchaseNo = RFQVoucherNumber;

                _rfqEntry.BeginTransaction();
                int maxcount = 0;
                maxcount = Convert.ToInt32(
                    _rfqEntry.GetAsQueryable()
                    .DefaultIfEmpty().Max(o => o == null ? 0 : o.RfqEntryRfqEnqId) + 1);
                RfqEntry.RfqEntryRfqEnqId = maxcount;
                _rfqEntry.Update(RfqEntry);


                int maxcount1 = 0;
                maxcount1 = Convert.ToInt32(
                   _rfqEntryDetails.GetAsQueryable()
                   .DefaultIfEmpty().Max(o => o == null ? 0 : o.RFQEntryDetailsDetID) + 1);

                foreach (var elemt in RfqEntryDetails)
                {
                    elemt.RFQ_Entry_ID = maxcount;
                    if (elemt.RFQEntryDetailsDetID == 0)
                    {
                        elemt.RFQEntryDetailsDetID = maxcount1;
                        _rfqEntryDetails.Insert(elemt);
                        maxcount1++;
                    }
                    else
                    {
                        _rfqEntryDetails.Update(elemt);

                    }

                }

                //_rfqEntryDetails.UpdateList(RfqEntryDetails);
                _rfqEntry.TransactionCommit();

                return this.GetRfqById(RfqEntry.RFQEntryID);
            }
            catch (Exception ex)
            {
                _rfqEntry.TransactionRollback();
                throw ex;
            }
        }

        public IEnumerable<RfqEntry> DeleteRfq(string vouchaseNo)
        {
            try
            {
                var dataObj = _rfqEntry.GetAsQueryable().Where(o => o.RFQ_EntryvouchaseNo == vouchaseNo).FirstOrDefault();
                dataObj.RfqEntryDelStatus = true;
                var detailData = _rfqEntryDetails.GetAsQueryable().Where(o => o.RFQ_EntryvouchaseNo == vouchaseNo).OrderBy(x => x.RFQ_EntryvouchaseNo).ToList();
                detailData.ForEach(elemt =>
                {
                    elemt.RFQEntryDetailsDelStatus = true;
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


        public RfqEntry GetRfqEntrybyvoucher(string VNo)
        {

            try
            {

                RfqEntry Rfq = new RfqEntry();
                Rfq = _rfqEntry.GetAsQueryable().Where(k => k.RFQ_EntryvouchaseNo == VNo).FirstOrDefault();
                Rfq.RfqEntryDetails = _rfqEntryDetails.GetAsQueryable().Where(x => x.RFQ_EntryvouchaseNo == VNo && (x.RFQEntryDetailsDelStatus == false || x.RFQEntryDetailsDelStatus == null)).OrderBy(x => x.RFQ_EntryvouchaseNo).ToList();
                return Rfq;
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
                var prefix = _programsettingsrepository.GetAsQueryable().Where(k => k.ProgramSettingsKeyValue == Prefix.RFQ_Prefix).FirstOrDefault().ProgramSettingsTextValue;
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
