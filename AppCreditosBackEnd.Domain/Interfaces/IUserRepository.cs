using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppCreditosBackEnd.Domain.Entities;

namespace AppCreditosBackEnd.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<Users> GetByIdAsync(Guid userId);
        Task<List<Users>> GetAllAsync();
        Task<Users> CreateAsync(Users user);
        Task<Users> UpdateAsync(Users user);
        Task DeleteAsync(Guid userId);
        Task<Users> GetByEmailAsync(string email);

    }
}
