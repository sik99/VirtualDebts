using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using System;
using VirtualDebts.Views;

namespace VirtualDebts.Binding
{
    public class MauiViewProvider

    {
        private readonly IServiceProvider serviceProvider;

        public MauiViewProvider(IServiceProvider serviceProvider)
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
                    throw new ArgumentException($"MauiViewProvider.GetView not implemented for {nameof(ViewId)}.{viewId}");
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
                    throw new ArgumentException($"MauiViewProvider.GetType not implemented for {nameof(ViewId)}.{viewId}");
            }
        }
    }
}