using AutoMapper;
using AppCreditosBackEnd.Application.DTOs;
using AppCreditosBackEnd.Application.DTOs.Input;
using AppCreditosBackEnd.Application.DTOs.Output;
using AppCreditosBackEnd.Domain.Entities;
using AppCreditosBackEnd.Domain.Enums;

namespace AppCreditosBackEnd.Application.Mappings
{
    public class MappingProfile : Profile
    {        public MappingProfile()
        {            // Mapeo de CreditApplication a CreditApplicationResponseDto
            CreateMap<CreditApplication, CreditApplicationResponseDto>()
                .ForMember(dest => dest.ApplicantName, 
                    opt => opt.MapFrom(src => src.User != null 
                        ? $"{src.User.FirstName} {src.User.LastName}" 
                        : "Unknown"));
            
            // Mapeos para la entidad Users
            CreateMap<Users, UserDTO>();
            CreateMap<RegisterRequestDto, Users>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
            CreateMap<RegisterClientRequestDto, Users>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => (UserRole)src.Role))
                .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => src.BirthDate))
                .ForMember(dest => dest.IdentificationType, opt => opt.MapFrom(src => src.IdentificationType))
                .ForMember(dest => dest.IdentificationNumber, opt => opt.MapFrom(src => src.IdentificationNumber));
                
            // Mapeo para AuditLog eliminado - ahora se maneja manualmente en el servicio

            // === NUEVOS MAPPINGS BANCARIOS ===

            // Mappings para Account
            CreateMap<Account, AccountDTO>()
                .ForMember(dest => dest.AccountType, opt => opt.MapFrom(src => (AccountType)src.AccountType))
                .ForMember(dest => dest.AccountTypeName, opt => opt.MapFrom(src => GetAccountTypeName((AccountType)src.AccountType)))
                .ForMember(dest => dest.IsApproved, opt => opt.MapFrom(src => (AccountStatus)src.isApproved))
                .ForMember(dest => dest.IsApprovedName, opt => opt.MapFrom(src => GetAccountStatusName((AccountStatus)src.isApproved)))
                .ForMember(dest => dest.UserFullName, opt => opt.MapFrom(src => src.User != null ? $"{src.User.FirstName} {src.User.LastName}" : ""))
                .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User != null ? src.User.Email : ""));

            CreateMap<CreateAccountDTO, Account>()
                .ForMember(dest => dest.AccountType, opt => opt.MapFrom(src => (int)src.AccountType))
                .ForMember(dest => dest.Balance, opt => opt.MapFrom(src => 0.00m))
                .ForMember(dest => dest.isApproved, opt => opt.MapFrom(src => (byte)AccountStatus.NotApproved))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            // Mappings para Employer
            CreateMap<Employer, EmployerDTO>()
                .ForMember(dest => dest.UserFullName, opt => opt.MapFrom(src => src.User != null ? $"{src.User.FirstName} {src.User.LastName}" : ""));

            CreateMap<CreateEmployerDTO, Employer>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            // Mappings para CardApplication
            CreateMap<CardApplication, CardApplicationDTO>()
                .ForMember(dest => dest.CardType, opt => opt.MapFrom(src => (CardType)src.CardType))
                .ForMember(dest => dest.CardTypeName, opt => opt.MapFrom(src => GetCardTypeName((CardType)src.CardType)))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (CardApplicationStatus)src.Status))
                .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => GetCardApplicationStatusName((CardApplicationStatus)src.Status)))
                .ForMember(dest => dest.UserFullName, opt => opt.MapFrom(src => src.User != null ? $"{src.User.FirstName} {src.User.LastName}" : ""))
                .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User != null ? src.User.Email : ""))
                .ForMember(dest => dest.AccountNumber, opt => opt.MapFrom(src => src.Account != null ? src.Account.AccountNumber : ""))
                .ForMember(dest => dest.ApprovedByName, opt => opt.MapFrom(src => src.ApprovedBy != null ? $"{src.ApprovedBy.FirstName} {src.ApprovedBy.LastName}" : null))
                .ForMember(dest => dest.HasCard, opt => opt.MapFrom(src => src.Cards != null && src.Cards.Any()))
                .ForMember(dest => dest.Card, opt => opt.MapFrom(src => src.Cards != null && src.Cards.Any() ? src.Cards.First() : null));

            CreateMap<CreateCardApplicationDTO, CardApplication>()
                .ForMember(dest => dest.CardType, opt => opt.MapFrom(src => (int)src.CardType))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (int)CardApplicationStatus.Pending))
                .ForMember(dest => dest.RequestedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            // Mappings para Card
            CreateMap<Card, CardDTO>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (CardStatus)src.Status))
                .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => GetCardStatusName((CardStatus)src.Status)))
                .ForMember(dest => dest.MaskedCardNumber, opt => opt.MapFrom(src => MaskCardNumber(src.CardNumber)));

            // Mappings para UserEmployment
            CreateMap<UserEmployment, UserEmploymentDTO>()
                .ForMember(dest => dest.EmployerName, opt => opt.MapFrom(src => src.Employer != null ? src.Employer.Name : ""))
                .ForMember(dest => dest.UserFullName, opt => opt.MapFrom(src => src.User != null ? $"{src.User.FirstName} {src.User.LastName}" : ""));

            // Mappings para Transaction
            CreateMap<Transaction, TransactionDTO>()
                .ForMember(dest => dest.TransactionType, opt => opt.MapFrom(src => (TransactionType)src.TransactionType))
                .ForMember(dest => dest.TransactionTypeName, opt => opt.MapFrom(src => GetTransactionTypeName((TransactionType)src.TransactionType)))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (TransactionStatus)src.Status))
                .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => GetTransactionStatusName((TransactionStatus)src.Status)))
                .ForMember(dest => dest.FromAccountNumber, opt => opt.MapFrom(src => src.FromAccount != null ? src.FromAccount.AccountNumber : null))
                .ForMember(dest => dest.ToAccountNumber, opt => opt.MapFrom(src => src.ToAccount != null ? src.ToAccount.AccountNumber : ""));
        }

        // Métodos auxiliares para nombres de enums
        private static string GetAccountTypeName(AccountType accountType)
        {
            return accountType switch
            {
                AccountType.Savings => "Ahorro",
                AccountType.Current => "Corriente",
                _ => "Desconocido"
            };
        }

        private static string GetAccountStatusName(AccountStatus accountStatus)
        {
            return accountStatus switch
            {
                AccountStatus.NotApproved => "No Aprobado",
                AccountStatus.Approved => "Aprobado",
                AccountStatus.Suspended => "Suspendida",
                AccountStatus.Deleted => "Eliminada",
                _ => "Desconocido"
            };
        }

        private static string GetCardTypeName(CardType cardType)
        {
            return cardType switch
            {
                CardType.Debit => "Débito",
                CardType.Credit => "Crédito",
                _ => "Desconocido"
            };
        }

        private static string GetCardApplicationStatusName(CardApplicationStatus status)
        {
            return status switch
            {
                CardApplicationStatus.Pending => "Pendiente",
                CardApplicationStatus.Approved => "Aprobada",
                CardApplicationStatus.Rejected => "Rechazada",
                _ => "Desconocido"
            };
        }

        private static string GetCardStatusName(CardStatus status)
        {
            return status switch
            {
                CardStatus.Active => "Activa",
                CardStatus.Blocked => "Bloqueada",
                CardStatus.Cancelled => "Cancelada",
                _ => "Desconocido"
            };
        }

        private static string GetTransactionTypeName(TransactionType transactionType)
        {
            return transactionType switch
            {
                TransactionType.Transfer => "Transferencia",
                TransactionType.CardPayment => "Pago con Tarjeta",
                TransactionType.Withdrawal => "Retiro",
                TransactionType.Deposit => "Depósito",
                _ => "Desconocido"
            };
        }

        private static string GetTransactionStatusName(TransactionStatus status)
        {
            return status switch
            {
                TransactionStatus.Success => "Exitosa",
                TransactionStatus.Pending => "Pendiente",
                TransactionStatus.Failed => "Fallida",
                _ => "Desconocido"
            };
        }

        private static string MaskCardNumber(string cardNumber)
        {
            if (string.IsNullOrEmpty(cardNumber) || cardNumber.Length < 4)
                return cardNumber;

            return "**** **** **** " + cardNumber.Substring(cardNumber.Length - 4);
        }
    }
}
