using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.EntityConfigurations;

public class EmailAuthenticatorConfiguration : IEntityTypeConfiguration<EmailAuthenticator>
{
    public void Configure(EntityTypeBuilder<EmailAuthenticator> builder)
    {
        builder.ToTable("EmailAuthenticators").HasKey(p=>p.Id);

        builder.Property(p => p.Id).HasColumnName("Id").IsRequired();
        builder.Property(p => p.ResetPasswordToken).HasColumnName("ResetPasswordToken").IsRequired(false);
        builder.Property(p => p.ResetPasswordTokenExpiry).HasColumnName("ResetPasswordTokenExpiry").IsRequired(false);
        builder.Property(p => p.VerifyEmailTokenExpiry).HasColumnName("VerifyEmailTokenExpiry").IsRequired(false);
        builder.Property(p => p.UserId).HasColumnName("UserId").IsRequired();
        builder.Property(p => p.ActivationKey).HasColumnName("ActivationKey");
        builder.Property(p => p.IsActive).HasColumnName("IsVerified").IsRequired();
        builder.Property(p => p.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(p => p.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(p => p.DeletedDate).HasColumnName("DeletedDate");

        builder.HasQueryFilter(p => !p.DeletedDate.HasValue);

        builder.HasOne(p => p.User);

        builder.HasBaseType((string)null!);
    }
}
