using Foundation;
using Microsoft.Maui;
using Microsoft.Maui.Hosting;

namespace VirtualDebts;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
