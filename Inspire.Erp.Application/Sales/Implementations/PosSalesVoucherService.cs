using Inspire.Erp.Application.Sales.Interfaces;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Enums;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Inspire.Erp.Application.Sales.Implementations
{
    public class PosSalesVoucherService : IPosSalesVoucherService
    {
       
        private IRepository<POS_Sales_Voucher> _posSalesVoucherRepository;
        private IRepository<POS_Sales_Voucher_Details> _posSalesVoucherDetailsRepository;
        private IRepository<ProgramSettings> _programsettingsRepository;
        private IRepository<VouchersNumbers> _voucherNumbersRepository;
        public PosSalesVoucherService(IRepository<POS_Sales_Voucher> posSalesVoucherRepository, IRepository<POS_Sales_Voucher_Details> posSalesVoucherDetailsRepository)
        {
            _posSalesVoucherDetailsRepository = posSalesVoucherDetailsRepository;
            _posSalesVoucherRepository = posSalesVoucherRepository;
          
        }
        public POS_Sales_Voucher InsertPosSalesVoucher(POS_Sales_Voucher salesVoucher ,List<POS_Sales_Voucher_Details> posSalesVoucherDetails)
        {
            try
            {
                _posSalesVoucherRepository.BeginTransaction();
                string salesVoucherNumber = this.GenerateVoucherNo(salesVoucher.VoucherDate).VouchersNumbersVNo;
                salesVoucher.VoucherNo = salesVoucherNumber;


                int maxcount = 0;
                maxcount = Convert.ToInt32(
                    _posSalesVoucherRepository.GetAsQueryable()
                    .DefaultIfEmpty().Max(o => o == null ? 0 : o.V_Id) + 1);

                //salesVoucher.SalesVoucherId = maxcount;
                //posSalesVoucherDetails = posSalesVoucherDetails.Select((x) =>
                //{
                //    x.SalesVoucherId = maxcount;
                //    x.SalesVoucherDetailsNo = salesVoucherNumber;
                //    return x;
                //}).ToList();

                _posSalesVoucherRepository.Insert(salesVoucher);
                _posSalesVoucherDetailsRepository.InsertList(posSalesVoucherDetails);
                _posSalesVoucherRepository.TransactionCommit();
                return this.GetSavedSalesVoucherDetails(salesVoucher.VoucherNo); ;

            }
            catch (Exception ex)
            {

                throw;
            }        
        
        }


        public VouchersNumbers GenerateVoucherNo(DateTime? VoucherGenDate)
        {
            try
            {






               // var prefix = this._programsettingsRepository.GetAsQueryable().Where(k => k.ProgramSettingsKeyValue == Prefix.SalesVoucher_Prefix).FirstOrDefault().ProgramSettingsTextValue;
                int vnoMaxVal = Convert.ToInt32(_voucherNumbersRepository.GetAsQueryable()
                                                        .Where(x => x.VouchersNumbersVNoNu > 0 && x.VouchersNumbersVType == VoucherType.PosSalesVoucher_TYPE)
                                                        .DefaultIfEmpty()
                                                        .Max(o => o == null ? 0 : o.VouchersNumbersVNoNu)) + 1;


          
                VouchersNumbers vouchersNumbers = new VouchersNumbers
                {
                    VouchersNumbersVNo = "POSSV" + vnoMaxVal,
                    VouchersNumbersVNoNu = vnoMaxVal,
                    VouchersNumbersVType = VoucherType.PosSalesVoucher_TYPE,
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

