using System;

namespace VirtualDebts.Models
{
    public struct User
    {
        public Guid Id { get; }
        public string Name { get; set; }
        public int Balance { get; set; }

        public User(Guid id, string name, int balance = 0)
        {
            this.Id = id;
            this.Name = name;
            this.Balance = balance;
        }

        public UserIdentity GetIdentity()
        {
            return new UserIdentity(this.Id, this.Name);
        }
    }
}
