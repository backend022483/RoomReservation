-- Unlock user account(s)
USE HotelReservationSystem;
GO

-- Unlock specific user (replace 'regin' with the username you want to unlock)
UPDATE Users
SET FailedLoginAttempts = 0,
    LockoutEndDate = NULL,
    IsActive = 1
WHERE Username = 'regin';
GO

-- Alternatively, unlock ALL locked accounts
-- UPDATE Users
-- SET FailedLoginAttempts = 0,
--     LockoutEndDate = NULL,
--     IsActive = 1
-- WHERE LockoutEndDate IS NOT NULL OR FailedLoginAttempts > 0;
-- GO

PRINT 'Account unlocked successfully.';
GO
