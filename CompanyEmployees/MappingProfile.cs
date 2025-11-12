using AutoMapper;
using CompanyEmployees.Core.Domain.Entities;
using Shared.DataTransferObjects;

namespace CompanyEmployees
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Company, CompanyDto>()
                .ForMember(c => c.FullAddress,
                    opt => opt.MapFrom(x => $"{x.Address} {x.Country}"));

            CreateMap<CompanyForCreationDto, Company>();
            CreateMap<CompanyForUpdateDto, Company>();

            // Attendance mappings
            CreateMap<Attendance, AttendanceDto>();
            // BUG: Missing reverse mapping for AttendanceDto to Attendance
            CreateMap<AttendanceForCreationDto, Attendance>();
            // BUG: Not mapping CheckOutTime which will cause it to be default DateTime value
        }
    }
}
