-- Delete all users to allow app to re-seed with new password hashing
USE HotelReservationSystem;
GO

-- Delete all users
DELETE FROM Users;
GO

PRINT 'All users deleted. The app will re-seed them with new password hashes on next startup.';
GO
