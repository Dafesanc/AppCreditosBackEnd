using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppCreditosBackEnd.Domain.Entities;
using AppCreditosBackEnd.Domain.Enums;
using AppCreditosBackEnd.Domain.Interfaces;
using AppCreditosBackEnd.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace AppCreditosBackEnd.Infrastructure.Repositories
{
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly CreditPlatformDbContext _context;

        public AuditLogRepository(CreditPlatformDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(AuditLog auditLog)
        {
            if (auditLog == null)
                throw new ArgumentNullException(nameof(auditLog));
            
            auditLog.Timestamp = DateTime.UtcNow;
            
            await _context.AuditLogs.AddAsync(auditLog);
            await _context.SaveChangesAsync();
        }
        
        public async Task<List<AuditLog>> GetAllAsync()
        {
            return await _context.AuditLogs
                .Include(a => a.CreditApplication)
                .Include(a => a.User)
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync();
        }
        
        public async Task<List<AuditLog>> GetByCreditApplicationIdAsync(int creditApplicationId)
        {
            return await _context.AuditLogs
                .Where(a => a.CreditApplicationId == creditApplicationId)
                .Include(a => a.CreditApplication)
                .Include(a => a.User)
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync();
        }
        
        public async Task<List<AuditLog>> GetByUserIdAsync(Guid userId)
        {
            return await _context.AuditLogs
                .Where(a => a.UserId == userId)
                .Include(a => a.CreditApplication)
                .Include(a => a.User)
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync();
        }
        
        public async Task<List<AuditLog>> GetByActionAsync(string action)
        {
            return await _context.AuditLogs
                .Where(a => a.Action == action)
                .Include(a => a.CreditApplication)
                .Include(a => a.User)
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync();
        }
        
        public async Task<List<AuditLog>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.AuditLogs
                .Where(a => a.Timestamp >= startDate && a.Timestamp <= endDate)
                .Include(a => a.CreditApplication)
                .Include(a => a.User)
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync();
        }
        
        public async Task<(List<AuditLog> logs, int totalCount)> GetPaginatedAsync(int page, int pageSize)
        {
            // Obtener el contador total de registros
            var totalCount = await _context.AuditLogs.CountAsync();
            
            // Aplicar paginación
            var logs = await _context.AuditLogs
                .Include(a => a.CreditApplication)
                .Include(a => a.User)
                .OrderByDescending(a => a.Timestamp)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            
            return (logs, totalCount);
        }
        
        public async Task<(List<AuditLog> logs, int totalCount)> GetFilteredPaginatedAsync(
            int page, int pageSize, 
            int? creditApplicationId = null, 
            Guid? userId = null,
            string action = null,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            // Construir query base
            IQueryable<AuditLog> query = _context.AuditLogs
                .Include(a => a.CreditApplication)
                .Include(a => a.User);
            
            // Aplicar filtros condicionales
            if (creditApplicationId.HasValue)
            {
                query = query.Where(a => a.CreditApplicationId == creditApplicationId.Value);
            }
            
            if (userId.HasValue && userId != Guid.Empty)
            {
                query = query.Where(a => a.UserId == userId.Value);
            }
            
            if (!string.IsNullOrWhiteSpace(action))
            {
                query = query.Where(a => a.Action == action);
            }
            
            if (startDate.HasValue)
            {
                query = query.Where(a => a.Timestamp >= startDate.Value);
            }
            
            if (endDate.HasValue)
            {
                query = query.Where(a => a.Timestamp <= endDate.Value);
            }
            
            // Obtener el contador total de registros con los filtros aplicados
            var totalCount = await query.CountAsync();
            
            // Aplicar paginación
            var logs = await query
                .OrderByDescending(a => a.Timestamp)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            
            return (logs, totalCount);
        }
    }
}
