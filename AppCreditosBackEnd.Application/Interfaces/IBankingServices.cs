using AppCreditosBackEnd.Application.DTOs.Input;
using AppCreditosBackEnd.Application.DTOs.Output;

namespace AppCreditosBackEnd.Application.Interfaces
{
    public interface IAccountService
    {
        Task<IEnumerable<AccountDTO>> GetAllAccountsAsync();
        Task<AccountDTO?> GetAccountByIdAsync(Guid id);
        Task<IEnumerable<AccountDTO>> GetAccountsByUserIdAsync(Guid userId);
        Task<AccountDTO> CreateAccountAsync(CreateAccountDTO createAccountDto);
        Task<AccountDTO> UpdateAccountAsync(Guid id, UpdateAccountDTO updateAccountDto);
        Task<bool> DeleteAccountAsync(Guid id);
        Task<StoredProcedureResultDTO> CreateAccountWithSPAsync(CreateAccountDTO createAccountDto);
    }

    public interface IEmployerService
    {
        Task<IEnumerable<EmployerDTO>> GetAllEmployersAsync();
        Task<EmployerDTO?> GetEmployerByIdAsync(Guid id);
        Task<IEnumerable<EmployerDTO>> GetEmployersByUserIdAsync(Guid userId);
        Task<EmployerDTO> CreateEmployerAsync(CreateEmployerDTO createEmployerDto, Guid userId);
        Task<EmployerDTO> UpdateEmployerAsync(Guid id, UpdateEmployerDTO updateEmployerDto);
        Task<bool> DeleteEmployerAsync(Guid id);
    }

    public interface IBankingCardApplicationService
    {
        Task<IEnumerable<CardApplicationDTO>> GetAllCardApplicationsAsync();
        Task<CardApplicationDTO?> GetCardApplicationByIdAsync(Guid id);
        Task<IEnumerable<CardApplicationDTO>> GetCardApplicationsByUserIdAsync(Guid userId);
        Task<CardApplicationDTO> CreateCardApplicationAsync(CreateCardApplicationDTO createCardApplicationDto, Guid userId);
        Task<CardApplicationDTO> UpdateCardApplicationStatusAsync(Guid id, UpdateCardApplicationStatusDTO updateStatusDto, Guid analystId);
        Task<bool> DeleteCardApplicationAsync(Guid id);
        Task<StoredProcedureResultDTO> ApplyForCardWithSPAsync(CreateCardApplicationDTO createCardApplicationDto, Guid userId);
        Task<StoredProcedureResultDTO> ApproveCardApplicationWithSPAsync(Guid applicationId, Guid analystId);
    }

    public interface ICardService
    {
        Task<IEnumerable<CardDTO>> GetAllCardsAsync();
        Task<CardDTO?> GetCardByIdAsync(Guid id);
        Task<IEnumerable<CardDTO>> GetMyCardsAsync(Guid userId);
        Task<CardDTO?> GetCardByNumberAsync(string cardNumber);
        Task<CardDTO?> GetCardByNumberWithSPAsync(string cardNumber);
        Task<bool> DeleteCardAsync(Guid id);
    }
}
