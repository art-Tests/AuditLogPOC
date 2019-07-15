using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AuditLogPOC.API.Models.DTO;
using AuditLogPOC.API.Models.Struct;
using Newtonsoft.Json;

namespace AuditLogPOC.API.Models.Service
{
    internal class AuditLogService
    {
        public AuditLogResponse Query(AuditLogQueryRequest request)
        {
            return ConvertOutput(GetElasticSearchData(request));
        }
        public async Task<string> Create(AuditLogCreateRequest createRequest)
        {
            return await SendCreateAuditLog(GetForm(createRequest));
        }

        #region private-methods

        private ElasticSearchQueryResponse GetElasticSearchData(AuditLogQueryRequest auditLogQueryRequest)
        {
            var client = new HttpClient();
            var response = client.GetAsync(GetRequestUri(auditLogQueryRequest)).GetAwaiter().GetResult();
            var result = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            return JsonConvert.DeserializeObject<ElasticSearchQueryResponse>(result);
        }
        private string GetRequestUri(AuditLogQueryRequest auditLogQueryRequest)
        {
            var size = 0 < auditLogQueryRequest.Size && auditLogQueryRequest.Size < 1000 ? auditLogQueryRequest.Size : 10;
            var from = auditLogQueryRequest.Page <= 1 ? 0 : (auditLogQueryRequest.Page - 1) * size;
            return $"{ElasticSearchConfig.AuditLog.SearchUri}?q=logType:{auditLogQueryRequest.LogType}&size={auditLogQueryRequest.Size}&from={from}&sort=modifiedDate:desc";
        }
        private AuditLogResponse ConvertOutput(ElasticSearchQueryResponse esData)
        {
            if (esData.hits == null) return new AuditLogResponse() { TotalCount = 0 };

            return new AuditLogResponse()
            {
                TotalCount = esData.hits.total.value,
                Logs = esData.hits.hits.Select(x => new FormModifyLog()
                {
                    Id = x._id,
                    DeptName = x._source.deptName,
                    Content = x._source.content,
                    ModifiedBy = x._source.modifiedBy,
                    ModifiedByName = x._source.modifiedByName,
                    ModifiedDate = x._source.modifiedDate,
                })
            };
        }

   
        private string GetForm(AuditLogCreateRequest createRequest)
        {
            var logType = createRequest.LogType.ToLower();
            var instance = LogServiceFactory.GetLogInstance(logType);
            return JsonConvert.SerializeObject(instance.GetElasticSearchRequest(createRequest));
        }
        private static async Task<string> SendCreateAuditLog(string sendData)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var httpContent = new StringContent(sendData, Encoding.UTF8, "application/json");
                    await client.PostAsync(ElasticSearchConfig.AuditLog.CreateUri, httpContent);
                }
                return "OK";
            }
            catch (Exception)
            {
                return "FAIL";
            }
        }

        #endregion
    }
}