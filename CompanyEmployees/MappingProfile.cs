using AutoMapper;
using CompanyEmployees.Core.Domain.Entities;
using Shared.DataTransferObjects;

namespace CompanyEmployees
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Company mappings
            CreateMap<Company, CompanyDto>()
                .ForMember(c => c.FullAddress,
                    opt => opt.MapFrom(x => $"{x.Address} {x.Country}"));

            CreateMap<CompanyForCreationDto, Company>();
            CreateMap<CompanyForUpdateDto, Company>();

            // Attendance mappings
            CreateMap<AttendanceRecord, AttendanceRecordDto>()
                .ForMember(dest => dest.EmployeeName, 
                    opt => opt.MapFrom(src => src.Employee != null ? src.Employee.Name : string.Empty))
                .ForMember(dest => dest.Status, 
                    opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<AttendanceForCreationDto, AttendanceRecord>();
            CreateMap<AttendanceForUpdateDto, AttendanceRecord>();
        }
    }
}
