using System.ComponentModel;

namespace AuditLogPOC.API.Models.Enum
{
    public enum LogType
    {
        [Description("Customer")]
        Customer,
        [Description("Contract")]
        Contract
    }
}