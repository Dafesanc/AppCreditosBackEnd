using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppCreditosBackEnd.Domain.Entities;

namespace AppCreditosBackEnd.Domain.Interfaces
{
    public interface IAuditLogRepository
    {
        Task CreateAsync(AuditLog auditLog);
    }
}
