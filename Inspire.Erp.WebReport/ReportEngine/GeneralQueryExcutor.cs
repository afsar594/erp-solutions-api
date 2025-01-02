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

        public DataSet getFilledDataSet(List<string> queries)
        {
            DataSet DsPrint = new DataSet();
            DsPrint = new DataSet();

            DataTable dt = null;

            foreach (var query in queries)
            {
                dt = new DataTable();
                dt = FetchDataTable(query).Copy();
                dt.TableName = $"{GetTableNameFromQuery(query)}";
                DsPrint.Merge(dt);
            }

            return DsPrint;
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