using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppCreditosBackEnd.Domain.Entities
{
    public class CardApplication
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public Guid AccountId { get; set; }

        [Required]
        public int CardType { get; set; } // 1 = Débito, 2 = Crédito

        [Required]
        public int Status { get; set; } // 1=Pending, 2=Approved, 3=Rejected

        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public Guid? ApprovedById { get; set; }

        public int? CreditApplicationId { get; set; }

        // Navegación
        [ForeignKey("UserId")]
        public virtual Users User { get; set; } = null!;

        [ForeignKey("AccountId")]
        public virtual Account Account { get; set; } = null!;

        [ForeignKey("ApprovedById")]
        public virtual Users? ApprovedBy { get; set; }

        [ForeignKey("CreditApplicationId")]
        public virtual CreditApplication? CreditApplication { get; set; }

        public virtual ICollection<Card> Cards { get; set; } = new List<Card>();
    }
}
