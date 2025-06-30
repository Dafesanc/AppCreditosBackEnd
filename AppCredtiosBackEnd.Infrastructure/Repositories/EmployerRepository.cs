using AppCreditosBackEnd.Domain.Entities;
using AppCreditosBackEnd.Domain.Interfaces;
using AppCreditosBackEnd.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace AppCreditosBackEnd.Infrastructure.Repositories
{
    public class EmployerRepository : IEmployerRepository
    {
        private readonly CreditPlatformDbContext _context;

        public EmployerRepository(CreditPlatformDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Employer>> GetAllAsync()
        {
            return await _context.Employers
                .Include(e => e.User)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();
        }

        public async Task<Employer?> GetByIdAsync(Guid id)
        {
            return await _context.Employers
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<Employer>> GetByUserIdAsync(Guid userId)
        {
            return await _context.Employers
                .Include(e => e.User)
                .Where(e => e.UserId == userId)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();
        }

        public async Task<Employer> CreateAsync(Employer employer)
        {
            _context.Employers.Add(employer);
            await _context.SaveChangesAsync();
            
            // Cargar la entidad con las relaciones
            await _context.Entry(employer)
                .Reference(e => e.User)
                .LoadAsync();
            
            return employer;
        }

        public async Task<Employer> UpdateAsync(Employer employer)
        {
            _context.Employers.Update(employer);
            await _context.SaveChangesAsync();
            
            // Cargar la entidad con las relaciones
            await _context.Entry(employer)
                .Reference(e => e.User)
                .LoadAsync();
            
            return employer;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var employer = await _context.Employers.FindAsync(id);
            if (employer == null)
                return false;

            _context.Employers.Remove(employer);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Employers.AnyAsync(e => e.Id == id);
        }

        public async Task<bool> RUCExistsAsync(string ruc)
        {
            return await _context.Employers.AnyAsync(e => e.RUC == ruc);
        }
    }
}
