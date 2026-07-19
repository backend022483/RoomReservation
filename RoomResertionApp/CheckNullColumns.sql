-- Check for NULL values in Users table
USE HotelReservationSystem;
GO

SELECT 
    Username,
    CASE WHEN Email IS NULL THEN 'Email is NULL' ELSE 'Email OK' END,
    CASE WHEN FirstName IS NULL THEN 'FirstName is NULL' ELSE 'FirstName OK' END,
    CASE WHEN LastName IS NULL THEN 'LastName is NULL' ELSE 'LastName OK' END,
    CASE WHEN PhoneNumber IS NULL THEN 'PhoneNumber is NULL' ELSE 'PhoneNumber OK' END,
    CASE WHEN Role IS NULL THEN 'Role is NULL' ELSE 'Role OK' END,
    CASE WHEN CreatedAt IS NULL THEN 'CreatedAt is NULL' ELSE 'CreatedAt OK' END,
    CASE WHEN IsActive IS NULL THEN 'IsActive is NULL' ELSE 'IsActive OK' END,
    CASE WHEN FailedLoginAttempts IS NULL THEN 'FailedLoginAttempts is NULL' ELSE 'FailedLoginAttempts OK' END,
    CASE WHEN LockoutEndDate IS NULL THEN 'LockoutEndDate is NULL (OK)' ELSE 'LockoutEndDate OK' END
FROM Users;
GO
