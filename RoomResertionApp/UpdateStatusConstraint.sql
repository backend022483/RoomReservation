-- Update CHK_Status constraint to include Checked-in and Checked-out status
USE HotelReservationSystem;
GO

-- Drop the existing constraint
ALTER TABLE Reservations
DROP CONSTRAINT CHK_Status;
GO

-- Add the updated constraint with all valid statuses
ALTER TABLE Reservations
ADD CONSTRAINT CHK_Status
CHECK (Status IN ('Pending', 'Confirmed', 'Cancelled', 'Checked-in', 'Checked-out'));
GO

-- Verify the constraint
SELECT name AS ConstraintName, definition AS ConstraintDefinition
FROM sys.check_constraints
WHERE parent_object_id = OBJECT_ID('Reservations');
GO
