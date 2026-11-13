using Dapper;
using Inspire.Erp.Infrastructure;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspire.Erp.Application.General
{
    public class GeneralSelectQueries : IGeneralSelectQueries
    {
        private readonly IDbConnection _dbConnection;
        public GeneralSelectQueries(IOptions<ApplicationSettings> applicationSettings)
        {
            var settings = applicationSettings.Value;
            _dbConnection = new SqlConnection(settings.ConnectionString);
        }
        public async Task<bool> GetBooleanValue(string id)
        {
            try
            {
                var sql = $@"select case when upper(WorkGroup_Master_WorkGroup_Name) = upper('Admin') 
                            then 'True' else 'False' end as result 
                            from WorkGroup_Master where WorkGroup_Master_WorkGroup_ID = {id}";
                var result = await _dbConnection.QueryFirstOrDefaultAsync<bool>(sql);

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
