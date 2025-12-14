using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.EntityConfigurations;

public class CorporateCustomerConfiguration : IEntityTypeConfiguration<CorporateCustomer>
{
    public void Configure(EntityTypeBuilder<CorporateCustomer> builder)
    {
        builder.ToTable("CorporateCustomers");

        builder.Property(p => p.Id).HasColumnName("Id").IsRequired();
        //builder.Property(p=>p.CurrentAccountCode).HasColumnName("CurrentAccountCode").IsRequired();
        builder.Property(p => p.CompanyName).HasColumnName("CompanyName");
        builder.Property(p => p.TaxNumber).HasColumnName("TaxNumber");
        builder.Property(p => p.TaxOffice).HasColumnName("TaxOffice");
    }
}
