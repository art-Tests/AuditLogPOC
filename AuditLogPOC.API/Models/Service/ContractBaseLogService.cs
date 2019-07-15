using System.Collections.Generic;
using AuditLogPOC.API.Models.DTO;
using Newtonsoft.Json;

namespace AuditLogPOC.API.Models.Service
{
    internal class ContractBaseLogService : BaseLogService
    {
        internal override List<EsFieldModifyLog> GetContents(AuditLogCreateRequest createRequest)
        {
            var contents = new List<EsFieldModifyLog>();
            var oldForm = JsonConvert.DeserializeObject<ContractForm>(createRequest.OldForm);
            var newForm = JsonConvert.DeserializeObject<ContractForm>(createRequest.NewForm);

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
                contents.Add(new EsFieldModifyLog { field = "EndDate", valueAfter = newForm.EndDate, valueBefore = oldForm.EndDate });
            }

            return contents;
        }
    }
}