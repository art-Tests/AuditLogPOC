namespace AuditLogPOC.API.Models
{
    public class ElasticSearchQueryResponse
    {
        public int took { get; set; }
        public bool timed_out { get; set; }
        public _Shards _shards { get; set; }
        public Hits hits { get; set; }
    }

    public class _Shards
    {
        public int total { get; set; }
        public int successful { get; set; }
        public int skipped { get; set; }
        public int failed { get; set; }
    }

    public class Hits
    {
        public Total total { get; set; }
        public float? max_score { get; set; }
        public Hit[] hits { get; set; }
    }

    public class Total
    {
        public int value { get; set; }
        public string relation { get; set; }
    }

    public class Hit
    {
        public string _index { get; set; }
        public string _type { get; set; }
        public string _id { get; set; }
        public float? _score { get; set; }
        public EsModifyLog _source { get; set; }
    }

    public class EsModifyLog
    {
        public int refId { get; set; }
        public string logType { get; set; }
        public int modifiedBy { get; set; }
        public string modifiedByName { get; set; }
        public string modifiedDate { get; set; }
        public string deptName { get; set; }
        public EsFieldModifyLog[] content { get; set; }
    }

    public class EsFieldModifyLog
    {
        public string field { get; set; }
        public string valueBefore { get; set; }
        public string valueAfter { get; set; }
    }
}