using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using AuditLogPOC.API.Models;
using Newtonsoft.Json;

namespace AuditLogPOC.API.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class AuditLogController : ApiController
    {
        private const string ElasticSearchWebHost = "http://172.21.3.75:9200";

        public AuditLogResponse Get([FromUri] string logType, int size = 10, int page = 1)
        {
            var condition = new QueryCondition(logType, size, page);
            var esDataStr = GetElasticSearchData(condition);
            var esData = JsonConvert.DeserializeObject<ElasticSearchQueryResponse>(esDataStr);
            var result = ConvertOutput(esData);
            return result;
        }

        public async Task<string> Post([FromBody]AuditLogRequest request)
        {
            var logData = GetForm(request);
            var sendData = JsonConvert.SerializeObject(logData);
            return await CreateAuditLog(sendData);
        }

        public async Task<string> Delete()
        {
            return await DeleteAuditLog();
        }

        private AuditLog GetForm(AuditLogRequest request)
        {
            
            if (request.LogType.ToLower() == LogType.Customer.GetDescriptionText().ToLower())
            {
                return GetCustomerForm(request);
            }

            if (request.LogType.ToLower() == LogType.Contract.GetDescriptionText().ToLower())
            {
                return GetContractForm(request);

            }
            throw new Exception("logType error");
        }

 

        private string GetElasticSearchData(QueryCondition queryCondition)
        {
            //建立 HttpClient
            HttpClient client = new HttpClient()
            {
                BaseAddress = new Uri($"{ElasticSearchWebHost}/fugo/auditlog/_search")
            };

            var reqUri = GetRequestUri(queryCondition);
            HttpResponseMessage response = client.GetAsync(reqUri).GetAwaiter().GetResult();
            var result = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            return result;
        }

        private string GetRequestUri(QueryCondition queryCondition)
        {
            var size = 0 < queryCondition.Size && queryCondition.Size < 1000 ? queryCondition.Size : 10;
            var from = queryCondition.Page <= 1 ? 0 : (queryCondition.Page - 1) * size;
            return $"?q=logType:{queryCondition.LogType}&size={queryCondition.Size}&from={from}&sort=modifiedDate:desc";
        }

        private Models.AuditLog GetContractForm(AuditLogRequest request)
        {
            var contents = new List<EsFieldModifyLog>();

            var oldForm = JsonConvert.DeserializeObject<ContractForm>(request.OldForm);
            var newForm = JsonConvert.DeserializeObject<ContractForm>(request.NewForm);

            if (!string.Equals(oldForm.Name, newForm.Name))
            {
                contents.Add(new EsFieldModifyLog { field = "Name", valueAfter = newForm.Name, valueBefore = oldForm.Name });
            }
            if (!string.Equals(oldForm.BeginDate, newForm.BeginDate))
            {
                contents.Add(new EsFieldModifyLog { field = "BeginDate", valueAfter = newForm.BeginDate, valueBefore = oldForm.BeginDate });
            }
            if (!string.Equals(oldForm.EndDate, newForm.EndDate))
            {
                contents.Add(new EsFieldModifyLog { field = "EndDate", valueAfter = newForm.EndDate, valueBefore =oldForm.EndDate });
            }

            return AuditLogWithContent(request.RefId,request.LogType,contents);
        }


        private Models.AuditLog GetCustomerForm(AuditLogRequest request)
        {
            var contents = new List<EsFieldModifyLog>();

            var oldForm = JsonConvert.DeserializeObject<CustomerForm>(request.OldForm);
            var newForm = JsonConvert.DeserializeObject<CustomerForm>(request.NewForm);

            if (!string.Equals(oldForm.Name, newForm.Name))
            {
                contents.Add(new EsFieldModifyLog { field = "Name", valueAfter = newForm.Name, valueBefore = oldForm.Name });
            }

            if (!string.Equals(oldForm.Phone, newForm.Phone))
            {
                contents.Add(new EsFieldModifyLog { field = "Phone", valueAfter = newForm.Phone, valueBefore = oldForm.Phone });
            }

            return AuditLogWithContent(request.RefId, request.LogType, contents);
        }

       

        private Models.AuditLog AuditLogWithContent(int refId, string logType, List<EsFieldModifyLog> contents)
        {
            return new Models.AuditLog
            {
                refId = refId,
                logType = logType,
                modifiedBy = 382388,
                deptName = "OB部門",
                modifiedByName = "OB管理者",
                modifiedDate = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"),
                content = contents.ToArray()
            };
        }

        private static async Task<string> CreateAuditLog(string sendData)
        {
            try
            {
                var requestUri = $"{ElasticSearchWebHost}/fugo/auditlog";
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

        private async Task<string> DeleteAuditLog()
        {
            try
            {
                var requestUri = $"{ElasticSearchWebHost}/fugo";
                using (var client = new HttpClient())
                {
                    await client.DeleteAsync(requestUri);
                }
                return "OK";
            }
            catch (Exception e)
            {
                return "FAIL";
            }
        }

        private AuditLogResponse ConvertOutput(ElasticSearchQueryResponse esData)
        {
            if (esData.hits == null) return new AuditLogResponse() {TotalCount = 0};

            var result = new AuditLogResponse()
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
            return result;
        }
    }

  
}