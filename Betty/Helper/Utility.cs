using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NLog;
using System;
using System.IO;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Betty.Helper
{
    public static class Utility
    {
        //private static Logger _logger = LogManager.GetCurrentClassLogger();
        public static void LogException(Exception ex, Microsoft.Extensions.Logging.ILogger logger)
        {
            if(ex == null)  throw new ArgumentNullException();
            if(logger == null) throw new ArgumentNullException();
            logger.LogError(ex.GetType().ToString());
            logger.LogError(ex.Message);
            logger.LogError(ex.StackTrace);
            if (ex.InnerException != null)
            {
                logger.LogError("+++++++++Inner Ex: +++++++++");
                LogException(ex.InnerException, logger);
            }
        }
        public static void LogException(Exception ex, Logger logger)
        {
            if(ex == null)  throw new ArgumentNullException();
            if(logger == null) throw new ArgumentNullException();
            logger.Error(ex.GetType().ToString());
            logger.Error(ex.Message);
            logger.Error(ex.StackTrace);
            if (ex.InnerException != null)
            {
                logger.Error("+++++++++Inner Ex: +++++++++");
                LogException(ex.InnerException, logger);
            }
        }        public static string GetContextUsername(HttpContext context)
        {
            return context.User.FindFirst(ClaimTypes.Name).Value;
        }
        public static bool Decode64(string base64, out int decoded)
        {
            decoded = -1;
            if (string.IsNullOrEmpty(base64)) return false;
            try
            {
                var data = Convert.FromBase64String(base64);
                string decodedString = Encoding.UTF8.GetString(data);
                decoded = int.Parse(decodedString);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}
