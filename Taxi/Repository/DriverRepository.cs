using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Taxi.Models;
using Taxi.Services;

namespace Taxi.Repository
{
    public class DriverRepository
    {
        private readonly DatabaseService _databaseService;

        public DriverRepository(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public async Task<int> RegisterDriverAsync(string name, string surname, DateTime birthdate, string license, string car, string carNumber, string email, string password)
        {
            var sql = @"INSERT INTO [dbo].[TaxiDrivers] (Name, Surname, Birthdate, License, Car, CarNumber, Email, Password) VALUES (@Name, @Surname, @Birthdate, @License, @Car, @CarNumber, @Email, @Password); SELECT SCOPE_IDENTITY();";
            using (var connection = _databaseService.CreateConnection())
            {
                return await connection.QuerySingleAsync<int>(sql, new { Name = name, Surname = surname, Birthdate = birthdate, License = license, Car = car, CarNumber = carNumber, Email = email, Password = password });
            }
        }

        public async Task<TaxiDriver> GetDriverByEmailAsync(string email)
        {
            var sql = @"SELECT * FROM [dbo].[TaxiDrivers] WHERE Email = @Email;";
            using (var connection = _databaseService.CreateConnection())
            {
                return await connection.QuerySingleAsync<TaxiDriver>(sql, new { Email = email });
            }
        }

        public async Task UpdateDriverAsync(TaxiDriver driver)
        {
            var sql = @"UPDATE [dbo].[TaxiDrivers] SET Name = @Name, Surname = @Surname, Birthdate = @Birthdate, License = @License, Car = @Car, CarNumber = @CarNumber, Email = @Email, Password = @Password WHERE Id = @Id;";
            using (var connection = _databaseService.CreateConnection())
            {
                await connection.ExecuteAsync(sql, new { driver.Name, driver.Surname, driver.Birthdate, driver.License, driver.Car, driver.CarNumber, driver.Email, driver.Password, driver.Id });
            }
        }

        public async Task DeleteDriverAsync(int driverId)
        {
            var sql = @"DELETE FROM [dbo].[TaxiDrivers] WHERE Id = @DriverId;";
            using (var connection = _databaseService.CreateConnection())
            {
                await connection.ExecuteAsync(sql, new { DriverId = driverId });
            }
        }

        public async Task<IEnumerable<TaxiDriver>> GetAllDriversAsync()
        {
            var sql = @"SELECT * FROM [dbo].[TaxiDrivers];";
            using (var connection = _databaseService.CreateConnection())
            {
                return await connection.QueryAsync<TaxiDriver>(sql);
            }
        }
    }
}
