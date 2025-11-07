using Inspire.Erp.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Inspire.Erp.Application.General
{
    public interface IGeneralService
    {
        Task<List<ReportType>> GetReportByTypeAsync(string type);
    }
}
