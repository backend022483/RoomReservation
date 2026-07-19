-- Add Location and Amenities columns to Rooms table
USE HotelReservationSystem;
GO

-- Add Location column
ALTER TABLE Rooms
ADD Location NVARCHAR(255) NULL;
GO

-- Add Amenities column
ALTER TABLE Rooms
ADD Amenities NVARCHAR(MAX) NULL;
GO

PRINT 'Location and Amenities columns added successfully to Rooms table.';
GO