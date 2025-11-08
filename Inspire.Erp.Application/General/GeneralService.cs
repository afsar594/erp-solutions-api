using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Inspire.Erp.Application.General
{
    public class GeneralService : IGeneralService
    {
        private readonly IRepository<ReportType> _repository;

        public GeneralService(IRepository<ReportType> repository)
        {
            _repository = repository;
        }
        public async Task<List<ReportType>> GetReportByTypeAsync(string type)
        {
            try
            {
                var reportTypess = await _repository.GetAsQueryable().Where(x => x.ReportTypeFileName == type && x.ReportTypeIsVisible == true).ToListAsync();

                return reportTypess;
            }
            catch (Exception)
            {
                throw;
            }
        }



    }
}
