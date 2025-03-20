using Inspire.Erp.WebReport.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Inspire.Erp.WebReport.ReportEngine
{
    public class GeneralQueryExcutor
    {
        DbConnectionModel _dbSettings;

        public GeneralQueryExcutor(DbConnectionModel dbSettings)
        {
            _dbSettings = dbSettings;
        }

        public DataSet getFilledDataSet(ReportFilterModel reportFilterModel)
        {
            DataSet DsPrint = new DataSet();
            DsPrint = new DataSet();

            DataTable dt = null;

            if (reportFilterModel.SP_Queries.Any())
                ExecuteStoredProcedures(reportFilterModel.SP_Queries);

            reportFilterModel.Queries.ForEach(x =>
            {
                dt = new DataTable();
                dt = FetchDataTable(x).Copy();
                dt.TableName = $"{GetTableNameFromQuery(x)}";
                DsPrint.Merge(dt);
            });

            return DsPrint;
        }
        public void ExecuteStoredProcedures(List<SPQueries> sP_Queries)
        {
            if (sP_Queries == null || sP_Queries.Count == 0)
                return;

            var connStr = $"Server={_dbSettings.ServerName};Database={_dbSettings.DBName};Trusted_Connection=False;MultipleActiveResultSets=true;User Id={_dbSettings.UserName};Password={_dbSettings.Password};";

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                    conn.Open();
                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            foreach (var query in sP_Queries)
                            {
                                using (SqlCommand cmd = new SqlCommand(query.SP_Name, conn, transaction))
                                {
                                    cmd.CommandType = CommandType.StoredProcedure;

                                    if (query.Parameters != null && query.Parameters.Any())
                                    {
                                        foreach (var param in query.Parameters)
                                        {
                                            cmd.Parameters.Add(new SqlParameter(param.Key, param.Value ));
                                        }
                                    }

                                    cmd.ExecuteNonQuery();
                                }
                            }
                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            Console.WriteLine($"Error executing stored procedure: {ex.Message}");
                            throw;
                        }
                    }
               
            }
        }

        private string GetTableNameFromQuery(string query)
        {
            string pattern = @"\bFROM\b\s+(?:\[\w+\]\.)?(?:\w+\.)?(?<TableName>\w+)";
            Match match = Regex.Match(query, pattern, RegexOptions.IgnoreCase);

            if (match.Success)
            {
                return match.Groups["TableName"].Value;
            }

            return string.Empty;
        }
        public void ExecuteNonQurey(string query)
        {
            var connStr = $"Server= {_dbSettings.ServerName};Database={_dbSettings.DBName};Trusted_Connection=False;MultipleActiveResultSets=true;User Id = {_dbSettings.UserName};Password={_dbSettings.Password};";
            using (SqlConnection conn = new System.Data.SqlClient.SqlConnection(connStr))
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); }
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = query;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public DataTable FetchDataTable(string strSelectQury)
        {
            DataTable dt = new DataTable();
            var connStr = $"Server= {_dbSettings.ServerName};Database={_dbSettings.DBName};Trusted_Connection=False;MultipleActiveResultSets=true;User Id = {_dbSettings.UserName};Password={_dbSettings.Password};";
            using (SqlConnection conn = new System.Data.SqlClient.SqlConnection(connStr))
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); }
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = strSelectQury;
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }
            return dt;
        }
    }
}