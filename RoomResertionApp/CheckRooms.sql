-- Check existing rooms in database
USE HotelReservationSystem;
GO

SELECT Id, RoomNumber, Type, PricePerNight, Capacity, Description, IsAvailable 
FROM Rooms;
GO
