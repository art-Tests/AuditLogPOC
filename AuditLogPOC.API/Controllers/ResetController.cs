using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

namespace AuditLogPOC.API.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ResetController : ApiController
    {
        private const string ElasticSearchWebHost = "http://172.21.3.75:9200";

        public async Task<string> Post(string logType)
        {
            var sendData = GetDeleteRequestByLogType(logType);
            return await DeleteAuditLog(sendData);
        }
        private async Task<string> DeleteAuditLog(string sendData)
        {
            try
            {
                var requestUri = $"{ElasticSearchWebHost}/fugo/auditlog/_delete_by_query";
                using (var client = new HttpClient())
                {
                    var httpContent = new StringContent(sendData, Encoding.UTF8, "application/json");
                    await client.PostAsync(requestUri, httpContent);
                }
                return "OK";
            }
            catch (Exception e)
            {
                return "FAIL";
            }
        }

        private string GetDeleteRequestByLogType(string logType)
        {

            return @"
{
  ""query"": {
    ""bool"": {
      ""must"": [
        {
          ""term"": {
            ""logType.keyword"": ""$$logType$$""
          }
        }
      ]
    }
  }
}
".Replace("$$logType$$", logType);
        }

    }
}
