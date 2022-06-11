using System.Collections.Generic;

namespace VirtualDebts.Models
{
    public class AppState
    {
        public List<User> Users { get; set; } = new List<User>();

        public AppState Copy()
        {
            return new AppState
            {
                Users = new List<User>(this.Users)
            };
        }
    }
}
