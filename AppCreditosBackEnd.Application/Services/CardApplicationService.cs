using AppCreditosBackEnd.Application.DTOs.Input;
using AppCreditosBackEnd.Application.DTOs.Output;
using AppCreditosBackEnd.Application.Interfaces;
using AppCreditosBackEnd.Domain.Entities;
using AppCreditosBackEnd.Domain.Enums;
using AppCreditosBackEnd.Domain.Interfaces;
using AutoMapper;

namespace AppCreditosBackEnd.Application.Services
{
    public class CardApplicationService : IBankingCardApplicationService
    {
        private readonly ICardApplicationRepository _cardApplicationRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public CardApplicationService(
            ICardApplicationRepository cardApplicationRepository,
            IAccountRepository accountRepository,
            IUserRepository userRepository,
            IMapper mapper)
        {
            _cardApplicationRepository = cardApplicationRepository;
            _accountRepository = accountRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CardApplicationDTO>> GetAllCardApplicationsAsync()
        {
            var applications = await _cardApplicationRepository.GetAllWithDetailsAsync();
            return applications.Select(MapToCardApplicationDTO);
        }

        public async Task<CardApplicationDTO?> GetCardApplicationByIdAsync(Guid id)
        {
            var application = await _cardApplicationRepository.GetWithDetailsAsync(id);
            return application != null ? MapToCardApplicationDTO(application) : null;
        }

        public async Task<IEnumerable<CardApplicationDTO>> GetCardApplicationsByUserIdAsync(Guid userId)
        {
            var applications = await _cardApplicationRepository.GetByUserIdWithDetailsAsync(userId);
            return applications.Select(MapToCardApplicationDTO);
        }

        public async Task<CardApplicationDTO> CreateCardApplicationAsync(CreateCardApplicationDTO createCardApplicationDto, Guid userId)
        {
            // Validar que la cuenta existe y pertenece al usuario
            var account = await _accountRepository.GetByIdAsync(createCardApplicationDto.AccountId);
            if (account == null)
            {
                throw new ArgumentException("La cuenta especificada no existe.");
            }

            if (account.UserId != userId)
            {
                throw new ArgumentException("No tienes permisos para crear una aplicación de tarjeta para esta cuenta.");
            }

            // Validar que la cuenta esté aprobada
            if (account.isApproved != (byte)AccountStatus.Approved)
            {
                throw new ArgumentException("La cuenta debe estar aprobada para solicitar una tarjeta.");
            }

            var cardApplication = new CardApplication
            {
                UserId = userId,
                AccountId = createCardApplicationDto.AccountId,
                CardType = (int)createCardApplicationDto.CardType,
                Status = (int)CardApplicationStatus.Pending,
                CreditApplicationId = createCardApplicationDto.CreditApplicationId,
                RequestedAt = DateTime.UtcNow
            };

            var createdApplication = await _cardApplicationRepository.CreateAsync(cardApplication);
            return MapToCardApplicationDTO(createdApplication);
        }

        public async Task<CardApplicationDTO> UpdateCardApplicationStatusAsync(Guid id, UpdateCardApplicationStatusDTO updateStatusDto, Guid analystId)
        {
            var application = await _cardApplicationRepository.GetWithDetailsAsync(id);
            if (application == null)
            {
                throw new ArgumentException("La aplicación de tarjeta no existe.");
            }

            application.Status = (int)updateStatusDto.Status;
            application.UpdatedAt = DateTime.UtcNow;
            application.ApprovedById = analystId;

            var updatedApplication = await _cardApplicationRepository.UpdateAsync(application);
            return MapToCardApplicationDTO(updatedApplication);
        }

        public async Task<bool> DeleteCardApplicationAsync(Guid id)
        {
            return await _cardApplicationRepository.DeleteAsync(id);
        }

        public async Task<StoredProcedureResultDTO> ApplyForCardWithSPAsync(CreateCardApplicationDTO createCardApplicationDto, Guid userId)
        {
            try
            {
                var application = await CreateCardApplicationAsync(createCardApplicationDto, userId);
                return new StoredProcedureResultDTO
                {
                    Result = "SUCCESS"
                };
            }
            catch (Exception ex)
            {
                return new StoredProcedureResultDTO
                {
                    Result = "ERROR",
                    ErrorMessage = ex.Message
                };
            }
        }

        public async Task<StoredProcedureResultDTO> ApproveCardApplicationWithSPAsync(Guid applicationId, Guid analystId)
        {
            try
            {
                var updateDto = new UpdateCardApplicationStatusDTO
                {
                    Status = CardApplicationStatus.Approved
                };
                
                var application = await UpdateCardApplicationStatusAsync(applicationId, updateDto, analystId);
                return new StoredProcedureResultDTO
                {
                    Result = "SUCCESS"
                };
            }
            catch (Exception ex)
            {
                return new StoredProcedureResultDTO
                {
                    Result = "ERROR",
                    ErrorMessage = ex.Message
                };
            }
        }

        private CardApplicationDTO MapToCardApplicationDTO(CardApplication application)
        {
            var card = application.Cards?.FirstOrDefault();
            
            return new CardApplicationDTO
            {
                Id = application.Id,
                UserId = application.UserId,
                UserFullName = application.User != null ? $"{application.User.FirstName} {application.User.LastName}" : "",
                UserEmail = application.User?.Email ?? "",
                AccountId = application.AccountId,
                AccountNumber = application.Account?.AccountNumber ?? "",
                CardType = (CardType)application.CardType,
                CardTypeName = GetCardTypeName((CardType)application.CardType),
                Status = (CardApplicationStatus)application.Status,
                StatusName = GetCardApplicationStatusName((CardApplicationStatus)application.Status),
                RequestedAt = application.RequestedAt,
                UpdatedAt = application.UpdatedAt,
                ApprovedById = application.ApprovedById,
                ApprovedByName = application.ApprovedBy != null ? $"{application.ApprovedBy.FirstName} {application.ApprovedBy.LastName}" : null,
                CreditApplicationId = application.CreditApplicationId,
                HasCard = card != null,
                Card = card != null ? MapToCardDTO(card) : null
            };
        }

        private CardDTO MapToCardDTO(Card card)
        {
            return new CardDTO
            {
                Id = card.Id,
                CardApplicationId = card.CardApplicationId,
                CardNumber = card.CardNumber,
                MaskedCardNumber = MaskCardNumber(card.CardNumber),
                ExpiryDate = card.ExpiryDate,
                CVC = card.CVC,
                IssuedDate = card.IssuedDate,
                Status = (CardStatus)card.Status,
                StatusName = GetCardStatusName((CardStatus)card.Status)
            };
        }

        private string MaskCardNumber(string cardNumber)
        {
            if (string.IsNullOrEmpty(cardNumber) || cardNumber.Length < 4)
                return cardNumber;

            return "**** **** **** " + cardNumber.Substring(cardNumber.Length - 4);
        }

        private string GetCardTypeName(CardType cardType)
        {
            return cardType switch
            {
                CardType.Debit => "Débito",
                CardType.Credit => "Crédito",
                _ => "Desconocido"
            };
        }

        private string GetCardApplicationStatusName(CardApplicationStatus status)
        {
            return status switch
            {
                CardApplicationStatus.Pending => "Pendiente",
                CardApplicationStatus.Approved => "Aprobada",
                CardApplicationStatus.Rejected => "Rechazada",
                _ => "Desconocido"
            };
        }

        private string GetCardStatusName(CardStatus status)
        {
            return status switch
            {
                CardStatus.Active => "Activa",
                CardStatus.Blocked => "Bloqueada",
                CardStatus.Cancelled => "Cancelada",
                _ => "Desconocido"
            };
        }
    }
}
