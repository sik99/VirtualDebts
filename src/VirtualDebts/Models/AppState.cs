using System;
using System.Collections.Generic;

namespace VirtualDebts.Models;

public class AppState : ICloneable
{
    public List<User> Users { get; set; } = [];

    public object Clone()
    {
        return new AppState
        {
            Users = new List<User>(this.Users)
        };
    }
}
