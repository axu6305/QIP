USE QipWardDb;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Patients WHERE HospitalNumber = N'PX-1001')
BEGIN
    INSERT INTO dbo.Patients (Surname, Initials, HospitalNumber, DateOfBirth, Sex)
    VALUES
        (N'Adams', N'J', N'PX-1001', '1980-03-11', 1),
        (N'Brown', N'M', N'PX-1002', '1992-07-24', 2),
        (N'Clark', N'T', N'PX-1003', '1975-12-02', 1);
END
GO

DECLARE @Today DATE = CAST(GETDATE() AS DATE);

INSERT INTO dbo.WardPresences (PatientId, [Date])
SELECT p.Id, DATEADD(DAY, offsets.DayOffset, @Today)
FROM dbo.Patients p
JOIN (VALUES
    (-4), (-3), (-2),
    (0),
    (1)
) AS offsets(DayOffset)
    ON p.HospitalNumber = N'PX-1001'
WHERE NOT EXISTS (
    SELECT 1
    FROM dbo.WardPresences wp
    WHERE wp.PatientId = p.Id
      AND wp.[Date] = DATEADD(DAY, offsets.DayOffset, @Today)
);

INSERT INTO dbo.WardPresences (PatientId, [Date])
SELECT p.Id, DATEADD(DAY, offsets.DayOffset, @Today)
FROM dbo.Patients p
JOIN (VALUES
    (-5), (-2), (-1),
    (0)
) AS offsets(DayOffset)
    ON p.HospitalNumber = N'PX-1002'
WHERE NOT EXISTS (
    SELECT 1
    FROM dbo.WardPresences wp
    WHERE wp.PatientId = p.Id
      AND wp.[Date] = DATEADD(DAY, offsets.DayOffset, @Today)
);

INSERT INTO dbo.WardPresences (PatientId, [Date])
SELECT p.Id, DATEADD(DAY, offsets.DayOffset, @Today)
FROM dbo.Patients p
JOIN (VALUES
    (-1),
    (0),
    (2)
) AS offsets(DayOffset)
    ON p.HospitalNumber = N'PX-1003'
WHERE NOT EXISTS (
    SELECT 1
    FROM dbo.WardPresences wp
    WHERE wp.PatientId = p.Id
      AND wp.[Date] = DATEADD(DAY, offsets.DayOffset, @Today)
);
GO
