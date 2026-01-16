USE [TourAgencyDB]
GO

-- Очистка данных
DELETE FROM Applications;
DELETE FROM Tours;
DELETE FROM BusTypes;
DELETE FROM Countries;
DELETE FROM Users;
DELETE FROM ApplicationStatuses;
DELETE FROM Roles;
GO

-- Сброс идентификаторов
DBCC CHECKIDENT ('Roles', RESEED, 0);
DBCC CHECKIDENT ('ApplicationStatuses', RESEED, 0);
DBCC CHECKIDENT ('Countries', RESEED, 0);
DBCC CHECKIDENT ('BusTypes', RESEED, 0);
DBCC CHECKIDENT ('Users', RESEED, 0);
DBCC CHECKIDENT ('Tours', RESEED, 0);
DBCC CHECKIDENT ('Applications', RESEED, 0);
GO

-- 1. Роли
INSERT INTO Roles (RoleName) VALUES 
(N'Администратор'),
(N'Менеджер'),
(N'Авторизированный клиент');
GO

-- 2. Статусы
INSERT INTO ApplicationStatuses (StatusName) VALUES 
(N'Новая'),
(N'В обработке'),
(N'Подтверждена'),
(N'Отменена');
GO

-- 3. Страны
INSERT INTO Countries (CountryName) VALUES 
(N'Италия'),
(N'Франция'),
(N'Германия'),
(N'Австрия'),
(N'Чехия'),
(N'Испания'),
(N'Польша');
GO

-- 4. Типы автобусов
INSERT INTO BusTypes (TypeName, Description, Capacity) VALUES
(N'Стандарт', N'Автобус туристического класса', 45),
(N'Комфорт', N'Автобус повышенной комфортности', 35),
(N'Минивэн', N'Для небольших групп', 16);
GO

-- 5. Пользователи
INSERT INTO Users (RoleID, FullName, Login, PasswordHash) VALUES
(1, N'Петров Иван Сергеевич', 'admin@globus.ru', '7f8d2a'),
(2, N'Сидорова Анна Владимировна', 'manager@globus.ru', '9k3l5m'),
(2, N'Козлов Дмитрий Алексеевич', 'd.kozlov@globus.ru', '4p6q8r'),
(3, N'Иванова Мария Петровна', 'm.ivanova@mail.ru', '1a2b3c'),
(3, N'Смирнов Алексей Викторович', 'a.smirnov@yandex.ru', '5d6e7f'),
(3, N'Новикова Елена Олеговна', 'e.novikova@gmail.com', '8g9h0i'),
(3, N'Волков Павел Александрович', 'p.volkov@mail.ru', 'k2l3m4'),
(3, N'Крылова Ольга Сергеевна', 'o.krylova@yandex.ru', 'n5o6p7'),
(3, N'Борисов Артем Игоревич', 'a.borisov@gmail.com', 'q8r9s0'),
(3, N'Зотова Виктория Дмитриевна', 'v.zotova@mail.ru', 't1u2v3'),
(3, N'Громов Михаил Петрович', 'm.gromov@yandex.ru', 'w4x5y6'),
(1, N'Морозова Анастасия Андреевна', 'a.morozova@globus.ru', 'z7a8b9'),
(2, N'Тихонов Сергей Владимирович', 's.tikhonov@globus.ru', 'c0d1e2');
GO

-- 6. Туры
INSERT INTO Tours (Title, CountryID, DurationDays, StartDate, Price, Discount, BusTypeID, Capacity, AvailableSeats, PhotoFileName) VALUES
(N'Романтическая Италия: Рим, Флоренция, Венеция', 1, 10, '2024-06-15', 85000.00, 15.00, 2, 35, 12, 'italy.jpg'),
(N'Париж и замки Луары', 2, 7, '2024-07-22', 92500.00, 10.00, 1, 45, 5, 'france.jpg'),
(N'Австрийские Альпы: Зальцбург и Инсбрук', 4, 8, '2024-08-10', 78300.00, 5.00, 2, 35, 20, 'austria.jpg'),
(N'Берлин, Дрезден, Мюнхен', 3, 9, '2024-09-05', 88900.00, 20.00, 1, 45, 0, 'germany.jpg'),
(N'Прага и Карловы Вары', 5, 6, '2024-10-12', 65400.00, 0.00, 3, 16, 10, 'czech.jpg'),
(N'Мадрид и Барселона', 6, 8, '2024-08-20', 91000.00, 12.00, 1, 45, 18, 'spain.jpg'),
(N'Краков и Варшава', 7, 5, '2024-07-05', 52000.00, 25.00, 3, 16, 8, 'poland.jpg'),
(N'Вена и Будапешт', 4, 7, '2024-09-15', 74500.00, 8.00, 2, 35, 25, 'vienna.jpg'),
(N'Амстердам и Брюссель', 1, 6, '2024-10-25', 69000.00, 0.00, 1, 45, 15, 'amsterdam.jpg'),
(N'Стокгольм и Хельсинки', 1, 9, '2024-08-12', 105000.00, 30.00, 2, 35, 6, 'sweden.jpg');
GO

-- 7. Заявки
INSERT INTO Applications (TourID, ClientID, ApplicationDate, StatusID, PersonsCount, TotalPrice, Comment) VALUES
(1, 4, '2024-05-10', 3, 2, 170000.00, N'Пожелание: номера рядом'),
(2, 5, '2024-05-12', 1, 1, 92500.00, NULL),
(3, 6, '2024-05-15', 3, 4, 313200.00, N'Юбилейная поездка'),
(5, 4, '2024-05-20', 1, 2, 130800.00, NULL),
(6, 7, '2024-05-22', 3, 2, 182000.00, NULL),
(7, 8, '2024-05-25', 2, 1, 52000.00, NULL),
(8, 9, '2024-05-28', 3, 3, 223500.00, N'Поздний заезд'),
(9, 10, '2024-05-30', 1, 4, 276000.00, NULL),
(10, 4, '2024-06-01', 3, 2, 210000.00, N'VIP-обслуживание'),
(3, 11, '2024-06-03', 4, 5, 391500.00, NULL),
(4, 12, '2024-06-05', 1, 1, 88900.00, NULL),
(2, 7, '2024-06-07', 3, 3, 277500.00, NULL),
(5, 8, '2024-06-10', 2, 2, 130800.00, NULL),
(1, 9, '2024-06-12', 1, 4, 340000.00, N'Семейный тур');
GO

-- Проверка
SELECT 'Roles' as TableName, COUNT(*) as Count FROM Roles
UNION ALL SELECT 'ApplicationStatuses', COUNT(*) FROM ApplicationStatuses
UNION ALL SELECT 'Countries', COUNT(*) FROM Countries
UNION ALL SELECT 'BusTypes', COUNT(*) FROM BusTypes
UNION ALL SELECT 'Users', COUNT(*) FROM Users
UNION ALL SELECT 'Tours', COUNT(*) FROM Tours
UNION ALL SELECT 'Applications', COUNT(*) FROM Applications;
GO