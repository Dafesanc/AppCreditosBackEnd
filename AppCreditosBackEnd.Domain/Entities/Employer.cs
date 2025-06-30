using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppCreditosBackEnd.Domain.Entities
{
    public class Employer
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid UserId { get; set; }

        [Required]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string RUC { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Address { get; set; }

        [StringLength(20)]
        public string? ContactPhone { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navegación
        [ForeignKey("UserId")]
        public virtual Users User { get; set; } = null!;

        public virtual ICollection<UserEmployment> UserEmployments { get; set; } = new List<UserEmployment>();
    }
}
