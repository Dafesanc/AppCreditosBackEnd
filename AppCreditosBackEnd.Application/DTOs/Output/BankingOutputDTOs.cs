using AppCreditosBackEnd.Domain.Enums;

namespace AppCreditosBackEnd.Application.DTOs.Output
{
    public class AccountDTO
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
        public string AccountTypeName { get; set; } = string.Empty;
        public AccountType AccountType { get; set; }
        public decimal Balance { get; set; }
        public string IsApprovedName { get; set; } = string.Empty;
        public AccountStatus IsApproved { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserFullName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
    }

    public class EmployerDTO
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string RUC { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? ContactPhone { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserFullName { get; set; } = string.Empty;
    }

    public class CardApplicationDTO
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string UserFullName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public Guid AccountId { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
        public string CardTypeName { get; set; } = string.Empty;
        public CardType CardType { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public CardApplicationStatus Status { get; set; }
        public DateTime RequestedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? ApprovedById { get; set; }
        public string? ApprovedByName { get; set; }
        public int? CreditApplicationId { get; set; }
        public bool HasCard { get; set; }
        public CardDTO? Card { get; set; }
    }

    public class CardDTO
    {
        public Guid Id { get; set; }
        public Guid CardApplicationId { get; set; }
        public string CardNumber { get; set; } = string.Empty;
        public string MaskedCardNumber { get; set; } = string.Empty;
        public DateTime ExpiryDate { get; set; }
        public string CVC { get; set; } = string.Empty;
        public DateTime IssuedDate { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public CardStatus Status { get; set; }
    }

    public class UserEmploymentDTO
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string UserFullName { get; set; } = string.Empty;
        public Guid EmployerId { get; set; }
        public string EmployerName { get; set; } = string.Empty;
        public string? Position { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal MonthlyIncome { get; set; }
        public bool IsCurrent { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class TransactionDTO
    {
        public Guid Id { get; set; }
        public Guid? FromAccountId { get; set; }
        public string? FromAccountNumber { get; set; }
        public Guid ToAccountId { get; set; }
        public string ToAccountNumber { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string? Description { get; set; }
        public TransactionType TransactionType { get; set; }
        public string TransactionTypeName { get; set; } = string.Empty;
        public TransactionStatus Status { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public string? ReferenceCode { get; set; }
        public Guid? CardId { get; set; }
        public int? CreditApplicationId { get; set; }
    }

    public class StoredProcedureResultDTO
    {
        public string Result { get; set; } = string.Empty;
        public string? ErrorMessage { get; set; }
        public string? ReferenceCode { get; set; }
    }
}
