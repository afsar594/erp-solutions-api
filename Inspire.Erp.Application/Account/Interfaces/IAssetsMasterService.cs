using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Modals;
using Inspire.Erp.Domain.Modals.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Inspire.Erp.Application.Account.Interfaces
{
    public interface IAssetsMasterService
    {
        Task<Response<List<MasterAccountsTable>>> GetFixedAccountForAssetMaster(GenericGridViewModel model);
        Task<Response<List<MasterAccountsTable>>> GetRelativeAccountAssetMaster(string accNo);
    }
}
