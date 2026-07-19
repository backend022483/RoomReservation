-- Check Reservations Database
USE HotelReservationSystem;
GO

-- Check all reservations
SELECT 'All Reservations' AS QueryType, 
       Id, RoomId, GuestId, UserId, 
       CheckInDate, CheckOutDate, 
       NumberOfGuests, TotalPrice, 
       Status, BookingDate, ConfirmationNumber
FROM Reservations
ORDER BY BookingDate DESC;
GO

-- Check reservations for today
SELECT 'Reservations for Today' AS QueryType,
       r.Id, r.RoomId, r.GuestId,
       r.CheckInDate, r.CheckOutDate,
       r.Status, r.ConfirmationNumber,
       rm.RoomNumber, rm.Type,
       g.FirstName, g.LastName
FROM Reservations r
JOIN Rooms rm ON r.RoomId = rm.Id
JOIN Guests g ON r.GuestId = g.Id
WHERE r.CheckInDate = CAST(GETDATE() AS DATE)
   OR r.CheckOutDate = CAST(GETDATE() AS DATE)
   OR (r.CheckInDate < CAST(GETDATE() AS DATE) AND r.CheckOutDate > CAST(GETDATE() AS DATE))
ORDER BY r.CheckInDate;
GO

-- Check reservations for July 19-25, 2026
SELECT 'July 19-25 Reservations' AS QueryType,
       r.Id, r.RoomId, r.GuestId,
       r.CheckInDate, r.CheckOutDate,
       r.Status, r.ConfirmationNumber,
       rm.RoomNumber, rm.Type,
       g.FirstName, g.LastName
FROM Reservations r
JOIN Rooms rm ON r.RoomId = rm.Id
JOIN Guests g ON r.GuestId = g.Id
WHERE (r.CheckInDate = '2026-07-19' AND r.CheckOutDate = '2026-07-25')
   OR r.CheckInDate = '2026-07-19'
ORDER BY r.CheckInDate;
GO

-- Check guests
SELECT 'All Guests' AS QueryType, Id, FirstName, LastName, Email, PhoneNumber, Address
FROM Guests;
GO

-- Check rooms
SELECT 'All Rooms' AS QueryType, Id, RoomNumber, Type, Capacity, PricePerNight, IsAvailable, Location, Amenities
FROM Rooms;
GO

-- Check for any reservations with today's date specifically
SELECT 'Today Date Check' AS QueryType, 
       CAST(GETDATE() AS DATE) AS TodayDate,
       COUNT(*) AS TotalReservations
FROM Reservations
WHERE Status IN ('Confirmed', 'Pending');
GO
