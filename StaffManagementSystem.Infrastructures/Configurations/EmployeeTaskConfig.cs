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
    public class EmployeeTaskConfig : IEntityTypeConfiguration<EmployeeTask>
    {
        public void Configure(EntityTypeBuilder<EmployeeTask> builder)
        {
            builder.Property(p => p.Title)
                   .IsRequired()
                   .HasMaxLength(20);

            builder.Property(p => p.Description)
                   .IsRequired()
                   .HasMaxLength(50);


            builder.HasOne(p => p.Employee)
                   .WithMany(c => c.Tasks)
                   .HasForeignKey(p => p.EmployeeId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
