-- Create LoginActivities table in existing database
USE HotelReservationSystem;
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'LoginActivities')
BEGIN
    CREATE TABLE LoginActivities (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Username NVARCHAR(50) NOT NULL,
        Success BIT NOT NULL,
        ErrorMessage NVARCHAR(255),
        IpAddress NVARCHAR(45),
        Timestamp DATETIME2 NOT NULL DEFAULT GETDATE(),
        UserRole NVARCHAR(20)
    );
    
    PRINT 'LoginActivities table created successfully.';
END
ELSE
BEGIN
    PRINT 'LoginActivities table already exists.';
END
GO

-- Create indexes for performance
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_LoginActivity_Timestamp' AND object_id = OBJECT_ID('LoginActivities'))
BEGIN
    CREATE INDEX IX_LoginActivity_Timestamp ON LoginActivities(Timestamp);
    PRINT 'Index IX_LoginActivity_Timestamp created.';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_LoginActivity_Username' AND object_id = OBJECT_ID('LoginActivities'))
BEGIN
    CREATE INDEX IX_LoginActivity_Username ON LoginActivities(Username);
    PRINT 'Index IX_LoginActivity_Username created.';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_LoginActivity_Success' AND object_id = OBJECT_ID('LoginActivities'))
BEGIN
    CREATE INDEX IX_LoginActivity_Success ON LoginActivities(Success);
    PRINT 'Index IX_LoginActivity_Success created.';
END
GO

PRINT 'LoginActivities table setup completed.';
GO
