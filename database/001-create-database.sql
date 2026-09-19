IF DB_ID(N'QipWardDb') IS NULL
BEGIN
    CREATE DATABASE QipWardDb;
END
GO

USE QipWardDb;
GO

IF OBJECT_ID(N'dbo.Patients', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Patients
    (
        Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Patients PRIMARY KEY,
        Surname NVARCHAR(100) NOT NULL,
        Initials NVARCHAR(50) NOT NULL,
        HospitalNumber NVARCHAR(50) NOT NULL,
        DateOfBirth DATE NOT NULL,
        Sex INT NOT NULL,
        CONSTRAINT UQ_Patients_HospitalNumber UNIQUE (HospitalNumber)
    );
END
GO

IF OBJECT_ID(N'dbo.WardPresences', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.WardPresences
    (
        Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_WardPresences PRIMARY KEY,
        PatientId INT NOT NULL,
        [Date] DATE NOT NULL,
        CONSTRAINT FK_WardPresences_Patients FOREIGN KEY (PatientId)
            REFERENCES dbo.Patients (Id)
            ON DELETE CASCADE,
        CONSTRAINT UQ_WardPresences_Patient_Date UNIQUE (PatientId, [Date])
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_WardPresences_Date' AND object_id = OBJECT_ID(N'dbo.WardPresences'))
BEGIN
    CREATE INDEX IX_WardPresences_Date ON dbo.WardPresences([Date]);
END
GO
