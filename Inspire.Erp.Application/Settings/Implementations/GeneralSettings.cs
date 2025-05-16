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
        public async Task<(string GeneralSettingValue, string ApproverStatus)> GetApproverStatusAsync(string jobId, string orderType)
        {
            try
            {
                const string sql = @"SELECT 
                                            GS.General_Settings_Text_Value AS GeneralSettingValue,
                                            CASE 
                                                WHEN ApproverCount = 0 THEN 
                                                    CASE 
                                                        WHEN GS.General_Settings_Text_Value = 'JOB_WISE' THEN 'Approver Or Job not found for orderType'
                                                        WHEN GS.General_Settings_Text_Value = 'VOUCHER_WISE' THEN 'No approver found for the voucher'
                                                        ELSE 'No approver configuration found'
                                                    END

                                                WHEN ApproverCount < 3 THEN 
                                                    CASE 
                                                        WHEN GS.General_Settings_Text_Value = 'JOB_WISE' THEN 'Insufficient job-wise approvers configured'
                                                        WHEN GS.General_Settings_Text_Value = 'VOUCHER_WISE' THEN 'Insufficient voucher-wise approvers configured'
                                                        ELSE 'Insufficient approver configuration'
                                                    END

                                                ELSE ''
                                            END AS ApproverStatus
                                        FROM 
                                            (SELECT General_Settings_Text_Value 
                                             FROM General_Settings 
                                             WHERE General_Settings_Key_Value = 'DEFAULT_APPROVAL_TYPE') AS GS
                                        CROSS JOIN
                                            (
                                                SELECT COUNT(ApproverLevel) AS ApproverCount 
                                                FROM ApproverLevelSettings 
                                                WHERE FormType = @FormType
                                                  AND (
                                                        ( (SELECT General_Settings_Text_Value 
                                                             FROM General_Settings 
                                                             WHERE General_Settings_Key_Value = 'DEFAULT_APPROVAL_TYPE') = 'JOB_WISE'
                                                            AND JobId > 0 and JobId = @JobId)
                                                        OR
                                                        (  (SELECT General_Settings_Text_Value 
                                                             FROM General_Settings 
                                                             WHERE General_Settings_Key_Value = 'DEFAULT_APPROVAL_TYPE') <> 'JOB_WISE'
                                                            AND JobId = @JobId)
                                                    )
                                        ) AS AC;";

                var result = await _dbConnection.QueryFirstOrDefaultAsync<(string GeneralSettingValue, string ApproverStatus)>(sql, new
                {
                    FormType = orderType,
                    JobId = int.Parse(jobId)
                });

                return (result.GeneralSettingValue ?? string.Empty, result.ApproverStatus.Replace("orderType", orderType) ?? string.Empty);
            }
            catch (Exception)
            {
                throw;
            }
        }


    }
}
