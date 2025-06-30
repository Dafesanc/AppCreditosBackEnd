using AppCreditosBackEnd.Application.DTOs.Input;
using AppCreditosBackEnd.Application.DTOs.Output;
using AppCreditosBackEnd.Application.Interfaces;
using AppCreditosBackEnd.Domain.Entities;
using AppCreditosBackEnd.Domain.Interfaces;
using AutoMapper;

namespace AppCreditosBackEnd.Application.Services
{
    public class EmployerService : IEmployerService
    {
        private readonly IEmployerRepository _employerRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public EmployerService(
            IEmployerRepository employerRepository,
            IUserRepository userRepository,
            IMapper mapper)
        {
            _employerRepository = employerRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<EmployerDTO>> GetAllEmployersAsync()
        {
            var employers = await _employerRepository.GetAllAsync();
            return employers.Select(MapToEmployerDTO);
        }

        public async Task<EmployerDTO?> GetEmployerByIdAsync(Guid id)
        {
            var employer = await _employerRepository.GetByIdAsync(id);
            return employer != null ? MapToEmployerDTO(employer) : null;
        }

        public async Task<IEnumerable<EmployerDTO>> GetEmployersByUserIdAsync(Guid userId)
        {
            var employers = await _employerRepository.GetByUserIdAsync(userId);
            return employers.Select(MapToEmployerDTO);
        }

        public async Task<EmployerDTO> CreateEmployerAsync(CreateEmployerDTO createEmployerDto, Guid userId)
        {
            // Validar que el usuario existe
            var userExists = await _userRepository.ExistsAsync(userId);
            if (!userExists)
            {
                throw new ArgumentException("El usuario especificado no existe.");
            }

            // Validar que el RUC no existe
            var rucExists = await _employerRepository.RUCExistsAsync(createEmployerDto.RUC);
            if (rucExists)
            {
                throw new ArgumentException("El RUC ya existe.");
            }

            var employer = new Employer
            {
                UserId = userId,
                Name = createEmployerDto.Name,
                RUC = createEmployerDto.RUC,
                Address = createEmployerDto.Address,
                ContactPhone = createEmployerDto.ContactPhone,
                CreatedAt = DateTime.UtcNow
            };

            var createdEmployer = await _employerRepository.CreateAsync(employer);
            return MapToEmployerDTO(createdEmployer);
        }

        public async Task<EmployerDTO> UpdateEmployerAsync(Guid id, UpdateEmployerDTO updateEmployerDto)
        {
            var employer = await _employerRepository.GetByIdAsync(id);
            if (employer == null)
            {
                throw new ArgumentException("El empleador no existe.");
            }

            // Validar RUC único si se está actualizando
            if (!string.IsNullOrEmpty(updateEmployerDto.RUC) && updateEmployerDto.RUC != employer.RUC)
            {
                var rucExists = await _employerRepository.RUCExistsAsync(updateEmployerDto.RUC);
                if (rucExists)
                {
                    throw new ArgumentException("El RUC ya existe.");
                }
            }

            // Actualizar solo los campos proporcionados
            if (!string.IsNullOrEmpty(updateEmployerDto.Name))
            {
                employer.Name = updateEmployerDto.Name;
            }

            if (!string.IsNullOrEmpty(updateEmployerDto.RUC))
            {
                employer.RUC = updateEmployerDto.RUC;
            }

            if (updateEmployerDto.Address != null)
            {
                employer.Address = updateEmployerDto.Address;
            }

            if (updateEmployerDto.ContactPhone != null)
            {
                employer.ContactPhone = updateEmployerDto.ContactPhone;
            }

            var updatedEmployer = await _employerRepository.UpdateAsync(employer);
            return MapToEmployerDTO(updatedEmployer);
        }

        public async Task<bool> DeleteEmployerAsync(Guid id)
        {
            return await _employerRepository.DeleteAsync(id);
        }

        private EmployerDTO MapToEmployerDTO(Employer employer)
        {
            return new EmployerDTO
            {
                Id = employer.Id,
                UserId = employer.UserId,
                Name = employer.Name,
                RUC = employer.RUC,
                Address = employer.Address,
                ContactPhone = employer.ContactPhone,
                CreatedAt = employer.CreatedAt,
                UserFullName = employer.User != null ? $"{employer.User.FirstName} {employer.User.LastName}" : ""
            };
        }
    }
}
