using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppCreditosBackEnd.Application.DTOs;
using AppCreditosBackEnd.Domain.Entities;
using AppCreditosBackEnd.Domain.Enums;

namespace AppCreditosBackEnd.Application.Interfaces
{
    public interface IAuditLogService
    {
        Task LogCreditApplicationAction(
            int creditApplicationId, 
            Guid userId, 
            string action,
            string details,
            int? previousStatus = null,
            int? newStatus = null);
            
        Task<List<AuditLogDto>> GetAllLogsAsync();
        
        Task<List<AuditLogDto>> GetFilteredLogsAsync(AuditLogFilterDto filter);
        
        Task<List<AuditLogDto>> GetByCreditApplicationIdAsync(int creditApplicationId);
        
        Task<List<AuditLogDto>> GetByUserIdAsync(Guid userId);

        Task<PaginatedResult<AuditLogDto>> GetPaginatedLogsAsync(int page, int pageSize, AuditLogFilterDto? filter = null);
        
        /// <summary>
        /// Obtiene estadísticas de los logs de auditoría para dashboard
        /// </summary>
        /// <param name="startDate">Fecha inicial para el rango (opcional)</param>
        /// <param name="endDate">Fecha final para el rango (opcional)</param>
        /// <returns>Estadísticas de logs de auditoría</returns>
        Task<AuditLogStatisticsDto> GetAuditLogStatisticsAsync(DateTime? startDate = null, DateTime? endDate = null);
    }
}
