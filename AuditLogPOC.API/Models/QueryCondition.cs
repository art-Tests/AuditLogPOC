namespace AuditLogPOC.API.Models
{
    public class QueryCondition
    {
        public QueryCondition(string logType, int size, int page)
        {
            this.Size = size;
            this.Page = page;
            this.LogType = logType;
        }

        public int Size { get; set; }
        public int Page { get; set; }
        public string LogType { get; set; }
    }
}