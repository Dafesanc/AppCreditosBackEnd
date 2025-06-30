using AppCreditosBackEnd.Application.DTOs.Input;
using AppCreditosBackEnd.Application.DTOs.Output;
using AppCreditosBackEnd.Application.Interfaces;
using AppCreditosBackEnd.Domain.Entities;
using AppCreditosBackEnd.Domain.Enums;
using AppCreditosBackEnd.Domain.Interfaces;
using AutoMapper;

namespace AppCreditosBackEnd.Application.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public AccountService(
            IAccountRepository accountRepository,
            IUserRepository userRepository,
            IMapper mapper)
        {
            _accountRepository = accountRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AccountDTO>> GetAllAccountsAsync()
        {
            var accounts = await _accountRepository.GetAllAsync();
            return accounts.Select(MapToAccountDTO);
        }

        public async Task<AccountDTO?> GetAccountByIdAsync(Guid id)
        {
            var account = await _accountRepository.GetByIdAsync(id);
            return account != null ? MapToAccountDTO(account) : null;
        }

        public async Task<IEnumerable<AccountDTO>> GetAccountsByUserIdAsync(Guid userId)
        {
            var accounts = await _accountRepository.GetByUserIdAsync(userId);
            return accounts.Select(MapToAccountDTO);
        }

        public async Task<AccountDTO> CreateAccountAsync(CreateAccountDTO createAccountDto)
        {
            // Validar que el número de cuenta no existe
            var accountNumberExists = await _accountRepository.AccountNumberExistsAsync(createAccountDto.AccountNumber);
            if (accountNumberExists)
            {
                throw new ArgumentException("El número de cuenta ya existe.");
            }

            var account = new Account
            {
                UserId = createAccountDto.UserId,
                AccountNumber = createAccountDto.AccountNumber,
                AccountType = (int)createAccountDto.AccountType,
                Balance = 0.00m,
                isApproved = 0, // No aprobado por defecto
                CreatedAt = DateTime.UtcNow
            };

            var createdAccount = await _accountRepository.CreateAsync(account);
            return MapToAccountDTO(createdAccount);
        }

        public async Task<AccountDTO> UpdateAccountAsync(Guid id, UpdateAccountDTO updateAccountDto)
        {
            var account = await _accountRepository.GetByIdAsync(id);
            if (account == null)
            {
                throw new ArgumentException("La cuenta no existe.");
            }

            // Actualizar solo los campos proporcionados
            if (updateAccountDto.IsApproved.HasValue)
            {
                account.isApproved = (byte)updateAccountDto.IsApproved.Value;
            }

            if (updateAccountDto.Balance.HasValue)
            {
                account.Balance = updateAccountDto.Balance.Value;
            }

            var updatedAccount = await _accountRepository.UpdateAsync(account);
            return MapToAccountDTO(updatedAccount);
        }

        public async Task<bool> DeleteAccountAsync(Guid id)
        {
            return await _accountRepository.DeleteAsync(id);
        }

        public async Task<StoredProcedureResultDTO> CreateAccountWithSPAsync(CreateAccountDTO createAccountDto)
        {
            // Por ahora devolver un mock, se puede implementar con el repositorio si es necesario
            try
            {
                var account = await CreateAccountAsync(createAccountDto);
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

        private AccountDTO MapToAccountDTO(Account account)
        {
            return new AccountDTO
            {
                Id = account.Id,
                UserId = account.UserId,
                AccountNumber = account.AccountNumber,
                AccountType = (AccountType)account.AccountType,
                AccountTypeName = GetAccountTypeName((AccountType)account.AccountType),
                Balance = account.Balance,
                IsApproved = (AccountStatus)account.isApproved,
                IsApprovedName = GetAccountStatusName((AccountStatus)account.isApproved),
                CreatedAt = account.CreatedAt,
                UserFullName = account.User != null ? $"{account.User.FirstName} {account.User.LastName}" : "",
                UserEmail = account.User?.Email ?? ""
            };
        }

        private string GetAccountTypeName(AccountType accountType)
        {
            return accountType switch
            {
                AccountType.Savings => "Ahorro",
                AccountType.Current => "Corriente",
                _ => "Desconocido"
            };
        }

        private string GetAccountStatusName(AccountStatus accountStatus)
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
    }
}
