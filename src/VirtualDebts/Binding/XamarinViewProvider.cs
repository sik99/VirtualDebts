using Microsoft.Extensions.DependencyInjection;
using System;
using VirtualDebts.Views;
using Xamarin.Forms;

namespace VirtualDebts.Binding
{
    public class XamarinViewProvider
    {
        private readonly IServiceProvider serviceProvider;

        public XamarinViewProvider(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public Page GetView(ViewId viewId)
        {
            switch (viewId)
            {
                case ViewId.EditUsers:
                    return this.serviceProvider.GetRequiredService<EditUsersView>();
                case ViewId.NewPayment:
                case ViewId.CurrentBalance:
                    return this.serviceProvider.GetRequiredService<EmptyView>();
                default:
                    throw new ArgumentException($"XamarinViewProvider.GetView not implemented for {nameof(ViewId)}.{viewId}");
            }
        }

        public Type GetType(ViewId viewId)
        {
            switch (viewId)
            {
                case ViewId.EditUsers:
                    return typeof(EditUsersView);
                case ViewId.NewPayment:
                case ViewId.CurrentBalance:
                    return typeof(EmptyView);
                default:
                    throw new ArgumentException($"XamarinViewProvider.GetType not implemented for {nameof(ViewId)}.{viewId}");
            }
        }
    }
}