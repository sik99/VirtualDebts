using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using VirtualDebts.Binding;
using VirtualDebts.Views;

namespace VirtualDebts.Controllers;

[TestClass]
public class MainMenuControllerTests
{
    private MainMenuController givenInstance;
    private readonly GivenFixture givenFixture = new();

    [TestInitialize]
    public void TestInitialize()
    {
        this.givenInstance = new MainMenuController(
            this.givenFixture.NavigationServiceMock.Object,
            new CommandFactory(),
            this.givenFixture.Dispatcher);
    }

    #region OnEditUsers tests
    [TestMethod]
    public void OnEditUsers_navigates_to_EditUsersView()
    {
        // When
        this.givenInstance.EditUsersCommand.Execute(null);

        // Then
        ThenVerifyNavigatedToView(typeof(EditUsersView));
    }

    [TestMethod]
    public async Task OnEditUsers_prevents_any_other_navigation_to_take_place_during_its_execution()
    {
        // Given
        var canNavigateIsCopiedEvent = new AutoResetEvent(false);
        GivenNavigateToWaitsForEvent(canNavigateIsCopiedEvent);
        this.givenInstance.CanNavigate.Should().BeTrue();

        // When
        var onEditUsersTask = this.givenInstance.EditUsersCommand.ExecuteAsync();
        bool couldNavigateDuringTask = this.givenInstance.CanNavigate;
        canNavigateIsCopiedEvent.Set();
        await onEditUsersTask;

        // Then
        couldNavigateDuringTask.Should().BeFalse();
        this.givenInstance.CanNavigate.Should().BeTrue();

        ThenVerifyNavigatedToView(typeof(EditUsersView));
    }
    #endregion

    #region OnNewPayment tests
    [TestMethod]
    public void OnNewPayment_navigates_to_NewPaymentView()
    {
        // When
        this.givenInstance.NewPaymentCommand.Execute(null);

        // Then
        ThenVerifyNavigatedToView(typeof(EmptyView));
    }

    [TestMethod]
    public async Task OnNewPayment_prevents_any_other_navigation_to_take_place_during_its_execution()
    {
        // Given
        var canNavigateIsCopiedEvent = new AutoResetEvent(false);
        GivenNavigateToWaitsForEvent(canNavigateIsCopiedEvent);
        this.givenInstance.CanNavigate.Should().BeTrue();

        // When
        var onEditUsersTask = this.givenInstance.NewPaymentCommand.ExecuteAsync();
        bool couldNavigateDuringTask = this.givenInstance.CanNavigate;
        canNavigateIsCopiedEvent.Set();
        await onEditUsersTask;

        // Then
        couldNavigateDuringTask.Should().BeFalse();
        this.givenInstance.CanNavigate.Should().BeTrue();

        ThenVerifyNavigatedToView(typeof(EmptyView));
    }
    #endregion

    #region OnCurrentBalance tests
    [TestMethod]
    public void OnCurrentBalance_navigates_to_CurrentBalanceView()
    {
        // When
        this.givenInstance.CurrentBalanceCommand.Execute(null);

        // Then
        ThenVerifyNavigatedToView(typeof(EmptyView));
    }

    [TestMethod]
    public async Task OnCurrentBalance_prevents_any_other_navigation_to_take_place_during_its_execution()
    {
        // Given
        var canNavigateIsCopiedEvent = new AutoResetEvent(false);
        GivenNavigateToWaitsForEvent(canNavigateIsCopiedEvent);
        this.givenInstance.CanNavigate.Should().BeTrue();

        // When
        var onEditUsersTask = this.givenInstance.CurrentBalanceCommand.ExecuteAsync();
        bool couldNavigateDuringTask = this.givenInstance.CanNavigate;
        canNavigateIsCopiedEvent.Set();
        await onEditUsersTask;

        // Then
        couldNavigateDuringTask.Should().BeFalse();
        this.givenInstance.CanNavigate.Should().BeTrue();

        ThenVerifyNavigatedToView(typeof(EmptyView));
    }
    #endregion

    #region Then
    private void ThenVerifyNavigatedToView(Type viewType)
    {
        this.givenFixture.NavigationServiceMock.Verify(mock => mock.NavigateTo(viewType), Times.Exactly(1));
        this.givenFixture.NavigationServiceMock.VerifyNoOtherCalls();
    }
    #endregion

    #region Given
    private void GivenNavigateToWaitsForEvent(AutoResetEvent waitEvent)
    {
        this.givenFixture.NavigationServiceMock
                .Setup(mock => mock.NavigateTo(It.IsAny<Type>()))
                .Returns(Task.Factory.StartNew(() => waitEvent.WaitOne()));
    }

    internal class GivenFixture
    {
        public readonly Mock<INavigationService> NavigationServiceMock = new(MockBehavior.Strict);
        public readonly SynchronousDispatcher Dispatcher = new();

        public GivenFixture()
        {
            this.NavigationServiceMock
                .Setup(mock => mock.NavigateTo(It.IsAny<Type>()))
                .Returns(Task.CompletedTask);
        }
    }
    #endregion
}