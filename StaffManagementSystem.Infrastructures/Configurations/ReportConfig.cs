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
    public class ReportConfig : IEntityTypeConfiguration<Report>
    {
        public void Configure(EntityTypeBuilder<Report> builder)
        {
            builder.Property(p => p.Content)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(p => p.Content)
                  .IsRequired()
                  .HasColumnType("text");

            builder.HasOne(p => p.Employee)
                   .WithMany(c => c.Reports)
                   .HasForeignKey(p => p.EmployeeId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
