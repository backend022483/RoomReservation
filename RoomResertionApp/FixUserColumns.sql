-- Fix NULL values in Users table and set proper defaults
USE HotelReservationSystem;
GO

-- Update any NULL values in required columns to proper defaults
UPDATE Users
SET Email = ISNULL(Email, 'temp@email.com'),
    FirstName = ISNULL(FirstName, 'Temp'),
    LastName = ISNULL(LastName, 'User'),
    PhoneNumber = ISNULL(PhoneNumber, '000-000-0000'),
    Role = ISNULL(Role, 0),
    CreatedAt = ISNULL(CreatedAt, GETDATE()),
    IsActive = ISNULL(IsActive, 1),
    FailedLoginAttempts = ISNULL(FailedLoginAttempts, 0)
WHERE Email IS NULL OR FirstName IS NULL OR LastName IS NULL OR 
      PhoneNumber IS NULL OR Role IS NULL OR CreatedAt IS NULL OR 
      IsActive IS NULL OR FailedLoginAttempts IS NULL;
GO

-- Set proper defaults for the table
ALTER TABLE Users
ADD CONSTRAINT DF_Users_Email DEFAULT 'temp@email.com' FOR Email;
GO

ALTER TABLE Users
ADD CONSTRAINT DF_Users_FirstName DEFAULT 'Temp' FOR FirstName;
GO

ALTER TABLE Users
ADD CONSTRAINT DF_Users_LastName DEFAULT 'User' FOR LastName;
GO

ALTER TABLE Users
ADD CONSTRAINT DF_Users_PhoneNumber DEFAULT '000-000-0000' FOR PhoneNumber;
GO

ALTER TABLE Users
ADD CONSTRAINT DF_Users_Role DEFAULT 0 FOR Role;
GO

ALTER TABLE Users
ADD CONSTRAINT DF_Users_CreatedAt DEFAULT GETDATE() FOR CreatedAt;
GO

ALTER TABLE Users
ADD CONSTRAINT DF_Users_IsActive DEFAULT 1 FOR IsActive;
GO

ALTER TABLE Users
ADD CONSTRAINT DF_Users_FailedLoginAttempts DEFAULT 0 FOR FailedLoginAttempts;
GO

PRINT 'User columns fixed with proper defaults.';
GO
