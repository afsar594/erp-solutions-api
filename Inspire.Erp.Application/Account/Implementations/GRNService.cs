using Inspire.Erp.Application.Account.Implementations;
using Inspire.Erp.Application.StoreWareHouse.Interface;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Enums;
using Inspire.Erp.Domain.Modals;
using Inspire.Erp.Infrastructure.Database;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspire.Erp.Application.StoreWareHouse.Implementation
{
    public class GRNService : IGRNService
    {
        private IRepository<GRNResponse> _grnRepository;
        private IRepository<StockRegister> _stockRegisterRepository;
        private IRepository<AccountsTransactions> _accountTransactionRepository;
        private IRepository<PurchaseVoucher> _purchaseVoucherRepository;
        private IRepository<PurchaseVoucherDetails> _purchaseVoucherDetailsRepository;
        private IRepository<VouchersNumbers> _voucherNumbersRepository;
        IRepository<ProgramSettings> _programsettingsRepository;
        private readonly ILogger<PaymentVoucherService> _logger;
        private readonly InspireErpDBContext _context;
        public static string prefix;
        public GRNService(
            ILogger<PaymentVoucherService> logger,
            IRepository<GRNResponse> grnRepository,
            IRepository<AccountsTransactions> accountTransactionRepository,
            IRepository<StockRegister> stockRegisterRepository,
            IRepository<ProgramSettings> programsettingsRepository,
            IRepository<VouchersNumbers> voucherNumbers,
            IRepository<PurchaseVoucher> purchaseVoucherRepository,
            IRepository<PurchaseVoucherDetails> purchaseVoucherDetailsRepository,
            InspireErpDBContext context)
        {
            _accountTransactionRepository = accountTransactionRepository;
            _stockRegisterRepository = stockRegisterRepository;
            _purchaseVoucherRepository = purchaseVoucherRepository;
            _purchaseVoucherDetailsRepository = purchaseVoucherDetailsRepository;
            _voucherNumbersRepository = voucherNumbers;
            _programsettingsRepository = programsettingsRepository;
            _grnRepository = grnRepository;
            _logger = logger;
            _context = context;
            prefix = programsettingsRepository.GetAsQueryable().Where(k => k.ProgramSettingsKeyValue == Prefix.PurchaseVoucher_Prefix).FirstOrDefault().ProgramSettingsTextValue;
        }


        public IQueryable GetAllGRN()
        {
            try
            {
                var grnList = _context.PurchaseVoucher.Where(o => o.PurchaseVoucherVoucherNo.Contains("GRN")).AsQueryable();
                return grnList;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public PurchaseVoucher GetByID(string voucherNo)
        {
            try
            {
                PurchaseVoucher pv = new PurchaseVoucher();
                var userTrackngData = (from hdr in _context.UserTracking.Where(o => o.UserTrackingUserVpType == "GRN" && o.UserTrackingUserVpNo == voucherNo)
                                       join dtl in _context.UserFile on (long)hdr.UserTrackingUserUserId equals dtl.UserId
                                       select new UserTrackingDisplay()
                                       {
                                           Action = hdr.UserTrackingUserVpAction,
                                           Id = dtl.UserId.ToString(),
                                           Name = dtl.UserName,
                                           VType = hdr.UserTrackingUserVpType,
                                           Date = Convert.ToDateTime(hdr.UserTrackingUserChangeDt).ToString("dd/MMM/yyyy"),
                                           Time = Convert.ToDateTime(hdr.UserTrackingUserChangeTime).ToShortTimeString(),
                                           VNo = hdr.UserTrackingUserVpNo,
                                       }).ToList();
                pv = _purchaseVoucherRepository.GetAll().FirstOrDefault(o => o.PurchaseVoucherVoucherNo == voucherNo);
                pv.PurchaseVoucherDetails = _purchaseVoucherDetailsRepository.GetAll().Where(o => o.PurchaseVoucherDetailsVoucherNo == voucherNo).ToList();
                pv.AccountsTransactions = _accountTransactionRepository.GetAll().Where(o => o.AccountsTransactionsVoucherNo == voucherNo).ToList();
                pv.UserTrackingData = userTrackngData;
                return pv;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public IQueryable updateGRN(PurchaseVoucher grn, List<AccountsTransactions> accountsTransactions)
        {
            try
            {
                foreach (var k in accountsTransactions)
                {
                    k.AccountsTransactionsVoucherNo = grn.PurchaseVoucherVoucherNo;
                    k.AccountsTransactionsVoucherType = VoucherType.PurchaseVoucher_TYPE;
                    k.AccountsTransactionsStatus = AccountStatus.Approved;
                    k.AccountsTransactionsUserId = 1;
                }
                _context.AccountsTransactions.UpdateRange(accountsTransactions);
                _context.SaveChanges();

                _purchaseVoucherRepository.Update(grn);
                var existingGrnList = _purchaseVoucherDetailsRepository.GetAll().Where(o => o.PurchaseVoucherDetailsVoucherNo == grn.PurchaseVoucherVoucherNo).ToList();
                _purchaseVoucherDetailsRepository.DeleteList(existingGrnList);

                foreach (var item in grn.PurchaseVoucherDetails)
                {
                    item.PurchaseVoucherDetailsVoucherNo = grn.PurchaseVoucherVoucherNo;
                }
                _purchaseVoucherDetailsRepository.UpdateList(grn.PurchaseVoucherDetails);

                UserTracking trackingData = new UserTracking();
                trackingData.UserTrackingUserUserId = 1;
                trackingData.UserTrackingUserVpAction = "Update";
                trackingData.UserTrackingUserVpNo = grn.PurchaseVoucherVoucherNo;
                trackingData.UserTrackingUserChangeDt = DateTime.Now;
                trackingData.UserTrackingUserChangeTime = DateTime.Now;
                trackingData.UserTrackingUserVpType = "GRN";
                _context.UserTracking.Add(trackingData);
                _context.SaveChanges();
                return this.GetAllGRN();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IQueryable saveGRN(PurchaseVoucher grn)
        {
            try
            {
                grn.PurchaseVoucherDelStatus = false;
                foreach (var k in grn.AccountsTransactions)
                {
                    k.AccountsTransactionsVoucherNo = grn.PurchaseVoucherVoucherNo;
                    k.AccountsTransactionsVoucherType = VoucherType.PurchaseVoucher_TYPE;
                    k.AccountsTransactionsUserId = 1;
                    k.AccountsTransactionsStatus = AccountStatus.Approved;
                }
                _context.AccountsTransactions.AddRange(grn.AccountsTransactions);
                _context.SaveChanges();

                _purchaseVoucherRepository.Insert(grn);
                foreach (var item in grn.PurchaseVoucherDetails)
                {
                    item.PurchaseVoucherDetailsVoucherNo = grn.PurchaseVoucherVoucherNo;
                }

                _purchaseVoucherDetailsRepository.InsertList(grn.PurchaseVoucherDetails);
                
                List < StockRegister > sr = new List<StockRegister>();
                foreach (var item in grn.PurchaseVoucherDetails)
                {
                    sr.Add(new StockRegister()
                    {
                        StockRegisterRefVoucherNo = grn.PurchaseVoucherVoucherNo,
                        StockRegisterAssignedDate = DateTime.Now,
                        StockRegisterTransType = VoucherType.PurchaseVoucher_TYPE,
                        StockRegisterFCAmount = grn.PurchaseVoucherActualAmount,
                        StockRegisterFcRate = grn.PurchaseVoucherFCRate,
                        StockRegisterStatus = "A",
                        StockRegisterJobID = grn.PurchaseVoucherJobID,
                        StockRegisterLocationID = grn.PurchaseVoucherLocationID,
                        StockRegisterFSNO = null,
                        StockRegisterVoucherDate = grn.PurchaseVoucherGRDate,
                        StockRegisterSIN = item.PurchaseVoucherDetailsQuantity,
                        StockRegisterMaterialID = item.PurchaseVoucherDetailsMaterialId,
                    });
                }
                _context.StockRegister.AddRange(sr);

                UserTracking trackingData = new UserTracking();
                trackingData.UserTrackingUserUserId = 1;
                trackingData.UserTrackingUserVpAction = "Insert";
                trackingData.UserTrackingUserVpNo = grn.PurchaseVoucherVoucherNo;
                trackingData.UserTrackingUserChangeDt = DateTime.Now;
                trackingData.UserTrackingUserChangeTime = DateTime.Now;
                trackingData.UserTrackingUserVpType = "GRN";
                _context.UserTracking.Add(trackingData);
                _context.SaveChanges();
                return this.GetAllGRN();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public VouchersNumbers GenerateVoucherNo(DateTime? VoucherGenDate)
        {
            try
            {
                int vnoMaxVal = Convert.ToInt32(_voucherNumbersRepository.GetAsQueryable()
                                                        .Where(x => x.VouchersNumbersVNoNu > 0 && x.VouchersNumbersVType == "PurchaseVoucher")
                                                        .DefaultIfEmpty()
                                                        .Max(o => o == null ? 0 : o.VouchersNumbersVNoNu)) + 1;
                var voucherNo = "GRN" + vnoMaxVal;
                var voucherNumber = _context.VouchersNumbers.AsNoTracking().Where(o => o.VouchersNumbersVType == "PurchaseVoucher").ToList().OrderByDescending(o => o.VouchersNumbersVNoNu).FirstOrDefault();
                if (voucherNumber != null)
                {
                    vnoMaxVal = (int)voucherNumber.VouchersNumbersVNoNu + 1;
                    Console.WriteLine(vnoMaxVal);
                    VouchersNumbers vouchersNumbers = new VouchersNumbers
                    {
                        VouchersNumbersVNo = prefix + vnoMaxVal,
                        VouchersNumbersVNoNu = vnoMaxVal,
                        VouchersNumbersVType = "PurchaseVoucher",
                        VouchersNumbersFsno = 1,
                        VouchersNumbersStatus = AccountStatus.Pending,
                        VouchersNumbersVoucherDate = DateTime.Now
                    };
                    _voucherNumbersRepository.Insert(vouchersNumbers);
                    return vouchersNumbers;
                }
                else
                {
                    VouchersNumbers vouchersNumbers = new VouchersNumbers
                    {
                        VouchersNumbersVNo = prefix + vnoMaxVal,
                        VouchersNumbersVNoNu = vnoMaxVal,
                        VouchersNumbersVType = "PurchaseVoucher",
                        VouchersNumbersFsno = 1,
                        VouchersNumbersStatus = AccountStatus.Pending,
                        VouchersNumbersVoucherDate = DateTime.Now

                    };
                    _voucherNumbersRepository.Insert(vouchersNumbers);
                    return vouchersNumbers;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IQueryable DeleteGRN(PurchaseVoucher grn)
        {
            try
            {
                VouchersNumbers vs = _voucherNumbersRepository.GetAll().FirstOrDefault(k => k.VouchersNumbersVNo == grn.PurchaseVoucherVoucherNo);
                if (vs != null)
                {
                    vs.VouhersNumbersDelStatus = true;
                    _voucherNumbersRepository.Update(vs);
                }
        
                PurchaseVoucher pv = _purchaseVoucherRepository.GetAll().FirstOrDefault(k => k.PurchaseVoucherVoucherNo == grn.PurchaseVoucherVoucherNo);
                if (pv != null)
                {
                    pv.PurchaseVoucherDelStatus = true;
                    _purchaseVoucherRepository.Update(pv);
                }
        
                List<PurchaseVoucherDetails> pvd = _purchaseVoucherDetailsRepository.GetAll().Where(k => k.PurchaseVoucherDetailsVoucherNo == grn.PurchaseVoucherVoucherNo).ToList();
                foreach (var item in pvd)
                {
                    item.PurchaseVoucherDetailsDelStatus = false;
                }
                _purchaseVoucherDetailsRepository.UpdateList(pvd);
                List<StockRegister> sr = _stockRegisterRepository.GetAll().Where(k => k.StockRegisterRefVoucherNo == grn.PurchaseVoucherVoucherNo).ToList();
                foreach (var item in sr)
                {
                    item.StockRegisterDelStatus = false;
                }
                _stockRegisterRepository.UpdateList(sr);
                List<AccountsTransactions> at = _accountTransactionRepository.GetAll().Where(k => k.AccountsTransactionsVoucherNo == grn.PurchaseVoucherVoucherNo).ToList();
                foreach (var item in at)
                {
                    item.AccountstransactionsDelStatus = false;
                }
                _accountTransactionRepository.UpdateList(at);

                UserTracking trackingData = new UserTracking();
                trackingData.UserTrackingUserUserId = 1;
                trackingData.UserTrackingUserVpAction = "Delete";
                // trackingData.UserTrackingUserVpNo = grn.PurchaseVoucherVoucherNo;
                trackingData.UserTrackingUserChangeDt = DateTime.Now;
                trackingData.UserTrackingUserChangeTime = DateTime.Now;
                trackingData.UserTrackingUserVpType = "GRN";
                _context.UserTracking.Add(trackingData);
                _context.SaveChanges();
                return this.GetAllGRN();
            }
            catch (Exception)
            {
                throw;
            }
        }


        public IEnumerable<GRNResponse> loadPo(GRNRequest obj)
        {
            try
            {
                var x = _grnRepository.GetBySP($"EXEC LoadPoList").Where(x => x.Suppliers_INS_Name.Equals(obj.PurchaseID)).ToList();
                return x;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }


    }
}
