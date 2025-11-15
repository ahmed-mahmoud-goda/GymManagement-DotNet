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
    public class BookingConfigurations : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {
            builder.Ignore(x => x.Id);

            builder.Property(x => x.CreatedAt)
                .HasColumnName("BookingDate")
                .HasDefaultValueSql("GETDATE()");

            builder.HasOne(x => x.Session)
                .WithMany(x => x.SessionMembers)
                .HasForeignKey(x => x.SessionId);

            builder.HasOne(x => x.Member)
                .WithMany(x => x.MemberSessions)
                .HasForeignKey(x => x.MemberId);

            builder.HasKey(x => new { x.SessionId, x.MemberId, });
        }
    }
}
