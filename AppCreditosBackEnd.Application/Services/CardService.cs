using AppCreditosBackEnd.Application.DTOs.Output;
using AppCreditosBackEnd.Application.Interfaces;
using AppCreditosBackEnd.Domain.Entities;
using AppCreditosBackEnd.Domain.Enums;
using AppCreditosBackEnd.Domain.Interfaces;
using AutoMapper;

namespace AppCreditosBackEnd.Application.Services
{
    public class CardService : ICardService
    {
        private readonly ICardRepository _cardRepository;
        private readonly IMapper _mapper;

        public CardService(
            ICardRepository cardRepository,
            IMapper mapper)
        {
            _cardRepository = cardRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CardDTO>> GetAllCardsAsync()
        {
            var cards = await _cardRepository.GetAllAsync();
            return cards.Select(MapToCardDTO);
        }

        public async Task<CardDTO?> GetCardByIdAsync(Guid id)
        {
            var card = await _cardRepository.GetByIdAsync(id);
            return card != null ? MapToCardDTO(card) : null;
        }

        public async Task<IEnumerable<CardDTO>> GetMyCardsAsync(Guid userId)
        {
            var cards = await _cardRepository.GetActiveCardsByUserIdAsync(userId);
            return cards.Select(MapToCardDTO);
        }

        public async Task<CardDTO?> GetCardByNumberAsync(string cardNumber)
        {
            var card = await _cardRepository.GetByCardNumberAsync(cardNumber);
            return card != null ? MapToCardDTO(card) : null;
        }

        public async Task<CardDTO?> GetCardByNumberWithSPAsync(string cardNumber)
        {
            var card = await _cardRepository.GetByCardNumberWithSPAsync(cardNumber);
            return card != null ? MapToCardDTO(card) : null;
        }

        public async Task<bool> DeleteCardAsync(Guid id)
        {
            return await _cardRepository.DeleteAsync(id);
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

            // Mostrar solo los últimos 4 dígitos: **** **** **** 1234
            var masked = new string('*', cardNumber.Length - 4) + cardNumber.Substring(cardNumber.Length - 4);
            
            // Formatear con espacios cada 4 dígitos
            if (masked.Length == 16)
            {
                return $"{masked.Substring(0, 4)} {masked.Substring(4, 4)} {masked.Substring(8, 4)} {masked.Substring(12, 4)}";
            }
            
            return masked;
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
