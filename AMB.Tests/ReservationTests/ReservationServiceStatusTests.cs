using AMB.Application.Dtos;
using AMB.Application.Services;
using AMB.Domain.Entities;
using AMB.Domain.Enums;
using AMB.Tests.Mocks;

namespace AMB.Tests.ReservationTests
{
    public class ReservationServiceStatusTests
    {
        [Fact]
        public async Task GetReservationsPagedAsync_WithReservationStatusFilter_ReturnsOnlyMatchingReservations()
        {
            var reservationRepository = new TestReservationRepository();
            var configRepository = new TestConfigRepository();
            var service = new ReservationService(reservationRepository, configRepository, new TestEmployeeRepository(), new TestEmailService());

            reservationRepository.Reservations[1] = new Reservation
            {
                Id = 1,
                ReservationCode = "RES-BOOKED",
                PartySize = 2,
                ReservationStatus = (int)ReservationStatus.Booked,
                ReservationDate = new DateTimeOffset(2026, 3, 15, 18, 0, 0, TimeSpan.Zero),
                Status = (int)EntityStatus.Active,
                CreatedDate = new DateTimeOffset(2026, 3, 10, 10, 0, 0, TimeSpan.Zero),
                CustomerDetail = new CustomerDetail { Name = "John", Email = "john@example.com", PhoneNumber = "0771111111" },
                BookingSlot = new BookingSlot { Id = 1, StartTime = new TimeOnly(18, 0), EndTime = new TimeOnly(20, 0) },
                Table = new Table { Id = 1, TableName = "T1", Capacity = 4 }
            };

            reservationRepository.Reservations[2] = new Reservation
            {
                Id = 2,
                ReservationCode = "RES-ARRIVED",
                PartySize = 4,
                ReservationStatus = (int)ReservationStatus.Arrived,
                ReservationDate = new DateTimeOffset(2026, 3, 15, 19, 0, 0, TimeSpan.Zero),
                Status = (int)EntityStatus.Active,
                CreatedDate = new DateTimeOffset(2026, 3, 11, 10, 0, 0, TimeSpan.Zero),
                CustomerDetail = new CustomerDetail { Name = "Jane", Email = "jane@example.com", PhoneNumber = "0772222222" },
                BookingSlot = new BookingSlot { Id = 2, StartTime = new TimeOnly(19, 0), EndTime = new TimeOnly(21, 0) },
                Table = new Table { Id = 2, TableName = "T2", Capacity = 4 }
            };

            var result = await service.GetReservationsPagedAsync(new ReservationFilterRequestDto
            {
                ReservationStatus = (int)ReservationStatus.Arrived,
                PageNumber = 1,
                PageSize = 10
            });

            Assert.Single(result.Items);
            Assert.Equal("RES-ARRIVED", result.Items[0].ReservationCode);
            Assert.Equal((int)ReservationStatus.Arrived, result.Items[0].ReservationStatus);
            Assert.Equal(1, result.TotalItemCount);
        }

        [Fact]
        public async Task GetReservationsPagedAsync_WithPageSizeZero_ReturnsAllFilteredReservations()
        {
            var reservationRepository = new TestReservationRepository();
            var configRepository = new TestConfigRepository();
            var service = new ReservationService(reservationRepository, configRepository, new TestEmployeeRepository(), new TestEmailService());

            reservationRepository.Reservations[1] = new Reservation
            {
                Id = 1,
                ReservationCode = "RES-1",
                PartySize = 2,
                ReservationStatus = (int)ReservationStatus.Booked,
                ReservationDate = new DateTimeOffset(2026, 3, 15, 18, 0, 0, TimeSpan.Zero),
                Status = (int)EntityStatus.Active,
                CreatedDate = new DateTimeOffset(2026, 3, 10, 10, 0, 0, TimeSpan.Zero),
                CustomerDetail = new CustomerDetail { Name = "Alex", Email = "alex@example.com", PhoneNumber = "0771111111" },
                BookingSlot = new BookingSlot { Id = 1, StartTime = new TimeOnly(18, 0), EndTime = new TimeOnly(20, 0) },
                Table = new Table { Id = 1, TableName = "T1", Capacity = 4 }
            };

            reservationRepository.Reservations[2] = new Reservation
            {
                Id = 2,
                ReservationCode = "RES-2",
                PartySize = 3,
                ReservationStatus = (int)ReservationStatus.Booked,
                ReservationDate = new DateTimeOffset(2026, 3, 16, 18, 0, 0, TimeSpan.Zero),
                Status = (int)EntityStatus.Active,
                CreatedDate = new DateTimeOffset(2026, 3, 11, 10, 0, 0, TimeSpan.Zero),
                CustomerDetail = new CustomerDetail { Name = "Alex", Email = "alex2@example.com", PhoneNumber = "0772222222" },
                BookingSlot = new BookingSlot { Id = 2, StartTime = new TimeOnly(18, 0), EndTime = new TimeOnly(20, 0) },
                Table = new Table { Id = 2, TableName = "T2", Capacity = 4 }
            };

            reservationRepository.Reservations[3] = new Reservation
            {
                Id = 3,
                ReservationCode = "RES-3",
                PartySize = 5,
                ReservationStatus = (int)ReservationStatus.Cancelled,
                ReservationDate = new DateTimeOffset(2026, 3, 17, 18, 0, 0, TimeSpan.Zero),
                Status = (int)EntityStatus.Active,
                CreatedDate = new DateTimeOffset(2026, 3, 12, 10, 0, 0, TimeSpan.Zero),
                CustomerDetail = new CustomerDetail { Name = "Chris", Email = "chris@example.com", PhoneNumber = "0773333333" },
                BookingSlot = new BookingSlot { Id = 3, StartTime = new TimeOnly(18, 0), EndTime = new TimeOnly(20, 0) },
                Table = new Table { Id = 3, TableName = "T3", Capacity = 6 }
            };

            var result = await service.GetReservationsPagedAsync(new ReservationFilterRequestDto
            {
                ReservationStatus = (int)ReservationStatus.Booked,
                PageNumber = 1,
                PageSize = 0
            });

            Assert.Equal(2, result.Items.Count);
            Assert.Equal(2, result.TotalItemCount);
            Assert.Equal(0, result.PageSize);
            Assert.Equal(1, result.PageCount);
            Assert.All(result.Items, item => Assert.Equal((int)ReservationStatus.Booked, item.ReservationStatus));
        }

        [Fact]
        public async Task AssignWaiterAsync_WithReservationIdsAndEmployeeId_AssignsWaiterToAllReservations()
        {
            var reservationRepository = new TestReservationRepository();
            var configRepository = new TestConfigRepository();
            var employeeRepository = new TestEmployeeRepository();

            employeeRepository.Employees[7] = new Employee
            {
                Id = 7,
                EmployeeId = "EMP-007",
                FirstName = "John",
                LastName = "Doe",
                Username = "john.doe@example.com",
                MobileNumber = "0770000000",
                Address = "Address",
                Status = (int)EntityStatus.Active
            };

            reservationRepository.Reservations[1] = new Reservation
            {
                Id = 1,
                ReservationCode = "RES-1",
                PartySize = 2,
                ReservationStatus = (int)ReservationStatus.Booked,
                ReservationDate = DateTimeOffset.UtcNow.AddHours(1),
                Status = (int)EntityStatus.Active,
                CustomerDetail = new CustomerDetail { Name = "A", Email = "a@example.com", PhoneNumber = "1" },
                BookingSlot = new BookingSlot { Id = 1, StartTime = new TimeOnly(18, 0), EndTime = new TimeOnly(20, 0) },
                Table = new Table { Id = 1, TableName = "T1", Capacity = 4 }
            };

            reservationRepository.Reservations[2] = new Reservation
            {
                Id = 2,
                ReservationCode = "RES-2",
                PartySize = 4,
                ReservationStatus = (int)ReservationStatus.Booked,
                ReservationDate = DateTimeOffset.UtcNow.AddHours(2),
                Status = (int)EntityStatus.Active,
                CustomerDetail = new CustomerDetail { Name = "B", Email = "b@example.com", PhoneNumber = "2" },
                BookingSlot = new BookingSlot { Id = 2, StartTime = new TimeOnly(19, 0), EndTime = new TimeOnly(21, 0) },
                Table = new Table { Id = 2, TableName = "T2", Capacity = 4 }
            };

            var service = new ReservationService(reservationRepository, configRepository, employeeRepository, new TestEmailService());

            var result = await service.AssignWaiterAsync(new AssignWaiterRequestDto
            {
                ReservationIds = new List<int> { 1, 2 },
                EmployeeId = 7
            });

            Assert.Equal(2, result.Count);
            Assert.All(result, reservation => Assert.Equal(7, reservation.AssignedWaiterId));
            Assert.Equal(7, reservationRepository.Reservations[1].AssignedWaiterId);
            Assert.Equal(7, reservationRepository.Reservations[2].AssignedWaiterId);
        }

        [Fact]
        public async Task UnassignWaiterAsync_WithReservationIdsAndEmployeeId_UnassignsWaiterFromAllReservations()
        {
            var reservationRepository = new TestReservationRepository();
            var configRepository = new TestConfigRepository();
            var employeeRepository = new TestEmployeeRepository();

            employeeRepository.Employees[7] = new Employee
            {
                Id = 7,
                EmployeeId = "EMP-007",
                FirstName = "John",
                LastName = "Doe",
                Username = "john.doe@example.com",
                MobileNumber = "0770000000",
                Address = "Address",
                Status = (int)EntityStatus.Active
            };

            reservationRepository.Reservations[1] = new Reservation
            {
                Id = 1,
                ReservationCode = "RES-1",
                PartySize = 2,
                ReservationStatus = (int)ReservationStatus.Booked,
                ReservationDate = DateTimeOffset.UtcNow.AddHours(1),
                AssignedWaiterId = 7,
                Status = (int)EntityStatus.Active,
                CustomerDetail = new CustomerDetail { Name = "A", Email = "a@example.com", PhoneNumber = "1" },
                BookingSlot = new BookingSlot { Id = 1, StartTime = new TimeOnly(18, 0), EndTime = new TimeOnly(20, 0) },
                Table = new Table { Id = 1, TableName = "T1", Capacity = 4 }
            };

            reservationRepository.Reservations[2] = new Reservation
            {
                Id = 2,
                ReservationCode = "RES-2",
                PartySize = 4,
                ReservationStatus = (int)ReservationStatus.Booked,
                ReservationDate = DateTimeOffset.UtcNow.AddHours(2),
                AssignedWaiterId = 7,
                Status = (int)EntityStatus.Active,
                CustomerDetail = new CustomerDetail { Name = "B", Email = "b@example.com", PhoneNumber = "2" },
                BookingSlot = new BookingSlot { Id = 2, StartTime = new TimeOnly(19, 0), EndTime = new TimeOnly(21, 0) },
                Table = new Table { Id = 2, TableName = "T2", Capacity = 4 }
            };

            var service = new ReservationService(reservationRepository, configRepository, employeeRepository, new TestEmailService());

            var result = await service.UnassignWaiterAsync(new AssignWaiterRequestDto
            {
                ReservationIds = new List<int> { 1, 2 },
                EmployeeId = 7
            });

            Assert.Equal(2, result.Count);
            Assert.All(result, reservation => Assert.Null(reservation.AssignedWaiterId));
            Assert.Null(reservationRepository.Reservations[1].AssignedWaiterId);
            Assert.Null(reservationRepository.Reservations[2].AssignedWaiterId);
        }

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

            var service = new ReservationService(reservationRepository, configRepository, new TestEmployeeRepository(), new TestEmailService());

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

            var service = new ReservationService(reservationRepository, configRepository, new TestEmployeeRepository(), new TestEmailService());

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
            var service = new ReservationService(reservationRepository, configRepository, new TestEmployeeRepository(), new TestEmailService());

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

            var service = new ReservationService(reservationRepository, configRepository, new TestEmployeeRepository(), new TestEmailService());

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

            var service = new ReservationService(reservationRepository, configRepository, new TestEmployeeRepository(), new TestEmailService());

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
            var service = new ReservationService(reservationRepository, configRepository, new TestEmployeeRepository(), new TestEmailService());

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

            var service = new ReservationService(reservationRepository, configRepository, new TestEmployeeRepository(), new TestEmailService());

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

            var service = new ReservationService(reservationRepository, configRepository, new TestEmployeeRepository(), new TestEmailService());

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

            var service = new ReservationService(reservationRepository, configRepository, new TestEmployeeRepository(), new TestEmailService());

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

            var service = new ReservationService(reservationRepository, configRepository, new TestEmployeeRepository(), new TestEmailService());

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
            var service = new ReservationService(reservationRepository, configRepository, new TestEmployeeRepository(), new TestEmailService());

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

            var service = new ReservationService(reservationRepository, configRepository, new TestEmployeeRepository(), new TestEmailService());

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

            var service = new ReservationService(reservationRepository, configRepository, new TestEmployeeRepository(), new TestEmailService());

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

