using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppCreditosBackEnd.Application.DTOs;
using AppCreditosBackEnd.Application.DTOs.Input;

namespace AppCreditosBackEnd.Application.Interfaces
{
    public interface IUserService
    {
        Task<List<UserDTO>> GetUsersAsync();
        Task<UserDTO> GetUserByIdAsync(Guid userId);
        Task<UserDTO> CreateUserAsync(RegisterClientRequestDto dto);
        Task<UserDTO> UpdateUserAsync(Guid userId, UpdateUserDto dto);
        Task DeleteUserAsync(Guid userId);

    }
}
