using Core.Entities;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.EntityConfigurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshToken").HasKey(p=>p.Id);
        builder.Property(rt => rt.Id).HasColumnName("Id").IsRequired();
        builder.Property(rt => rt.UserId).HasColumnName("UserId").IsRequired();
        builder.Property(rt => rt.Token).HasColumnName("Token").IsRequired();
        builder.Property(rt => rt.DeviceId).HasColumnName("DeviceId").IsRequired();
        builder.Property(rt => rt.DevicePlatform).HasColumnName("DevicePlatform").IsRequired();
        builder.Property(rt => rt.DeviceName).HasColumnName("DeviceName").IsRequired(false);
        builder.Property(rt => rt.UserAgent).HasColumnName("UserAgent").IsRequired();
        builder.Property(rt => rt.ExpiresDate).HasColumnName("ExpiresDate").IsRequired();
        builder.Property(rt => rt.CreatedByIp).HasColumnName("CreatedByIp").IsRequired();
        builder.Property(rt => rt.RevokedDate).HasColumnName("RevokedDate").IsRequired(false);
        builder.Property(rt => rt.RevokedByIp).HasColumnName("RevokedByIp").IsRequired(false);
        builder.Property(rt => rt.ReplacedByToken).HasColumnName("ReplacedByToken").IsRequired(false);
        builder.Property(rt => rt.ReasonRevoked).HasColumnName("ReasonRevoked").IsRequired(false);
        builder.Property(rt => rt.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(rt => rt.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(rt => rt.DeletedDate).HasColumnName("DeletedDate");

        builder.HasQueryFilter(rt => !rt.DeletedDate.HasValue);

        builder.HasOne(x => x.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
