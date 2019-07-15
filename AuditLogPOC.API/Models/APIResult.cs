namespace AuditLogPOC.API.Models
{
    public class APIResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Payload { get; set; }
    }
}