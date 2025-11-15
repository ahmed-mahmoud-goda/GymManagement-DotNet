using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymManagementDAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManagementDAL.Data.Configurations
{
    public class GymUserConfigurations<T> : IEntityTypeConfiguration<T> where T : GymUser
    {
        public void Configure(EntityTypeBuilder<T> builder)
        {
            builder.Property(x => x.Name)
                .HasColumnType("varchar")
                .HasMaxLength(50);

            builder.Property(x => x.Email)
                .HasColumnType("varchar")
                .HasMaxLength(100);

            builder.Property(x => x.Phone)
                .HasColumnType("varchar")
                .HasMaxLength(11);

            builder.OwnsOne(x => x.Address, address =>
            {
                address.Property(a => a.BuildingNumber)
                .HasColumnName("BuildingNumber");

                address.Property(a => a.City)
                .HasColumnType("varchar")
                .HasColumnName("City")
                .HasMaxLength(30);

                address.Property(a => a.Street)
                .HasColumnType("varchar")
                .HasColumnName("Street")
                .HasMaxLength(30);
            });

            builder.HasIndex(x => x.Email).IsUnique();

            builder.HasIndex(x => x.Phone).IsUnique();
            builder.ToTable(x =>
            {
                x.HasCheckConstraint("GymUser_EmailCheck", "Email LIKE '_%@_%._%'");
                x.HasCheckConstraint("GymUser_PhoneCheck", "Phone LIKE '01%' and Phone Not LIKE '%[^0-9]%'");
            });
        }
    }
}
