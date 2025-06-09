using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppCreditosBackEnd.Domain.Entities;
using AppCreditosBackEnd.Domain.Enums;

namespace AppCreditosBackEnd.Domain.Interfaces
{
    public interface ICreditApplicationRepository
    {
        Task<List<CreditApplication>> GetAllAsync(ApplicationStatus? status);
        Task<CreditApplication?> GetByIdAsync(int id);
        Task<CreditApplication> CreateAsync(CreditApplication application);
        Task<List<CreditApplication>> GetByUserIdAsync(int userId);

        Task UpdateAsync(CreditApplication application);
        Task DeleteAsync(int idApplication);
    }
}
