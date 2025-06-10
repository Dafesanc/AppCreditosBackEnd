using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppCreditosBackEnd.Domain.Entities;
using AppCreditosBackEnd.Domain.Enums;
using AppCreditosBackEnd.Domain.Interfaces;
using AppCreditosBackEnd.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace AppCreditosBackEnd.Infrastructure.Repositories
{
    // Infrastructure/Repositories/CreditApplicationRepository.cs
    public class CreditApplicationRepository : ICreditApplicationRepository
    {
        private readonly CreditPlatformDbContext _context;

        public CreditApplicationRepository(CreditPlatformDbContext context)
        {
            _context = context;
        }

        public async Task<CreditApplication> CreateAsync(CreditApplication application)
        {
            _context.CreditApplications.Add(application);
            await _context.SaveChangesAsync();
            return application;
        }

        public async Task<CreditApplication?> GetByIdAsync(int id)
        {
            return await _context.CreditApplications
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<CreditApplication>> GetByUserIdAsync(Guid userId)
        {
            return await _context.CreditApplications
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<CreditApplication>> GetAllAsync(ApplicationStatus? status = null)
        {
            var query = _context.CreditApplications
                .Include(x => x.User)
                .AsQueryable();

            if (status.HasValue)
                query = query.Where(x => x.Status == status.Value);

            return await query
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task UpdateAsync(CreditApplication application)
        {
            _context.CreditApplications.Update(application);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int idApplication)
        {
            var application = await _context.CreditApplications.FindAsync(idApplication);
            if (application != null)
            {
                _context.CreditApplications.Remove(application);
                await _context.SaveChangesAsync();
            }
        }
    }
}
