using EventSeatBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventSeatBooking.Infrastructure.Persistence.Configurations
{
    public class BookingConfiguration : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {
            builder.HasKey(b => b.Id);

            builder.Property(b => b.CustomerId).IsRequired();
            builder.Property(b => b.ScreeningId).IsRequired();
            builder.Property(b => b.Status).HasConversion<string>();


            // Booking owns its BookedSeats — this is the Aggregate boundary expressed in EF Core.
            // OwnsMany means BookedSeat has no independent identity outside a Booking.
            builder.OwnsMany(b => b.Seats, seat =>
            {
                seat.WithOwner().HasForeignKey("BookingId");
                seat.Property<int>("Id");
                seat.HasKey("Id");

                // SeatNumber is a Value Object glued onto BookedSeat, not its own table
                seat.OwnsOne(s => s.SeatNumber, sn =>
                {
                    sn.Property(x => x.Row).HasColumnName("SeatRow");
                    sn.Property(x => x.Number).HasColumnName("SeatColumnNumber");
                });
            });

            // Domain events are never persisted — they're transient, published then cleared.
            builder.Ignore(b => b.DomainEvents);
        }
    }
}