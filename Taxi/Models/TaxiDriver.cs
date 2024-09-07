using System;

namespace Taxi.Models
{
    public class TaxiDriver
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime Birthdate { get; set; }
        public string License { get; set; }
        public string Car { get; set; }
        public string CarNumber { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
