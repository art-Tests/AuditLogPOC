namespace AuditLogPOC.API.Models
{
    public class CustomerAuditLogForm
    {
        public CustomerForm NewForm { get; set; }
        public CustomerForm OldForm { get; set; }
    }

    public class CustomerForm
    {
        public string Name { get; set; }
        public string Phone { get; set; }
    }
}