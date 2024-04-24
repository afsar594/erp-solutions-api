using Inspire.Erp.Application.Common;
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
        private IRepository<StockRegister> _stockRegisterRepository;
        private IRepository<CustomerMaster> _customerMasterRepository;
        private IRepository<CurrencyMaster> _currencyMasterRepository;
        public PosSalesVoucherService(IRepository<POS_Sales_Voucher> posSalesVoucherRepository, IRepository<POS_Sales_Voucher_Details> posSalesVoucherDetailsRepository, IRepository<ProgramSettings> programsettingsRepository, 
            IRepository<VouchersNumbers> voucherNumbersRepository, IRepository<StockRegister> stockRegisterRepository, IRepository<CustomerMaster> customerMasterRepository, IRepository<CurrencyMaster> currencyMasterRepository)
        {
            _posSalesVoucherDetailsRepository = posSalesVoucherDetailsRepository;
            _posSalesVoucherRepository = posSalesVoucherRepository;
            _programsettingsRepository = programsettingsRepository;
            _voucherNumbersRepository = voucherNumbersRepository;
            _stockRegisterRepository = stockRegisterRepository;
            _customerMasterRepository = customerMasterRepository;
            _currencyMasterRepository = currencyMasterRepository;
            
          
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

                var stockReg = new List<StockRegister>();
                foreach (var item in salesVoucher.posSalesVoucherDetails)
                {
                    stockReg.Add(new StockRegister()
                    {
                        StockRegisterPurchaseID = salesVoucher.VoucherNo,
                        StockRegisterRefVoucherNo = salesVoucher.VoucherNo,
                        StockRegisterVoucherDate = DateTime.Now.Date,
                        StockRegisterMaterialID = item.SalesVoucherDetailsMatId,
                        StockRegisterQuantity = item.SalesVoucherDetailsQuantity,
                        StockRegisterSIN = 0,
                        StockRegisterSout = item.SalesVoucherDetailsQuantity,
                        StockRegisterRate = item.SalesVoucherDetailsRate,
                        StockRegisterAmount = item.SalesVoucherDetailsActualAmount ?? 0,
                        StockRegisterFCAmount = item.SalesVoucherDetailsActualAmount ?? 0 * this.getCurrencyRate(salesVoucher.SalesVoucherCurrencyId),
                        StockRegisterAssignedDate = DateTime.Now.Date,
                        StockRegisterStatus = "A",
                        StockRegisterTransType = salesVoucher.Voucher_Type,
                        StockRegisterUnitID = item.SalesVoucherDetailsUnitId,
                        StockRegisterLocationID = salesVoucher.Location,
                        StockRegisterDepID = Convert.ToInt32(salesVoucher.DeptID),
                        StockRegisterDelStatus = false
                    });
                }
                  _stockRegisterRepository.InsertList(stockReg);


                _posSalesVoucherRepository.Insert(salesVoucher);
                _posSalesVoucherDetailsRepository.InsertList(posSalesVoucherDetails);
                _posSalesVoucherRepository.TransactionCommit();
                return this.GetSavedSalesVoucherDetails(salesVoucher.VoucherNo);

            }
            catch (Exception ex)
            {
                _posSalesVoucherRepository.TransactionRollback();
                throw;
            }        
        
        }

        public POS_Sales_Voucher GetSavedSalesVoucherDetails(string pvno)
        {
            POS_Sales_Voucher salesVoucher = new POS_Sales_Voucher();
           
            salesVoucher = _posSalesVoucherRepository.GetAsQueryable().Where(k => k.VoucherNo == pvno).SingleOrDefault();
            salesVoucher.posSalesVoucherDetails = _posSalesVoucherDetailsRepository.GetAsQueryable().Where(x => x.SalesVoucherDetailsNo == pvno && (x.SalesVoucherDetailsDelStatus == false || x.SalesVoucherDetailsDelStatus == null)).ToList();

            
            return salesVoucher;
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

        public POS_Sales_Voucher UpdatePosSalesVoucher(POS_Sales_Voucher salesVoucher, List<POS_Sales_Voucher_Details> PosSalesVoucherDetails)
        {
            try
            {
                _posSalesVoucherRepository.BeginTransaction();
                _posSalesVoucherRepository.Update(salesVoucher);
                _posSalesVoucherDetailsRepository.UpdateList(PosSalesVoucherDetails);
                _posSalesVoucherRepository.TransactionCommit();

                return this.GetSavedSalesVoucherDetails(salesVoucher.VoucherNo);
            }
            catch (Exception ex)
            {
                _posSalesVoucherRepository.TransactionRollback();
                throw;
            }
        }

        public IEnumerable<POS_Sales_Voucher> GetPosSalesVoucher()
        {
            var salV = (from s in _posSalesVoucherRepository.GetAsQueryable()
                        join c in _customerMasterRepository.GetAsQueryable() on s.CustomerId equals c.CustomerMasterCustomerNo
                        select new POS_Sales_Voucher()
                        {
                            V_Id = s.V_Id,
                            VoucherNo = s.VoucherNo,
                            VoucherDate = s.VoucherDate,
                            Voucher_Type = s.Voucher_Type,
                            CustomerId = c.CustomerMasterCustomerNo,
                            CustomerName = c.CustomerMasterCustomerName,
                            SalesVoucherPartyAddress = c.CustomerMasterCustomerLocation,
                            SalesVoucherPartyVatNo = c.CustomerMasterCustomerVatNo,
                            RefNo = s.SalesVoucherRefNo,
                            SalesVoucherContPerson = s.SalesVoucherContPerson,
                            SalesVoucherDlvNo = s.SalesVoucherDlvNo,
                            SalesVoucherDlvDate = s.SalesVoucherDlvDate,
                            SalesVoucherSONo = s.SalesVoucherSONo,
                            SalesVoucherSODate = s.SalesVoucherSODate,
                            SalesVoucherQtnNo = s.SalesVoucherQtnNo,
                            SalesVoucherQtnDate = s.SalesVoucherQtnDate,
                            SalesVoucherSalesManID = s.SalesVoucherSalesManID,
                            SalesVoucherDptID = s.SalesVoucherDptID,
                            SalesVoucherEnqNo = s.SalesVoucherEnqNo,
                            SalesVoucherEnqDate = s.SalesVoucherEnqDate,
                            SalesVoucherDescription = s.SalesVoucherDescription,
                            SalesVoucherExcludeVAT = s.SalesVoucherExcludeVAT,
                            SalesVoucherLocationID = s.SalesVoucherLocationID,
                            SalesVoucherUserID = s.SalesVoucherUserID,
                            SalesVoucherCurrencyId = cr.CurrencyMasterCurrencyId,
                            CurrencyMasterCurrencyName = cr.CurrencyMasterCurrencyName,
                            SalesVoucherCompanyId = s.SalesVoucherCompanyId,
                            SalesVoucherJobId = s.SalesVoucherJobId,
                            SalesVoucherFSNO = s.SalesVoucherFSNO,
                            SalesVoucherFc_Rate = s.SalesVoucherFc_Rate,
                            SalesVoucherStatus = s.SalesVoucherStatus,
                            SalesVoucherTotalGrossAmount = s.SalesVoucherTotalGrossAmount,
                            SalesVoucherTotalItemDisAmount = s.SalesVoucherTotalItemDisAmount,
                            TotalActualAmount = s.SalesVoucherTotalActualAmount,
                            TotalDisPer = s.TotalDisPer,
                            TotalDisAmount = s.TotalDisAmount,
                            Vat_AMT = s.Vat_AMT,
                            SalesVoucherVat_Per = s.SalesVoucherVat_Per,
                            SalesVoucherVat_RoundSign = s.SalesVoucherVat_RoundSign,
                            Vat_RountAmt = s.Vat_RountAmt,
                            NetDisAmount = s.NetDisAmount,
                            NetAmount = s.NetAmount,
                            SalesVoucherHeader = s.SalesVoucherHeader,
                            SalesVoucherDelivery = s.SalesVoucherDelivery,
                            SalesVoucherNotes = s.SalesVoucherNotes,
                            SalesVoucherFooter = s.SalesVoucherFooter,
                            SalesVoucherPaymentTerms = s.SalesVoucherPaymentTerms,
                            SalesVoucherSubject = s.SalesVoucherSubject,
                            SalesVoucherContent = s.SalesVoucherContent,
                            Remarks = s.Remarks,
                            SalesVoucherTermsAndCondition = s.SalesVoucherTermsAndCondition,
                            SalesVoucherShippinAddress = s.SalesVoucherShippinAddress,
                            ShortNo = s.ShortNo,
                        }).ToList();





            return salV;
        }
    }
}

