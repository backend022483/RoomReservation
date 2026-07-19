-- Hotel Reservation System Database Schema
-- Microsoft SQL Server

-- Create Database
CREATE DATABASE HotelReservationSystem;
GO

USE HotelReservationSystem;
GO

-- Create Users Table
CREATE TABLE Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    Password NVARCHAR(255) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    PhoneNumber NVARCHAR(20),
    Role NVARCHAR(20) NOT NULL DEFAULT 'Guest',
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    IsActive BIT NOT NULL DEFAULT 1,
    CONSTRAINT CHK_Role CHECK (Role IN ('Guest', 'Receptionist', 'ReservationAgent', 'HotelManager', 'Administrator'))
);
GO

-- Create Rooms Table
CREATE TABLE Rooms (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    RoomNumber NVARCHAR(10) NOT NULL UNIQUE,
    Type NVARCHAR(50) NOT NULL,
    PricePerNight DECIMAL(10,2) NOT NULL,
    Capacity INT NOT NULL,
    Description NVARCHAR(500),
    IsAvailable BIT NOT NULL DEFAULT 1,
    ImageUrl NVARCHAR(255)
);
GO

-- Create Reservations Table
CREATE TABLE Reservations (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    RoomId INT NOT NULL,
    GuestId INT NOT NULL,
    CheckInDate DATE NOT NULL,
    CheckOutDate DATE NOT NULL,
    NumberOfGuests INT NOT NULL,
    TotalPrice DECIMAL(10,2) NOT NULL,
    Status NVARCHAR(20) NOT NULL DEFAULT 'Pending',
    BookingDate DATETIME2 NOT NULL DEFAULT GETDATE(),
    ConfirmationNumber NVARCHAR(50),
    CONSTRAINT FK_Reservations_Rooms FOREIGN KEY (RoomId) REFERENCES Rooms(Id) ON DELETE CASCADE,
    CONSTRAINT FK_Reservations_Users FOREIGN KEY (GuestId) REFERENCES Users(Id) ON DELETE CASCADE,
    CONSTRAINT CHK_Dates CHECK (CheckOutDate > CheckInDate),
    CONSTRAINT CHK_Status CHECK (Status IN ('Pending', 'Confirmed', 'Cancelled', 'Completed')),
    CONSTRAINT CHK_Guests CHECK (NumberOfGuests > 0)
);
GO

-- Create LoginActivity Table
CREATE TABLE LoginActivity (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL,
    Success BIT NOT NULL,
    ErrorMessage NVARCHAR(255),
    IpAddress NVARCHAR(45),
    Timestamp DATETIME2 NOT NULL DEFAULT GETDATE(),
    UserRole NVARCHAR(20)
);
GO

-- Create Indexes for Performance
CREATE INDEX IX_Reservations_RoomId ON Reservations(RoomId);
CREATE INDEX IX_Reservations_GuestId ON Reservations(GuestId);
CREATE INDEX IX_Reservations_CheckInDate ON Reservations(CheckInDate);
CREATE INDEX IX_Reservations_CheckOutDate ON Reservations(CheckOutDate);
CREATE INDEX IX_Reservations_Status ON Reservations(Status);
CREATE INDEX IX_LoginActivity_Timestamp ON LoginActivity(Timestamp);
CREATE INDEX IX_LoginActivity_Username ON LoginActivity(Username);
CREATE INDEX IX_LoginActivity_Success ON LoginActivity(Success);
GO

-- Insert Sample Data for Users
INSERT INTO Users (Username, Password, Email, FirstName, LastName, PhoneNumber, Role, IsActive) VALUES
('guest', HASHBYTES('SHA2_256', 'guest123'), 'guest@hotel.com', 'John', 'Guest', '+1234567890', 'Guest', 1),
('receptionist', HASHBYTES('SHA2_256', 'rec123'), 'receptionist@hotel.com', 'Sarah', 'Smith', '+1234567891', 'Receptionist', 1),
('agent', HASHBYTES('SHA2_256', 'agent123'), 'agent@hotel.com', 'Mike', 'Johnson', '+1234567892', 'ReservationAgent', 1),
('manager', HASHBYTES('SHA2_256', 'manager123'), 'manager@hotel.com', 'David', 'Williams', '+1234567893', 'HotelManager', 1),
('regin', HASHBYTES('SHA2_256', 'M0ig3l0L@g1n2009'), 'admin@hotel.com', 'Regin', 'Admin', '+1234567894', 'Administrator', 1);
GO

-- Insert Sample Data for Rooms
INSERT INTO Rooms (RoomNumber, Type, PricePerNight, Capacity, Description, IsAvailable) VALUES
('101', 'Standard Single', 100.00, 1, 'Comfortable single room with free WiFi', 1),
('102', 'Standard Double', 150.00, 2, 'Spacious double room with city view', 1),
('201', 'Deluxe Suite', 250.00, 2, 'Luxury suite with balcony and ocean view', 1),
('202', 'Family Room', 300.00, 4, 'Large family room with two bedrooms', 1),
('301', 'Penthouse', 500.00, 6, 'Exclusive penthouse with panoramic views', 1);
GO

-- Create Stored Procedure for Login Activity Logging
CREATE PROCEDURE sp_LogLoginActivity
    @Username NVARCHAR(50),
    @Success BIT,
    @ErrorMessage NVARCHAR(255) = NULL,
    @IpAddress NVARCHAR(45) = NULL,
    @UserRole NVARCHAR(20) = NULL
AS
BEGIN
    INSERT INTO LoginActivity (Username, Success, ErrorMessage, IpAddress, Timestamp, UserRole)
    VALUES (@Username, @Success, @ErrorMessage, @IpAddress, GETDATE(), @UserRole);
END
GO

-- Create Stored Procedure for User Authentication
CREATE PROCEDURE sp_AuthenticateUser
    @Username NVARCHAR(50),
    @Password NVARCHAR(255)
AS
BEGIN
    SELECT Id, Username, Email, FirstName, LastName, PhoneNumber, Role, IsActive
    FROM Users
    WHERE Username = @Username 
    AND Password = HASHBYTES('SHA2_256', @Password)
    AND IsActive = 1;
END
GO

-- Create View for Active Reservations
CREATE VIEW vw_ActiveReservations AS
SELECT 
    r.Id,
    r.ConfirmationNumber,
    rm.RoomNumber,
    rm.Type AS RoomType,
    u.FirstName + ' ' + u.LastName AS GuestName,
    u.Email AS GuestEmail,
    r.CheckInDate,
    r.CheckOutDate,
    r.NumberOfGuests,
    r.TotalPrice,
    r.Status,
    r.BookingDate
FROM Reservations r
JOIN Rooms rm ON r.RoomId = rm.Id
JOIN Users u ON r.GuestId = u.Id
WHERE r.Status IN ('Pending', 'Confirmed');
GO

-- Create View for Room Availability
CREATE VIEW vw_RoomAvailability AS
SELECT 
    rm.Id,
    rm.RoomNumber,
    rm.Type,
    rm.PricePerNight,
    rm.Capacity,
    rm.Description,
    rm.IsAvailable,
    COUNT(r.Id) AS ActiveReservations
FROM Rooms rm
LEFT JOIN Reservations r ON rm.Id = r.RoomId 
    AND r.Status IN ('Pending', 'Confirmed')
    AND r.CheckOutDate > CAST(GETDATE() AS DATE)
GROUP BY rm.Id, rm.RoomNumber, rm.Type, rm.PricePerNight, rm.Capacity, rm.Description, rm.IsAvailable;
GO

-- Grant Permissions (adjust as needed for your security requirements)
-- GRANT SELECT, INSERT, UPDATE, DELETE ON Users TO YourAppUser;
-- GRANT SELECT, INSERT, UPDATE, DELETE ON Rooms TO YourAppUser;
-- GRANT SELECT, INSERT, UPDATE, DELETE ON Reservations TO YourAppUser;
-- GRANT SELECT, INSERT ON LoginActivity TO YourAppUser;
-- GRANT EXECUTE ON sp_LogLoginActivity TO YourAppUser;
-- GRANT EXECUTE ON sp_AuthenticateUser TO YourAppUser;
-- GRANT SELECT ON vw_ActiveReservations TO YourAppUser;
-- GRANT SELECT ON vw_RoomAvailability TO YourAppUser;
GO

PRINT 'Database schema created successfully.';
PRINT 'Sample data inserted.';
PRINT 'Stored procedures and views created.';
GO
