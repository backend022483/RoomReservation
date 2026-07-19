-- Add more rooms to the database
USE HotelReservationSystem;
GO

INSERT INTO Rooms (RoomNumber, Type, PricePerNight, Capacity, Description, IsAvailable)
VALUES 
('103', 'Standard Single', 100.00, 1, 'Comfortable single room with garden view', 1),
('104', 'Standard Double', 150.00, 2, 'Spacious double room with mountain view', 1),
('105', 'Standard Twin', 160.00, 2, 'Twin room with two single beds', 1),
('206', 'Deluxe Suite', 250.00, 2, 'Luxury suite with city view and jacuzzi', 1),
('207', 'Deluxe Suite', 280.00, 2, 'Premium suite with kitchenette', 1),
('208', 'Family Suite', 350.00, 4, 'Family suite with separate living area', 1),
('209', 'Family Room', 320.00, 4, 'Family room with connecting doors', 1),
('302', 'Executive Suite', 400.00, 2, 'Executive suite with office space', 1),
('303', 'Presidential Suite', 600.00, 4, 'Presidential suite with full amenities', 1),
('304', 'Penthouse', 550.00, 6, 'Luxury penthouse with private terrace', 1),
('401', 'Studio Apartment', 200.00, 2, 'Self-contained studio with kitchen', 1),
('402', 'One Bedroom Apartment', 280.00, 3, 'Apartment with separate bedroom', 1),
('501', 'Accessible Room', 120.00, 2, 'Wheelchair accessible room with grab bars', 1),
('502', 'Accessible Suite', 220.00, 2, 'Accessible suite with roll-in shower', 1),
('601', 'Economy Room', 80.00, 1, 'Budget-friendly single room', 1);
GO

-- Verify the insert
SELECT Id, RoomNumber, Type, PricePerNight, Capacity, Description, IsAvailable 
FROM Rooms
ORDER BY RoomNumber;
GO
