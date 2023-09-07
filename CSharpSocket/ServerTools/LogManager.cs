using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpServer.ServerTools
{
    public static class LogManager
    {
        private const string LOGPATH = "RuningLog.log";

        public static void LogInfo(string message, bool writeInFile = false)
        {
            string infoMessage = $"Info: {message}";
            Log(infoMessage, writeInFile);
        }

        public static void LogWarning(string message, bool writeInFile = true)
        {
            string warningMessage = $"Warning: {message}";
            Log(warningMessage, writeInFile);
        }

        public static void LogError(string message, bool writeInFile = true)
        {
            string errorMessage = $"Error: {message}";
            Log(errorMessage, writeInFile);
        }

        private static void Log(string message, bool writeInFile)
        {
            string logEntry = $"{DateTime.Now} - {message}";

            try
            {
                Console.WriteLine(logEntry);
                if (writeInFile)
                    using (StreamWriter writer = new StreamWriter(LOGPATH, true))
                    {
                        writer.WriteLine(logEntry);
                    }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"日志记录失败: {ex.Message}");
            }
        }
    }
}
