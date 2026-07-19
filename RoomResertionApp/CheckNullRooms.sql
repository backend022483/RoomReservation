-- Check for NULL values in Rooms table
USE HotelReservationSystem;
GO

SELECT Id, RoomNumber, Type, PricePerNight, Capacity, Description, IsAvailable, ImageUrl 
FROM Rooms 
WHERE RoomNumber IS NULL OR Type IS NULL OR Description IS NULL OR ImageUrl IS NULL;
GO
