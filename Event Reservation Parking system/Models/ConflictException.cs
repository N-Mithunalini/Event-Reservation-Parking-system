<<<<<<< HEAD
﻿using System;

namespace EventParkingReservationSystem.Models;

public class ConflictException : Exception
{
    public ConflictException(string message) : base(message) { }
=======
﻿namespace EventParkingReservationSystem.Models;

public class ConflictException : Exception
{
    public ConflictException(string message)
        : base(message)
    {
    }
}
//     public ConflictException(string message) : base(message) { }
>>>>>>> origin/master
}
