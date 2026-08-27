
using EventSeatBooking.Application.Interfaces;
using EventSeatBooking.Application.UseCases;
using EventSeatBooking.Domain.Interfaces;
using EventSeatBooking.Domain.Services;
using EventSeatBooking.Infrastructure.Persistence;
using EventSeatBooking.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EventSeatBooking.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<EventSeatBookingDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));



            builder.Services.AddScoped<IBookingRepository, BookingRepository>();
            builder.Services.AddScoped<IScreeningRepository, ScreeningRepository>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<SeatAvailabilityService>();
            builder.Services.AddScoped<ReserveSeatUseCase>();
            builder.Services.AddScoped<ConfirmBookingUseCase>();
            builder.Services.AddScoped<CancelBookingUseCase>();
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
