using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Taxi.Models;
using Taxi.Services;

namespace Taxi.Repository
{
    public class ClientRepository
    {
        private readonly DatabaseService _databaseService;

        public ClientRepository(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public async Task<int> RegisterClientAsync(string name, string email, string password)
        {
            var sql = @"INSERT INTO [dbo].[Clients] (Name, Email, Password) VALUES (@Name, @Email, @Password); SELECT SCOPE_IDENTITY();";
            using (var connection = _databaseService.CreateConnection())
            {
                return await connection.QuerySingleAsync<int>(sql, new { Name = name, Email = email, Password = password });
            }
        }

        public async Task<TaxiClient> GetClientByEmailAsync(string email)
        {
            var sql = @"SELECT * FROM [dbo].[Clients] WHERE Email = @Email;";
            using (var connection = _databaseService.CreateConnection())
            {
                return await connection.QuerySingleAsync<TaxiClient>(sql, new { Email = email });
            }
        }

        public async Task<TaxiClient> GetClientByIdAsync(int clientId)
        {
            var sql = @"SELECT * FROM [dbo].[Clients] WHERE Id = @ClientId;";
            using (var connection = _databaseService.CreateConnection())
            {
                return await connection.QuerySingleAsync<TaxiClient>(sql, new { ClientId = clientId });
            }
        }

        public async Task UpdateClientAsync(TaxiClient client)
        {
            var sql = @"UPDATE [dbo].[Clients] SET Name = @Name, Email = @Email, Password = @Password WHERE Id = @Id;";
            using (var connection = _databaseService.CreateConnection())
            {
                await connection.ExecuteAsync(sql, new { client.Name, client.Email, client.Password, client.Id });
            }
        }

        public async Task DeleteClientAsync(int clientId)
        {
            var sql = @"DELETE FROM [dbo].[Clients] WHERE Id = @ClientId;";
            using (var connection = _databaseService.CreateConnection())
            {
                await connection.ExecuteAsync(sql, new { ClientId = clientId });
            }
        }

        public async Task<IEnumerable<TaxiClient>> GetAllClientsAsync()
        {
            var sql = @"SELECT * FROM [dbo].[Clients];";
            using (var connection = _databaseService.CreateConnection())
            {
                return await connection.QueryAsync<TaxiClient>(sql);
            }
        }
    }
}
