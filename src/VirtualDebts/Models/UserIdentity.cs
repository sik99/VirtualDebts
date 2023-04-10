using System;

namespace VirtualDebts.Models;

public struct UserIdentity
{
    public Guid Id { get; }
    public string Name { get; set; }

    public UserIdentity(Guid id, string name)
    {
        this.Id = id;
        this.Name = name;
    }
}
