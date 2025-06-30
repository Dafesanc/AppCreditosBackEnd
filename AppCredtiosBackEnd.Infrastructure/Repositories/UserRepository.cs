using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using AppCreditosBackEnd.Domain.Entities;
using AppCreditosBackEnd.Domain.Interfaces;
using AppCreditosBackEnd.Infrastructure.DbContext;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace AppCreditosBackEnd.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly CreditPlatformDbContext _context;

        public UserRepository(CreditPlatformDbContext context)
        {
            _context = context;
        }        
    
        public async Task<Users> CreateAsync(Users user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrEmpty(user.Email)) throw new ArgumentException("Email is required", nameof(user.Email));
            
            // Obtener la cadena de conexión del DbContext
            var connectionString = _context.Database.GetConnectionString();
            if (connectionString == null) throw new InvalidOperationException("Database connection string is null");
            
            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand("[dbo].[AddUser]", connection);
                command.CommandType = CommandType.StoredProcedure;
                      // Añadir parámetros
                command.Parameters.AddWithValue("@FirstName", user.FirstName != null ? user.FirstName : DBNull.Value);
                command.Parameters.AddWithValue("@LastName", user.LastName != null ? user.LastName : DBNull.Value);
                command.Parameters.AddWithValue("@Email", user.Email);
                command.Parameters.AddWithValue("@Password", user.Password != null ? user.Password : DBNull.Value);
                command.Parameters.AddWithValue("@IdentificationNumber", user.IdentificationNumber != null ? user.IdentificationNumber : DBNull.Value);
                command.Parameters.AddWithValue("@IdentificationType", user.IdentificationType != null ? user.IdentificationType : DBNull.Value);
                command.Parameters.AddWithValue("@Role", (int)user.Role);
                command.Parameters.AddWithValue("@BirthDate", user.BirthDate);

                try
                {
                    await connection.OpenAsync();
                    
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            var resultColumnIndex = reader.GetOrdinal("Result");
                            string result = reader.GetString(resultColumnIndex);
                            
                            if (result == "SUCCESS")
                            {
                                // Cerrar el reader antes de ejecutar otra consulta
                                reader.Close();
                                
                                // Buscar el usuario creado por su email
                                var createdUser = await GetByEmailAsync(user.Email);
                                
                                // Si encontramos el usuario, lo devolvemos
                                if (createdUser != null)
                                {
                                    return createdUser;
                                }
                                
                                // Si no lo encontramos, creamos un objeto con los datos que tenemos                            // Creamos un usuario con los datos disponibles
                            var newUser = new Users
                            {
                                Id = Guid.NewGuid(),
                                FirstName = user.FirstName,
                                LastName = user.LastName,
                                Email = user.Email!, // Sabemos que no es null por la validación inicial
                                Password = user.Password ?? string.Empty,
                                IdentificationNumber = user.IdentificationNumber,
                                IdentificationType = user.IdentificationType,
                                Role = user.Role,
                                BirthDate = user.BirthDate,
                                CreatedAt = DateTime.UtcNow
                            };
                            
                            return newUser;
                            }
                            else
                            {
                                // En caso de error, leemos el mensaje de error si existe
                                string errorMessage = "Failed to create user";
                                
                                if (reader.FieldCount > 1 && !reader.IsDBNull(1))
                                {
                                    errorMessage = reader.GetString(1);
                                }
                                
                                throw new Exception(errorMessage);
                            }
                        }
                        else
                        {
                            throw new Exception("No result returned from stored procedure.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error creating user: {ex.Message}", ex);
                }
            }
        }
    
        // Helper class to map the stored procedure result
    // Helper class to map the stored procedure result
    private class SpResult
    {
        public string Result { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
    }public async Task DeleteAsync(Guid userId)
        {
            // Obtener la cadena de conexión del DbContext
            var connectionString = _context.Database.GetConnectionString();
            if (connectionString == null) throw new InvalidOperationException("Database connection string is null");
            
            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand("[dbo].[DeleteUserById]", connection);
                command.CommandType = CommandType.StoredProcedure;
                
                // Añadir parámetro
                command.Parameters.AddWithValue("@UserId", userId);

                try
                {
                    await connection.OpenAsync();
                    
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            var resultColumnIndex = reader.GetOrdinal("Result");
                            string result = reader.GetString(resultColumnIndex);
                            
                            if (result != "SUCCESS")
                            {
                                // En caso de error, leemos el mensaje de error si existe
                                string errorMessage = "Failed to delete user";
                                
                                if (reader.FieldCount > 1 && !reader.IsDBNull(1))
                                {
                                    errorMessage = reader.GetString(1);
                                }
                                
                                throw new Exception(errorMessage);
                            }
                        }
                        else
                        {
                            throw new Exception("No result returned from stored procedure.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error deleting user: {ex.Message}", ex);
                }
            }
        }

        public async Task<List<Users>> GetAllAsync()
        {
            // Using EF Core to get all users
            return await _context.Users.ToListAsync();
        }        public async Task<Users> GetByEmailAsync(string email)
        {
            // Using EF Core to get user by email
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email) ?? 
                  throw new KeyNotFoundException($"User with email {email} not found");
        }

        public async Task<Users> GetByIdAsync(Guid userId)
        {
            // Using EF Core to get user by ID
            return await _context.Users.FindAsync(userId) ?? 
                  throw new KeyNotFoundException($"User with ID {userId} not found");
        }public async Task<Users> UpdateAsync(Users user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (user.Id == Guid.Empty) throw new ArgumentException("User ID must be provided", nameof(user.Id));
            
            // Obtener la cadena de conexión del DbContext
            var connectionString = _context.Database.GetConnectionString();
            if (connectionString == null) throw new InvalidOperationException("Database connection string is null");
            
            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand("[dbo].[UpdateUserById]", connection);
                command.CommandType = CommandType.StoredProcedure;
                
                // Añadir parámetros
                command.Parameters.AddWithValue("@UserId", user.Id);
                command.Parameters.AddWithValue("@FirstName", user.FirstName != null ? user.FirstName : DBNull.Value);
                command.Parameters.AddWithValue("@LastName", user.LastName != null ? user.LastName : DBNull.Value);
                command.Parameters.AddWithValue("@Email", user.Email);
                command.Parameters.AddWithValue("@Password", user.Password != null ? user.Password : DBNull.Value);
                command.Parameters.AddWithValue("@IdentificationNumber", user.IdentificationNumber != null ? user.IdentificationNumber : DBNull.Value);
                command.Parameters.AddWithValue("@IdentificationType", user.IdentificationType != null ? user.IdentificationType : DBNull.Value);
                command.Parameters.AddWithValue("@Role", (int)user.Role);
                command.Parameters.AddWithValue("@BirthDate", user.BirthDate);

                try
                {
                    await connection.OpenAsync();
                    
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            var resultColumnIndex = reader.GetOrdinal("Result");
                            string result = reader.GetString(resultColumnIndex);
                            
                            if (result == "SUCCESS")
                            {
                                // Cerrar el reader antes de ejecutar otra consulta
                                reader.Close();
                                
                                // Buscar el usuario actualizado por su ID
                                var updatedUser = await GetByIdAsync(user.Id);
                                
                                // Si encontramos el usuario, lo devolvemos
                                if (updatedUser != null)
                                {
                                    return updatedUser;
                                }
                                
                                throw new KeyNotFoundException($"User with ID {user.Id} was updated but could not be retrieved.");
                            }
                            else
                            {
                                // En caso de error, leemos el mensaje de error si existe
                                string errorMessage = "Failed to update user";
                                
                                if (reader.FieldCount > 1 && !reader.IsDBNull(1))
                                {
                                    errorMessage = reader.GetString(1);
                                }
                                
                                throw new Exception(errorMessage);
                            }
                        }
                        else
                        {
                            throw new Exception("No result returned from stored procedure.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error updating user: {ex.Message}", ex);
                }
            }
        }

        public async Task<bool> ExistsAsync(Guid userId)
        {
            return await _context.Users.AnyAsync(u => u.Id == userId);
        }
    }
}
