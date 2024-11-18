using Inspire.Erp.Domain.DTO.Asset_Register;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Inspire.Erp.Application.Master.Interfaces
{
    public interface IAssetRegisterService
    {
        public Task<List<AssetRegisterDto>> GetAssetsRegisterAsync(DateTime? fromDate, DateTime? toDate, string searchText);
    }
}
