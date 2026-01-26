using Microsoft.Extensions.DependencyInjection;

namespace HRChatbot;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
	}
	
	protected override void OnHandlerChanged()
	{
		base.OnHandlerChanged();
		
		// Set MainPage once Handler is available
		if (Handler?.MauiContext?.Services != null && MainPage == null)
		{
			var shell = Handler.MauiContext.Services.GetService<AppShell>();
			if (shell != null)
			{
				MainPage = shell;
			}
		}
	}
}