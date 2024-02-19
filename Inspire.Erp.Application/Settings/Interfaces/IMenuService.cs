using Inspire.Erp.Domain.Modals;
using Inspire.Erp.Domain.Models.Settings;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Inspire.Erp.Application.Settings.Interfaces
{
    public interface IMenuService
    {
        Task<Response<List<GetPrimeMenusResponse>>> GetMenusByWorkgroupId(int id = 0);
        Task<bool> SaveMenu();
        Task<bool> DeleteMenu();
        Task<bool> UpdateMenu();
    }
}
