-- Update all user passwords to use new PasswordHasher format
USE HotelReservationSystem;
GO

-- Update admin password (regin / Admin@2009)
UPDATE Users
SET Password = 'AQAAAAEAACcQAAAAEPoCXNNMAa0FhAR8HPzAzmEFTXdm6f9wPlKJ/O0ReCf2Qpdjbl9fsTTW0xaEFU1Jaw=='
WHERE Username = 'regin';
GO

-- Update guest password (guest / Guest@123)
UPDATE Users
SET Password = 'AQAAAAEAACcQAAAAEHaR4eXEnTgSRQBQSyhFvbD/4JVy2nLzmywEGqDwgJbsuEAsLpCiwxirjnBP3E9TGg=='
WHERE Username = 'guest';
GO

-- Update receptionist password (receptionist / Reception@123)
UPDATE Users
SET Password = 'AQAAAAEAACcQAAAAEGNPvfDftRCb9KzZfZ1SY/MTlWizzjTSg5jJU+rfioSxuiI6zGuM7eig6pTSGwA/wA=='
WHERE Username = 'receptionist';
GO

-- Update agent password (agent / Agent@123)
UPDATE Users
SET Password = 'AQAAAAEAACcQAAAAENph9vVx2/nmQX9QOMnB873CqZ0eqF3BLqmUWWAIH+tF7e7a++3+M/qzOmycZuOm5A=='
WHERE Username = 'agent';
GO

-- Update manager password (manager / Manager@123)
UPDATE Users
SET Password = 'AQAAAAEAACcQAAAAEHmO0pzsZ30bEXO/tLP08AJaiqnBkXn7xMFbyHoEos+dTJZSRzrhhuvWPHKTVTZ7/w=='
WHERE Username = 'manager';
GO

-- Reset failed login attempts and lockout for all users
UPDATE Users
SET FailedLoginAttempts = 0,
    LockoutEndDate = NULL,
    IsActive = 1;
GO

PRINT 'All passwords updated successfully to new PasswordHasher format.';
GO
