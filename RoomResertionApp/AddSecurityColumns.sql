-- Add security columns to Users table for account lockout functionality
USE HotelReservationSystem;
GO

-- Add FailedLoginAttempts column
ALTER TABLE Users
ADD FailedLoginAttempts INT NOT NULL DEFAULT 0;
GO

-- Add LockoutEndDate column
ALTER TABLE Users
ADD LockoutEndDate DATETIME2 NULL;
GO

PRINT 'Security columns added successfully to Users table.';
GO
