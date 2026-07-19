-- Update amenities for all rooms
USE HotelReservationSystem;
GO

-- Update Amenities for all rooms
UPDATE Rooms
SET Amenities = 'WiFi, TV, Washing Machine, Dryer'
WHERE Amenities IS NULL OR Amenities = '';
GO

PRINT 'Amenities updated successfully for all rooms.';
GO

-- Verify the update
SELECT Id, RoomNumber, Type, Amenities
FROM Rooms;
GO
