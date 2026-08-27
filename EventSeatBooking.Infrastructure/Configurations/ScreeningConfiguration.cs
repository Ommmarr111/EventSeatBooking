using EventSeatBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventSeatBooking.Infrastructure.Persistence.Configurations
{
    public class ScreeningConfiguration : IEntityTypeConfiguration<Screening>
    {
        public void Configure(EntityTypeBuilder<Screening> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.Title).IsRequired().HasMaxLength(200);
            builder.Property(s => s.ShowTime).IsRequired();

            builder.OwnsMany(s => s.Seats, seat =>
            {
                seat.WithOwner().HasForeignKey("ScreeningId");
                seat.Property<int>("Id");
                seat.HasKey("Id");

                seat.Property(x => x.Status).HasConversion<string>();

                seat.OwnsOne(x => x.SeatNumber, sn =>
                {
                    sn.Property(x => x.Row).HasColumnName("SeatRow");
                    sn.Property(x => x.Number).HasColumnName("SeatColumnNumber");
                });
            });
        }
    }
}