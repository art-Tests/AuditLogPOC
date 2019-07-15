
namespace AuditLogPOC.API.Models.DTO
{
    public class AuditLogCreateRequest
    {
        public int RefId { get; set; }
        public string LogType { get; set; }
        public string NewForm { get; set; }
        public string OldForm { get; set; }
    }
}