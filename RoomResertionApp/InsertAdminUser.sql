-- Insert admin user with correct credentials
USE HotelReservationSystem;
GO

-- Insert admin user with hashed password using SHA2_256
INSERT INTO Users (Username, Password, Email, FirstName, LastName, PhoneNumber, Role, CreatedAt, IsActive)
VALUES ('regin', HASHBYTES('SHA2_256', 'M0ig3l0L@g1n2009'), 'admin@hotel.com', 'Regin', 'Admin', '+1234567894', 'Administrator', GETDATE(), 1);
GO

-- Insert other demo users
INSERT INTO Users (Username, Password, Email, FirstName, LastName, PhoneNumber, Role, CreatedAt, IsActive)
VALUES 
('guest', HASHBYTES('SHA2_256', 'guest123'), 'guest@hotel.com', 'John', 'Guest', '+1234567890', 'Guest', GETDATE(), 1),
('receptionist', HASHBYTES('SHA2_256', 'rec123'), 'receptionist@hotel.com', 'Sarah', 'Smith', '+1234567891', 'Receptionist', GETDATE(), 1),
('agent', HASHBYTES('SHA2_256', 'agent123'), 'agent@hotel.com', 'Mike', 'Johnson', '+1234567892', 'ReservationAgent', GETDATE(), 1),
('manager', HASHBYTES('SHA2_256', 'manager123'), 'manager@hotel.com', 'David', 'Williams', '+1234567893', 'HotelManager', GETDATE(), 1);
GO

-- Verify the insert
SELECT Id, Username, Email, FirstName, LastName, Role, IsActive 
FROM Users;
GO
