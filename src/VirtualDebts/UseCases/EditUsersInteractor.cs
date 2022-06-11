using System;
using System.Linq;
using System.Threading.Tasks;
using VirtualDebts.Binding;
using VirtualDebts.Models;
using VirtualDebts.Services;

namespace VirtualDebts.UseCases
{
    public class EditUsersInteractor
    {
        private readonly Store store;
        private readonly INavigationService navigationService;

        public EditUsersInteractor(Store store, INavigationService navigationService)
        {
            this.store = store ?? throw new ArgumentNullException(nameof(store));
            this.navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        }

        public async Task AddUser(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                await this.navigationService.ShowMessageBox(Properties.Resources.EditUsers_AddFailedMsg, Properties.Resources.EditUsers_AddWhitespaceMsg);
                return;
            }

            userName = userName.Trim();
            bool isSuccess = this.store.Update(appState =>
            {
                bool isUserExistent = appState.Users.FindIndex(user => user.Name == userName) >= 0;
                if (isUserExistent)
                    return false;

                appState.Users.Add(new User(userName));
                return true;
            });

            if (!isSuccess)
                await this.navigationService.ShowMessageBox(Properties.Resources.EditUsers_AddFailedMsg, string.Format(Properties.Resources.EditUsers_AddExistentMsg, userName));
        }

        // TODO RemoveUser using Guid instead of name
        public async Task RemoveUser(string userName)
        {
            bool isSuccess = this.store.Update(appState =>
            {
                var userIndex = appState.Users.FindIndex(user => user.Name == userName);
                if (userIndex < 0)
                    return true; // user is already removed

                var user = appState.Users.ElementAt(userIndex);
                if (user.Balance != 0)
                    return false;

                bool isRemoved = appState.Users.Remove(user);
                if (!isRemoved)
                    throw new ArgumentOutOfRangeException($"User \"{userName}\" couldn't be removed");
                return true;
            });

            if (!isSuccess)
                await this.navigationService.ShowMessageBox(Properties.Resources.EditUsers_RemoveFailedMsg, string.Format(Properties.Resources.EditUsers_RemoveDebtorMsg, userName));
        }
    }
}
