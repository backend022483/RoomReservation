-- Add UserId column to Reservations table
USE HotelReservationSystem;
GO

-- Add UserId column
ALTER TABLE Reservations
ADD UserId INT NULL;
GO

-- Add foreign key constraint to Users table
ALTER TABLE Reservations
ADD CONSTRAINT FK_Reservations_Users
FOREIGN KEY (UserId) REFERENCES Users(Id);
GO

PRINT 'UserId column added successfully to Reservations table.';
GO
