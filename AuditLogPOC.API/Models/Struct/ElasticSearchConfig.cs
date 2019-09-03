using System;

namespace AuditLogPOC.API.Models.Struct
{
    public struct ElasticSearchConfig
    {
        public static string WebHost = "http://172.21.14.106:9200";

        public struct AuditLog
        {
            private static readonly string IndexType = $"/fugoaudit-{DateTime.Now:yyyy-MM-dd}/auditlog";
            public static string DeleteUri = WebHost + IndexType + "/_delete_by_query";
            public static string SearchUri = WebHost + IndexType + "/_search";
            public static string CreateUri = WebHost + IndexType;
        }
    }
}