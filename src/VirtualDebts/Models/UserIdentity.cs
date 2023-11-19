using System;

namespace VirtualDebts.Models;

public struct UserIdentity(Guid id, string name)
{
    public Guid Id { get; } = id;
    public string Name { get; set; } = name;
}
