using System.Collections.Generic;

namespace AuditLogPOC.API.Models
{
    public class FormModifyLog
    {
        public string Id { get; set; }
        public string DeptName { get; set; }
        public string ModifiedByName { get; set; }
        public int ModifiedBy { get; set; }
        public string ModifiedDate { get; set; }
        public IEnumerable<EsFieldModifyLog> Content { get; set; }
    }
}