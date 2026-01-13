
CREATE TABLE Roles (
    Id INT IDENTITY PRIMARY KEY,
    Role NVARCHAR(500) NOT NULL UNIQUE
);

CREATE TABLE Countries (
    Id INT IDENTITY PRIMARY KEY,
    CountryName NVARCHAR(500) NOT NULL UNIQUE,
    CountryCode NVARCHAR(5) NOT NULL UNIQUE
);


CREATE TABLE Users (
    Id INT IDENTITY PRIMARY KEY,
    Username NVARCHAR(2000) NOT NULL,
    Email NVARCHAR(2000) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(4000) NOT NULL,
    Salt NVARCHAR(4000) NOT NULL,
    RoleId INT NOT NULL,
    CountryId INT NULL,
    Verified BIT DEFAULT 0,
    FOREIGN KEY (CountryId) REFERENCES Countries(Id),
    FOREIGN KEY(RoleId) REFERENCES Roles(Id)
);


CREATE TABLE Performers (
    Id INT IDENTITY PRIMARY KEY,
    Name NVARCHAR(2000) NOT NULL,
    Description NVARCHAR(MAX) not null,
    CountryId INT NULL,
    Avatar NVARCHAR(4000) NULL,
    CreatedAt DATETIME DEFAULT GETDATE()
    FOREIGN KEY (CountryId) REFERENCES Countries(Id)
    
);
CREATE TABLE PerformerGenres (
    PerformerId INT NOT NULL,
    GenreId INT NOT NULL,
    PRIMARY KEY (PerformerId, GenreId),
    FOREIGN KEY (PerformerId) REFERENCES Performers(Id),
    FOREIGN KEY (GenreId) REFERENCES Genres(Id),

);

CREATE TABLE EventTypes (
    Id INT IDENTITY PRIMARY KEY,
    Type NVARCHAR(2000) NOT NULL UNIQUE 
);

Create table Genres (
    Id INT IDENTITY PRIMARY KEY,
    Genre NVARCHAR(2000) NOT NULL UNIQUE
);


CREATE TABLE Events (
    Id INT IDENTITY PRIMARY KEY,
    OrganizerId INT NOT NULL,
    TypeId INT NOT NULL,
    Title NVARCHAR(2000) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    -- place
    Latitude DECIMAL(9,6) NOT NULL,
    Longitude DECIMAL(9,6) NOT NULL,
    Address NVARCHAR(1000),
    City NVARCHAR(1000),
    CountryId INT NOT NULL,
    Poster NvarChar(max) null,
    ----------------------
    StartDate DATETIME NOT NULL,
    EndDate DATETIME NULL,
    TicketPrice DECIMAL(10,2) NULL,
    TicketAmount int not null,
    -------------------------
    Status int null ,
    RejectionReason nvarchar(max) null,
    FOREIGN KEY (OrganizerId) REFERENCES Users(Id),
    FOREIGN KEY (TypeId) REFERENCES EventTypes(Id),
    FOREIGN KEY (CountryId) REFERENCES Countries(Id)
);
-- Statuses
/* Draft = 0,       // draft only org can view
Pending = 1,        // on moderation
Approved = 2,       // 
Rejected = 3,       // need to be rewrited */
     


Create table EventGenres(
    EventId Int NOT NULL,
    GenreId Int not null,
    PRIMARY KEY (EventId, GenreId),
    FOREIGN KEY (EventId) REFERENCES Events(Id),
    FOREIGN KEY (GenreId) REFERENCES Genres(Id),
)


CREATE TABLE EventPerformers (
    EventId INT NOT NULL,
    PerformerId INT NOT NULL,
    PRIMARY KEY (EventId, PerformerId),
    FOREIGN KEY (EventId) REFERENCES Events(Id),
    FOREIGN KEY (PerformerId) REFERENCES Performers(Id)
);


CREATE TABLE Tickets (
    Id INT IDENTITY PRIMARY KEY,
    EventId INT NOT NULL,
    UserId INT NOT NULL,
    PurchaseDate DATETIME DEFAULT GETDATE(),
    Price DECIMAL(10,2) NOT NULL,
    FOREIGN KEY (EventId) REFERENCES Events(Id),
    FOREIGN KEY (UserId) REFERENCES Users(Id)
);


Create table WishList (
    UserId INT NOT NULL,
    EventId INT NOT NULL,
    PRIMARY KEY (UserId, EventId),
    FOREIGN KEY (EventId) REFERENCES Events(Id),
    FOREIGN KEY (UserId) REFERENCES Users(Id),
)


CREATE TABLE Reviews (
    Id INT IDENTITY PRIMARY KEY,
    UserId INT NOT NULL,
    EventId INT NOT NULL,
    IsLike BIT NOT NULL, -- 1-like, 0 -dislike
    Comment NVARCHAR(MAX) NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    
    FOREIGN KEY (UserId) REFERENCES Users(Id),
    FOREIGN KEY (EventId) REFERENCES Events(Id),
    CONSTRAINT UK_User_Event_Review UNIQUE (UserId, EventId) 
);


SELECT 
    fk.name AS ForeignKeyName,
    OBJECT_NAME(fk.parent_object_id) AS TableName,
    COL_NAME(fk.parent_object_id, fkc.parent_column_id) AS ColumnName, 
    OBJECT_NAME(fk.referenced_object_id) AS ReferencedTableName,
    COL_NAME(fk.referenced_object_id, fkc.referenced_column_id) AS ReferencedColumnName 
FROM sys.foreign_keys AS fk
INNER JOIN sys.foreign_key_columns AS fkc
    ON fk.object_id = fkc.constraint_object_id
WHERE OBJECT_NAME(fk.parent_object_id) IN ('EventGenres', 'EventPerformers', 'RolePermissions');


ALTER TABLE EventGenres
DROP CONSTRAINT FK__EventGenr__Event__45F365D3;    
ALTER TABLE EventGenres
DROP CONSTRAINT FK__EventGenr__Genre__46E78A0C;     

ALTER TABLE EventGenres
ADD CONSTRAINT FK_EventGenres_Events_EventId
FOREIGN KEY (EventId) REFERENCES Events(Id)
ON DELETE CASCADE;

ALTER TABLE EventGenres
ADD CONSTRAINT FK_EventGenres_Genres_GenreId
FOREIGN KEY (GenreId) REFERENCES Genres(Id)
ON DELETE CASCADE;


-- query for organizer stats 

SELECT
    U.Username AS OrganizerName,
    COUNT(T.Id) AS TotalTicketsSold,
    SUM(T.Price) AS TotalNetRevenue
FROM Users U
JOIN Events E ON U.Id = E.OrganizerId
JOIN Tickets T ON E.Id = T.EventId
WHERE U.Id = 2 
GROUP BY U.Username;



SELECT
    E.Id AS EventId,
    E.Title AS EventTitle,
    ET.Type AS EventType,
    E.StartDate,
    COUNT(T.Id) AS TicketsSold,
    SUM(T.Price) AS NetRevenue
FROM Events E
JOIN Users U ON E.OrganizerId = U.Id
JOIN Tickets T ON E.Id = T.EventId
JOIN EventTypes ET ON E.TypeId = ET.Id
WHERE U.Id = 2 
GROUP BY E.Id, E.Title, ET.Type, E.StartDate
ORDER BY E.StartDate DESC;


SELECT
    FORMAT(T.CreatedAt, 'yyyy-MM') AS SaleMonth, 
    
    COUNT(T.Id) AS TicketsSold,
    SUM(T.Price) AS MonthlyNetRevenue
FROM Users U
JOIN Events E ON U.Id = E.OrganizerId
JOIN Tickets T ON E.Id = T.EventId
WHERE U.Id = 2 
GROUP BY FORMAT(T.CreatedAt, 'yyyy-MM')
ORDER BY SaleMonth;

SELECT
    C.CountryName,
    COUNT(T.Id) AS TicketsSold,
    SUM(T.Price) AS NetRevenue
FROM Users U_Organizer
JOIN Events E ON U_Organizer.Id = E.OrganizerId
JOIN Tickets T ON E.Id = T.EventId
JOIN Users U_Buyer ON T.UserId = U_Buyer.Id 
JOIN Countries C ON U_Buyer.CountryId = C.Id
WHERE U_Organizer.Id = 2 
GROUP BY C.CountryName
ORDER BY TicketsSold DESC;


SELECT
    E.Id AS EventId,
    E.Title AS EventTitle,
    E.TicketAmount AS TotalTicketsAvailable,
    COUNT(T.Id) AS TicketsSold,
    CAST(COUNT(T.Id) AS DECIMAL(10,2)) / NULLIF(E.TicketAmount, 0) * 100 AS PercentageSold,
    AVG(T.Price) AS AverageTicketPrice,
    SUM(T.Price) AS NetRevenue
FROM Events E
LEFT JOIN Tickets T ON E.Id = T.EventId
WHERE E.OrganizerId = 2 
GROUP BY E.Id, E.Title, E.TicketAmount
ORDER BY
    MAX(E.StartDate) DESC;


--  changed trigger 

-- for tickets table 
USE [GlobalFests]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER TRIGGER [dbo].[ReduceTicketAmount]
ON [dbo].[Tickets]
AFTER INSERT
AS
BEGIN

    SET NOCOUNT ON;
    UPDATE e
    SET e.TicketAmount = e.TicketAmount - i.TicketCount
    FROM Events e
    INNER JOIN (
        SELECT EventId, COUNT(*) as TicketCount
        FROM inserted
        GROUP BY EventId
    ) i ON e.Id = i.EventId;
    IF EXISTS (
        SELECT 1 
        FROM Events e
        INNER JOIN inserted i ON e.Id = i.EventId
        WHERE e.TicketAmount < 0
    )
    BEGIN
        RAISERROR ('Transaction failed: Not enough tickets available.', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
END



USE [GlobalFests]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER TRIGGER [dbo].[RestoreTicketAmount]
ON [dbo].[Tickets]
AFTER DELETE
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE e
    SET e.TicketAmount = e.TicketAmount + d.TicketCount
    FROM Events e
    INNER JOIN (
        SELECT EventId, COUNT(*) as TicketCount
        FROM deleted
        GROUP BY EventId
    ) d ON e.Id = d.EventId;
END
-- =============================================================================