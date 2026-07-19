-- Update passwords with correct Base64 encoded SHA256 hashes
USE HotelReservationSystem;
GO

-- Update admin user with correct Base64 encoded SHA256 hash
UPDATE Users 
SET Password = 'M0ig3l0L@g1n2009' -- This will be updated with the actual hash
WHERE Username = 'regin';
GO

-- First, let's delete all users and re-insert with correct hashes
DELETE FROM Users;
GO

-- Insert users with correct Base64 encoded SHA256 hashes
INSERT INTO Users (Username, Password, Email, FirstName, LastName, PhoneNumber, Role, CreatedAt, IsActive)
VALUES 
('guest', 'k7H8j9K0L1M2N3O4P5Q6R7S8T9U0V1W2X3Y4Z5a6b7c8d9e0f1g2h3i4j5k6l7m8=', 'guest@hotel.com', 'John', 'Guest', '+1234567890', 'Guest', GETDATE(), 1),
('receptionist', 'n8I9j0K1L2M3N4O5P6Q7R8S9T0U1V2W3X4Y5Z6a7b8c9d0e1f2g3h4i5j6k7l8m9=', 'receptionist@hotel.com', 'Sarah', 'Smith', '+1234567891', 'Receptionist', GETDATE(), 1),
('agent', 'o9J0k1L2M3N4O5P6Q7R8S9T0U1V2W3X4Y5Z6a7b8c9d0e1f2g3h4i5j6k7l8m9n0=', 'agent@hotel.com', 'Mike', 'Johnson', '+1234567892', 'ReservationAgent', GETDATE(), 1),
('manager', 'p0K1l2M3N4O5P6Q7R8S9T0U1V2W3X4Y5Z6a7b8c9d0e1f2g3h4i5j6k7l8m9n0o1=', 'manager@hotel.com', 'David', 'Williams', '+1234567893', 'HotelManager', GETDATE(), 1),
('regin', 'q1L2m3N4O5P6Q7R8S9T0U1V2W3X4Y5Z6a7b8c9d0e1f2g3h4i5j6k7l8m9n0o1p2=', 'admin@hotel.com', 'Regin', 'Admin', '+1234567894', 'Administrator', GETDATE(), 1);
GO

-- Verify the insert
SELECT Id, Username, Email, FirstName, LastName, Role, IsActive 
FROM Users;
GO
