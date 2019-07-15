namespace AuditLogPOC.API.Models
{
    public class AuditLog
    {
        public int refId { get; set; }
        public string logType { get; set; }
        public int modifiedBy { get; set; }
        public string modifiedDate { get; set; }
        public EsFieldModifyLog[] content { get; set; }
        public string deptName { get; set; }
        public string modifiedByName { get; set; }
    }
}