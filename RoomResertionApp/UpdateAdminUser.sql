-- Update admin user to regin with correct password
USE HotelReservationSystem;
GO

-- Update the admin user (id=5) to regin with hashed password
UPDATE Users 
SET Username = 'regin', 
    FirstName = 'Regin',
    LastName = 'Admin',
    Password = 0x6B6A7C6E656B6E6D6C6A6B6C6A6B6C6A6B6C6A6B6C6A6B6C6A6B6C6A6B6C6A6B6C6A
WHERE Id = 5;
GO

-- Verify the update
SELECT Id, Username, Email, FirstName, LastName, Role, IsActive 
FROM Users 
WHERE Id = 5;
GO
