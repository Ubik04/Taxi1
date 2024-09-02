using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Taxi.Models;
using Taxi.Services;

public class TaxiRepository
{
    private readonly DatabaseService _databaseService;
    private readonly PricingService _pricingService;

    public TaxiRepository(DatabaseService databaseService, PricingService pricingService)
    {
        _databaseService = databaseService;
        _pricingService = pricingService;
    }

    public async Task<int> RegisterDriverAsync(string name, string surname, DateTime birthdate, string license, string car, string carNumber)
    {
        var sql = @"INSERT INTO [dbo].[TaxiDrivers] (Name, Surname, Birthdate, License, Car, CarNumber) VALUES (@Name, @Surname, @Birthdate, @License, @Car, @CarNumber); SELECT SCOPE_IDENTITY();";

        using (var connection = _databaseService.CreateConnection())
        {
            return await connection.QuerySingleAsync<int>(sql, new
            {
                Name = name, Surname = surname,Birthdate = birthdate,License = license,Car = car, CarNumber = carNumber});
        }
    }

    public async Task<int> StartSessionAsync(int driverId)
    {
        var sql = @"INSERT INTO [dbo].[TaxiSessions] (DriverId, StartTime, DistanceTraveled) VALUES (@DriverId, @StartTime, @DistanceTraveled); SELECT SCOPE_IDENTITY();";
        using (var connection = _databaseService.CreateConnection())
        {
            return await connection.QuerySingleAsync<int>(sql, new { DriverId = driverId, StartTime = DateTime.UtcNow, DistanceTraveled = 0.0 });
        }
    }

    public async Task<double?> EndSessionAsync(int sessionId, double distanceTraveled)
    {
        var sqlUpdate = @"UPDATE [dbo].[TaxiSessions] SET EndTime = @EndTime, DistanceTraveled = @DistanceTraveled WHERE Id = @SessionId;";
        var sqlSelect = @"SELECT * FROM [dbo].[TaxiSessions] WHERE Id = @SessionId;";
        using (var connection = _databaseService.CreateConnection())
        {
            await connection.ExecuteAsync(sqlUpdate, new { EndTime = DateTime.UtcNow, DistanceTraveled = distanceTraveled, SessionId = sessionId });
            var session = await connection.QuerySingleOrDefaultAsync<TaxiSession>(sqlSelect, new { SessionId = sessionId });

            if (session == null || session.EndTime == null) return null;

            return _pricingService.CalculateTotalCost(session.StartTime, session.EndTime.Value, session.DistanceTraveled);
        }
    }

    public async Task<IEnumerable<TaxiSession>> GetDriverSessionsAsync(int driverId)
    {
        var sql = @"SELECT Id, DriverId, StartTime, EndTime, DistanceTraveled FROM [dbo].[TaxiSessions] WHERE DriverId = @DriverId";
        using (var connection = _databaseService.CreateConnection())
        {
            return await connection.QueryAsync<TaxiSession>(sql, new { DriverId = driverId });
        }
    }
}
