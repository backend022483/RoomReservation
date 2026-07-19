-- Fix NULL PhoneNumber values in Users table
USE HotelReservationSystem;
GO

-- Update NULL PhoneNumber values with proper phone numbers
UPDATE Users
SET PhoneNumber = '000-000-0000'
WHERE PhoneNumber IS NULL;
GO

PRINT 'PhoneNumber values fixed.';
GO
