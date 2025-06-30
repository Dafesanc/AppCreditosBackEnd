using AppCreditosBackEnd.Domain.Entities;
using AppCreditosBackEnd.Domain.Interfaces;
using AppCreditosBackEnd.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace AppCreditosBackEnd.Infrastructure.Repositories
{
    public class CardApplicationRepository : ICardApplicationRepository
    {
        private readonly CreditPlatformDbContext _context;

        public CardApplicationRepository(CreditPlatformDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CardApplication>> GetAllAsync()
        {
            return await _context.CardApplications
                .Include(ca => ca.User)
                .Include(ca => ca.Account)
                .Include(ca => ca.ApprovedBy)
                .Include(ca => ca.CreditApplication)
                .OrderByDescending(ca => ca.RequestedAt)
                .ToListAsync();
        }

        public async Task<CardApplication?> GetByIdAsync(Guid id)
        {
            return await _context.CardApplications
                .Include(ca => ca.User)
                .Include(ca => ca.Account)
                .Include(ca => ca.ApprovedBy)
                .Include(ca => ca.CreditApplication)
                .FirstOrDefaultAsync(ca => ca.Id == id);
        }

        public async Task<IEnumerable<CardApplication>> GetByUserIdAsync(Guid userId)
        {
            return await _context.CardApplications
                .Include(ca => ca.User)
                .Include(ca => ca.Account)
                .Include(ca => ca.ApprovedBy)
                .Include(ca => ca.CreditApplication)
                .Where(ca => ca.UserId == userId)
                .OrderByDescending(ca => ca.RequestedAt)
                .ToListAsync();
        }

        public async Task<CardApplication> CreateAsync(CardApplication cardApplication)
        {
            _context.CardApplications.Add(cardApplication);
            await _context.SaveChangesAsync();
            
            // Cargar la entidad con las relaciones
            await _context.Entry(cardApplication)
                .Reference(ca => ca.User)
                .LoadAsync();
            await _context.Entry(cardApplication)
                .Reference(ca => ca.Account)
                .LoadAsync();
            
            return cardApplication;
        }

        public async Task<CardApplication> UpdateAsync(CardApplication cardApplication)
        {
            _context.CardApplications.Update(cardApplication);
            await _context.SaveChangesAsync();
            
            // Cargar la entidad con las relaciones
            await _context.Entry(cardApplication)
                .Reference(ca => ca.User)
                .LoadAsync();
            await _context.Entry(cardApplication)
                .Reference(ca => ca.Account)
                .LoadAsync();
            await _context.Entry(cardApplication)
                .Reference(ca => ca.ApprovedBy)
                .LoadAsync();
            
            return cardApplication;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var cardApplication = await _context.CardApplications.FindAsync(id);
            if (cardApplication == null)
                return false;

            _context.CardApplications.Remove(cardApplication);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.CardApplications.AnyAsync(ca => ca.Id == id);
        }

        public async Task<CardApplication?> GetWithDetailsAsync(Guid id)
        {
            return await _context.CardApplications
                .Include(ca => ca.User)
                .Include(ca => ca.Account)
                    .ThenInclude(a => a.User)
                .Include(ca => ca.ApprovedBy)
                .Include(ca => ca.CreditApplication)
                .FirstOrDefaultAsync(ca => ca.Id == id);
        }

        public async Task<IEnumerable<CardApplication>> GetAllWithDetailsAsync()
        {
            return await _context.CardApplications
                .Include(ca => ca.User)
                .Include(ca => ca.Account)
                    .ThenInclude(a => a.User)
                .Include(ca => ca.ApprovedBy)
                .Include(ca => ca.CreditApplication)
                .OrderByDescending(ca => ca.RequestedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<CardApplication>> GetByUserIdWithDetailsAsync(Guid userId)
        {
            return await _context.CardApplications
                .Include(ca => ca.User)
                .Include(ca => ca.Account)
                    .ThenInclude(a => a.User)
                .Include(ca => ca.ApprovedBy)
                .Include(ca => ca.CreditApplication)
                .Where(ca => ca.UserId == userId)
                .OrderByDescending(ca => ca.RequestedAt)
                .ToListAsync();
        }
    }
}
