using System;

namespace VirtualDebts.Server;

public interface IUserIdGenerator
{
    Guid Next();
}
