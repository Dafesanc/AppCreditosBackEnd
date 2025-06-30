using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppCreditosBackEnd.Domain.Entities
{
    public class Account
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid UserId { get; set; }

        [Required]
        [StringLength(50)]
        public string AccountNumber { get; set; } = string.Empty;

        [Required]
        public int AccountType { get; set; } // 1 = Ahorro, 2 = Corriente

        [Column(TypeName = "decimal(18,2)")]
        public decimal Balance { get; set; } = 0.00m;

        public byte isApproved { get; set; } = 0; // 0=No aprobado, 1=Aprobado, 2=Suspendida, 5=Eliminada

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navegación
        [ForeignKey("UserId")]
        public virtual Users User { get; set; } = null!;

        public virtual ICollection<CardApplication> CardApplications { get; set; } = new List<CardApplication>();
        public virtual ICollection<Transaction> FromTransactions { get; set; } = new List<Transaction>();
        public virtual ICollection<Transaction> ToTransactions { get; set; } = new List<Transaction>();
    }
}
