-- Create Guests table
USE HotelReservationSystem;
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Guests')
BEGIN
    CREATE TABLE Guests (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        FirstName NVARCHAR(100) NOT NULL,
        LastName NVARCHAR(100) NOT NULL,
        Email NVARCHAR(255) NOT NULL,
        PhoneNumber NVARCHAR(50) NOT NULL,
        Address NVARCHAR(500) NULL
    );
    PRINT 'Guests table created successfully.';
END
ELSE
BEGIN
    PRINT 'Guests table already exists.';
END
GO
