using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Taxi.Models;
using Taxi.Repository;

namespace Taxi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : Controller
    {
        private readonly ClientRepository _clientRepository;
        private readonly DriverRepository _driverRepository;
        private readonly TaxiRepository _taxiRepository;

        public AdminController(ClientRepository clientRepository, DriverRepository driverRepository, TaxiRepository taxiRepository)
        {
            _clientRepository = clientRepository;
            _driverRepository = driverRepository;
            _taxiRepository = taxiRepository;
        }

        [HttpPost("AddClient")]
        public async Task<IActionResult> AddClient([FromBody] TaxiClient client)
        {
            if (client == null)
                return BadRequest("Client information is required.");

            var clientId = await _clientRepository.RegisterClientAsync(client.Name, client.Email, client.Password);
            return Ok(new { Id = clientId });
        }

        [HttpPut("UpdateClient")]
        public async Task<IActionResult> UpdateClient([FromBody] TaxiClient client)
        {
            if (client == null)
                return BadRequest("Client information is required.");

            await _clientRepository.UpdateClientAsync(client);
            return NoContent();
        }

        [HttpDelete("DeleteClient")]
        public async Task<IActionResult> DeleteClient([FromQuery] int clientId)
        {
            await _clientRepository.DeleteClientAsync(clientId);
            return NoContent();
        }

        [HttpPost("AddDriver")]
        public async Task<IActionResult> AddDriver([FromBody] TaxiDriver driver)
        {
            if (driver == null)
                return BadRequest("Driver information is required.");

            var driverId = await _driverRepository.RegisterDriverAsync(driver.Name, driver.Surname, driver.Birthdate, driver.License, driver.Car, driver.CarNumber, driver.Email, driver.Password);
            return Ok(new { Id = driverId });
        }

        [HttpPut("UpdateDriver")]
        public async Task<IActionResult> UpdateDriver([FromBody] TaxiDriver driver)
        {
            if (driver == null)
                return BadRequest("Driver information is required.");

            await _driverRepository.UpdateDriverAsync(driver);
            return NoContent();
        }

        [HttpDelete("DeleteDriver")]
        public async Task<IActionResult> DeleteDriver([FromQuery] int driverId)
        {
            await _driverRepository.DeleteDriverAsync(driverId);
            return NoContent();
        }

        [HttpGet("GetAllClients")]
        public async Task<IActionResult> GetAllClients()
        {
            var clients = await _clientRepository.GetAllClientsAsync();
            return Ok(clients);
        }

        [HttpGet("GetAllDrivers")]
        public async Task<IActionResult> GetAllDrivers()
        {
            var drivers = await _driverRepository.GetAllDriversAsync();
            return Ok(drivers);
        }

        [HttpGet("GetAllSessions")]
        public async Task<IActionResult> GetAllSessions()
        {
            var sessions = await _taxiRepository.GetAllSessionsAsync();
            return Ok(sessions);
        }
    }
}
