using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppCreditosBackEnd.Application.DTOs;
using AppCreditosBackEnd.Application.DTOs.Input;
using AppCreditosBackEnd.Application.Interfaces;
using AppCreditosBackEnd.Domain.Interfaces;
using AutoMapper;

namespace AppCreditosBackEnd.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _repository;

        public UserService(IMapper mapper, IUserRepository repository)
        {
            _mapper = mapper;
            _repository = repository;
        }
        public async Task<UserDTO> CreateUserAsync(RegisterClientRequestDto dto)
        {
            try
            {
                // Intentamos buscar el usuario por email
                try
                {
                    var existingUser = await _repository.GetByEmailAsync(dto.Email);
                    // Si llegamos aquí, significa que encontramos un usuario con ese email
                    throw new Exception("User with this email already exists");
                }
                catch (KeyNotFoundException)
                {
                    // Si el usuario no existe, continuamos con la creación (esto es lo que queremos)
                }
                
                var user = _mapper.Map<Domain.Entities.Users>(dto);
                user.Id = Guid.NewGuid(); // Ensure a new ID is generated
                var createdUser = await _repository.CreateAsync(user);
                return _mapper.Map<UserDTO>(createdUser);
            }
            catch (Exception ex) when (!(ex is KeyNotFoundException))
            {
                throw new Exception($"Error creating user: {ex.Message}", ex);
            }
        }        
        public async Task DeleteUserAsync(Guid userId)
        {
            try
            {
                // Verificamos que el usuario exista
                await _repository.GetByIdAsync(userId);
                // Si llegamos aquí, el usuario existe, procedemos a eliminarlo
                await _repository.DeleteAsync(userId);
            }
            catch (KeyNotFoundException)
            {
                // Simplemente reenviamos la excepción
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting user: {ex.Message}", ex);
            }
        }
        
        public async Task<UserDTO> GetUserByIdAsync(Guid userId)
        {
            var user = await _repository.GetByIdAsync(userId);
            // No necesitamos verificar si user es null porque GetByIdAsync ya lanza KeyNotFoundException
            return _mapper.Map<UserDTO>(user);
        }

        public async Task<List<UserDTO>> GetUsersAsync()
        {
            var Users = await _repository.GetAllAsync();
            return _mapper.Map<List<UserDTO>>(Users);
        }        
        public async Task<UserDTO> UpdateUserAsync(Guid userId, UpdateUserDto dto)
        {
            try
            {
                var user = await _repository.GetByIdAsync(userId);
                // No necesitamos verificar si user es null porque GetByIdAsync ya lanza KeyNotFoundException
                
                // Update user properties
                user.FirstName = dto.FirstName ?? user.FirstName;
                user.LastName = dto.LastName ?? user.LastName;
                user.Email = dto.Email ?? user.Email;
                user.IdentificationNumber = dto.IdentificationNumber ?? user.IdentificationNumber;
                user.BirthDate = dto.BirthDate;
                user.Role = dto.Role ?? user.Role;
                //user.Password = dto.Password ?? user.Password;
                
                var updatedUser = await _repository.UpdateAsync(user);
                return _mapper.Map<UserDTO>(updatedUser);
            }
            catch (KeyNotFoundException)
            {
                // Simplemente reenviamos la excepción
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating user: {ex.Message}", ex);
            }
        }
    }
}
