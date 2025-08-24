using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.EntityConfigurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .IsRequired()
            .HasColumnType("uniqueidentifier");

        builder.Property(c => c.ChargeName)
            .IsRequired()
            .HasColumnType("nvarchar(max)");

        builder.Property(c => c.Debit)
            .IsRequired()
            .HasColumnType("int");

        builder.Property(c => c.CreatedDate)
            .IsRequired()
            .HasColumnType("datetime2(7)");

        builder.Property(c => c.UpdatedDate)
            .HasColumnType("datetime2(7)");

        builder.Property(c => c.DeletedDate)
            .HasColumnType("datetime2(7)");

        builder.Property(c => c.Name)
            .IsRequired()
            .HasColumnType("nvarchar(max)");

        builder.Property(c => c.PhoneNumber)
            .IsRequired()
            .HasColumnType("nvarchar(max)");

        builder.Property(c => c.IdentityNumber)
            .IsRequired()
            .HasColumnType("nvarchar(max)");

        builder.Property(c => c.Email)
            .IsRequired()
            .HasColumnType("nvarchar(max)");

        builder.Property(c => c.PasswordHash)
            .IsRequired()
            .HasColumnType("varbinary(max)");

        builder.Property(c => c.PasswordSalt)
            .IsRequired()
            .HasColumnType("varbinary(max)");

        builder.Property(c => c.IsActive)
            .HasColumnType("bit")
            .HasColumnName("isActive");

    }
}
