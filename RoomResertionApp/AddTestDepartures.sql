-- Add test data for depart functionality (3 guests checked in July 17, departing July 19)
USE HotelReservationSystem;
GO

-- Insert test guests
INSERT INTO Guests (FirstName, LastName, Email, PhoneNumber, Address)
VALUES 
    ('John', 'Doe', 'john.doe@test.com', '555-0101', '123 Main St'),
    ('Jane', 'Smith', 'jane.smith@test.com', '555-0102', '456 Oak Ave'),
    ('Bob', 'Johnson', 'bob.johnson@test.com', '555-0103', '789 Pine Rd');
GO

-- Insert test reservations (checked in July 17, departing July 19)
-- Using subqueries to get guest IDs
INSERT INTO Reservations (RoomId, GuestId, CheckInDate, CheckOutDate, NumberOfGuests, TotalPrice, Status, BookingDate, ConfirmationNumber, UserId)
VALUES 
    (1, (SELECT TOP 1 Id FROM Guests WHERE Email = 'john.doe@test.com' ORDER BY Id DESC), '2026-07-17', '2026-07-19', 1, 400.00, 'Checked-in', '2026-07-15', 'CONF-DEP-001', NULL),
    (2, (SELECT TOP 1 Id FROM Guests WHERE Email = 'jane.smith@test.com' ORDER BY Id DESC), '2026-07-17', '2026-07-19', 2, 600.00, 'Checked-in', '2026-07-15', 'CONF-DEP-002', NULL),
    (3, (SELECT TOP 1 Id FROM Guests WHERE Email = 'bob.johnson@test.com' ORDER BY Id DESC), '2026-07-17', '2026-07-19', 1, 500.00, 'Checked-in', '2026-07-15', 'CONF-DEP-003', NULL);
GO

-- Verify the data
SELECT r.Id, r.ConfirmationNumber, r.CheckInDate, r.CheckOutDate, r.Status, 
       g.FirstName, g.LastName, g.Email, rm.RoomNumber
FROM Reservations r
JOIN Guests g ON r.GuestId = g.Id
JOIN Rooms rm ON r.RoomId = rm.Id
WHERE r.CheckInDate = '2026-07-17' AND r.CheckOutDate = '2026-07-19';
GO
