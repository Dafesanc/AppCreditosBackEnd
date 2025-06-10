using AutoMapper;
using AppCreditosBackEnd.Application.DTOs;
using AppCreditosBackEnd.Domain.Entities;
using AppCreditosBackEnd.Domain.Enums;

namespace AppCreditosBackEnd.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {            // Mapeo de CreditApplication a CreditApplicationResponseDto
            CreateMap<CreditApplication, CreditApplicationResponseDto>()
                .ForMember(dest => dest.ApplicantName, 
                    opt => opt.MapFrom(src => src.User != null 
                        ? $"{src.User.FirstName} {src.User.LastName}" 
                        : "Unknown"));
            
            // Mapeos para la entidad Users
            CreateMap<Users, UserDTO>();
            CreateMap<RegisterRequestDto, Users>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
            CreateMap<RegisterClientRequestDto, Users>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => (UserRole)src.Role))
                .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => src.BirthDate))
                .ForMember(dest => dest.IdentificationType, opt => opt.MapFrom(src => src.IdentificationType))
                .ForMember(dest => dest.IdentificationNumber, opt => opt.MapFrom(src => src.IdentificationNumber));

        }
    }
}
