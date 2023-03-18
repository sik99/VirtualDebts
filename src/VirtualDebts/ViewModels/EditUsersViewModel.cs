using System.Collections.Generic;
using VirtualDebts.Models;

namespace VirtualDebts.ViewModels
{
    public class EditUsersViewModel
    {
        public IList<UserIdentity> Users { get; set; } = new List<UserIdentity>();
        public string UserNamesAsString { get; set; }
    }
}
