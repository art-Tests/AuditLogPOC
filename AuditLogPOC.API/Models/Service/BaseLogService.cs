using System;
using System.Collections.Generic;
using AuditLogPOC.API.Models.DTO;

namespace AuditLogPOC.API.Models.Service
{
    internal abstract class BaseLogService
    {
        internal abstract List<EsFieldModifyLog> GetContents(AuditLogCreateRequest createRequest);

        public AuditLog GetElasticSearchRequest(AuditLogCreateRequest createRequest)
        {
            return new AuditLog
            {
                refId = createRequest.RefId,
                logType = createRequest.LogType,
                modifiedBy = 382388,
                deptName = "OB部門",
                modifiedByName = "OB管理者",
                modifiedDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
                content = GetContents(createRequest).ToArray()
            };
        }

    }
}