using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppCreditosBackEnd.Domain.Entities
{
    public class Transaction
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid? FromAccountId { get; set; }

        [Required]
        public Guid ToAccountId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [StringLength(255)]
        public string? Description { get; set; }

        [Required]
        public int TransactionType { get; set; } // 1=Transferencia, 2=PagoTarjeta, 3=Retiro, 4=Depósito

        [Required]
        public int Status { get; set; } // 1=Success, 2=Pending, 3=Failed

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ProcessedAt { get; set; }

        [StringLength(50)]
        public string? ReferenceCode { get; set; }

        public Guid? CardId { get; set; }

        public int? CreditApplicationId { get; set; }

        // Navegación
        [ForeignKey("FromAccountId")]
        public virtual Account? FromAccount { get; set; }

        [ForeignKey("ToAccountId")]
        public virtual Account ToAccount { get; set; } = null!;

        [ForeignKey("CardId")]
        public virtual Card? Card { get; set; }

        [ForeignKey("CreditApplicationId")]
        public virtual CreditApplication? CreditApplication { get; set; }
    }
}
