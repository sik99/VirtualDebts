using System;

namespace VirtualDebts.Server
{
    public class IdGenerator : IUserIdGenerator
    {
        public Guid Next()
        {
            return Guid.NewGuid();
        }
    }
}
