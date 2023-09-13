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

namespace Inspire.Erp.Application.Account.Implementation
{
    public class PDCService:IPDC
    {
        private readonly IRepository<PDCGetList> _PDCGetListResponse;
        private readonly IRepository<PDCPostedList> _PDCPostedListRepo;
        private readonly IRepository<MasterAccountsTable> _MasterAccountsTableRepo;
        private readonly IRepository<ProgramSettings> _programsettingsRepository;
        private static string prefix;
        public PDCService(
            IRepository<MasterAccountsTable> MasterAccountsTableRepo
            , IRepository<PDCGetList> PDCGetListResponse,
            IRepository<PDCPostedList> PDCPostedListRepo,
            IRepository<ProgramSettings> programsettingsRepository)
        {
            _MasterAccountsTableRepo=MasterAccountsTableRepo;
            _programsettingsRepository = programsettingsRepository;
           _PDCPostedListRepo=PDCPostedListRepo;
            _PDCGetListResponse = PDCGetListResponse;
        }

        public async Task<IEnumerable<PDCPostedList>> PDCPostedList()
        {
            try
            {
                return _PDCPostedListRepo.GetBySP($"EXEC getPDCPostedList");
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
                return _PDCGetListResponse.GetBySP($"EXEC getPDCList");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
     //   public async Task<IEnumerable<PDCGetList>> getFilteredPDCList(PDCFilters obj)
     //   {
     //       try
     //       {
     ////           string s = "EXEC getFilteredPDCList @postDate='"+DateTime.Parse(obj.postDate.ToString())+"',@dateFrom='"+DateTime.Parse(obj.dateFrom.ToString())+"'" +
					////",@dateTo='"+DateTime.Parse(obj.dateTo.ToString())+"',@pdcType='"+obj.pdcType+"',@keyWords='" +
					////""+obj.keyWords[0]+"',@searchType='"+obj.searchType+"'";
     //           return _PDCGetListResponse.GetBySPParams($"EXEC getPDCList",obj);
     //       }
     //       catch (Exception ex)
     //       {
     //           throw ex;
     //       }
     //   }
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
