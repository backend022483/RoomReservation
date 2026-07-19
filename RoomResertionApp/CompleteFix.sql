-- Complete fix: Update NULL values and delete users for re-seeding
USE HotelReservationSystem;
GO

-- Fix all NULL values in Users table
UPDATE Users
SET Email = ISNULL(Email, 'temp@email.com'),
    FirstName = ISNULL(FirstName, 'Temp'),
    LastName = ISNULL(LastName, 'User'),
    PhoneNumber = ISNULL(PhoneNumber, '000-000-0000'),
    Role = ISNULL(Role, 0),
    CreatedAt = ISNULL(CreatedAt, GETDATE()),
    IsActive = ISNULL(IsActive, 1),
    FailedLoginAttempts = ISNULL(FailedLoginAttempts, 0);
GO

-- Delete all users to allow app to re-seed with new password hashing
DELETE FROM Users;
GO

PRINT 'All users deleted. The app will re-seed them with new password hashes on next startup.';
GO
