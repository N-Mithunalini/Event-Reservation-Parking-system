using System;

namespace Event_Reservation_Parking_system.Models
{
    public class ConflictException:Exception
    {
        public ConflictException(string message) : base(message) { }
    }
}

