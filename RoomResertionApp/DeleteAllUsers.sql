-- Delete all users to trigger re-seeding
USE HotelReservationSystem;
GO

DELETE FROM Users;
GO

-- Verify deletion
SELECT COUNT(*) as UserCount FROM Users;
GO
