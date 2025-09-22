using AutoMapper;
using StaffManagementSystem.Application.DTOs.EmployeeTasks;
using StaffManagementSystem.Domain.Entities;


namespace StaffManagementSystem.Application.Mappings
{
    public class EmployeeTaskProfile : Profile
    {
        public EmployeeTaskProfile() 
        {
            CreateMap<EmployeeTask, EmployeeTaskResponse>();
            CreateMap<EmployeeTaskRequest, EmployeeTask>();
        }
    }
}
