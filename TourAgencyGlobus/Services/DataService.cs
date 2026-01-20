using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using TourAgencyGlobus.Data;
using TourAgencyGlobus.Models;

namespace TourAgencyGlobus.Services
{
    public class DataService : IDisposable
    {
        private readonly AppDbContext _context;

        public DataService()
        {
            try
            {
                _context = new AppDbContext();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка создания DataService: {ex.Message}", "Ошибка");
                throw;
            }
        }

        // === ТУРЫ ===
        public List<Tour> GetAllTours()
        {
            try
            {
                return _context.Tours
                    .Include(t => t.Country)
                    .Include(t => t.BusType)
                    .AsNoTracking()
                    .OrderBy(t => t.StartDate)
                    .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки туров: {ex.Message}", "Ошибка");
                return new List<Tour>();
            }
        }

        public Tour GetTourById(int id)
        {
            try
            {
                return _context.Tours
                    .Include(t => t.Country)
                    .Include(t => t.BusType)
                    .AsNoTracking()
                    .FirstOrDefault(t => t.Id == id);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки тура: {ex.Message}", "Ошибка");
                return null;
            }
        }

        public bool UpdateTourSeats(int tourId, int seatsChange)
        {
            try
            {
                var tour = _context.Tours.Find(tourId);
                if (tour != null)
                {
                    tour.FreeSeats += seatsChange;
                    _context.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обновления мест: {ex.Message}", "Ошибка");
                return false;
            }
        }

        public bool AddTour(Tour tour)
        {
            try
            {
                _context.Tours.Add(tour);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка добавления тура: {ex.Message}", "Ошибка");
                return false;
            }
        }

        public bool UpdateTour(Tour tour)
        {
            try
            {
                _context.Entry(tour).State = EntityState.Modified;
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обновления тура: {ex.Message}", "Ошибка");
                return false;
            }
        }

        public bool DeleteTour(int tourId)
        {
            try
            {
                var tour = _context.Tours.Find(tourId);
                if (tour != null)
                {
                    _context.Tours.Remove(tour);
                    _context.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка удаления тура: {ex.Message}", "Ошибка");
                return false;
            }
        }

        // === СТРАНЫ И ТИПЫ АВТОБУСОВ ===
        public List<Country> GetCountries()
        {
            try
            {
                return _context.Countries
                    .AsNoTracking()
                    .OrderBy(c => c.Name)
                    .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки стран: {ex.Message}", "Ошибка");
                return new List<Country>();
            }
        }

        public List<BusType> GetBusTypes()
        {
            try
            {
                return _context.BusTypes
                    .AsNoTracking()
                    .OrderBy(b => b.Name)
                    .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки типов автобусов: {ex.Message}", "Ошибка");
                return new List<BusType>();
            }
        }

        // === ПОЛЬЗОВАТЕЛИ ===
        public List<User> GetAllUsers()
        {
            try
            {
                return _context.Users
                    .AsNoTracking()
                    .OrderBy(u => u.FullName)
                    .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки пользователей: {ex.Message}", "Ошибка");
                return new List<User>();
            }
        }

        public List<User> GetClients()
        {
            try
            {
                return _context.Users
                    .Where(u => u.RoleId == 3) // Только клиенты
                    .AsNoTracking()
                    .OrderBy(u => u.FullName)
                    .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки клиентов: {ex.Message}", "Ошибка");
                return new List<User>();
            }
        }

        public User GetUserByLogin(string login)
        {
            try
            {
                return _context.Users
                    .AsNoTracking()
                    .FirstOrDefault(u => u.Login.Equals(login));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка поиска пользователя: {ex.Message}", "Ошибка");
                return null;
            }
        }

        // === ЗАЯВКИ ===
        public List<TourApplication> GetAllApplications()
        {
            try
            {
                return _context.Applications
                    .Include(a => a.Tour)
                    .Include(a => a.Client)
                    .AsNoTracking()
                    .OrderByDescending(a => a.ApplicationDate)
                    .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки заявок: {ex.Message}", "Ошибка");
                return new List<TourApplication>();
            }
        }

        public List<TourApplication> GetApplicationsFiltered(string searchText = "", string statusFilter = "Все")
        {
            try
            {
                var query = _context.Applications
                    .Include(a => a.Tour)
                    .Include(a => a.Client)
                    .AsNoTracking();

                // Фильтрация по поиску
                if (!string.IsNullOrEmpty(searchText))
                {
                    query = query.Where(a =>
                        a.Id.ToString().Contains(searchText) ||
                        a.Client.FullName.Contains(searchText) ||
                        a.Tour.Name.Contains(searchText));
                }

                // Фильтрация по статусу
                if (statusFilter != "Все")
                {
                    int statusId = statusFilter switch
                    {
                        "Новая" => 1,
                        "В обработке" => 2,
                        "Подтверждена" => 3,
                        "Отменена" => 4,
                        _ => 0
                    };

                    if (statusId > 0)
                    {
                        query = query.Where(a => a.StatusId == statusId);
                    }
                }

                return query.OrderByDescending(a => a.ApplicationDate).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка фильтрации заявок: {ex.Message}", "Ошибка");
                return new List<TourApplication>();
            }
        }

        public TourApplication GetApplicationById(int id)
        {
            try
            {
                return _context.Applications
                    .Include(a => a.Tour)
                    .Include(a => a.Client)
                    .AsNoTracking()
                    .FirstOrDefault(a => a.Id == id);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки заявки: {ex.Message}", "Ошибка");
                return null;
            }
        }

        public bool AddApplication(TourApplication application)
        {
            try
            {
                _context.Applications.Add(application);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка добавления заявки: {ex.Message}", "Ошибка");
                return false;
            }
        }

        public bool UpdateApplication(TourApplication application)
        {
            try
            {
                var existingApp = _context.Applications.Find(application.Id);
                if (existingApp != null)
                {
                    // Проверяем изменение статуса на "Подтверждена"
                    if (existingApp.StatusId != 3 && application.StatusId == 3)
                    {
                        var tour = _context.Tours.Find(application.TourId);
                        if (tour != null && tour.FreeSeats < application.NumberOfPeople)
                        {
                            MessageBox.Show($"Недостаточно свободных мест! Доступно: {tour.FreeSeats}, требуется: {application.NumberOfPeople}", "Ошибка");
                            return false;
                        }

                        // Уменьшаем количество свободных мест
                        tour.FreeSeats -= application.NumberOfPeople;
                    }
                    // Если статус меняется с "Подтверждена" на другой
                    else if (existingApp.StatusId == 3 && application.StatusId != 3)
                    {
                        var tour = _context.Tours.Find(application.TourId);
                        if (tour != null)
                        {
                            // Возвращаем места
                            tour.FreeSeats += existingApp.NumberOfPeople;
                        }
                    }

                    _context.Entry(existingApp).CurrentValues.SetValues(application);
                    _context.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обновления заявки: {ex.Message}", "Ошибка");
                return false;
            }
        }

        public bool DeleteApplication(int applicationId)
        {
            try
            {
                var application = _context.Applications.Find(applicationId);
                if (application != null)
                {
                    // Если удаляем подтвержденную заявку, возвращаем места
                    if (application.StatusId == 3)
                    {
                        var tour = _context.Tours.Find(application.TourId);
                        if (tour != null)
                        {
                            tour.FreeSeats += application.NumberOfPeople;
                        }
                    }

                    _context.Applications.Remove(application);
                    _context.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка удаления заявки: {ex.Message}", "Ошибка");
                return false;
            }
        }

        // === ТЕСТ БД ===
        public string TestDatabase()
        {
            try
            {
                var toursCount = _context.Tours.Count();
                var usersCount = _context.Users.Count();
                var countriesCount = _context.Countries.Count();
                var busTypesCount = _context.BusTypes.Count();
                var appsCount = _context.Applications.Count();

                return $"✅ База данных подключена!\n\n" +
                       $"🗺️ Туров: {toursCount}\n" +
                       $"👤 Пользователей: {usersCount}\n" +
                       $"🌍 Стран: {countriesCount}\n" +
                       $"🚌 Типов автобусов: {busTypesCount}\n" +
                       $"📋 Заявок: {appsCount}";
            }
            catch (Exception ex)
            {
                return $"❌ Ошибка подключения к БД:\n{ex.Message}";
            }
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}