

-- 1. Создаём новую БД
CREATE DATABASE TourAgencyDB;
GO

USE TourAgencyDB;
GO

-- 2. Таблица Стран (из Страны_import)
CREATE TABLE Countries (
    CountryID INT PRIMARY KEY IDENTITY(1,1),
    CountryCode INT NOT NULL UNIQUE,  -- Код страны из импорта
    CountryName NVARCHAR(100) NOT NULL UNIQUE
);
GO

-- 3. Таблица Типов автобусов (из Типы_автобусов_import)
CREATE TABLE BusTypes (
    BusTypeID INT PRIMARY KEY IDENTITY(1,1),
    TypeCode INT NOT NULL UNIQUE,      -- Код типа из импорта
    TypeName NVARCHAR(50) NOT NULL UNIQUE,
    Description NVARCHAR(255),
    Capacity INT NOT NULL CHECK (Capacity > 0)
);
GO

-- 4. Таблица Ролей (из Пользователи_import)
CREATE TABLE Roles (
    RoleID INT PRIMARY KEY IDENTITY(1,1),
    RoleName NVARCHAR(50) NOT NULL UNIQUE
);
GO

-- 5. Таблица Пользователей (из Пользователи_import)
CREATE TABLE Users (
    UserID INT PRIMARY KEY IDENTITY(1,1),
    RoleID INT NOT NULL,
    FullName NVARCHAR(200) NOT NULL,
    Login NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    Email NVARCHAR(255),
    Phone NVARCHAR(20),
    CreatedAt DATETIME DEFAULT GETDATE(),
    -- Внешние ключи
    FOREIGN KEY (RoleID) REFERENCES Roles(RoleID) ON DELETE CASCADE,
    -- Проверка email
    CHECK (Email LIKE '%@%.%')
);
GO

-- 6. Таблица Туров (из Туры_import)
CREATE TABLE Tours (
    TourID INT PRIMARY KEY IDENTITY(1,1),
    TourCode INT NOT NULL UNIQUE,          -- Код тура из импорта
    Title NVARCHAR(200) NOT NULL,
    CountryID INT NOT NULL,
    DurationDays INT NOT NULL CHECK (DurationDays > 0),
    StartDate DATE NOT NULL,
    Price DECIMAL(10,2) NOT NULL CHECK (Price > 0),
    BusTypeID INT NOT NULL,
    Capacity INT NOT NULL CHECK (Capacity > 0),
    AvailableSeats INT NOT NULL DEFAULT 0,
    PhotoFileName NVARCHAR(255),
    IsActive BIT DEFAULT 1,
    -- Внешние ключи
    FOREIGN KEY (CountryID) REFERENCES Countries(CountryID) ON DELETE CASCADE,
    FOREIGN KEY (BusTypeID) REFERENCES BusTypes(BusTypeID),
    -- Проверки
    CHECK (AvailableSeats <= Capacity),
    CHECK (AvailableSeats >= 0)
);
GO

-- 7. Таблица Статусов заявок (из Заявки_import)
CREATE TABLE ApplicationStatuses (
    StatusID INT PRIMARY KEY IDENTITY(1,1),
    StatusName NVARCHAR(50) NOT NULL UNIQUE
);
GO

-- 8. Таблица Заявок (из Заявки_import)
CREATE TABLE Applications (
    ApplicationID INT PRIMARY KEY IDENTITY(1,1),
    ApplicationCode INT NOT NULL UNIQUE,    -- Код заявки из импорта
    TourID INT NOT NULL,
    ClientID INT NOT NULL,
    ApplicationDate DATE NOT NULL DEFAULT GETDATE(),
    StatusID INT NOT NULL,
    PersonsCount INT NOT NULL CHECK (PersCount > 0),
    TotalPrice DECIMAL(10,2) NOT NULL CHECK (TotalPrice > 0),
    Comment NVARCHAR(1000),
    ManagerID INT,
    -- Внешние ключи
    FOREIGN KEY (TourID) REFERENCES Tours(TourID) ON DELETE CASCADE,
    FOREIGN KEY (ClientID) REFERENCES Users(UserID) ON DELETE CASCADE,
    FOREIGN KEY (StatusID) REFERENCES ApplicationStatuses(StatusID),
    FOREIGN KEY (ManagerID) REFERENCES Users(UserID) ON DELETE SET NULL,
    -- Проверка даты
    CHECK (ApplicationDate <= GETDATE())
);
GO

-- 9. Индексы для ускорения поиска
CREATE INDEX IX_Tours_StartDate ON Tours(StartDate);
CREATE INDEX IX_Tours_Country ON Tours(CountryID);
CREATE INDEX IX_Applications_Status ON Applications(StatusID);
CREATE INDEX IX_Applications_Client ON Applications(ClientID);
CREATE INDEX IX_Users_Login ON Users(Login);
GO

PRINT 'База данных создана успешно!';