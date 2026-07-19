-- Check existing users in database
USE HotelReservationSystem;
GO

SELECT Id, Username, Email, FirstName, LastName, Role, IsActive, CreatedAt 
FROM Users;
GO
