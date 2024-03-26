using Inspire.Erp.Application.Store.Interfaces;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Enums;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspire.Erp.Application.Store.Implementation
{
    public class StockAdjustmentService : IStockAdjustmentService
    {
        private readonly IRepository<StockAdjustmentVoucher> _stockAdjustmentVoucherRepo;
        private readonly IRepository<StockAdjustmentVoucherDetails> _stockAdjustmentVoucherDetailsRepo;
        private IRepository<ProgramSettings> _programsettingsRepository;
        private readonly IRepository<VouchersNumbers> _VoucherNumberRepo;
        public StockAdjustmentService(IRepository<StockAdjustmentVoucher> stockAdjustmentVoucherRepo, IRepository<VouchersNumbers> voucherNumberRepo, IRepository<StockAdjustmentVoucherDetails> stockAdjustmentVoucherDetailsRepo, IRepository<ProgramSettings> programsettingsRepository)
        {
            _stockAdjustmentVoucherRepo = stockAdjustmentVoucherRepo;
            _VoucherNumberRepo = voucherNumberRepo;
            _stockAdjustmentVoucherDetailsRepo = stockAdjustmentVoucherDetailsRepo;
            _programsettingsRepository = programsettingsRepository;
        }
        public IEnumerable<StockAdjustmentVoucher> GetStockAdjustment()
        {
            IEnumerable<StockAdjustmentVoucher> stockAdjusment = _stockAdjustmentVoucherRepo.GetAsQueryable().Where(k => k.StockAdjustmentVoucherDelStatus == false || k.StockAdjustmentVoucherDelStatus == null);
            return stockAdjusment;
        }

        public async Task<StockAdjustmentVoucher> InsertStockAdjustmentVoucher(StockAdjustmentVoucher stockAdjustmentVoucher, List<StockAdjustmentVoucherDetails> stockAdjustmentVoucherDetails)
        {
            try
            {
                _stockAdjustmentVoucherRepo.BeginTransaction();
                string stockVoucherNumber = this.GenerateVoucherNo(stockAdjustmentVoucher.StockAdjustmentVoucherSaDate.Value).VouchersNumbersVNo;
                stockAdjustmentVoucher.StockAdjustmentVoucherSaNo = stockVoucherNumber;


                int maxcount = 0;
                maxcount = Convert.ToInt32(
                    await _stockAdjustmentVoucherRepo.GetAsQueryable()
                    .DefaultIfEmpty().MaxAsync(o => o == null ? 0 : o.StockAdjustmentVoucherSaId) + 1);

                stockAdjustmentVoucher.StockAdjustmentVoucherSaId = maxcount;

                
                stockAdjustmentVoucherDetails = stockAdjustmentVoucherDetails.Select((x) =>
                {
                    x.StockAdjustmentVoucherDetailsSaNo = stockVoucherNumber;
                    x.StockAdjustmentVoucherDetailsSaId = maxcount;
                    x.StockAdjustmentVoucherDetailsLocationId = stockAdjustmentVoucher.StockAdjustmentVoucherLocationId;
                    return x;
                }).ToList();

                _stockAdjustmentVoucherRepo.Insert(stockAdjustmentVoucher);
                _stockAdjustmentVoucherDetailsRepo.InsertList(stockAdjustmentVoucherDetails);
                _stockAdjustmentVoucherRepo.TransactionCommit();
                return this.GetSavedStockAdjustmentferDetails(stockAdjustmentVoucher.StockAdjustmentVoucherSaNo);

            }
            catch (Exception ex)
            {
                _stockAdjustmentVoucherRepo.TransactionRollback();
                throw;
            }
        }

        public VouchersNumbers GenerateVoucherNo(DateTime? VoucherGenDate)
        {
            try
            {


                //var prefix = this._programsettingsRepository.GetAsQueryable().Where(k => k.ProgramSettingsKeyValue == Prefix.StockTransfer_Prefix).FirstOrDefault().ProgramSettingsTextValue;
                int vnoMaxVal = Convert.ToInt32(_VoucherNumberRepo.GetAsQueryable()
                                                        .Where(x => x.VouchersNumbersVNoNu > 0 && x.VouchersNumbersVType == VoucherType.StockAdjustmentVoucher_TYPE)
                                                        .DefaultIfEmpty()
                                                        .Max(o => o == null ? 0 : o.VouchersNumbersVNoNu)) + 1;



                VouchersNumbers vouchersNumbers = new VouchersNumbers
                {
                    VouchersNumbersVNo = "SA" + vnoMaxVal,
                    VouchersNumbersVNoNu = vnoMaxVal,
                    VouchersNumbersVType = VoucherType.StockAdjustmentVoucher_TYPE,
                    VouchersNumbersFsno = 1,
                    VouchersNumbersStatus = AccountStatus.Pending,
                    VouchersNumbersVoucherDate = VoucherGenDate

                };
                _VoucherNumberRepo.Insert(vouchersNumbers);
                return vouchersNumbers;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public StockAdjustmentVoucher GetSavedStockAdjustmentferDetails(string sano)
        {
            StockAdjustmentVoucher stockAdjustmentVoucher = new StockAdjustmentVoucher();
            stockAdjustmentVoucher = _stockAdjustmentVoucherRepo.GetAsQueryable().Where(k => k.StockAdjustmentVoucherSaNo == sano).SingleOrDefault();
            stockAdjustmentVoucher.stockAdjustmentVoucherDetails = _stockAdjustmentVoucherDetailsRepo.GetAsQueryable().Where(x => x.StockAdjustmentVoucherDetailsSaNo == sano).ToList();
            return stockAdjustmentVoucher;
        }

        public async Task<StockAdjustmentVoucher> UpdateStockAdjustmentVoucher(StockAdjustmentVoucher stockAdjustmentVoucher, List<StockAdjustmentVoucherDetails> stockAdjustmentVoucherDetails)
        {
            try
            {
                _stockAdjustmentVoucherRepo.BeginTransaction();
                _stockAdjustmentVoucherRepo.Update(stockAdjustmentVoucher);
                _stockAdjustmentVoucherDetailsRepo.UpdateList(stockAdjustmentVoucherDetails);

                _stockAdjustmentVoucherRepo.TransactionCommit();

                return this.GetSavedStockAdjustmentferDetails(stockAdjustmentVoucher.StockAdjustmentVoucherSaNo);
            }
            catch (Exception ex)
            {
                _stockAdjustmentVoucherRepo.TransactionRollback();
                throw;
            }
        }

        public int DeleteStockAdjustmentVoucher(StockAdjustmentVoucher stockAdjustmentVoucher, List<StockAdjustmentVoucherDetails> stockAdjustmentVoucherDetails)
        {
            int i = 0;
            try
            {
                _stockAdjustmentVoucherRepo.BeginTransaction();
                stockAdjustmentVoucher.StockAdjustmentVoucherDelStatus = true;
                stockAdjustmentVoucherDetails = stockAdjustmentVoucherDetails.Select((k) =>
                {
                    k.StockAdjustmentVoucherDetailsDelStatus = true;
                    return k;
                }).ToList();
            
          
                _stockAdjustmentVoucherRepo.Update(stockAdjustmentVoucher);
                _stockAdjustmentVoucherDetailsRepo.UpdateList(stockAdjustmentVoucherDetails);

                _stockAdjustmentVoucherRepo.TransactionCommit();

                i = 1;
            }
            catch (Exception ex)
            {
                _stockAdjustmentVoucherRepo.TransactionRollback();
                i = 0;
                throw ex;
            }
            return i;
        }
    }
}
