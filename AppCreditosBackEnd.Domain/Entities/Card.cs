using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppCreditosBackEnd.Domain.Entities
{
    public class Card
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid CardApplicationId { get; set; }

        [Required]
        [StringLength(19)]
        public string CardNumber { get; set; } = string.Empty;

        [Required]
        public DateTime ExpiryDate { get; set; }

        [Required]
        [StringLength(4)]
        public string CVC { get; set; } = string.Empty;

        public DateTime IssuedDate { get; set; } = DateTime.UtcNow;

        [Required]
        public int Status { get; set; } // 1=Active, 2=Blocked, 3=Cancelled

        // Navegación
        [ForeignKey("CardApplicationId")]
        public virtual CardApplication CardApplication { get; set; } = null!;

        public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
