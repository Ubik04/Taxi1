using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Taxi.Models;
using Taxi.Services;

namespace Taxi.Repository
{
    public class TaxiRepository
    {
        private readonly DatabaseService _databaseService;
        private readonly PricingService _pricingService;

        public TaxiRepository(DatabaseService databaseService, PricingService pricingService)
        {
            _databaseService = databaseService;
            _pricingService = pricingService;
        }

        public async Task<int> StartSessionAsync(int driverId, int clientId)
        {
            var sql = @"INSERT INTO [dbo].[TaxiSessions] (DriverId, ClientId, StartTime, DistanceTraveled) VALUES (@DriverId, @ClientId, @StartTime, @DistanceTraveled); SELECT SCOPE_IDENTITY();";
            using (var connection = _databaseService.CreateConnection())
            {
                return await connection.QuerySingleAsync<int>(sql, new { DriverId = driverId, ClientId = clientId, StartTime = DateTime.UtcNow, DistanceTraveled = 0.0 });
            }
        }

        public async Task<double?> EndSessionAsync(int sessionId, double distanceTraveled)
        {
            var sqlUpdate = @"UPDATE [dbo].[TaxiSessions] SET EndTime = @EndTime, DistanceTraveled = @DistanceTraveled WHERE Id = @SessionId;";
            var sqlSelect = @"SELECT * FROM [dbo].[TaxiSessions] WHERE Id = @SessionId;";
            using (var connection = _databaseService.CreateConnection())
            {
                await connection.ExecuteAsync(sqlUpdate, new { EndTime = DateTime.UtcNow, DistanceTraveled = distanceTraveled, SessionId = sessionId });
                var session = await connection.QuerySingleAsync<TaxiSession>(sqlSelect, new { SessionId = sessionId });

                if (session == null || session.EndTime == null) return null;

                return _pricingService.CalculateTotalCost(session.StartTime, session.EndTime.Value, session.DistanceTraveled);
            }
        }

        public async Task<TaxiSession> GetSessionAsync(int sessionId)
        {
            var sql = @"SELECT * FROM [dbo].[TaxiSessions] WHERE Id = @SessionId;";
            using (var connection = _databaseService.CreateConnection())
            {
                return await connection.QuerySingleAsync<TaxiSession>(sql, new { SessionId = sessionId });
            }
        }

        public async Task<IEnumerable<TaxiSession>> GetDriverSessionsAsync(int driverId)
        {
            var sql = @"SELECT Id, DriverId, ClientId, StartTime, EndTime, DistanceTraveled FROM [dbo].[TaxiSessions] WHERE DriverId = @DriverId";
            using (var connection = _databaseService.CreateConnection())
            {
                return await connection.QueryAsync<TaxiSession>(sql, new { DriverId = driverId });
            }
        }

        public async Task<IEnumerable<TaxiSession>> GetAllSessionsAsync()
        {
            var sql = @"SELECT * FROM [dbo].[TaxiSessions];";
            using (var connection = _databaseService.CreateConnection())
            {
                return await connection.QueryAsync<TaxiSession>(sql);
            }
        }

        public async Task<TaxiDriver> GetDriverAsync(int driverId)
        {
            var sql = @"SELECT * FROM [dbo].[TaxiDrivers] WHERE Id = @DriverId;";
            using (var connection = _databaseService.CreateConnection())
            {
                return await connection.QuerySingleAsync<TaxiDriver>(sql, new { DriverId = driverId });
            }
        }
    }
}



