using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using Microsoft.Build.Framework;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static ASI.Basecode.Resources.Constants.Enums;

namespace ASI.Basecode.WebApp
{
    public class BackgroundWorkerService : BackgroundService

    {
        private readonly ILogger<BackgroundWorkerService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private Timer? timer;

        public BackgroundWorkerService(ILogger<BackgroundWorkerService> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Booking Status Updater Service is starting.");
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Updating booking statuses...");

                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();

                        List<BookingViewModel> bookings = new List<BookingViewModel>();
                        try
                        {
                            bookings = bookingService.GetBookings()
                                .Where(b => b.BookingStatus.Equals(BookingStatus.PENDING.ToString()) ||
                                            b.BookingStatus.Equals(BookingStatus.APPROVED.ToString()))
                                .ToList();
                        }
                        catch (Exception e)
                        {
                            _logger.LogError(e, "An error occurred while retrieving bookings.");
                            continue;
                        }

                        foreach (var booking in bookings)
                        {
                            try
                            {
                                DateTime? bookingEndDateTime = new DateTime();
                                if (booking.EndDate.HasValue && booking.TimeTo.HasValue)
                                {
                                    bookingEndDateTime = booking.EndDate.Value + booking.TimeTo.Value;
                                }

                                Console.WriteLine(bookingEndDateTime);

                                switch (booking.BookingStatus)
                                {
                                    case nameof(BookingStatus.PENDING):
                                        if (bookingEndDateTime < DateTime.Now)
                                        {
                                            booking.BookingStatus = BookingStatus.CANCELLED.ToString();
                                            booking.BookingChangeOnly = true;
                                            bookingService.UpdateBooking(booking);
                                        }
                                        break;
                                    case nameof(BookingStatus.APPROVED):
                                        if (bookingEndDateTime < DateTime.Now)
                                        {
                                            booking.BookingStatus = BookingStatus.COMPLETED.ToString();
                                            booking.BookingChangeOnly = true;
                                            bookingService.UpdateBooking(booking);
                                        }
                                        break;
                                    default:
                                        break;
                                }
                            }
                            catch (Exception e)
                            {
                                _logger.LogError(e, $"An error occurred while updating booking status for booking ID {booking.BookingId}.");
                                continue;
                            }
                        }
                    }
                } 
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while resolving services from the scope.");
                }
                
                try
                {
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                }
                catch (TaskCanceledException ex)
                {
                    _logger.LogInformation(ex, "Task was canceled.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred during the delay.");
                }
            }
            _logger.LogInformation("Booking Status Updater Service is stopping.");
        }
    }
}