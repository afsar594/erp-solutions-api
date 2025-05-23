using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Inspire.Erp.WebReport.ReportEngine
{
    public static class Logger
    {
        private static readonly string logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", "ApplicationLog.txt");

        static Logger()
        {
            var logDirectory = Path.GetDirectoryName(logFilePath);
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }
        }

        public static void LogInfo(string message)
        {
            WriteLog("INFO", message);
        }

        public static void LogError(string message, Exception ex)
        {
            WriteLog("ERROR", $"{message} - Exception: {ex.Message}\nStackTrace: {ex.StackTrace}");
        }

        private static void WriteLog(string logLevel, string message)
        {
            try
            {
                string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{logLevel}] {message}";
                File.AppendAllText(logFilePath, logEntry + Environment.NewLine);
            }
            catch
            {
                // Silent fail, avoid recursive logging failure
            }
        }
    }
}