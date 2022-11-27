using System;
using System.Threading.Tasks;
using System.Windows.Input;
using VirtualDebts.Binding;

namespace VirtualDebts.Controllers
{
    public class MainMenuController : ControllerBase
    {
        public ICommand EditUsersCommand { get; }
        public ICommand NewPaymentCommand { get; }
        public ICommand CurrentBalanceCommand { get; }

        private readonly ICommandFactory commandFactory;
        private readonly INavigationService navigationService;

        // Disable navigation to prevent double clicks adding multiple pages on the view stack.
        // It doesn't work when I am pressing the button for the first time after staring the app.
        private bool canNavigate = true;
        public bool CanNavigate
        {
            get => canNavigate;
            private set
            {
                this.dispatcher?.InvokeInMainThread(() =>
                {
                    canNavigate = value;
                    this.NotifyPropertyChanged(null);
                });
            }
        }

        public MainMenuController(
            ICommandFactory commandFactory,
            INavigationService navigationService,
            IDispatcher dispatcher)
            : base(dispatcher)
        {
            this.commandFactory = commandFactory ?? throw new ArgumentNullException(nameof(commandFactory));
            this.navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));

            this.EditUsersCommand = this.commandFactory.CreateAsync(this.OnEditUsers);
            this.NewPaymentCommand = this.commandFactory.CreateAsync(this.OnNewPayment);
            this.CurrentBalanceCommand = this.commandFactory.CreateAsync(this.OnCurrentBalance);
        }

        public async Task OnEditUsers()
        {
            this.CanNavigate = false;
            await this.navigationService.NavigateTo(ViewId.EditUsers);
            this.CanNavigate = true;
        }

        public async Task OnNewPayment()
        {
            this.CanNavigate = false;
            await this.navigationService.NavigateTo(ViewId.NewPayment);
            this.CanNavigate = true;
        }

        public async Task OnCurrentBalance()
        {
            this.CanNavigate = false;
            await this.navigationService.NavigateTo(ViewId.CurrentBalance);
            this.CanNavigate = true;
        }
    }
}
