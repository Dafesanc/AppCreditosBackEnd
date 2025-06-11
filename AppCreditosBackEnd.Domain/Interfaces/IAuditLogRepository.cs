using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppCreditosBackEnd.Domain.Entities;
using AppCreditosBackEnd.Domain.Enums;

namespace AppCreditosBackEnd.Domain.Interfaces
{
    public interface IAuditLogRepository
    {
        Task CreateAsync(AuditLog auditLog);
        Task<List<AuditLog>> GetAllAsync();
        Task<List<AuditLog>> GetByCreditApplicationIdAsync(int creditApplicationId);
        Task<List<AuditLog>> GetByUserIdAsync(Guid userId);
        Task<List<AuditLog>> GetByActionAsync(string action);
        Task<List<AuditLog>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        
        // Métodos para paginación
        Task<(List<AuditLog> logs, int totalCount)> GetPaginatedAsync(int page, int pageSize);
        Task<(List<AuditLog> logs, int totalCount)> GetFilteredPaginatedAsync(
            int page, int pageSize, 
            int? creditApplicationId = null, 
            Guid? userId = null,
            string action = null,
            DateTime? startDate = null,
            DateTime? endDate = null);
    }
}
