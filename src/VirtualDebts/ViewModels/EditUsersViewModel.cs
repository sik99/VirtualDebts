using System.Collections.Generic;

namespace VirtualDebts.ViewModels
{
    public class EditUsersViewModel
    {
        // TODO Propagate also user Guids to ViewModel
        public IList<string> UserList { get; set; } = new List<string>();
        public string UserListAsString { get; set; }
    }
}
