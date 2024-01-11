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

namespace Inspire.Erp.Application.Account.Implementation
{
    public class PDCService:IPDC
    {
        private readonly IRepository<PDCGetList> _PDCGetListResponse;
        private readonly IRepository<PDCPostedList> _PDCPostedListRepo;
        private readonly IRepository<MasterAccountsTable> _MasterAccountsTableRepo;
        private readonly IRepository<ProgramSettings> _programsettingsRepository;
        private readonly IRepository<PdcDetails> _pdcDetails;
        private static string prefix;
        public PDCService(
            IRepository<MasterAccountsTable> MasterAccountsTableRepo
            , IRepository<PDCGetList> PDCGetListResponse,
            IRepository<PDCPostedList> PDCPostedListRepo,
            IRepository<PdcDetails> pdcDetails,
            IRepository<ProgramSettings> programsettingsRepository)
        {
            _MasterAccountsTableRepo=MasterAccountsTableRepo;
            _programsettingsRepository = programsettingsRepository;
           _PDCPostedListRepo=PDCPostedListRepo;
            _PDCGetListResponse = PDCGetListResponse;
            _pdcDetails = pdcDetails;
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
                var result = _PDCGetListResponse.GetBySPParams($"{s}").ToList();
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
    }
}
