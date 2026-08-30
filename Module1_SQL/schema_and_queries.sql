/* =========================================================
   AVMLabs - Lab Client Management System
   Module 1: SQL Schema Design & Queries (T-SQL / SQL Server)
   ========================================================= */

/* ---------- 1.1 SCHEMA DESIGN ---------- */

IF OBJECT_ID('dbo.WorkOrderItems', 'U') IS NOT NULL DROP TABLE dbo.WorkOrderItems;
IF OBJECT_ID('dbo.Invoices', 'U') IS NOT NULL DROP TABLE dbo.Invoices;
IF OBJECT_ID('dbo.WorkOrders', 'U') IS NOT NULL DROP TABLE dbo.WorkOrders;
IF OBJECT_ID('dbo.Tests', 'U') IS NOT NULL DROP TABLE dbo.Tests;
IF OBJECT_ID('dbo.Clients', 'U') IS NOT NULL DROP TABLE dbo.Clients;
GO

CREATE TABLE dbo.Clients (
    ClientId        INT             IDENTITY(1,1)   PRIMARY KEY,
    ClientName      NVARCHAR(150)   NOT NULL,
    ContactPerson   NVARCHAR(100)   NULL,
    Phone           NVARCHAR(20)    NULL,
    Email           NVARCHAR(150)   NULL,
    City            NVARCHAR(100)   NULL,
    Country         NVARCHAR(100)   NULL,
    IsActive        BIT             NOT NULL DEFAULT 1
);
GO

CREATE TABLE dbo.Tests (
    TestId          INT             IDENTITY(1,1)   PRIMARY KEY,
    TestCode        NVARCHAR(20)    NOT NULL UNIQUE,
    TestName        NVARCHAR(150)   NOT NULL,
    SampleType      NVARCHAR(50)    NOT NULL,
    Rate            DECIMAL(10,2)   NOT NULL,
    IsActive        BIT             NOT NULL DEFAULT 1
);
GO

CREATE TABLE dbo.WorkOrders (
    WOId            INT             IDENTITY(1,1)   PRIMARY KEY,
    ClientId        INT             NOT NULL,
    WODate          DATETIME2       NOT NULL DEFAULT SYSUTCDATETIME(),
    Status          NVARCHAR(20)    NOT NULL DEFAULT 'Pending', -- Pending / Completed
    TotalAmount     DECIMAL(12,2)   NOT NULL DEFAULT 0,
    CONSTRAINT FK_WorkOrders_Clients FOREIGN KEY (ClientId)
        REFERENCES dbo.Clients(ClientId),
    CONSTRAINT CK_WorkOrders_Status CHECK (Status IN ('Pending','Completed'))
);
GO

CREATE TABLE dbo.WorkOrderItems (
    WOItemId        INT             IDENTITY(1,1)   PRIMARY KEY,
    WOId            INT             NOT NULL,
    TestId          INT             NOT NULL,
    Quantity        INT             NOT NULL DEFAULT 1,
    Rate            DECIMAL(10,2)   NOT NULL,
    Amount          AS (Quantity * Rate) PERSISTED,
    CONSTRAINT FK_WorkOrderItems_WorkOrders FOREIGN KEY (WOId)
        REFERENCES dbo.WorkOrders(WOId),
    CONSTRAINT FK_WorkOrderItems_Tests FOREIGN KEY (TestId)
        REFERENCES dbo.Tests(TestId)
);
GO

CREATE TABLE dbo.Invoices (
    InvoiceId       INT             IDENTITY(1,1)   PRIMARY KEY,
    ClientId        INT             NOT NULL,
    InvoiceDate     DATETIME2       NOT NULL DEFAULT SYSUTCDATETIME(),
    TotalAmount     DECIMAL(12,2)   NOT NULL DEFAULT 0,
    Status          NVARCHAR(20)    NOT NULL DEFAULT 'Pending', -- Pending / Paid
    CONSTRAINT FK_Invoices_Clients FOREIGN KEY (ClientId)
        REFERENCES dbo.Clients(ClientId),
    CONSTRAINT CK_Invoices_Status CHECK (Status IN ('Pending','Paid'))
);
GO

/* ---------- Seed data (for testing queries locally) ---------- */

INSERT INTO dbo.Clients (ClientName, ContactPerson, Phone, Email, City, Country, IsActive) VALUES
('Al Noor Hospital', 'Ahmed Khalid', '+971500000001', 'ahmed@alnoor.ae', 'Dubai', 'UAE', 1),
('Apollo Diagnostics', 'Priya Menon', '+919840000002', 'priya@apollo.in', 'Chennai', 'India', 1),
('Gulf Care Clinic', 'Sara Ali', '+96550000003', 'sara@gulfcare.qa', 'Doha', 'Qatar', 1),
('City Health Labs', 'Rahul Verma', '+919820000004', 'rahul@cityhealth.in', 'Mumbai', 'India', 0);

INSERT INTO dbo.Tests (TestCode, TestName, SampleType, Rate, IsActive) VALUES
('CBC001', 'Complete Blood Count', 'Blood', 15.00, 1),
('LFT001', 'Liver Function Test', 'Blood', 25.00, 1),
('KFT001', 'Kidney Function Test', 'Blood', 25.00, 1),
('URN001', 'Urine Routine', 'Urine', 10.00, 1),
('THY001', 'Thyroid Profile', 'Blood', 30.00, 1);

-- WorkOrders inserted with TotalAmount = 0 first, then updated after items are inserted
-- (this mirrors how the API will calculate totals from items).
INSERT INTO dbo.WorkOrders (ClientId, WODate, Status, TotalAmount) VALUES
(1, '2026-08-01', 'Completed', 0),
(2, '2026-08-05', 'Pending', 0),
(3, '2026-08-10', 'Completed', 0);

INSERT INTO dbo.WorkOrderItems (WOId, TestId, Quantity, Rate) VALUES
(1, 1, 1, 15.00),   -- WO1: CBC
(1, 2, 1, 25.00),   -- WO1: LFT   => WO1 total = 40.00
(2, 3, 1, 25.00),   -- WO2: KFT
(2, 4, 1, 10.00),   -- WO2: Urine => WO2 total = 35.00
(3, 5, 1, 30.00);   -- WO3: Thyroid => WO3 total = 30.00

UPDATE wo
SET wo.TotalAmount = agg.SumAmount
FROM dbo.WorkOrders wo
JOIN (
    SELECT WOId, SUM(Amount) AS SumAmount
    FROM dbo.WorkOrderItems
    GROUP BY WOId
) agg ON agg.WOId = wo.WOId;

INSERT INTO dbo.Invoices (ClientId, InvoiceDate, TotalAmount, Status) VALUES
(1, '2026-08-02', 55.00, 'Paid'),
(2, '2026-08-06', 40.00, 'Pending'),
(2, '2026-08-15', 20.00, 'Pending'),
(3, '2026-08-11', 30.00, 'Paid');

GO

/* =========================================================
   1.2 SQL QUERIES
   ========================================================= */

-- Query 1 (5 Marks)
-- List all active clients with ClientName, City, Country, ordered alphabetically by ClientName.
SELECT ClientName, City, Country
FROM dbo.Clients
WHERE IsActive = 1
ORDER BY ClientName ASC;
GO

-- Query 2 (5 Marks)
-- For each client, total amount of all their Work Orders.
SELECT
    c.ClientName,
    SUM(wo.TotalAmount) AS TotalWorkOrderAmount
FROM dbo.Clients c
JOIN dbo.WorkOrders wo ON wo.ClientId = c.ClientId
GROUP BY c.ClientName;
GO

-- Query 3 (5 Marks)
-- List all Work Order Items along with TestName and ClientName they belong to.
SELECT
    woi.WOItemId,
    t.TestName,
    c.ClientName,
    woi.Quantity,
    woi.Rate,
    woi.Amount
FROM dbo.WorkOrderItems woi
JOIN dbo.WorkOrders wo ON wo.WOId = woi.WOId
JOIN dbo.Clients c ON c.ClientId = wo.ClientId
JOIN dbo.Tests t ON t.TestId = woi.TestId;
GO

-- Query 4 (5 Marks)
-- Find all clients with at least one Invoice with Status = 'Pending'.
-- Show ClientName and the count of pending invoices.
SELECT
    c.ClientName,
    COUNT(i.InvoiceId) AS PendingInvoiceCount
FROM dbo.Clients c
JOIN dbo.Invoices i ON i.ClientId = c.ClientId
WHERE i.Status = 'Pending'
GROUP BY c.ClientName;
GO
