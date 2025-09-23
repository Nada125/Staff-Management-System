using AutoMapper;
using StaffManagementSystem.Application.DTOs.Employee;
using StaffManagementSystem.Domain.Entities;


namespace StaffManagementSystem.Application.Mappings
{
    public class EmployeeProfile : Profile
    {
        public EmployeeProfile() 
        {
            CreateMap<Employee, EmployeeResponse>();
            CreateMap<EmployeeRequest, Employee>();
        }
    }
}
