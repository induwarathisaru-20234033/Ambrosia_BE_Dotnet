using AMB.Application.Dtos;
using AMB.Application.Services;
using AMB.Domain.Entities;
using AMB.Domain.Enums;
using AMB.Tests.Mocks;

namespace AMB.Tests.ReservationTests
{
    public class ReservationServiceCreateTests
    {
        [Fact]
        public async Task CreateReservationAsync_WithAvailableSlotAndTable_CreatesReservationSuccessfully()
        {
            // Arrange
            var reservationRepository = new TestReservationRepository();
            var configRepository = new TestConfigRepository();

            // Setup booking slot
            var bookingSlot = new BookingSlot
            {
                Id = 1,
                SlotId = Guid.NewGuid(),
                StartTime = new TimeOnly(18, 0),
                EndTime = new TimeOnly(20, 0),
                Day = 6,
                Status = (int)EntityStatus.Active
            };
            configRepository.BookingSlots[1] = bookingSlot;

            var service = new ReservationService(reservationRepository, configRepository, new TestEmployeeRepository(), new TestEmailService());

            var request = new CreateReservationRequestDto
            {
                PartySize = 4,
                ReservationDate = new DateTimeOffset(2026, 3, 15, 18, 0, 0, TimeSpan.Zero),
                Occasion = "Birthday",
                SpecialRequests = "Window seat preferred",
                CustomerName = "John Doe",
                CustomerEmail = "john.doe@example.com",
                CustomerPhoneNumber = "0771234567",
                BookingSlotId = 1,
                TableId = 5
            };

            // Act
            var result = await service.CreateReservationAsync(request);

            // Assert
            Assert.NotNull(reservationRepository.LastAddedReservation);
            var addedReservation = reservationRepository.LastAddedReservation!;

            // Check reservation properties
            Assert.NotEmpty(addedReservation.ReservationCode);
            Assert.StartsWith("RES-", addedReservation.ReservationCode);
            Assert.Equal(4, addedReservation.PartySize);
            Assert.Equal((int)ReservationStatus.Booked, addedReservation.ReservationStatus);
            Assert.Equal(request.ReservationDate, addedReservation.ReservationDate);
            Assert.Equal("Birthday", addedReservation.Occasion);
            Assert.Equal("Window seat preferred", addedReservation.SpecialRequests);
            Assert.Equal(1, addedReservation.BookingSlotId);
            Assert.Equal(5, addedReservation.TableId);
            Assert.Equal((int)EntityStatus.Active, addedReservation.Status);

            // Check customer detail was created
            Assert.NotNull(addedReservation.CustomerDetail);
            Assert.Equal("John Doe", addedReservation.CustomerDetail.Name);
            Assert.Equal("john.doe@example.com", addedReservation.CustomerDetail.Email);
            Assert.Equal("0771234567", addedReservation.CustomerDetail.PhoneNumber);
            Assert.Equal((int)EntityStatus.Active, addedReservation.CustomerDetail.Status);

            // Check returned DTO
            Assert.NotNull(result);
            Assert.Equal(addedReservation.Id, result.Id);
            Assert.Equal(addedReservation.ReservationCode, result.ReservationCode);
            Assert.Equal(4, result.PartySize);
            Assert.Equal((int)ReservationStatus.Booked, result.ReservationStatus);
            Assert.Null(result.ArrivedAt);
            Assert.Null(result.NoShowMarkedAt);
            Assert.Null(result.CancelledAt);
        }

        [Fact]
        public async Task CreateReservationAsync_WithUnavailableSlot_ThrowsInvalidOperationException()
        {
            // Arrange
            var reservationRepository = new TestReservationRepository();
            var configRepository = new TestConfigRepository();

            // Setup booking slot
            var bookingSlot = new BookingSlot
            {
                Id = 1,
                SlotId = Guid.NewGuid(),
                StartTime = new TimeOnly(18, 0),
                EndTime = new TimeOnly(20, 0),
                Day = 6,
                Status = (int)EntityStatus.Active
            };
            configRepository.BookingSlots[1] = bookingSlot;

            // Add existing reservation for the same slot
            var existingReservation = new Reservation
            {
                Id = 1,
                ReservationCode = "RES-20260315-ABCD",
                PartySize = 2,
                ReservationStatus = (int)ReservationStatus.Booked,
                ReservationDate = new DateTimeOffset(2026, 3, 15, 18, 0, 0, TimeSpan.Zero),
                BookingSlotId = 1,
                TableId = 3,
                Status = (int)EntityStatus.Active,
                CustomerDetail = new CustomerDetail
                {
                    Name = "Existing Customer",
                    Email = "existing@example.com",
                    PhoneNumber = "0771111111",
                    Status = (int)EntityStatus.Active
                },
                BookingSlot = bookingSlot
            };
            reservationRepository.Reservations[1] = existingReservation;

            var service = new ReservationService(reservationRepository, configRepository, new TestEmployeeRepository(), new TestEmailService());

            var request = new CreateReservationRequestDto
            {
                PartySize = 4,
                ReservationDate = new DateTimeOffset(2026, 3, 15, 18, 0, 0, TimeSpan.Zero),
                CustomerName = "John Doe",
                CustomerEmail = "john.doe@example.com",
                CustomerPhoneNumber = "0771234567",
                BookingSlotId = 1,
                TableId = 5
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.CreateReservationAsync(request));

            Assert.Contains("not available", exception.Message);
            Assert.Contains("existing reservations", exception.Message);
        }

        [Fact]
        public async Task CreateReservationAsync_WithUnavailableTable_ThrowsInvalidOperationException()
        {
            // Arrange
            var reservationRepository = new TestReservationRepository();
            var configRepository = new TestConfigRepository();

            // Setup booking slot
            var bookingSlot = new BookingSlot
            {
                Id = 1,
                SlotId = Guid.NewGuid(),
                StartTime = new TimeOnly(18, 0),
                EndTime = new TimeOnly(20, 0),
                Day = 6,
                Status = (int)EntityStatus.Active
            };
            configRepository.BookingSlots[1] = bookingSlot;

            // Add existing reservation for the same table and time slot
            var existingReservation = new Reservation
            {
                Id = 1,
                ReservationCode = "RES-20260315-ABCD",
                PartySize = 2,
                ReservationStatus = (int)ReservationStatus.Booked,
                ReservationDate = new DateTimeOffset(2026, 3, 15, 18, 0, 0, TimeSpan.Zero),
                BookingSlotId = 2, // Different slot
                TableId = 5, // Same table
                Status = (int)EntityStatus.Active,
                CustomerDetail = new CustomerDetail
                {
                    Name = "Existing Customer",
                    Email = "existing@example.com",
                    PhoneNumber = "0771111111",
                    Status = (int)EntityStatus.Active
                },
                BookingSlot = new BookingSlot
                {
                    Id = 2,
                    SlotId = Guid.NewGuid(),
                    StartTime = new TimeOnly(18, 0),
                    EndTime = new TimeOnly(20, 0),
                    Day = 6,
                    Status = (int)EntityStatus.Active
                }
            };
            reservationRepository.Reservations[1] = existingReservation;

            var service = new ReservationService(reservationRepository, configRepository, new TestEmployeeRepository(), new TestEmailService());

            var request = new CreateReservationRequestDto
            {
                PartySize = 4,
                ReservationDate = new DateTimeOffset(2026, 3, 15, 18, 0, 0, TimeSpan.Zero),
                CustomerName = "John Doe",
                CustomerEmail = "john.doe@example.com",
                CustomerPhoneNumber = "0771234567",
                BookingSlotId = 1,
                TableId = 5 // Same table as existing reservation
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.CreateReservationAsync(request));

            Assert.Contains("not available", exception.Message);
        }

        [Fact]
        public async Task CreateReservationAsync_SetsCorrectInitialStatus()
        {
            // Arrange
            var reservationRepository = new TestReservationRepository();
            var configRepository = new TestConfigRepository();

            var bookingSlot = new BookingSlot
            {
                Id = 1,
                SlotId = Guid.NewGuid(),
                StartTime = new TimeOnly(18, 0),
                EndTime = new TimeOnly(20, 0),
                Day = 6,
                Status = (int)EntityStatus.Active
            };
            configRepository.BookingSlots[1] = bookingSlot;

            var service = new ReservationService(reservationRepository, configRepository, new TestEmployeeRepository(), new TestEmailService());

            var request = new CreateReservationRequestDto
            {
                PartySize = 2,
                ReservationDate = new DateTimeOffset(2026, 3, 15, 18, 0, 0, TimeSpan.Zero),
                CustomerName = "Jane Smith",
                CustomerEmail = "jane.smith@example.com",
                CustomerPhoneNumber = "0779876543",
                BookingSlotId = 1,
                TableId = 3
            };

            // Act
            var result = await service.CreateReservationAsync(request);

            // Assert
            Assert.Equal((int)ReservationStatus.Booked, result.ReservationStatus);
            Assert.Null(result.ArrivedAt);
            Assert.Null(result.NoShowMarkedAt);
            Assert.Null(result.CancelledAt);
        }
    }
}

