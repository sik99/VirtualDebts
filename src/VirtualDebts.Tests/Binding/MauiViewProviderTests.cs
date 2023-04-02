using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using VirtualDebts.Views;
using System;

namespace VirtualDebts.Binding
{
    [TestClass()]
    public class MauiViewProviderTests
    {
        private MauiViewProvider givenInstance;
        private readonly GivenFixture givenFixture = new GivenFixture();

        [TestInitialize]
        public void TestInitialize()
        {
            this.givenInstance = new MauiViewProvider(
                this.givenFixture.ServiceProviderMock.Object);
        }

        #region GetView tests
        [TestMethod()]
        [Ignore] // TODO Make test pass
        public void GetView_returns_EditUsersView_when_given_id_EditUsers()
        {
            // Given
            var viewId = ViewId.EditUsers;

            // When
            var result = givenInstance.GetView(viewId);

            // Then
            result.Should().BeOfType<EditUsersView>();
        }

        [TestMethod()]
        public void GetView_returns_EmptyView_when_given_id_NewPayment()
        {
            // Given
            var viewId = ViewId.NewPayment;

            // When
            var result = givenInstance.GetView(viewId);

            // Then
            result.Should().BeOfType<EmptyView>();
        }

        [TestMethod()]
        public void GetView_returns_EmptyView_when_given_id_CurrentBalance()
        {
            // Given
            var viewId = ViewId.CurrentBalance;

            // When
            var result = givenInstance.GetView(viewId);

            // Then
            result.Should().BeOfType<EmptyView>();
        }
        #endregion

        #region GetType tests
        [TestMethod()]
        public void GetType_returns_EditUsersView_when_given_id_EditUsers()
        {
            // Given
            var viewId = ViewId.EditUsers;

            // When
            var result = givenInstance.GetType(viewId);

            // Then
            result.Should().Be(typeof(EditUsersView));
        }

        [TestMethod()]
        public void GetType_returns_EmptyView_when_given_id_NewPayment()
        {
            // Given
            var viewId = ViewId.NewPayment;

            // When
            var result = givenInstance.GetType(viewId);

            // Then
            result.Should().Be(typeof(EmptyView));
        }

        [TestMethod()]
        public void GetType_returns_EmptyView_when_given_id_CurrentBalance()
        {
            // Given
            var viewId = ViewId.CurrentBalance;

            // When
            var result = givenInstance.GetType(viewId);

            // Then
            result.Should().Be(typeof(EmptyView));
        }
        #endregion

        #region Given
        internal class GivenFixture
        {
            public readonly Mock<IServiceProvider> ServiceProviderMock = new Mock<IServiceProvider>(MockBehavior.Loose);

            public GivenFixture()
            {
                // Mock Xamarin initialization so that we can instantiate class Page in ServiceProviderMock
                // Device.PlatformServices = Mock.Of<IPlatformServices>(MockBehavior.Loose);

                this.ServiceProviderMock
                    .Setup(mock => mock.GetService(It.IsAny<Type>()))
                    .Returns((Type type) => Activator.CreateInstance(type));
            }
        }
        #endregion
    }
}