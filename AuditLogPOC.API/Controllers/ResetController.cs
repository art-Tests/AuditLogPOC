using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using AuditLogPOC.API.Models.Struct;

namespace AuditLogPOC.API.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ResetController : ApiController
    {

        public async Task<string> Post(string logType)
        {
            
            var sendData = GetDeleteRequestByLogType(logType);
            return await DeleteAuditLog(sendData);
        }
        private async Task<string> DeleteAuditLog(string sendData)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var httpContent = new StringContent(sendData, Encoding.UTF8, "application/json");
                    await client.PostAsync(ElasticSearchConfig.AuditLog.DeleteUri, httpContent);
                }
                return "OK";
            }
            catch (Exception)
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
