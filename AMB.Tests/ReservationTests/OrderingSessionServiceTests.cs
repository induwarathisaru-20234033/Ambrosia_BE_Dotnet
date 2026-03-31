using AMB.Application.Dtos;
using AMB.Application.Services;
using AMB.Domain.Entities;
using AMB.Domain.Enums;
using AMB.Tests.Mocks;
using Microsoft.Extensions.Configuration;

namespace AMB.Tests.ReservationTests
{
    public class OrderingSessionServiceTests
    {
        [Fact]
        public async Task ConfirmSessionAsync_WithValidArrivedReservationImplicit_ReturnsToken()
        {
            // Arrange
            var reservationRepository = new TestReservationRepository();
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Authentication:GuestSecret"] = "test-guest-secret-1234567890-1234567890-1234567890-1234",
                    ["Authentication:GuestIssuer"] = "https://guest.example.com",
                    ["Authentication:GuestAudience"] = "guest-audience"
                })
                .Build();

            var service = new OrderingSessionService(reservationRepository, configuration);

            var tableGuid = Guid.NewGuid();
            reservationRepository.Reservations[1] = new Reservation
            {
                Id = 1,
                ReservationCode = "RES-123",
                ReservationStatus = (int)ReservationStatus.Arrived,
                ArrivedAt = DateTimeOffset.UtcNow.AddMinutes(-30),
                Status = (int)EntityStatus.Active,
                TableId = 1,
                Table = new Table
                {
                    Id = 1,
                    TableName = "T-1",
                    Capacity = 4,
                    QrIdentifier = tableGuid
                }
            };

            // Act
            var response = await service.ConfirmSessionAsync(new ConfirmSessionRequestDto
            {
                TableCode = tableGuid,
                ConfirmationType = "Implicit"
            });

            // Assert
            Assert.NotNull(response);
            Assert.False(string.IsNullOrWhiteSpace(response!.Token));
        }

        [Fact]
        public async Task ConfirmSessionAsync_WithMissingReservation_ReturnsNull()
        {
            // Arrange
            var reservationRepository = new TestReservationRepository();
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Authentication:GuestSecret"] = "test-guest-secret-1234567890-1234567890-1234567890-1234",
                    ["Authentication:GuestIssuer"] = "https://guest.example.com",
                    ["Authentication:GuestAudience"] = "guest-audience"
                })
                .Build();

            var service = new OrderingSessionService(reservationRepository, configuration);

            // Act
            var response = await service.ConfirmSessionAsync(new ConfirmSessionRequestDto
            {
                TableCode = Guid.NewGuid(),
                ConfirmationType = "Implicit"
            });

            // Assert
            Assert.Null(response);
        }

        [Fact]
        public async Task ConfirmSessionAsync_WithManualConfirmationWrongCode_ReturnsNull()
        {
            // Arrange
            var reservationRepository = new TestReservationRepository();
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Authentication:GuestSecret"] = "test-guest-secret-1234567890-1234567890-1234567890-1234",
                    ["Authentication:GuestIssuer"] = "https://guest.example.com",
                    ["Authentication:GuestAudience"] = "guest-audience"
                })
                .Build();

            var service = new OrderingSessionService(reservationRepository, configuration);

            var tableGuid = Guid.NewGuid();
            reservationRepository.Reservations[1] = new Reservation
            {
                Id = 1,
                ReservationCode = "RES-123",
                ReservationStatus = (int)ReservationStatus.Arrived,
                ArrivedAt = DateTimeOffset.UtcNow.AddMinutes(-30),
                Status = (int)EntityStatus.Active,
                TableId = 1,
                Table = new Table
                {
                    Id = 1,
                    TableName = "T-1",
                    Capacity = 4,
                    QrIdentifier = tableGuid
                }
            };

            // Act
            var response = await service.ConfirmSessionAsync(new ConfirmSessionRequestDto
            {
                TableCode = tableGuid,
                ConfirmationType = "Manual",
                ReservationCode = "WRONG-CODE"
            });

            // Assert
            Assert.Null(response);
        }
    }
}
