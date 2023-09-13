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

        public async Task<bool> updateGRN(PurchaseVoucher grn, List<AccountsTransactions> accountsTransactions)
        {
            try
            {
                foreach (var k in accountsTransactions)
                {
                    k.AccountsTransactionsVoucherNo = grn.PurchaseVoucherVoucherNo;
                    k.AccountsTransactionsVoucherType = VoucherType.PurchaseVoucher_TYPE;
                    k.AccountsTransactionsStatus = AccountStatus.Approved;
                }
                _context.AccountsTransactions.UpdateRange(accountsTransactions);
                _context.SaveChanges();

                _purchaseVoucherRepository.Update(grn);
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
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> saveGRN(PurchaseVoucher grn, List<AccountsTransactions> accountsTransactions)
        {
            try
            {
                grn.PurchaseVoucherDelStatus = false;
                foreach (var k in accountsTransactions)
                {
                    k.AccountsTransactionsVoucherNo = grn.PurchaseVoucherVoucherNo;
                    k.AccountsTransactionsVoucherType = VoucherType.PurchaseVoucher_TYPE;
                    k.AccountsTransactionsStatus = AccountStatus.Approved;
                }
                _context.AccountsTransactions.AddRange(accountsTransactions);
                _context.SaveChanges();

                _purchaseVoucherRepository.Insert(grn);
                foreach (var item in grn.PurchaseVoucherDetails)
                {
                    item.PurchaseVoucherDetailsVoucherNo = grn.PurchaseVoucherVoucherNo;
                }

                _purchaseVoucherDetailsRepository.InsertList(grn.PurchaseVoucherDetails);
                /////////////Model check karna hai 
                StockRegister sr = new StockRegister();
                sr.StockRegisterStoreID = 0;
                sr.StockRegisterRefVoucherNo = grn.PurchaseVoucherVoucherNo;
                sr.StockRegisterAssignedDate = DateTime.Now;
                sr.StockRegisterTransType = grn.PurchaseVoucherPurchaseType;
                sr.StockRegisterFCAmount = grn.PurchaseVoucherFCActualAmount;
                sr.StockRegisterFcRate = grn.PurchaseVoucherFcRate;
                sr.StockRegisterStatus = "A";
                sr.StockRegisterTransType = "PurchaseVoucher";
                sr.StockRegisterJobID = grn.PurchaseVoucherJobId;
                sr.StockRegisterLocationID = grn.PurchaseVoucherLocationId;
                sr.StockRegisterFSNO = grn.PurchaseVoucherFsno;
                _stockRegisterRepository.Insert(sr);

                UserTracking trackingData = new UserTracking();
                trackingData.UserTrackingUserUserId = 1;
                trackingData.UserTrackingUserVpAction = "Insert";
                trackingData.UserTrackingUserVpNo = grn.PurchaseVoucherVoucherNo;
                trackingData.UserTrackingUserChangeDt = DateTime.Now;
                trackingData.UserTrackingUserChangeTime = DateTime.Now;
                trackingData.UserTrackingUserVpType = "GRN";
                _context.UserTracking.Add(trackingData);
                _context.SaveChanges();
                return true;
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
                                                        .Where(x => x.VouchersNumbersVNoNu > 0 && x.VouchersNumbersVType == "GRN")
                                                        .DefaultIfEmpty()
                                                        .Max(o => o == null ? 0 : o.VouchersNumbersVNoNu)) + 1;
                var voucherNo = "GRN" + vnoMaxVal;
                var voucherNumber = _context.VouchersNumbers.AsNoTracking().Where(o => o.VouchersNumbersVType == "GRN_Voucher").ToList().OrderByDescending(o => o.VouchersNumbersVNoNu).FirstOrDefault();
                if (voucherNumber != null)
                {
                    vnoMaxVal = (int)voucherNumber.VouchersNumbersVNoNu + 1;
                    Console.WriteLine(vnoMaxVal);
                    VouchersNumbers vouchersNumbers = new VouchersNumbers
                    {
                        VouchersNumbersVNo = prefix + vnoMaxVal,
                        VouchersNumbersVNoNu = vnoMaxVal,
                        VouchersNumbersVType = "GRN_Voucher",
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
                        VouchersNumbersVType = "GRN_Voucher",
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

        public IEnumerable<GRNVoucherRquest> saveGRNVoucher(List<GRNVoucherRquest> obj)
        {
            try
            {
                VouchersNumbers vs = this.GenerateVoucherNo(DateTime.Now);

                GRNRequest grn = new GRNRequest();
                grn.V_NO = prefix + vs.VouchersNumbersVNoNu;
                grn.V_NO_NU = vs.VouchersNumbersVNoNu.ToString();
                grn.VoucherType = vs.VouchersNumbersVType;
                grn.GRDate = vs.VouchersNumbersVType;

                PurchaseVoucher pv = new PurchaseVoucher();
                PurchaseVoucherDetails pvd = new PurchaseVoucherDetails();
                List<PurchaseVoucherDetails> pvdList = new List<PurchaseVoucherDetails>();
                StockRegister sr = new StockRegister();
                List<StockRegister> srList = new List<StockRegister>();
                // AccountsTransactions at = new AccountsTransactions();

                // //////////   Accoutn Transations Record Insertion   ///////////
                // at.AccountsTransactionsVoucherNo = grn.V_NO;
                // at.AccountsTransactionsVoucherType = grn.VoucherType.Trim();
                // at.AccountsTransactionsStatus = "P";
                // at.AccountsTransactionsAccNo = "null";
                // at.AccountsTransactionsCredit = 0;
                // at.AccountsTransactionsDebit = 0;
                // at.AccountsTransactionsAllocBalance = 0;
                // at.AccountsTransactionsAllocCredit = 0;
                // at.AccountsTransactionsAllocDebit = 0;
                // at.AccountsTransactionsAllocUpdateBal = 0;
                // at.AccountsTransactionsFsno = 0;
                // at.AccountsTransactionsTransDate = DateTime.Now;
                // at.AccountsTransactionsUserId = 0;
                // at.AccountsTransactionsTstamp = DateTime.Now;
                // _accountTransactionRepository.Insert(at);


                //////////   Purchase Voucher Record Insertion   ///////////

                pv.PurchaseVoucherPoNo = int.Parse(grn.V_NO_NU);
                pv.PurchaseVoucherVoucherNo = prefix + grn.V_NO_NU;
                pv.PurchaseVoucherActualAmount = decimal.Parse(obj[0].netAmt.Trim()) + decimal.Parse(obj[0].disc.Trim());
                pv.PurchaseVoucherNetAmount = decimal.Parse(obj[0].netAmt.Trim());
                pv.PurchaseVoucherActualAmount = decimal.Parse(obj[0].netAmt.Trim());
                pv.PurchaseVoucherCurrencyId = int.Parse(obj[0].currency.Trim());
                pv.PurchaseVoucherBatchCode = obj[0].batch.Trim();
                pv.PurchaseVoucherCashSupplierName = obj[0].supplier.Trim();
                pv.PurchaseVoucherDescription = obj[0].pvDesc.Trim();
                pv.PurchaseVoucherDisAmount = int.Parse(obj[0].disc.Trim());
                pv.PurchaseVoucherGRNo = obj[0].grnNo.Trim();
                pv.PurchaseVoucherGRDate = DateTime.Parse(obj[0].grnDate.Trim());
                pv.PurchaseVoucherLPONo = obj[0].lpoNo.Trim();
                pv.PurchaseVoucherLPODate = DateTime.Parse(obj[0].lpoDate.Trim());
                pv.PurchaseVoucherQuatationNo = obj[0].qoutNo.Trim();
                pv.PurchaseVoucherQuatationDate = DateTime.Parse(obj[0].qoutDate.Trim());
                pv.PurchaseVoucherLocationId = int.Parse(obj[0].location.Trim());

                pv.PurchaseVoucherDiscountPercentage =
                    decimal.Parse(((float.Parse(obj[0].disc.Trim()) / (float.Parse(obj[0].netAmt.Trim()) + float.Parse(obj[0].disc.Trim())) == 0 ? 1 : (float.Parse(obj[0].netAmt.Trim()) + float.Parse(obj[0].disc.Trim()))) * 100).ToString());
                pv.PurchaseVoucherExcludeVat = false;
                pv.PurchaseVoucherActualAmount = decimal.Parse(obj[0].netAmt.Trim()) + decimal.Parse(obj[0].disc.Trim());
                pv.PurchaseVoucherFCDisAmount = decimal.Parse(obj[0].disc.Trim());
                pv.PurchaseVoucherFCNetAmount = decimal.Parse(obj[0].netAmt.Trim());
                pv.PurchaseVoucherPoNo = int.Parse(grn.V_NO_NU.Trim());
                pv.PurchaseVoucherPurchaseDate = DateTime.Parse(obj[0].voucherDate.Trim());
                pv.PurchaseVoucherPurID = int.Parse(obj[0].poId.Trim());
                // pv.PurchaseVoucherPurchaseId = obj[0].poId.Trim();
                pv.PurchaseVoucherPurchaseIdNum = int.Parse(obj[0].poId.Trim());
                pv.PurchaseVoucherPurchaseRef = obj[0].refNo.Trim();
                pv.PurchaseVoucherPurchaseType = obj[0].purchaseType;
                pv.PurchaseVoucherStatus = "P";
                pv.PurchaseVoucherSupplyInvoiceNo = obj[0].supplierInvoiceNo.Trim();
                pv.PurchaseVoucherVatAmount = decimal.Parse(obj[0].vatAmt.Trim());
                pv.PurchaseVoucherVatPercentage = decimal.Parse(obj[0].vatPer.Trim());
                pv.PurchaseVoucherDelStatus = false;
                _purchaseVoucherRepository.Insert(pv);
                for (int i = 0; i < obj.Count; i++)
                {
                    //////////   Purchase Voucher Details Record Insertion   ///////////

                    pvd.PurchaseVoucherDetailsGrnNo = grn.V_NO.Trim();
                    pvd.PurchaseVoucherDetailsAmount = decimal.Parse(obj[i].netAmt.Trim());
                    pvd.PurchaseVoucherDetailsBatchCode = obj[i].batch.Trim();
                    pvd.PurchaseVoucherDetailsDiscountAmoutPurchase = decimal.Parse(obj[i].disc.Trim());
                    pvd.PurchaseVoucherDetailsFcAmount = decimal.Parse(obj[i].netAmt.Trim());
                    pvd.PurchaseVoucherDetailsFcSmRate = decimal.Parse(obj[i].rate.Trim());
                    pvd.PurchaseVoucherDetailsRate = decimal.Parse(obj[i].rate.Trim());
                    pvd.PurchaseVoucherDetailsLoacationId = int.Parse(obj[i].location.Trim());
                    pvd.PurchaseVoucherDetailsMaterialId = int.Parse(obj[i].itemId.Trim());
                    pvd.PurchaseVoucherDetailsLpoId = int.Parse(obj[i].lpoNo.Trim());
                    // pvd.PurchaseVoucherDetailsPurchaseId.ToString() = obj[i].poId.Trim();
                    pvd.PurchaseVoucherDetailsQuantity = decimal.Parse(obj[i].purchaseQty.Trim());
                    pvd.PurchaseVoucherDetailsRemarks = obj[i].pvDesc.Trim();
                    pvd.PurchaseVoucherDetailsSalesPrice1 = decimal.Parse(obj[i].price.Trim());
                    pvd.PurchaseVoucherDetailsUnit = obj[i].unit.Trim();
                    pvd.PurchaseVoucherDetailsUnitId = int.Parse(obj[i].unitId.Trim());
                    pvd.PurchaseVoucherDetailsVatAmount = decimal.Parse(obj[i].vatAmt.Trim());
                    pvd.PurchaseVoucherDetailsDelStatus = false;
                    pvdList.Add(pvd);
                    //////////   Stock Register Record Insertion   ///////////
                    sr.StockRegisterRefVoucherNo = grn.V_NO;
                    sr.StockRegisterAssignedDate = DateTime.Now;
                    sr.StockRegisterTransType = grn.VoucherType.Trim();
                    sr.StockRegisterAmount = decimal.Parse(obj[i].netAmt.Trim());
                    sr.StockRegisterAssignedDate = DateTime.Now;
                    sr.StockRegisterBatchCode = obj[i].batch.Trim();
                    sr.StockRegisterFCAmount = decimal.Parse(obj[i].netAmt.Trim());
                    sr.StockRegisterFcRate = decimal.Parse(obj[i].rate.Trim());
                    sr.StockRegisterLocationID = int.Parse(obj[i].location.Trim());
                    sr.StockRegisterMaterialID = int.Parse(obj[i].itemId.Trim());
                    sr.StockRegisterPurchaseID = obj[i].poId.Trim();
                    sr.StockRegisterQuantity = decimal.Parse(obj[i].issueQty.Trim());
                    sr.StockRegisterRate = decimal.Parse(obj[i].rate.Trim());
                    sr.StockRegisterRemarks = obj[i].pvDesc.Trim();
                    sr.StockRegisterSIN = 0;
                    sr.StockRegisterSout = decimal.Parse(obj[i].issueQty.Trim());
                    sr.StockRegisterStatus = "A";
                    sr.StockRegisterTransType = "PurchaseVoucher";
                    sr.StockRegisterUnitID = int.Parse(obj[i].unitId.Trim());
                    sr.StockRegisterUsedQTY = decimal.Parse(obj[i].issueQty.Trim());
                    sr.StockRegisterDelStatus = false;
                    srList.Add(sr);
                }
                _stockRegisterRepository.InsertList(srList);
                _purchaseVoucherDetailsRepository.InsertList(pvdList);
                return obj;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> DeleteGRN(GRNRequest grn)
        {
            try
            {
                VouchersNumbers vs = _voucherNumbersRepository.GetAsQueryable().Where(k => k.VouchersNumbersVNoNu == int.Parse(grn.V_NO)).SingleOrDefault();
                if (vs == null)
                {
                    return false;
                }
                vs.VouhersNumbersDelStatus = true;
                _voucherNumbersRepository.Update(vs);
                PurchaseVoucher pv = _purchaseVoucherRepository.GetAsQueryable().Where(k => k.PurchaseVoucherPoNo == int.Parse(grn.V_NO)).SingleOrDefault();
                if (vs == null)
                {
                    return false;
                }
                pv.PurchaseVoucherDelStatus = true;
                _purchaseVoucherRepository.Update(pv);
                PurchaseVoucherDetails pvd = _purchaseVoucherDetailsRepository.GetAsQueryable().Where(k => k.PurchaseVoucherDetailsGrnNo == grn.V_NO).SingleOrDefault();
                if (vs == null)
                {
                    return false;
                }
                pvd.PurchaseVoucherDetailsDelStatus = true;
                _purchaseVoucherDetailsRepository.Update(pvd);
                StockRegister sr = _stockRegisterRepository.GetAsQueryable().Where(k => k.StockRegisterRefVoucherNo == grn.V_NO).SingleOrDefault();
                if (vs == null)
                {
                    return false;
                }
                sr.StockRegisterDelStatus = true;
                _stockRegisterRepository.Update(sr);
                AccountsTransactions at = _accountTransactionRepository.GetAsQueryable().Where(k => k.AccountsTransactionsVoucherNo == grn.V_NO).SingleOrDefault();
                if (vs == null)
                {
                    return false;
                }
                at.AccountstransactionsDelStatus = true;
                _accountTransactionRepository.Update(at);

                UserTracking trackingData = new UserTracking();
                trackingData.UserTrackingUserUserId = 1;
                trackingData.UserTrackingUserVpAction = "Delete";
                // trackingData.UserTrackingUserVpNo = grn.PurchaseVoucherVoucherNo;
                trackingData.UserTrackingUserChangeDt = DateTime.Now;
                trackingData.UserTrackingUserChangeTime = DateTime.Now;
                trackingData.UserTrackingUserVpType = "GRN";
                _context.UserTracking.Add(trackingData);
                _context.SaveChanges();
                return true;
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
