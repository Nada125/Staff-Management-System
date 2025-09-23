using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StaffManagementSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaffManagementSystem.Infrastructures.Configurations
{
    public class EmployeeConfig : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.Property(p => p.UserName)
                   .IsRequired()
                   .HasMaxLength(20);

            builder.Property(p => p.Position)
                   .IsRequired()
                   .HasMaxLength(20);

            builder.Property(p => p.Salary)
                   .HasColumnType("decimal(18,2)");

        }
    }
}
