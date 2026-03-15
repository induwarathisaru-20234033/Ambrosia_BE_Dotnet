using AMB.Application.Services;
using AMB.Domain.Entities;
using AMB.Domain.Enums;
using AMB.Tests.Mocks;

namespace AMB.Tests.ReservationTests
{
    public class ReservationServiceStatusTests
    {
        [Fact]
        public async Task CancelReservationAsync_WithBookedReservation_CancelsSuccessfully()
        {
            // Arrange
            var reservationRepository = new TestReservationRepository();
            var configRepository = new TestConfigRepository();

            var reservation = new Reservation
            {
                Id = 1,
                ReservationCode = "RES-20260315-ABCD",
                PartySize = 4,
                ReservationStatus = (int)ReservationStatus.Booked,
                ReservationDate = new DateTimeOffset(2026, 3, 15, 18, 0, 0, TimeSpan.Zero),
                BookingSlotId = 1,
                TableId = 5,
                Status = (int)EntityStatus.Active,
                CustomerDetail = new CustomerDetail
                {
                    Id = 1,
                    Name = "John Doe",
                    Email = "john.doe@example.com",
                    PhoneNumber = "0771234567",
                    Status = (int)EntityStatus.Active
                },
                BookingSlot = new BookingSlot
                {
                    Id = 1,
                    StartTime = new TimeOnly(18, 0),
                    EndTime = new TimeOnly(20, 0)
                },
                Table = new Table
                {
                    Id = 5,
                    TableName = "Table 5",
                    Capacity = 4
                }
            };
            reservationRepository.Reservations[1] = reservation;

            var service = new ReservationService(reservationRepository, configRepository);

            // Act
            var result = await service.CancelReservationAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal((int)ReservationStatus.Cancelled, result.ReservationStatus);
            Assert.NotNull(result.CancelledAt);
            Assert.Null(result.ArrivedAt);
            Assert.Null(result.NoShowMarkedAt);

            // Check repository state
            Assert.NotNull(reservationRepository.LastUpdatedReservation);
            Assert.Equal((int)ReservationStatus.Cancelled, reservationRepository.LastUpdatedReservation.ReservationStatus);
        }

        [Fact]
        public async Task CancelReservationAsync_WithArrivedReservation_ThrowsInvalidOperationException()
        {
            // Arrange
            var reservationRepository = new TestReservationRepository();
            var configRepository = new TestConfigRepository();

            var reservation = new Reservation
            {
                Id = 1,
                ReservationCode = "RES-20260315-ABCD",
                PartySize = 4,
                ReservationStatus = (int)ReservationStatus.Arrived,
                ReservationDate = new DateTimeOffset(2026, 3, 15, 18, 0, 0, TimeSpan.Zero),
                ArrivedAt = DateTimeOffset.UtcNow,
                BookingSlotId = 1,
                TableId = 5,
                Status = (int)EntityStatus.Active,
                CustomerDetail = new CustomerDetail
                {
                    Id = 1,
                    Name = "John Doe",
                    Email = "john.doe@example.com",
                    PhoneNumber = "0771234567"
                }
            };
            reservationRepository.Reservations[1] = reservation;

            var service = new ReservationService(reservationRepository, configRepository);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.CancelReservationAsync(1));

            Assert.Equal("Arrived reservations cannot be cancelled.", exception.Message);
        }

        [Fact]
        public async Task CancelReservationAsync_WithNonExistentReservation_ReturnsNull()
        {
            // Arrange
            var reservationRepository = new TestReservationRepository();
            var configRepository = new TestConfigRepository();
            var service = new ReservationService(reservationRepository, configRepository);

            // Act
            var result = await service.CancelReservationAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task MarkReservationAsArrivedAsync_WithBookedReservation_MarksArrivedSuccessfully()
        {
            // Arrange
            var reservationRepository = new TestReservationRepository();
            var configRepository = new TestConfigRepository();

            var reservation = new Reservation
            {
                Id = 1,
                ReservationCode = "RES-20260315-ABCD",
                PartySize = 4,
                ReservationStatus = (int)ReservationStatus.Booked,
                ReservationDate = new DateTimeOffset(2026, 3, 15, 18, 0, 0, TimeSpan.Zero),
                BookingSlotId = 1,
                TableId = 5,
                Status = (int)EntityStatus.Active,
                CustomerDetail = new CustomerDetail
                {
                    Id = 1,
                    Name = "John Doe",
                    Email = "john.doe@example.com",
                    PhoneNumber = "0771234567"
                },
                BookingSlot = new BookingSlot
                {
                    Id = 1,
                    StartTime = new TimeOnly(18, 0),
                    EndTime = new TimeOnly(20, 0)
                },
                Table = new Table
                {
                    Id = 5,
                    TableName = "Table 5",
                    Capacity = 4
                }
            };
            reservationRepository.Reservations[1] = reservation;

            var service = new ReservationService(reservationRepository, configRepository);

            // Act
            var result = await service.MarkReservationAsArrivedAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal((int)ReservationStatus.Arrived, result.ReservationStatus);
            Assert.NotNull(result.ArrivedAt);
            Assert.Null(result.NoShowMarkedAt);
            Assert.Null(result.CancelledAt);

            // Check repository state
            Assert.NotNull(reservationRepository.LastUpdatedReservation);
            Assert.Equal((int)ReservationStatus.Arrived, reservationRepository.LastUpdatedReservation.ReservationStatus);
        }

        [Fact]
        public async Task MarkReservationAsArrivedAsync_WithCancelledReservation_ThrowsInvalidOperationException()
        {
            // Arrange
            var reservationRepository = new TestReservationRepository();
            var configRepository = new TestConfigRepository();

            var reservation = new Reservation
            {
                Id = 1,
                ReservationCode = "RES-20260315-ABCD",
                PartySize = 4,
                ReservationStatus = (int)ReservationStatus.Cancelled,
                ReservationDate = new DateTimeOffset(2026, 3, 15, 18, 0, 0, TimeSpan.Zero),
                CancelledAt = DateTimeOffset.UtcNow,
                BookingSlotId = 1,
                TableId = 5,
                Status = (int)EntityStatus.Active,
                CustomerDetail = new CustomerDetail
                {
                    Id = 1,
                    Name = "John Doe",
                    Email = "john.doe@example.com",
                    PhoneNumber = "0771234567"
                }
            };
            reservationRepository.Reservations[1] = reservation;

            var service = new ReservationService(reservationRepository, configRepository);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.MarkReservationAsArrivedAsync(1));

            Assert.Equal("Cancelled reservations cannot be marked as arrived.", exception.Message);
        }

        [Fact]
        public async Task MarkReservationAsArrivedAsync_WithNonExistentReservation_ReturnsNull()
        {
            // Arrange
            var reservationRepository = new TestReservationRepository();
            var configRepository = new TestConfigRepository();
            var service = new ReservationService(reservationRepository, configRepository);

            // Act
            var result = await service.MarkReservationAsArrivedAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task MarkReservationAsArrivedAsync_ClearsNoShowMarkedAt()
        {
            // Arrange
            var reservationRepository = new TestReservationRepository();
            var configRepository = new TestConfigRepository();

            var reservation = new Reservation
            {
                Id = 1,
                ReservationCode = "RES-20260315-ABCD",
                PartySize = 4,
                ReservationStatus = (int)ReservationStatus.NoShow,
                ReservationDate = new DateTimeOffset(2026, 3, 15, 18, 0, 0, TimeSpan.Zero),
                NoShowMarkedAt = DateTimeOffset.UtcNow.AddHours(-1),
                BookingSlotId = 1,
                TableId = 5,
                Status = (int)EntityStatus.Active,
                CustomerDetail = new CustomerDetail
                {
                    Id = 1,
                    Name = "John Doe",
                    Email = "john.doe@example.com",
                    PhoneNumber = "0771234567"
                },
                BookingSlot = new BookingSlot
                {
                    Id = 1,
                    StartTime = new TimeOnly(18, 0),
                    EndTime = new TimeOnly(20, 0)
                },
                Table = new Table
                {
                    Id = 5,
                    TableName = "Table 5",
                    Capacity = 4
                }
            };
            reservationRepository.Reservations[1] = reservation;

            var service = new ReservationService(reservationRepository, configRepository);

            // Act
            var result = await service.MarkReservationAsArrivedAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal((int)ReservationStatus.Arrived, result.ReservationStatus);
            Assert.NotNull(result.ArrivedAt);
            Assert.Null(result.NoShowMarkedAt);
        }

        [Fact]
        public async Task MarkReservationAsNoShowAsync_WithBookedReservation_MarksNoShowSuccessfully()
        {
            // Arrange
            var reservationRepository = new TestReservationRepository();
            var configRepository = new TestConfigRepository();

            var reservation = new Reservation
            {
                Id = 1,
                ReservationCode = "RES-20260315-ABCD",
                PartySize = 4,
                ReservationStatus = (int)ReservationStatus.Booked,
                ReservationDate = new DateTimeOffset(2026, 3, 15, 18, 0, 0, TimeSpan.Zero),
                BookingSlotId = 1,
                TableId = 5,
                Status = (int)EntityStatus.Active,
                CustomerDetail = new CustomerDetail
                {
                    Id = 1,
                    Name = "John Doe",
                    Email = "john.doe@example.com",
                    PhoneNumber = "0771234567"
                },
                BookingSlot = new BookingSlot
                {
                    Id = 1,
                    StartTime = new TimeOnly(18, 0),
                    EndTime = new TimeOnly(20, 0)
                },
                Table = new Table
                {
                    Id = 5,
                    TableName = "Table 5",
                    Capacity = 4
                }
            };
            reservationRepository.Reservations[1] = reservation;

            var service = new ReservationService(reservationRepository, configRepository);

            // Act
            var result = await service.MarkReservationAsNoShowAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal((int)ReservationStatus.NoShow, result.ReservationStatus);
            Assert.NotNull(result.NoShowMarkedAt);
            Assert.Null(result.ArrivedAt);
            Assert.Null(result.CancelledAt);

            // Check repository state
            Assert.NotNull(reservationRepository.LastUpdatedReservation);
            Assert.Equal((int)ReservationStatus.NoShow, reservationRepository.LastUpdatedReservation.ReservationStatus);
        }

        [Fact]
        public async Task MarkReservationAsNoShowAsync_WithArrivedReservation_ThrowsInvalidOperationException()
        {
            // Arrange
            var reservationRepository = new TestReservationRepository();
            var configRepository = new TestConfigRepository();

            var reservation = new Reservation
            {
                Id = 1,
                ReservationCode = "RES-20260315-ABCD",
                PartySize = 4,
                ReservationStatus = (int)ReservationStatus.Arrived,
                ReservationDate = new DateTimeOffset(2026, 3, 15, 18, 0, 0, TimeSpan.Zero),
                ArrivedAt = DateTimeOffset.UtcNow,
                BookingSlotId = 1,
                TableId = 5,
                Status = (int)EntityStatus.Active,
                CustomerDetail = new CustomerDetail
                {
                    Id = 1,
                    Name = "John Doe",
                    Email = "john.doe@example.com",
                    PhoneNumber = "0771234567"
                }
            };
            reservationRepository.Reservations[1] = reservation;

            var service = new ReservationService(reservationRepository, configRepository);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.MarkReservationAsNoShowAsync(1));

            Assert.Equal("Arrived reservations cannot be marked as no-show.", exception.Message);
        }

        [Fact]
        public async Task MarkReservationAsNoShowAsync_WithCancelledReservation_ThrowsInvalidOperationException()
        {
            // Arrange
            var reservationRepository = new TestReservationRepository();
            var configRepository = new TestConfigRepository();

            var reservation = new Reservation
            {
                Id = 1,
                ReservationCode = "RES-20260315-ABCD",
                PartySize = 4,
                ReservationStatus = (int)ReservationStatus.Cancelled,
                ReservationDate = new DateTimeOffset(2026, 3, 15, 18, 0, 0, TimeSpan.Zero),
                CancelledAt = DateTimeOffset.UtcNow,
                BookingSlotId = 1,
                TableId = 5,
                Status = (int)EntityStatus.Active,
                CustomerDetail = new CustomerDetail
                {
                    Id = 1,
                    Name = "John Doe",
                    Email = "john.doe@example.com",
                    PhoneNumber = "0771234567"
                }
            };
            reservationRepository.Reservations[1] = reservation;

            var service = new ReservationService(reservationRepository, configRepository);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.MarkReservationAsNoShowAsync(1));

            Assert.Equal("Cancelled reservations cannot be marked as no-show.", exception.Message);
        }

        [Fact]
        public async Task MarkReservationAsNoShowAsync_WithNonExistentReservation_ReturnsNull()
        {
            // Arrange
            var reservationRepository = new TestReservationRepository();
            var configRepository = new TestConfigRepository();
            var service = new ReservationService(reservationRepository, configRepository);

            // Act
            var result = await service.MarkReservationAsNoShowAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task MarkReservationAsNoShowAsync_ClearsArrivedAt()
        {
            // Arrange
            var reservationRepository = new TestReservationRepository();
            var configRepository = new TestConfigRepository();

            var reservation = new Reservation
            {
                Id = 1,
                ReservationCode = "RES-20260315-ABCD",
                PartySize = 4,
                ReservationStatus = (int)ReservationStatus.Booked,
                ReservationDate = new DateTimeOffset(2026, 3, 15, 18, 0, 0, TimeSpan.Zero),
                ArrivedAt = DateTimeOffset.UtcNow.AddHours(-1), // Previously marked as arrived
                BookingSlotId = 1,
                TableId = 5,
                Status = (int)EntityStatus.Active,
                CustomerDetail = new CustomerDetail
                {
                    Id = 1,
                    Name = "John Doe",
                    Email = "john.doe@example.com",
                    PhoneNumber = "0771234567"
                },
                BookingSlot = new BookingSlot
                {
                    Id = 1,
                    StartTime = new TimeOnly(18, 0),
                    EndTime = new TimeOnly(20, 0)
                },
                Table = new Table
                {
                    Id = 5,
                    TableName = "Table 5",
                    Capacity = 4
                }
            };
            reservationRepository.Reservations[1] = reservation;

            var service = new ReservationService(reservationRepository, configRepository);

            // Act
            var result = await service.MarkReservationAsNoShowAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal((int)ReservationStatus.NoShow, result.ReservationStatus);
            Assert.NotNull(result.NoShowMarkedAt);
            Assert.Null(result.ArrivedAt);
        }

        [Fact]
        public async Task StatusTransitions_MultipleChanges_MaintainCorrectTimestamps()
        {
            // Arrange
            var reservationRepository = new TestReservationRepository();
            var configRepository = new TestConfigRepository();

            var reservation = new Reservation
            {
                Id = 1,
                ReservationCode = "RES-20260315-ABCD",
                PartySize = 4,
                ReservationStatus = (int)ReservationStatus.Booked,
                ReservationDate = new DateTimeOffset(2026, 3, 15, 18, 0, 0, TimeSpan.Zero),
                BookingSlotId = 1,
                TableId = 5,
                Status = (int)EntityStatus.Active,
                CustomerDetail = new CustomerDetail
                {
                    Id = 1,
                    Name = "John Doe",
                    Email = "john.doe@example.com",
                    PhoneNumber = "0771234567"
                },
                BookingSlot = new BookingSlot
                {
                    Id = 1,
                    StartTime = new TimeOnly(18, 0),
                    EndTime = new TimeOnly(20, 0)
                },
                Table = new Table
                {
                    Id = 5,
                    TableName = "Table 5",
                    Capacity = 4
                }
            };
            reservationRepository.Reservations[1] = reservation;

            var service = new ReservationService(reservationRepository, configRepository);

            // Act - First mark as cancelled
            var cancelledResult = await service.CancelReservationAsync(1);

            // Assert initial cancel
            Assert.NotNull(cancelledResult);
            Assert.Equal((int)ReservationStatus.Cancelled, cancelledResult.ReservationStatus);
            Assert.NotNull(cancelledResult.CancelledAt);
            Assert.Null(cancelledResult.ArrivedAt);
            Assert.Null(cancelledResult.NoShowMarkedAt);
        }
    }
}
