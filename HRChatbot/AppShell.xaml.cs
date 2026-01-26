using Microsoft.Extensions.DependencyInjection;

namespace HRChatbot;

public partial class AppShell : Shell
{
	public AppShell(IServiceProvider? serviceProvider = null)
	{
		InitializeComponent();
		
		// Resolve MainPage from DI container
		var mainPage = serviceProvider?.GetService<MainPage>() ?? 
		               Handler?.MauiContext?.Services?.GetService<MainPage>();
		
		if (mainPage != null)
		{
			Items.Clear();
			Items.Add(new ShellContent
			{
				Content = mainPage,
				Title = "Home",
				Route = "MainPage"
			});
		}
	}
	
	protected override void OnHandlerChanged()
	{
		base.OnHandlerChanged();
		
		// Try to resolve MainPage again once Handler is set
		if (Items.Count == 0)
		{
			var mainPage = Handler?.MauiContext?.Services?.GetService<MainPage>();
			if (mainPage != null)
			{
				Items.Add(new ShellContent
				{
					Content = mainPage,
					Title = "Home",
					Route = "MainPage"
				});
			}
		}
	}
}
