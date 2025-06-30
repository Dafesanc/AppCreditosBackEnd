using AppCreditosBackEnd.Domain.Entities;
using AppCreditosBackEnd.Domain.Interfaces;
using AppCreditosBackEnd.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace AppCreditosBackEnd.Infrastructure.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly CreditPlatformDbContext _context;

        public AccountRepository(CreditPlatformDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Account>> GetAllAsync()
        {
            return await _context.Accounts
                .Include(a => a.User)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<Account?> GetByIdAsync(Guid id)
        {
            return await _context.Accounts
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Account>> GetByUserIdAsync(Guid userId)
        {
            return await _context.Accounts
                .Include(a => a.User)
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<Account> CreateAsync(Account account)
        {
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();
            
            // Cargar la entidad con las relaciones
            await _context.Entry(account)
                .Reference(a => a.User)
                .LoadAsync();
            
            return account;
        }

        public async Task<Account> UpdateAsync(Account account)
        {
            _context.Accounts.Update(account);
            await _context.SaveChangesAsync();
            
            // Cargar la entidad con las relaciones
            await _context.Entry(account)
                .Reference(a => a.User)
                .LoadAsync();
            
            return account;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
                return false;

            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Accounts.AnyAsync(a => a.Id == id);
        }

        public async Task<bool> AccountNumberExistsAsync(string accountNumber)
        {
            return await _context.Accounts.AnyAsync(a => a.AccountNumber == accountNumber);
        }
    }
}
