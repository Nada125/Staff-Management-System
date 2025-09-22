using AutoMapper;
using StaffManagementSystem.Application.DTOs.Report;
using StaffManagementSystem.Domain.Entities;


namespace StaffManagementSystem.Application.Mappings
{
    public class ReportProfile : Profile
    {
        public ReportProfile() 
        {
            CreateMap<Report, ReportResponse>();
            CreateMap<ReportRequest, Report>();
        }
    }
}
