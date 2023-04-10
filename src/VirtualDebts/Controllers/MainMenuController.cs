using AsyncAwaitBestPractices.MVVM;
using System;
using System.Threading.Tasks;
using VirtualDebts.Binding;
using VirtualDebts.Views;

using IDispatcher = VirtualDebts.Binding.IDispatcher;

namespace VirtualDebts.Controllers;

public class MainMenuController : ControllerBase
{
    public IAsyncCommand EditUsersCommand { get; }
    public IAsyncCommand NewPaymentCommand { get; }
    public IAsyncCommand CurrentBalanceCommand { get; }

    private readonly INavigationService navigationService;
    private readonly ICommandFactory commandFactory;

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
        INavigationService navigationService,
        ICommandFactory commandFactory,
        IDispatcher dispatcher)
        : base(dispatcher)
    {
        this.navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        this.commandFactory = commandFactory ?? throw new ArgumentNullException(nameof(commandFactory));

        this.EditUsersCommand = this.commandFactory.CreateAsync(this.OnEditUsers);
        this.NewPaymentCommand = this.commandFactory.CreateAsync(this.OnNewPayment);
        this.CurrentBalanceCommand = this.commandFactory.CreateAsync(this.OnCurrentBalance);
    }

    public async Task OnEditUsers()
    {
        this.CanNavigate = false;
        await this.navigationService.NavigateTo(typeof(EditUsersView));
        this.CanNavigate = true;
    }

    public async Task OnNewPayment()
    {
        this.CanNavigate = false;
        await this.navigationService.NavigateTo(typeof(EmptyView));
        this.CanNavigate = true;
    }

    public async Task OnCurrentBalance()
    {
        this.CanNavigate = false;
        await this.navigationService.NavigateTo(typeof(EmptyView));
        this.CanNavigate = true;
    }
}
