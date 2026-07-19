-- Delete existing admin users to allow re-seeding
USE HotelReservationSystem;
GO

-- Delete all admin users
DELETE FROM Users WHERE Role = 'Administrator';
GO

-- Verify deletion
SELECT COUNT(*) as AdminCount FROM Users WHERE Role = 'Administrator';
GO
