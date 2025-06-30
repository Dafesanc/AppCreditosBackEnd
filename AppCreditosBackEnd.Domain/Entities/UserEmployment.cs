using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppCreditosBackEnd.Domain.Entities
{
    public class UserEmployment
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public Guid EmployerId { get; set; }

        [StringLength(100)]
        public string? Position { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal MonthlyIncome { get; set; }

        public bool IsCurrent { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navegación
        [ForeignKey("UserId")]
        public virtual Users User { get; set; } = null!;

        [ForeignKey("EmployerId")]
        public virtual Employer Employer { get; set; } = null!;
    }
}
