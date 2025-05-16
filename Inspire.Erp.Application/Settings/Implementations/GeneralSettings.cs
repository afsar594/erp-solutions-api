using Inspire.Erp.Application.Common;
using Inspire.Erp.Application.Settings.Interfaces;
using Inspire.Erp.Infrastructure;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace Inspire.Erp.Application.Settings.Implementations
{
    public class GeneralSettings : IGeneralSettings
    {
        private readonly IDbConnection _dbConnection;
        public GeneralSettings(IConfiguration config)
        {
            var settings = config.GetSection("ApplicationSettings").Get<ApplicationSettings>();
            _dbConnection = new SqlConnection(settings.ConnectionString);
        }

        public async Task<string> GetJobWiseStatus(string jobId, string orderType)
        {
            try
            {
                const string sql = @"
                                    SELECT 
                                        CASE 
                                            WHEN COUNT(ApproverLevel) = 0 THEN 'No approver was found for the mentioned job'
                                            WHEN COUNT(ApproverLevel) < 3 THEN 'Number of approvers is missing for the mentioned job'
                                            ELSE ''
                                        END AS Result
                                    FROM ApproverLevelSettings
                                    WHERE FormType = @FormType AND JobId = @JobId";

                var result = await _dbConnection.QueryFirstOrDefaultAsync<string>(sql, new
                {
                    FormType = orderType,
                    JobId = jobId
                });

                return result ?? string.Empty;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<string> GetVoucherWiseStatus(string jobId, string orderType)
        {
            try
            {
                const string sql = @"
                                    SELECT 
                                        CASE 
                                            WHEN COUNT(ApproverLevel) = 0 THEN 'No approver was found'
                                            WHEN COUNT(ApproverLevel) < 3 THEN 'Number of approvers is missing'
                                            ELSE ''
                                        END AS Result
                                    FROM ApproverLevelSettings
                                    WHERE FormType = @FormType AND JobId = @JobId";

                var result = await _dbConnection.QueryFirstOrDefaultAsync<string>(sql, new
                {
                    FormType = orderType,
                    JobId = jobId
                });

                return result ?? string.Empty;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public async Task<string> GetValueByKey(string key)
        {
            try
            {
                const string sql = "SELECT General_Settings_Text_Value FROM General_Settings WHERE General_Settings_Key_Value = @Key";

                var result = await _dbConnection.QueryFirstOrDefaultAsync<string>(sql, new { Key = key });

                return result ?? string.Empty;
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
