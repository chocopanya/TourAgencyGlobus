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
                var existingTour = _context.Tours.Find(tour.Id);
                if (existingTour != null)
                {
                    _context.Entry(existingTour).CurrentValues.SetValues(tour);
                    _context.SaveChanges();
                    return true;
                }
                return false;
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

        public User GetUserByLogin(string login)
        {
            try
            {
                return _context.Users
                    .AsNoTracking()
                    .FirstOrDefault(u => u.Login.Equals(login, StringComparison.OrdinalIgnoreCase));
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