using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AppCreditosBackEnd.Domain.Enums;

namespace AppCreditosBackEnd.Domain.Entities
{
    public class AuditLog
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int CreditApplicationId { get; set; }
        
        [Required]
        public Guid UserId { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Action { get; set; } = string.Empty;
        
        [Required]
        [StringLength(500)]
        public string Details { get; set; } = string.Empty;
        
        public int? PreviousStatus { get; set; } // 1, 2, 3 según tu CHECK constraint
        
        public int? NewStatus { get; set; } // 1, 2, 3 según tu CHECK constraint
        
        [Required]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        
        // Eliminamos las propiedades de navegación para evitar conflictos
        // Las relaciones se cargarán manualmente en el repositorio cuando sea necesario
    }
}
