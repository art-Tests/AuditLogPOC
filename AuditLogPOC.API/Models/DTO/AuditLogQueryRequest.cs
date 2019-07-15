namespace AuditLogPOC.API.Models.DTO
{
    public class AuditLogQueryRequest
    {
  
        public int Size { get; set; } = 10;
        public int Page { get; set; } = 1;
        public string LogType { get; set; }
    }
}