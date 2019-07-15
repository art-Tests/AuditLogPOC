using System.Collections.Generic;

namespace AuditLogPOC.API.Models.DTO
{
    public class AuditLogResponse
    {
        public int TotalCount { get; set; }
        public IEnumerable<FormModifyLog> Logs { get; set; }
    }
}