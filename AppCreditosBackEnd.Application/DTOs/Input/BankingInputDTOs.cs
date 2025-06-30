using AppCreditosBackEnd.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace AppCreditosBackEnd.Application.DTOs.Input
{
    public class CreateAccountDTO
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        [Range(1, 2, ErrorMessage = "AccountType debe ser 1 (Ahorro) o 2 (Corriente)")]
        public AccountType AccountType { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "El número de cuenta no puede exceder 50 caracteres")]
        public string AccountNumber { get; set; } = string.Empty;
    }

    public class UpdateAccountDTO
    {
        [Range(0, 5, ErrorMessage = "IsApproved debe estar entre 0 y 5")]
        public AccountStatus? IsApproved { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "El balance debe ser positivo")]
        public decimal? Balance { get; set; }
    }

    public class CreateEmployerDTO
    {
        [Required]
        [StringLength(150, ErrorMessage = "El nombre no puede exceder 150 caracteres")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(20, ErrorMessage = "El RUC no puede exceder 20 caracteres")]
        public string RUC { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "La dirección no puede exceder 200 caracteres")]
        public string? Address { get; set; }

        [StringLength(20, ErrorMessage = "El teléfono no puede exceder 20 caracteres")]
        public string? ContactPhone { get; set; }
    }

    public class UpdateEmployerDTO
    {
        [StringLength(150, ErrorMessage = "El nombre no puede exceder 150 caracteres")]
        public string? Name { get; set; }

        [StringLength(20, ErrorMessage = "El RUC no puede exceder 20 caracteres")]
        public string? RUC { get; set; }

        [StringLength(200, ErrorMessage = "La dirección no puede exceder 200 caracteres")]
        public string? Address { get; set; }

        [StringLength(20, ErrorMessage = "El teléfono no puede exceder 20 caracteres")]
        public string? ContactPhone { get; set; }
    }

    public class CreateCardApplicationDTO
    {
        [Required]
        public Guid AccountId { get; set; }

        [Required]
        [Range(1, 2, ErrorMessage = "CardType debe ser 1 (Débito) o 2 (Crédito)")]
        public CardType CardType { get; set; }

        public int? CreditApplicationId { get; set; }
    }

    public class UpdateCardApplicationStatusDTO
    {
        [Required]
        [Range(1, 3, ErrorMessage = "Status debe ser 1 (Pending), 2 (Approved) o 3 (Rejected)")]
        public CardApplicationStatus Status { get; set; }
    }
}
