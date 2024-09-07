using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Taxi.Models;
using Taxi.Repository;

namespace Taxi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientController : Controller
    {
        private readonly ClientRepository _clientRepository;
        private readonly TaxiRepository _taxiRepository;

        public ClientController(ClientRepository clientRepository, TaxiRepository taxiRepository)
        {
            _clientRepository = clientRepository;
            _taxiRepository = taxiRepository;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> RegisterClient([FromBody] TaxiClient client)
        {
            if (client == null)
                return BadRequest("Client information is required.");

            var clientId = await _clientRepository.RegisterClientAsync(client.Name, client.Email, client.Password);
            return Ok(new { Id = clientId });
        }

        [HttpPost("Login")]
        public async Task<IActionResult> LoginClient([FromBody] LoginModel loginModel)
        {
            var client = await _clientRepository.GetClientByEmailAsync(loginModel.Email);
            if (client == null || client.Password != loginModel.Password)
                return Unauthorized("Invalid email or password.");

            return Ok(new { client.Id });
        }

        [HttpPost("StartSession")]
        public async Task<IActionResult> StartSession([FromQuery] int clientId, [FromQuery] int driverId)
        {
            var sessionId = await _taxiRepository.StartSessionAsync(driverId, clientId);
            return Ok(new { SessionId = sessionId });
        }

        [HttpPost("EndSession")]
        public async Task<IActionResult> EndSession([FromQuery] int sessionId, [FromQuery] double distanceTraveled)
        {
            var totalCost = await _taxiRepository.EndSessionAsync(sessionId, distanceTraveled);
            if (totalCost == null)
                return NotFound("Session not found or has not been ended.");

            return Ok(new { TotalCost = totalCost });
        }

        [HttpGet("GetSessionInfo")]
        public async Task<IActionResult> GetSessionInfo([FromQuery] int sessionId)
        {
            var session = await _taxiRepository.GetSessionAsync(sessionId);
            if (session == null)
                return NotFound("Session not found.");

            var driver = await _taxiRepository.GetDriverAsync(session.DriverId);
            var client = await _clientRepository.GetClientByIdAsync(session.ClientId);

            return Ok(new
            {
                Session = session,
                Driver = new { driver.Name, driver.Surname, driver.Car, driver.CarNumber },
                Client = new { client.Name }
            });
        }
    }
}
