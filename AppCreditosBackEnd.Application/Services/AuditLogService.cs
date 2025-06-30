using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppCreditosBackEnd.Application.DTOs;
using AppCreditosBackEnd.Application.Interfaces;
using AppCreditosBackEnd.Domain.Entities;
using AppCreditosBackEnd.Domain.Enums;
using AppCreditosBackEnd.Domain.Interfaces;
using AutoMapper;

namespace AppCreditosBackEnd.Application.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly ICreditApplicationRepository _creditApplicationRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public AuditLogService(
            IAuditLogRepository auditLogRepository,
            ICreditApplicationRepository creditApplicationRepository,
            IUserRepository userRepository,
            IMapper mapper)
        {
            _auditLogRepository = auditLogRepository;
            _creditApplicationRepository = creditApplicationRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task LogCreditApplicationAction(
            int creditApplicationId,
            Guid userId,
            string action,
            string details,
            int? previousStatus = null,
            int? newStatus = null)
        {
            // Crear el registro de auditoría
            var auditLog = new AuditLog
            {
                CreditApplicationId = creditApplicationId,
                UserId = userId,
                Action = action,
                Details = details,
                PreviousStatus = previousStatus,
                NewStatus = newStatus,
                Timestamp = DateTime.UtcNow
            };

            // Guardar el registro
            await _auditLogRepository.CreateAsync(auditLog);
        }

        public async Task<List<AuditLogDto>> GetAllLogsAsync()
        {
            var logs = await _auditLogRepository.GetAllAsync();
            return await MapLogsToDto(logs);
        }

        public async Task<List<AuditLogDto>> GetFilteredLogsAsync(AuditLogFilterDto filter)
        {
            List<AuditLog> logs;

            if (filter.CreditApplicationId.HasValue)
            {
                logs = await _auditLogRepository.GetByCreditApplicationIdAsync(filter.CreditApplicationId.Value);
            }
            else if (filter.UserId.HasValue)
            {
                logs = await _auditLogRepository.GetByUserIdAsync(filter.UserId.Value);
            }
            else if (!string.IsNullOrEmpty(filter.Action))
            {
                logs = await _auditLogRepository.GetByActionAsync(filter.Action);
            }
            else if (filter.StartDate.HasValue && filter.EndDate.HasValue)
            {
                logs = await _auditLogRepository.GetByDateRangeAsync(filter.StartDate.Value, filter.EndDate.Value);
            }
            else
            {
                logs = await _auditLogRepository.GetAllAsync();
            }

            return await MapLogsToDto(logs);
        }

        public async Task<List<AuditLogDto>> GetByCreditApplicationIdAsync(int creditApplicationId)
        {
            var logs = await _auditLogRepository.GetByCreditApplicationIdAsync(creditApplicationId);
            return await MapLogsToDto(logs);
        }

        public async Task<List<AuditLogDto>> GetByUserIdAsync(Guid userId)
        {
            var logs = await _auditLogRepository.GetByUserIdAsync(userId);
            return await MapLogsToDto(logs);
        }        private async Task<List<AuditLogDto>> MapLogsToDto(List<AuditLog> logs)
        {
            var dtoList = new List<AuditLogDto>();
            
            foreach (var log in logs)
            {
                // Crear el DTO básico desde el AuditLog
                var dto = new AuditLogDto
                {
                    Id = log.Id,
                    CreditApplicationId = log.CreditApplicationId,
                    UserId = log.UserId,
                    Action = log.Action,
                    Details = log.Details,
                    PreviousStatus = log.PreviousStatus,
                    NewStatus = log.NewStatus,
                    Timestamp = log.Timestamp
                };
                
                // Cargar datos relacionados manualmente para el DTO
                try
                {
                    var application = await _creditApplicationRepository.GetByIdAsync(log.CreditApplicationId);
                    if (application != null)
                    {
                        dto.RequestedAmount = application.RequestedAmount;
                    }
                }
                catch
                {
                    dto.RequestedAmount = 0;
                }
                
                try
                {
                    var user = await _userRepository.GetByIdAsync(log.UserId);
                    if (user != null)
                    {
                        dto.UserEmail = user.Email;
                        dto.UserFullName = $"{user.FirstName} {user.LastName}".Trim();
                        dto.UserRole = user.Role;
                    }
                    else
                    {
                        dto.UserEmail = "Unknown";
                        dto.UserFullName = "Unknown";
                        dto.UserRole = UserRole.Applicant;
                    }
                }
                catch
                {
                    dto.UserEmail = "Unknown";
                    dto.UserFullName = "Unknown";
                    dto.UserRole = UserRole.Applicant;
                }
                
                dtoList.Add(dto);
            }
            
            return dtoList;
        }

        public async Task<PaginatedResult<AuditLogDto>> GetPaginatedLogsAsync(int page, int pageSize, AuditLogFilterDto? filter = null)
        {
            (List<AuditLog> logs, int totalCount) result;
            
            if (filter == null)
            {
                result = await _auditLogRepository.GetPaginatedAsync(page, pageSize);
            }
            else
            {
                result = await _auditLogRepository.GetFilteredPaginatedAsync(
                    page,
                    pageSize,
                    filter.CreditApplicationId,
                    filter.UserId,
                    filter.Action ?? string.Empty,
                    filter.StartDate,
                    filter.EndDate);
            }
            
            // Mapear los logs a DTOs
            var dtoList = await MapLogsToDto(result.logs);
            
            // Calcular el total de páginas
            int totalPages = (int)Math.Ceiling(result.totalCount / (double)pageSize);
            
            // Crear el resultado paginado
            return new PaginatedResult<AuditLogDto>
            {
                Items = dtoList,
                TotalCount = result.totalCount,
                TotalPages = totalPages,
                CurrentPage = page,
                PageSize = pageSize
            };
        }

        public async Task<AuditLogStatisticsDto> GetAuditLogStatisticsAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            // Si no se especifica fecha final, usar la fecha actual
            endDate ??= DateTime.UtcNow;
            
            // Si no se especifica fecha inicial, usar 30 días antes de la fecha final
            startDate ??= endDate.Value.AddDays(-30);
            
            // Obtener todos los logs dentro del rango de fechas
            var logs = await _auditLogRepository.GetByDateRangeAsync(startDate.Value, endDate.Value);
            
            // Crear objeto de estadísticas
            var statistics = new AuditLogStatisticsDto
            {
                TotalLogs = logs.Count
            };
            
            // Calcular logs por acción
            statistics.LogsByAction = logs
                .GroupBy(l => l.Action)
                .ToDictionary(g => g.Key, g => g.Count());
            
            // Calcular logs por fecha
            statistics.LogsByDate = logs
                .GroupBy(l => l.Timestamp.Date.ToString("yyyy-MM-dd"))
                .OrderBy(g => g.Key)
                .ToDictionary(g => g.Key, g => g.Count());
            
            // Calcular top usuarios activos
            var userGroups = logs
                .GroupBy(l => l.UserId)
                .Select(g => new 
                { 
                    UserId = g.Key, 
                    ActionCount = g.Count()
                })
                .OrderByDescending(x => x.ActionCount)
                .Take(5);
                
            foreach (var userGroup in userGroups)
            {
                try
                {
                    var user = await _userRepository.GetByIdAsync(userGroup.UserId);
                    if (user != null)
                    {
                        statistics.TopActiveUsers.Add(new UserActivityDto
                        {
                            UserId = userGroup.UserId,
                            UserEmail = user.Email,
                            UserFullName = $"{user.FirstName} {user.LastName}".Trim(),
                            UserRole = user.Role,
                            ActionCount = userGroup.ActionCount
                        });
                    }
                }
                catch
                {
                    // Ignorar usuarios que no se pueden cargar
                }
            }
            
            // Calcular cambios de estado
            statistics.StatusChanges = logs
                .Where(l => l.PreviousStatus.HasValue && l.NewStatus.HasValue && l.PreviousStatus != l.NewStatus)
                .GroupBy(l => $"{l.PreviousStatus} → {l.NewStatus}")
                .ToDictionary(g => g.Key, g => g.Count());
            
            return statistics;
        }
    }
}
