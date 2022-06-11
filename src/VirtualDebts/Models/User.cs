using System;

namespace VirtualDebts.Models
{
    public struct User
    {
        public Guid Id { get; }
        public string Name { get; set; }
        public int Balance { get; set; }

        public User(string name)
        {
            this.Id = Guid.NewGuid();
            this.Name = name;
            this.Balance = 0;
        }
    }
}
