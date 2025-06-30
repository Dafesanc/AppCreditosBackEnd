using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppCreditosBackEnd.Domain.Enums;

namespace AppCreditosBackEnd.Application.DTOs
{
    public class AuditLogDto
    {
        public int Id { get; set; }
        public int CreditApplicationId { get; set; }
        public decimal RequestedAmount { get; set; }
        public Guid UserId { get; set; }
        public string UserEmail { get; set; } = string.Empty;
        public string UserFullName { get; set; } = string.Empty;
        public UserRole UserRole { get; set; }
        public string Action { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public int? PreviousStatus { get; set; } // Cambiado de ApplicationStatus a int
        public int? NewStatus { get; set; } // Cambiado de ApplicationStatus a int
        public DateTime Timestamp { get; set; }
        
        // Propiedades adicionales para mostrar el texto del status
        public string PreviousStatusText => GetStatusText(PreviousStatus);
        public string NewStatusText => GetStatusText(NewStatus);
        
        private static string GetStatusText(int? status)
        {
            return status switch
            {
                1 => "Pending",
                2 => "Approved", 
                3 => "Rejected",
                _ => "Unknown"
            };
        }
    }
    
    public record AuditLogFilterDto(
        int? CreditApplicationId,
        Guid? UserId,
        string? Action,
        DateTime? StartDate,
        DateTime? EndDate
    );

    /// <summary>
    /// DTO para filtros con fechas como string (para el frontend)
    /// </summary>
    public record AuditLogFilterStringDto(
        int? CreditApplicationId,
        Guid? UserId,
        string? Action,
        string? StartDate,
        string? EndDate
    );

    /// <summary>
    /// DTO genérico para resultados paginados
    /// </summary>
    /// <typeparam name="T">Tipo de dato que se pagina</typeparam>
    public class PaginatedResult<T>
    {
        /// <summary>
        /// Los elementos en la página actual
        /// </summary>
        public List<T> Items { get; set; } = new List<T>();
        
        /// <summary>
        /// Número total de registros
        /// </summary>
        public int TotalCount { get; set; }
        
        /// <summary>
        /// Número total de páginas
        /// </summary>
        public int TotalPages { get; set; }
        
        /// <summary>
        /// Página actual
        /// </summary>
        public int CurrentPage { get; set; }
        
        /// <summary>
        /// Tamaño de página
        /// </summary>
        public int PageSize { get; set; }
        
        /// <summary>
        /// Si hay página anterior
        /// </summary>
        public bool HasPrevious => CurrentPage > 1;
        
        /// <summary>
        /// Si hay página siguiente
        /// </summary>
        public bool HasNext => CurrentPage < TotalPages;
    }

    /// <summary>
    /// DTO para estadísticas de logs de auditoría
    /// </summary>
    public class AuditLogStatisticsDto
    {
        /// <summary>
        /// Cantidad total de logs de auditoría
        /// </summary>
        public int TotalLogs { get; set; }
        
        /// <summary>
        /// Cantidad de logs por tipo de acción
        /// </summary>
        public Dictionary<string, int> LogsByAction { get; set; } = new Dictionary<string, int>();
        
        /// <summary>
        /// Cantidad de logs por fecha (agrupados por día)
        /// </summary>
        public Dictionary<string, int> LogsByDate { get; set; } = new Dictionary<string, int>();
        
        /// <summary>
        /// Top 5 de usuarios con más acciones registradas
        /// </summary>
        public List<UserActivityDto> TopActiveUsers { get; set; } = new List<UserActivityDto>();
        
        /// <summary>
        /// Distribución de cambios de estado en solicitudes
        /// </summary>
        public Dictionary<string, int> StatusChanges { get; set; } = new Dictionary<string, int>();
    }
    
    /// <summary>
    /// DTO para actividad de usuarios
    /// </summary>
    public class UserActivityDto
    {
        public Guid UserId { get; set; }
        public string UserEmail { get; set; } = string.Empty;
        public string UserFullName { get; set; } = string.Empty;
        public UserRole UserRole { get; set; }
        public int ActionCount { get; set; }
    }
}
