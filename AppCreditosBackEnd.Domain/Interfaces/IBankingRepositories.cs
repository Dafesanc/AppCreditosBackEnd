using AppCreditosBackEnd.Domain.Entities;

namespace AppCreditosBackEnd.Domain.Interfaces
{
    public interface IAccountRepository
    {
        Task<IEnumerable<Account>> GetAllAsync();
        Task<Account?> GetByIdAsync(Guid id);
        Task<IEnumerable<Account>> GetByUserIdAsync(Guid userId);
        Task<Account> CreateAsync(Account account);
        Task<Account> UpdateAsync(Account account);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<bool> AccountNumberExistsAsync(string accountNumber);
    }

    public interface IEmployerRepository
    {
        Task<IEnumerable<Employer>> GetAllAsync();
        Task<Employer?> GetByIdAsync(Guid id);
        Task<IEnumerable<Employer>> GetByUserIdAsync(Guid userId);
        Task<Employer> CreateAsync(Employer employer);
        Task<Employer> UpdateAsync(Employer employer);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<bool> RUCExistsAsync(string ruc);
    }

    public interface ICardApplicationRepository
    {
        Task<IEnumerable<CardApplication>> GetAllAsync();
        Task<CardApplication?> GetByIdAsync(Guid id);
        Task<IEnumerable<CardApplication>> GetByUserIdAsync(Guid userId);
        Task<CardApplication> CreateAsync(CardApplication cardApplication);
        Task<CardApplication> UpdateAsync(CardApplication cardApplication);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<CardApplication?> GetWithDetailsAsync(Guid id);
        Task<IEnumerable<CardApplication>> GetAllWithDetailsAsync();
        Task<IEnumerable<CardApplication>> GetByUserIdWithDetailsAsync(Guid userId);
    }

    public interface ICardRepository
    {
        Task<IEnumerable<Card>> GetAllAsync();
        Task<Card?> GetByIdAsync(Guid id);
        Task<IEnumerable<Card>> GetByUserIdAsync(Guid userId);
        Task<IEnumerable<Card>> GetActiveCardsByUserIdAsync(Guid userId);
        Task<Card?> GetByCardNumberAsync(string cardNumber);
        Task<Card> CreateAsync(Card card);
        Task<Card> UpdateAsync(Card card);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<bool> CardNumberExistsAsync(string cardNumber);
        Task<Card?> GetByCardNumberWithSPAsync(string cardNumber);
    }
}
