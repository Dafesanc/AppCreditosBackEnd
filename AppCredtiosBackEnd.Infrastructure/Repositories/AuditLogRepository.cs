using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppCreditosBackEnd.Domain.Entities;
using AppCreditosBackEnd.Domain.Interfaces;

namespace AppCreditosBackEnd.Infrastructure.Repositories
{
    public class AuditLogRepository : IAuditLogRepository
    {
        public Task CreateAsync(AuditLog auditLog)
        {
            throw new NotImplementedException();
        }
    }
}
