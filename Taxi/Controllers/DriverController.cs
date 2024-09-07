using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Taxi.Models;
using Taxi.Repository;

namespace Taxi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DriverController : Controller
    {
        private readonly DriverRepository _driverRepository;
        private readonly TaxiRepository _taxiRepository;

        public DriverController(DriverRepository driverRepository, TaxiRepository taxiRepository)
        {
            _driverRepository = driverRepository;
            _taxiRepository = taxiRepository;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> RegisterDriver([FromBody] TaxiDriver driver)
        {
            if (driver == null)
                return BadRequest("Driver information is required.");

            var driverId = await _driverRepository.RegisterDriverAsync(driver.Name, driver.Surname, driver.Birthdate, driver.License, driver.Car, driver.CarNumber, driver.Email, driver.Password);
            return Ok(new { Id = driverId });
        }

        [HttpPost("Login")]
        public async Task<IActionResult> LoginDriver([FromBody] LoginModel loginModel)
        {
            var driver = await _driverRepository.GetDriverByEmailAsync(loginModel.Email);
            if (driver == null || driver.Password != loginModel.Password)
                return Unauthorized("Invalid email or password.");

            return Ok(new { driver.Id });
        }

        [HttpPost("StartSession")]
        public async Task<IActionResult> StartSession([FromQuery] int driverId)
        {
            var sessionId = await _taxiRepository.StartSessionAsync(driverId, -1);
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

        [HttpGet("GetMySessions")]
        public async Task<IActionResult> GetMySessions([FromQuery] int driverId)
        {
            var sessions = await _taxiRepository.GetDriverSessionsAsync(driverId);
            return Ok(sessions);
        }
    }
}
