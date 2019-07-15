using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using AuditLogPOC.API.Models.DTO;
using AuditLogPOC.API.Models.Service;

namespace AuditLogPOC.API.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class AuditLogController : ApiController
    {
        private readonly AuditLogService _service = new AuditLogService();

        public AuditLogResponse Get([FromUri] AuditLogQueryRequest queryRequest)
        {
            return _service.Query(queryRequest);
        }

        public Task<string> Post([FromBody] AuditLogCreateRequest createRequest)
        {
            return _service.Create(createRequest);
        }
    }
}