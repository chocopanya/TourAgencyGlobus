USE [TourAgencyDB]
GO

-- Удаление старых таблиц
IF OBJECT_ID('Applications', 'U') IS NOT NULL DROP TABLE Applications;
IF OBJECT_ID('Tours', 'U') IS NOT NULL DROP TABLE Tours;
IF OBJECT_ID('BusTypes', 'U') IS NOT NULL DROP TABLE BusTypes;
IF OBJECT_ID('Countries', 'U') IS NOT NULL DROP TABLE Countries;
IF OBJECT_ID('Users', 'U') IS NOT NULL DROP TABLE Users;
IF OBJECT_ID('ApplicationStatuses', 'U') IS NOT NULL DROP TABLE ApplicationStatuses;
IF OBJECT_ID('Roles', 'U') IS NOT NULL DROP TABLE Roles;
GO

-- 1. Роли пользователей
CREATE TABLE Roles (
    RoleID INT PRIMARY KEY IDENTITY(1,1),
    RoleName NVARCHAR(50) NOT NULL
);
GO

-- 2. Статусы заявок
CREATE TABLE ApplicationStatuses (
    StatusID INT PRIMARY KEY IDENTITY(1,1),
    StatusName NVARCHAR(50) NOT NULL
);
GO

-- 3. Страны
CREATE TABLE Countries (
    CountryID INT PRIMARY KEY IDENTITY(1,1),
    CountryName NVARCHAR(100) NOT NULL
);
GO

-- 4. Типы автобусов
CREATE TABLE BusTypes (
    BusTypeID INT PRIMARY KEY IDENTITY(1,1),
    TypeName NVARCHAR(50) NOT NULL,
    Description NVARCHAR(255),
    Capacity INT NOT NULL
);
GO

-- 5. Пользователи
CREATE TABLE Users (
    UserID INT PRIMARY KEY IDENTITY(1,1),
    RoleID INT NOT NULL,
    FullName NVARCHAR(200) NOT NULL,
    Login NVARCHAR(100) NOT NULL,
    PasswordHash NVARCHAR(255) NOT NULL,
    FOREIGN KEY (RoleID) REFERENCES Roles(RoleID)
);
GO

-- 6. Туры
CREATE TABLE Tours (
    TourID INT PRIMARY KEY IDENTITY(1,1),
    Title NVARCHAR(200) NOT NULL,
    CountryID INT NOT NULL,
    DurationDays INT NOT NULL,
    StartDate DATE NOT NULL,
    Price DECIMAL(10,2) NOT NULL,
    Discount DECIMAL(5,2) NOT NULL DEFAULT 0,
    BusTypeID INT NOT NULL,
    Capacity INT NOT NULL,
    AvailableSeats INT NOT NULL DEFAULT 0,
    PhotoFileName NVARCHAR(255),
    FOREIGN KEY (CountryID) REFERENCES Countries(CountryID),
    FOREIGN KEY (BusTypeID) REFERENCES BusTypes(BusTypeID)
);
GO

-- 7. Заявки
CREATE TABLE Applications (
    ApplicationID INT PRIMARY KEY IDENTITY(1,1),
    TourID INT NOT NULL,
    ClientID INT NOT NULL,
    ApplicationDate DATE NOT NULL DEFAULT GETDATE(),
    StatusID INT NOT NULL,
    PersonsCount INT NOT NULL,
    TotalPrice DECIMAL(10,2) NOT NULL,
    Comment NVARCHAR(1000),
    FOREIGN KEY (TourID) REFERENCES Tours(TourID),
    FOREIGN KEY (ClientID) REFERENCES Users(UserID),
    FOREIGN KEY (StatusID) REFERENCES ApplicationStatuses(StatusID)
);
GO