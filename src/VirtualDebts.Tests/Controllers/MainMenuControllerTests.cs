using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using VirtualDebts.Binding;

namespace VirtualDebts.Controllers
{
    [TestClass]
    public class MainMenuControllerTests
    {
        private MainMenuController givenInstance;
        private readonly GivenFixture givenFixture = new GivenFixture();

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
            ThenVerifyNavigatedToView(ViewId.EditUsers);
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

            ThenVerifyNavigatedToView(ViewId.EditUsers);
        }
        #endregion

        #region OnNewPayment tests
        [TestMethod]
        public void OnNewPayment_navigates_to_NewPaymentView()
        {
            // When
            this.givenInstance.NewPaymentCommand.Execute(null);

            // Then
            ThenVerifyNavigatedToView(ViewId.NewPayment);
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

            ThenVerifyNavigatedToView(ViewId.NewPayment);
        }
        #endregion

        #region OnCurrentBalance tests
        [TestMethod]
        public void OnCurrentBalance_navigates_to_CurrentBalanceView()
        {
            // When
            this.givenInstance.CurrentBalanceCommand.Execute(null);

            // Then
            ThenVerifyNavigatedToView(ViewId.CurrentBalance);
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

            ThenVerifyNavigatedToView(ViewId.CurrentBalance);
        }
        #endregion

        #region Then
        private void ThenVerifyNavigatedToView(ViewId viewId)
        {
            this.givenFixture.NavigationServiceMock.Verify(mock => mock.NavigateTo(viewId), Times.Exactly(1));
            this.givenFixture.NavigationServiceMock.VerifyNoOtherCalls();
        }
        #endregion

        #region Given
        private void GivenNavigateToWaitsForEvent(AutoResetEvent waitEvent)
        {
            this.givenFixture.NavigationServiceMock
                    .Setup(mock => mock.NavigateTo(It.IsAny<ViewId>()))
                    .Returns(Task.Factory.StartNew(() => waitEvent.WaitOne()));
        }

        internal class GivenFixture
        {
            public readonly Mock<INavigationService> NavigationServiceMock = new Mock<INavigationService>(MockBehavior.Strict);
            public readonly SynchronousDispatcher Dispatcher = new SynchronousDispatcher();

            public GivenFixture()
            {
                this.NavigationServiceMock
                    .Setup(mock => mock.NavigateTo(It.IsAny<ViewId>()))
                    .Returns(Task.CompletedTask);
            }
        }
        #endregion
    }
}