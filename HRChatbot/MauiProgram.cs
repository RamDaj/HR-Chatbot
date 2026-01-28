using HRChatbot.Data;
using HRChatbot.Functions;
using HRChatbot.Services;
using HRChatbot.ViewModels;
using Microsoft.Extensions.Logging;

namespace HRChatbot;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		// Register services
		var dataContext = new DataContext();
		builder.Services.AddSingleton(dataContext);
		
		var functionRegistry = new FunctionRegistry(dataContext);
		
		// Register example functions
		functionRegistry.RegisterFunction(new ChangeEmployeeName());
		
		builder.Services.AddSingleton<IFunctionRegistry>(functionRegistry);
		
		// Register OpenAI service factory
		builder.Services.AddSingleton<IOpenAIService>(serviceProvider =>
		{
			var registry = serviceProvider.GetRequiredService<IFunctionRegistry>();
			// Try to load API key from secure storage
			var apiKey = Task.Run(async () => await SecureStorage.GetAsync("openai_api_key")).Result;
			return new OpenAIService(registry, apiKey);
		});
		
		// Register ViewModels
		builder.Services.AddTransient<ChatViewModel>();
		builder.Services.AddTransient<MainPage>();
		
		// Register AppShell with factory to inject services
		builder.Services.AddSingleton<AppShell>(serviceProvider => 
		{
			var shell = new AppShell(serviceProvider);
			return shell;
		});

		return builder.Build();
	}
}
