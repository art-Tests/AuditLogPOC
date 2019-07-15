namespace AuditLogPOC.API.Models.Struct
{
    public struct ElasticSearchConfig
    {
        public static string WebHost = "http://172.21.3.75:9200";

        public struct AuditLog
        {
            private const string IndexType = "/fugo/auditlog";
            public static string DeleteUri = WebHost + IndexType + "/_delete_by_query";
            public static string SearchUri = WebHost + IndexType + "/_search";
            public static string CreateUri = WebHost + IndexType ;
        }
    }
}