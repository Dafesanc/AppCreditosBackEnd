using AutoMapper;
using AppCreditosBackEnd.Application.DTOs;
using AppCreditosBackEnd.Domain.Entities;

namespace AppCreditosBackEnd.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Mapeo de CreditApplication a CreditApplicationResponseDto
            CreateMap<CreditApplication, CreditApplicationResponseDto>()
                .ForMember(dest => dest.ApplicantName, 
                    opt => opt.MapFrom(src => src.User != null 
                        ? $"{src.User.FirstName} {src.User.LastName}" 
                        : "Unknown"));
            
            // Otros mapeos que puedas necesitar
            // Por ejemplo:
            // CreateMap<Users, UserDto>();
        }
    }
}
