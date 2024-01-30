using Inspire.Erp.Application.Account.Interfaces;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Modals.Common;
using Inspire.Erp.Domain.Modals;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Inspire.Erp.Application.Account.Implementations
{
    public class AssetsMasterService : IAssetsMasterService
    {
        private readonly IRepository<MasterAccountsTable> _masterAccountTable;
        public AssetsMasterService(IRepository<MasterAccountsTable> masterAccountTable)
        {
            _masterAccountTable = masterAccountTable;
        }
        public async Task<Response<List<MasterAccountsTable>>> GetFixedAccountForAssetMaster(GenericGridViewModel model)
        {
            try
            {
                var response = new List<MasterAccountsTable>();
                var fixedAccount = await _masterAccountTable.GetAsQueryable().Where(x => x.MaAccName == "Fixed Asset").FirstOrDefaultAsync();
                var result = await _masterAccountTable.GetAsQueryable().Where(x => x.MaRelativeNo == fixedAccount.MaAccNo).ToListAsync();
                //var data = RecursionMasterAccountTable(result);
                return Response<List<MasterAccountsTable>>.Success(result, "Data found");
            }
            catch (Exception ex)
            {
                return Response<List<MasterAccountsTable>>.Fail(new List<MasterAccountsTable>(), ex.Message);
            }
        }
        public async Task<Response<List<MasterAccountsTable>>> GetRelativeAccountAssetMaster(string accNo)
        {
            try
            {
                var result = await _masterAccountTable.GetAsQueryable().Where(x => x.MaAccNo== accNo).ToListAsync();

                return Response<List<MasterAccountsTable>>.Success(result, "Data found");
            }
            catch (Exception ex)
            {
                return Response<List<MasterAccountsTable>>.Fail(new List<MasterAccountsTable>(), ex.Message);
            }
        }
        private List<MasterAccountsTable> RecursionMasterAccountTable(List<MasterAccountsTable> model)
        {
            var result = new List<MasterAccountsTable>();
            foreach (var account in model)
            {
                var masteraccount =  _masterAccountTable.GetAsQueryable().Where(x => x.MaRelativeNo == account.MaAccNo).ToList();
                if (masteraccount.Count >0)
                {
                    result.AddRange( RecursionMasterAccountTable(masteraccount));
                }
                result.Add(account);
            }
            return result;
        }
    }
}
