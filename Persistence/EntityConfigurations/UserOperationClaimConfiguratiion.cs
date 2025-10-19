using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.EntityConfigurations;

public class UserOperationClaimConfiguratiion : IEntityTypeConfiguration<UserOperationClaim>
{
    public void Configure(EntityTypeBuilder<UserOperationClaim> builder)
    {
        builder.ToTable("UserOperationClaim").HasKey(p=>p.Id);

        builder.Property(p => p.Id).HasColumnName("Id").IsRequired();
        builder.Property(p => p.UserId).HasColumnName("UserId").IsRequired();
        builder.Property(p => p.OperationClaimId).HasColumnName("OperationClaimId").IsRequired();
        builder.Property(a => a.IsActive).HasColumnName("IsActive");
        builder.Property(p => p.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(p => p.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(p => p.DeletedDate).HasColumnName("DeletedDate");

        builder.HasOne(p=>p.User);
        builder.HasOne(p => p.OperationClaim);
        //builder.HasData(_seeds);
        builder.HasBaseType((Type)null!);
    }
    //private IEnumerable<UserOperationClaim> _seeds
    //{
    //    get
    //    {
    //        yield return new()
    //        {
    //            Id = Guid.NewGuid(),
    //            UserId = UserConfiguration.AdminId,
    //            OperationClaimId = OperationClaimConfiguration.AdminId
    //        };
    //    }
   // }
}
