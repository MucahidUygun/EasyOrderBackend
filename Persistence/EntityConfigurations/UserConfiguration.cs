using Core.Entities;
using Core.Security.Hashing;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.EntityConfigurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.Property(a => a.Id).HasColumnName("Id").IsRequired();
        builder.Property(a => a.PhoneNumber).HasColumnName("PhoneNumber");
        builder.Property(a => a.Email).HasColumnName("Email");
        builder.Property(a => a.Adress).HasColumnName("Adress");
        builder.Property(a => a.PasswordHash).HasColumnName("PasswordHash");
        builder.Property(a => a.PasswordSalt).HasColumnName("PasswordSalt");
        builder.Property(a => a.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(a => a.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(a => a.DeletedDate).HasColumnName("DeletedDate");


        builder.HasQueryFilter(u => !u.DeletedDate.HasValue);
        //builder.HasData(_seeds);
        builder.HasBaseType((Type)null!);
    }
    //public static Guid AdminId { get; } = Guid.NewGuid();
    //private IEnumerable<User> _seeds
    //{
    //    get
    //    {
    //        HashingHelper.CreatePasswordHash(
    //            password: "Passw0rd!",
    //            passwordHash: out byte[] passwordHash,
    //            passwordSalt: out byte[] passwordSalt
    //        );
    //        User adminUser =
    //            new()
    //            {
    //                Id = AdminId,
    //                Email = "uygun@uygun.yg",
    //                Adress = "Admin",
    //                PhoneNumber ="555 555 55 22",
    //                IsActive = true,
    //                PasswordHash = passwordHash,
    //                PasswordSalt = passwordSalt,
    //                CreatedDate = DateTime.Now
    //            };
    //        yield return adminUser;
    //    }
    //}
}
