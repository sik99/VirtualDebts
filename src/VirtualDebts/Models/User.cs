using System;

namespace VirtualDebts.Models;

public struct User(Guid id, string name, int balance = 0)
{
    public Guid Id { get; } = id;
    public string Name { get; set; } = name;
    public int Balance { get; set; } = balance;

    public readonly UserIdentity GetIdentity()
    {
        return new UserIdentity(this.Id, this.Name);
    }
}
