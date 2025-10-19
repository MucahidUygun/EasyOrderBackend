using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.EntityConfigurations;

public class OperationClaimConfiguration : IEntityTypeConfiguration<OperationClaim>
{
    public void Configure(EntityTypeBuilder<OperationClaim> builder)
    {
        builder.ToTable("OperationClaim");

        builder.Property(p => p.Id).HasColumnName("Id").IsRequired();
        builder.Property(p => p.Name).HasColumnName("Name");
        builder.Property(a => a.IsActive).HasColumnName("IsActive");
        builder.Property(a => a.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(a => a.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(a => a.DeletedDate).HasColumnName("DeletedDate");

        builder.HasQueryFilter(a => !a.DeletedDate.HasValue);

        //builder.HasData(_seeds);

        builder.HasBaseType((Type)null!);
    }
//    public static int AdminId => 1;
//    private IEnumerable<OperationClaim> _seeds
//    {
//        get
//        {
//            yield return new() { Id = AdminId, Name = "Admin",CreatedDate=DateTime.Now };

//            //IEnumerable<OperationClaim> featureOperationClaims = getFeatureOperationClaims(AdminId);
//            //foreach (OperationClaim claim in featureOperationClaims)
//            //    yield return claim;
//        }
//    }
}
