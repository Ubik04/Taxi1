using Dapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Taxi.Models;
using Taxi.Services;

namespace Taxi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaxiController : ControllerBase
    {
        private readonly TaxiRepository _taxiRepository;

        public TaxiController(TaxiRepository taxiRepository)
        {
            _taxiRepository = taxiRepository;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> RegisterDriver([FromBody] TaxiDriver driver)
        {
            if (driver == null)
                return BadRequest("Driver information is required.");

            var driverId = await _taxiRepository.RegisterDriverAsync(driver.Name, driver.Surname, driver.Birthdate, driver.License, driver.Car, driver.CarNumber);

            return Ok(new TaxiDriver
            {
                Id = driverId,
                Name = driver.Name,
                Surname = driver.Surname,
                Birthdate = driver.Birthdate,
                License = driver.License,
                Car = driver.Car,
                CarNumber = driver.CarNumber
            });
        }

        [HttpPost("StartSession")]
        public async Task<IActionResult> StartSession([FromQuery] int driverId)
        {
            var sessionId = await _taxiRepository.StartSessionAsync(driverId);
            return Ok(new TaxiSession { Id = sessionId, DriverId = driverId, StartTime = DateTime.UtcNow, DistanceTraveled = 0.0 });
        }

        [HttpPost("EndSession")]
        public async Task<IActionResult> EndSession([FromQuery] int sessionId, [FromQuery] double distanceTraveled)
        {
            var totalCost = await _taxiRepository.EndSessionAsync(sessionId, distanceTraveled);
            if (totalCost == null)
                return NotFound("Session not found or has not been ended.");

            return Ok(new { TotalCost = totalCost });
        }

        [HttpGet("GetDriverSessions")]
        public async Task<IActionResult> GetDriverSessions([FromQuery] int driverId)
        {
            var sessions = await _taxiRepository.GetDriverSessionsAsync(driverId);
            return Ok(sessions);
        }
    }
}
