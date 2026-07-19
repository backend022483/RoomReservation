-- Make UserId nullable in Reservations table
USE HotelReservationSystem;
GO

-- Check current constraint
SELECT OBJECT_NAME(parent_object_id) AS TableName, 
       name AS ConstraintName,
       type_desc AS ConstraintType
FROM sys.foreign_keys
WHERE parent_object_id = OBJECT_ID('Reservations');

-- Drop the foreign key constraint
ALTER TABLE Reservations
DROP CONSTRAINT FK_Reservations_Users;
GO

-- Make UserId nullable
ALTER TABLE Reservations
ALTER COLUMN UserId INT NULL;
GO

-- Re-add the foreign key constraint with ON DELETE SET NULL
ALTER TABLE Reservations
ADD CONSTRAINT FK_Reservations_Users
FOREIGN KEY (UserId) REFERENCES Users(Id)
ON DELETE SET NULL;
GO

-- Verify the change
SELECT COLUMN_NAME, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Reservations' AND COLUMN_NAME = 'UserId';
GO
