using System.Collections.Generic;
using AuditLogPOC.API.Models.DTO;
using Newtonsoft.Json;

namespace AuditLogPOC.API.Models.Service
{
    internal class CustomerBaseLogService : BaseLogService
    {
        internal override List<EsFieldModifyLog> GetContents(AuditLogCreateRequest createRequest)
        {
            var contents = new List<EsFieldModifyLog>();

            var oldForm = JsonConvert.DeserializeObject<CustomerForm>(createRequest.OldForm);
            var newForm = JsonConvert.DeserializeObject<CustomerForm>(createRequest.NewForm);

            if (!string.Equals(oldForm.Name, newForm.Name))
            {
                contents.Add(new EsFieldModifyLog { field = "Name", valueAfter = newForm.Name, valueBefore = oldForm.Name });
            }

            if (!string.Equals(oldForm.Phone, newForm.Phone))
            {
                contents.Add(new EsFieldModifyLog { field = "Phone", valueAfter = newForm.Phone, valueBefore = oldForm.Phone });
            }

            return contents;
        }
    }
}