using Inspire.Erp.Application.Account.Interface;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Modals;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using System.Threading.Tasks;
using static Inspire.Erp.Domain.Modals.PDCResponse;
using Microsoft.EntityFrameworkCore;
using Inspire.Erp.Domain.Enums;

namespace Inspire.Erp.Application.Account.Implementation
{
    public class PDCService:IPDC
    {
        private readonly IRepository<PDCGetList> _PDCGetListResponse;
        private readonly IRepository<PDCPostedList> _PDCPostedListRepo;
        private readonly IRepository<MasterAccountsTable> _MasterAccountsTableRepo;
        private readonly IRepository<ProgramSettings> _programsettingsRepository;
        private readonly IRepository<PdcDetails> _pdcDetails;
        private IRepository<VouchersNumbers> _voucherNumbersRepository;
        private IRepository<PdcVoucher> _pdcVoucherRepository;
        private IRepository<AccountsTransactions> _accountTransactionRepository;
        private IRepository<PdcVoucherDetails> _pdcVoucherDetailsRepository;


        private static string prefix;
        public PDCService(
            IRepository<MasterAccountsTable> MasterAccountsTableRepo
            , IRepository<PDCGetList> PDCGetListResponse,
            IRepository<PDCPostedList> PDCPostedListRepo,
            IRepository<PdcDetails> pdcDetails,
            IRepository<ProgramSettings> programsettingsRepository,
            IRepository<VouchersNumbers> voucherNumbersRepository,
            IRepository<PdcVoucher> pdcVoucherRepository,
            IRepository<AccountsTransactions> accountTransactionRepository,
            IRepository<PdcVoucherDetails> pdcVoucherDetailsRepository)
        {
            _MasterAccountsTableRepo = MasterAccountsTableRepo;
            _programsettingsRepository = programsettingsRepository;
            _PDCPostedListRepo = PDCPostedListRepo;
            _PDCGetListResponse = PDCGetListResponse;
            _pdcDetails = pdcDetails;
            _voucherNumbersRepository = voucherNumbersRepository;
            _pdcVoucherRepository = pdcVoucherRepository;
            _accountTransactionRepository = accountTransactionRepository;
            _pdcVoucherDetailsRepository = pdcVoucherDetailsRepository;
        }

        public async Task<IEnumerable<PDCPostedList>> PDCPostedList()
        {
            try
            {
                //C
                var result =await _pdcDetails.GetAsQueryable().Where(x=>x.PDC_Details_Cheque_Status=="C").Select(x=>new PDCPostedList
                {
                    BAccNo=x.PDC_Details_Bank_Account_No,
                    CAmount=x.PDC_Details_Cheque_Amount,
                    CBName=x.PDC_Details_Cheque_Bank_Name,
                    CNO=x.PDC_Details_Cheque_No,
                    PartyName=x.PDC_Details_PartyAccNo,
                    PDate=x.PDC_Details_PDC_Date,
                    PID=x.PDC_Details_ID,
                    POSTDt=x.PDC_Details_PDC_Voucher_Date,
                    TDate=x.PDC_Details_Trans_Date,
                    VNO=x.PDC_Details_V_No,
                    VType=x.PDC_Details_V_Type,
                    PDCVNO=x.PDC_Details_V_No,
                }).ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<PDCGetList>> PDCGetList()
        {
            try
            {
                //NC
                var result = await _pdcDetails.GetAsQueryable().Where(x => x.PDC_Details_Cheque_Status == "NC").Select(x => new PDCGetList
                {
                    BAccNo = x.PDC_Details_Bank_Account_No,
                    CAmount = x.PDC_Details_Cheque_Amount,
                    CBName = x.PDC_Details_Cheque_Bank_Name,
                    CNO = x.PDC_Details_Cheque_No,
                    PartyName = x.PDC_Details_PartyAccNo,
                    PDate = x.PDC_Details_PDC_Date,
                    PID = x.PDC_Details_ID,
                    AccName=!string.IsNullOrEmpty(x.PDC_Details_PartyAccNo)? _MasterAccountsTableRepo.GetAsQueryable().FirstOrDefault(c => c.MaAccNo == x.PDC_Details_PartyAccNo).MaAccName :"",
                    TDate = x.PDC_Details_Trans_Date,
                    VNO = x.PDC_Details_V_No,
                    VType = x.PDC_Details_V_Type,
                    Narration = x.PDC_Details_PDC_Voucher_Narration
                }).ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<PDCGetList>> PDCGetReturnList(PDCFilters obj)
        {
            try
            {
                //NC

                string s = "EXEC getFilteredPDCList @postDate='" + obj.postDate + "',@dateFrom='" + obj.dateFrom + "'" +
     ",@dateTo='" + obj.dateTo + "',@pdcType='" + obj.pdcType + "',@keyWords='" +
     "" + obj.keyWords[0] + "',@searchType='" + obj.searchType + "'";
                var result = await _pdcDetails.GetAsQueryable()
                   .Where(x => (obj.postDate != null ? x.PDC_Details_Trans_Date == obj.postDate : true) && (x.PDC_Details_PDC_Date >= obj.dateFrom && x.PDC_Details_PDC_Date <= obj.dateFrom) &&
                   (obj.keyWords != null && obj.keyWords.Count > 0 && !string.IsNullOrEmpty(obj.keyWords[0]) ? x.PDC_Details_Bank_Account_No.Contains(obj.keyWords[0]) : true) &&
                   (!string.IsNullOrEmpty(obj.pdcType) ? x.PDC_Details_Trans_Type == obj.pdcType : true))
                   .Select(x => new PDCGetList
                   {
                       BAccNo = x.PDC_Details_Bank_Account_No,
                       CAmount = x.PDC_Details_Cheque_Amount,
                       CBName = x.PDC_Details_Cheque_Bank_Name,
                       CNO = x.PDC_Details_Cheque_No,
                       PartyName = x.PDC_Details_PartyAccNo,
                       PDate = x.PDC_Details_PDC_Date,
                       PID = x.PDC_Details_ID,
                       AccName = !string.IsNullOrEmpty(x.PDC_Details_PartyAccNo) ? _MasterAccountsTableRepo.GetAsQueryable().FirstOrDefault(c => c.MaAccNo == x.PDC_Details_PartyAccNo).MaAccName : "",
                       TDate = x.PDC_Details_Trans_Date,
                       VNO = x.PDC_Details_V_No,
                       VType = x.PDC_Details_V_Type,
                       Narration = x.PDC_Details_PDC_Voucher_Narration
                   }).ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<PDCGetList>> getFilteredPDCList(PDCFilters obj)
        {
            try
            {
                string s = "EXEC getFilteredPDCList @postDate='" + obj.postDate+ "',@dateFrom='" + obj.dateFrom + "'" +
     ",@dateTo='" + obj.dateTo + "',@pdcType='" + obj.pdcType + "',@keyWords='" +
     "" + obj.keyWords[0] + "',@searchType='" + obj.searchType + "'";
               
                 var result = await _pdcDetails.GetAsQueryable()
                    .Where(x => /*(obj.postDate != null?x.PDC_Details_Trans_Date== obj.postDate:true)
                                 * && (x.PDC_Details_PDC_Date>=obj.dateFrom  && x.PDC_Details_PDC_Date <= obj.dateFrom) &&*/
                    //(obj.keyWords != null && obj.keyWords.Count >0 && !string.IsNullOrEmpty(obj.keyWords[0]) ? x.PDC_Details_Bank_Account_No.Contains(obj.keyWords[0]):true) && 
                    (!string.IsNullOrEmpty(obj.pdcType) ? x.PDC_Details_Trans_Type== obj.pdcType :true) && x.PDC_Details_Cheque_Status == "NC")
                    .Select(x => new PDCGetList
                {
                    BAccNo = x.PDC_Details_Bank_Account_No,
                    CAmount = x.PDC_Details_Cheque_Amount,
                    CBName = x.PDC_Details_Cheque_Bank_Name,
                    CNO = x.PDC_Details_Cheque_No,
                    PartyName = x.PDC_Details_PartyAccNo,
                    PDate = x.PDC_Details_PDC_Date,
                    PID = x.PDC_Details_ID,
                    AccName = !string.IsNullOrEmpty(x.PDC_Details_PartyAccNo) ? _MasterAccountsTableRepo.GetAsQueryable().FirstOrDefault(c => c.MaAccNo == x.PDC_Details_PartyAccNo).MaAccName : "",
                    TDate = x.PDC_Details_Trans_Date,
                    VNO = x.PDC_Details_V_No,
                    VType = x.PDC_Details_V_Type,
                    Narration = x.PDC_Details_PDC_Voucher_Narration
                    }).ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<List<BankNamesModel>> GetBankNames()
        {
            try
            {
                List<BankNamesModel> l = new List<BankNamesModel>();
                 l = (from an in _MasterAccountsTableRepo.GetAsQueryable()
                            where (an.MaStatus ?? "R") == "R"
                            select new BankNamesModel
                            {
                                AccName = an.MaAccName
                            }).ToList();
                return l;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<PDCGetList>> PostPDC(List<PDCGetList> list)
        {
            try
            {
                _pdcVoucherRepository.BeginTransaction();
                List<PdcVoucher> pdcVouchers = new List<PdcVoucher>();
                List<AccountsTransactions> accountsTransactions = new List<AccountsTransactions>();
                List<PdcDetails> pdcDetails = new List<PdcDetails>();
                List<PdcVoucherDetails> pdcVoucherDetails = new List<PdcVoucherDetails>(); 
                foreach (var item in list)
                {
                    if (item.Pdc)
                    {
                        string pdcVoucherNumber = this.GenerateVoucherNo(item.PDate).VouchersNumbersVNo;
                        //int maxcount = 0;
                        //maxcount = Convert.ToInt32(
                        //    _pdcVoucherRepository.GetAsQueryable()
                        //    .DefaultIfEmpty().Max(o => o == null ? 0 : o.PdcVoucherSno) + 1);
                        pdcVouchers.Add(new PdcVoucher()
                        {
                            PdcVoucherVid = pdcVoucherNumber,
                            PdcVoucherRef = item.VNO,
                            PdcVoucherPdcDate = item.PDate,
                            PdcVoucherPdcAmount = item.CAmount,
                            PdcVoucherFcPdcAmount = item.CAmount,
                            PdcVoucherDelStatus = false,
                            PdcVoucherNarration = item.Narration,
                            
                        });


                        accountsTransactions.Add(new AccountsTransactions()
                        {
                            AccountsTransactionsAccNo = item.BAccNo,
                            AccountsTransactionsCheqNo = item.CNO,
                            AccountsTransactionsCredit = (decimal)item.CAmount,
                            AccountsTransactionsFcCredit = (decimal)item.CAmount,
                            AccountsTransactionsVoucherNo = pdcVoucherNumber,
                            AccountsTransactionsVoucherType = VoucherType.PDC_TYPE,
                            AccountsTransactionsStatus = AccountStatus.Approved,
                            AccountsTransactionsTransDate = DateTime.Now,
                            AccountsTransactionsParticulars = item.Narration
                           
                        });

                        var pd = _pdcDetails.GetAsQueryable().Where(x => x.PDC_Details_ID == item.PID).FirstOrDefault();
                        if(pd != null)
                        {
                            if (item.Type == "Post")
                                pd.PDC_Details_Cheque_Status = "C";
                            else
                                pd.PDC_Details_Cheque_Status = "R";
                        }
                        _pdcDetails.Update(pd);





                        pdcVoucherDetails.Add(new PdcVoucherDetails()
                        {
                            PdcVoucherDetailsAccno = item.BAccNo,
                            PdcVoucherDetailsAmount = item.CAmount,
                            PdcVoucherDetailsFcAmount = item.CAmount,
                            PdcVoucherDetailsNarration = item.Narration,
                            PdcVoucherDetailsDelStatus = false,
                        });



                    }
                }
                //if(accountsTransactions.Count > 0)
                //{
                //    _accountTransactionRepository.InsertList(accountsTransactions);
                //}
                if(pdcVouchers.Count > 0)
                {
                    _pdcVoucherRepository.InsertList(pdcVouchers);
                }
                if (pdcVoucherDetails.Count > 0)
                {
                    _pdcVoucherDetailsRepository.InsertList(pdcVoucherDetails);
                }
               
                _pdcVoucherRepository.TransactionCommit();
                return list;
            }
            catch (Exception ex)
            {
                _pdcVoucherRepository.TransactionRollback();
                throw;
            }
                       
        }

        public VouchersNumbers GenerateVoucherNo(DateTime? VoucherGenDate)
        {
            try
            {
                var prefix = this._programsettingsRepository.GetAsQueryable().Where(k => k.ProgramSettingsKeyValue == Prefix.PDC_Voucher_Prefix).FirstOrDefault().ProgramSettingsTextValue;
                int vnoMaxVal = Convert.ToInt32(_voucherNumbersRepository.GetAsQueryable()
                                                        .Where(x => x.VouchersNumbersVNoNu > 0 && x.VouchersNumbersVType == VoucherType.PDC_TYPE)
                                                        .DefaultIfEmpty()
                                                        .Max(o => o == null ? 0 : o.VouchersNumbersVNoNu)) + 1;
                VouchersNumbers vouchersNumbers = new VouchersNumbers
                {
                    VouchersNumbersVNo = prefix + vnoMaxVal,
                    VouchersNumbersVNoNu = vnoMaxVal,
                    VouchersNumbersVType = VoucherType.PDC_TYPE,
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

        public async Task<IEnumerable<PDCPostedList>> PDCReturnLists()
        {
            try
            {
                //R
                var result = await _pdcDetails.GetAsQueryable().Where(x => x.PDC_Details_Cheque_Status == "R").Select(x => new PDCPostedList
                {
                    BAccNo = x.PDC_Details_Bank_Account_No,
                    CAmount = x.PDC_Details_Cheque_Amount,
                    CBName = x.PDC_Details_Cheque_Bank_Name,
                    CNO = x.PDC_Details_Cheque_No,
                    PartyName = x.PDC_Details_PartyAccNo,
                    PDate = x.PDC_Details_PDC_Date,
                    PID = x.PDC_Details_ID,
                    POSTDt = x.PDC_Details_PDC_Voucher_Date,
                    TDate = x.PDC_Details_Trans_Date,
                    VNO = x.PDC_Details_V_No,
                    VType = x.PDC_Details_V_Type,
                }).ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
