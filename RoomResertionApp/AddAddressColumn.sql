-- Add Address column to Users table
USE HotelReservationSystem;
GO

-- Add Address column
ALTER TABLE Users
ADD Address NVARCHAR(MAX) NULL;
GO

PRINT 'Address column added successfully to Users table.';
GO
