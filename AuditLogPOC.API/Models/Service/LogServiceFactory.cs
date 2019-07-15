using System;
using AuditLogPOC.API.Models.Enum;
using AuditLogPOC.API.Models.Extension;

namespace AuditLogPOC.API.Models.Service
{
    internal class LogServiceFactory
    {
        public static BaseLogService GetLogInstance(string logType)
        {
            if (logType == LogType.Customer.GetDescriptionText().ToLower())
            {
                return new CustomerBaseLogService();
            }

            if (logType == LogType.Contract.GetDescriptionText().ToLower())
            {
                return new ContractBaseLogService();

            }
            throw new Exception("logType error");

        }
    }
}