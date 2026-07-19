-- Update passwords with correct Base64 encoded SHA256 hashes
USE HotelReservationSystem;
GO

-- Delete all users and re-insert with correct hashes
DELETE FROM Users;
GO

-- Insert users with correct Base64 encoded SHA256 hashes
INSERT INTO Users (Username, Password, Email, FirstName, LastName, PhoneNumber, Role, CreatedAt, IsActive)
VALUES 
('guest', 'a5PMukFKwdCuHnfz+sVgx0imcB7WlGc1pJ1GM1FRjhY=', 'guest@hotel.com', 'John', 'Guest', '+1234567890', 'Guest', GETDATE(), 1),
('receptionist', 'EnDdvTiOMJsSNPTlAOp4qDydERBA+mzOhsMd8BRKNlk=', 'receptionist@hotel.com', 'Sarah', 'Smith', '+1234567891', 'Receptionist', GETDATE(), 1),
('agent', '9E0ayb8MabCDOAuG2987c3lxUOPMpIIKw5n3kX5gdkc=', 'agent@hotel.com', 'Mike', 'Johnson', '+1234567892', 'ReservationAgent', GETDATE(), 1),
('manager', 'hmSFeWz6jXwM9xEWQCBbgwdkM1R1d1EdgfgDCumezqU=', 'manager@hotel.com', 'David', 'Williams', '+1234567893', 'HotelManager', GETDATE(), 1),
('regin', 'EE+d5BEmTI8j0b/LIqf/qNig2rpz47vQDMC8ILG3oTI=', 'admin@hotel.com', 'Regin', 'Admin', '+1234567894', 'Administrator', GETDATE(), 1);
GO

-- Verify the insert
SELECT Id, Username, Email, FirstName, LastName, Role, IsActive 
FROM Users;
GO
