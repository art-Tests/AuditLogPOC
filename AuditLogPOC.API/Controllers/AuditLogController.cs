using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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

        public AuditLog Get([FromUri] string logType, int size = 10, int page = 1)
        {
            var condition = new QueryCondition(logType, size, page);
            var esDataStr = GetElasticSearchData(condition);
            var esData = JsonConvert.DeserializeObject<ElasticSearchQueryResponse>(esDataStr);
            var result = ConvertOutput(esData);
            return result;
        }

        public async Task<string> Post([FromBody]CustomerAuditLogForm form)
        {
            var logData = GetCustomerForm(form);
            var sendData = JsonConvert.SerializeObject(logData);
            return await CreateAuditLog(sendData);
        }

        public async Task<string> Delete()
        {
            return await DeleteAuditLog();
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

        private Models.AuditLog GetCustomerForm(CustomerAuditLogForm form)
        {
            var contents = new List<FieldModifyLog>();

            if (!string.Equals(form.OldForm.Name, form.NewForm.Name))
            {
                contents.Add(new FieldModifyLog { field = "Name", valueAfter = form.NewForm.Name, valueBefore = form.OldForm.Name });
            }

            if (!string.Equals(form.OldForm.Phone, form.NewForm.Phone))
            {
                contents.Add(new FieldModifyLog { field = "Phone", valueAfter = form.NewForm.Phone, valueBefore = form.OldForm.Phone });
            }

            return new Models.AuditLog
            {
                refId = 28007357,
                logType = "customer",
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

        private AuditLog ConvertOutput(ElasticSearchQueryResponse esData)
        {
            if (esData.hits == null) return new AuditLog() {TotalCount = 0};

            var result = new AuditLog()
            {
                TotalCount = esData.hits.total.value,
                Logs = esData.hits.hits.Select(x => new CustomerModifyLog()
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

    public interface IDepartmentModule
    {
        /// <summary>
        /// 取得部門關係Dictionary
        /// </summary>
        /// <returns>內含整理後的部門階層關係的Dictionary物件</returns>
        IDictionary<int, DepartmentRelationship> GetDepartments();
    }

    /// <summary>
    /// 部門階層關係
    /// </summary>
    [Serializable]
    public class DepartmentRelationship
    {
        /// <summary>
        /// 部門代碼
        /// </summary>
        public int DepartmentId { get; set; }

        /// <summary>
        /// 部門名稱
        /// </summary>
        public string DeptName { get; set; }

        /// <summary>
        /// 上層部門代碼
        /// </summary>
        public int ParentDept { get; set; }

        /// <summary>
        /// 階層 (從最上層部門算下來是第幾階層)
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// 部門階層關係 (逗號分隔)
        /// </summary>
        public string Relationship { get; set; }
    }

    public class AuditLog
    {
        public int TotalCount { get; set; }
        public IEnumerable<CustomerModifyLog> Logs { get; set; }
    }

    public class CustomerModifyLog
    {
        public string Id { get; set; }
        public string DeptName { get; set; }
        public string ModifiedByName { get; set; }
        public int ModifiedBy { get; set; }
        public string ModifiedDate { get; set; }
        public IEnumerable<FieldModifyLog> Content { get; set; }
    }
}