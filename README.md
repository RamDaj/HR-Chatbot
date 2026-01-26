# HR-Chatbot

AI HR Chatbot - A .NET MAUI desktop application with OpenAI API integration and function calling capabilities.

## Features

- **Chat Interface**: Clean, modern chat UI for interacting with OpenAI's GPT models
- **API Key Management**: Secure storage of OpenAI API keys using MAUI's SecureStorage
- **Function Calling**: Extensible system for the AI to call custom methods that modify data structures
- **Example Functions**: 
  - `search_list`: Search for strings in a list
  - `sort_list`: Sort lists in ascending or descending order
- **Session Data**: Global `DataContext` for managing data structures (resets on app restart)

## Prerequisites

- .NET 10.0 SDK or later
- MAUI workload installed (`dotnet workload install maui`)
- OpenAI API key ([Get one here](https://platform.openai.com/api-keys))

## Setup

1. Clone or download this repository
2. Open the solution in Visual Studio or your preferred IDE
3. Build the project:
   ```bash
   dotnet build HRChatbot/HRChatbot.csproj -f net10.0-windows10.0.19041.0
   ```
4. Run the application
5. Enter your OpenAI API key in the app and click "Save Key"
6. Start chatting!

## Building for Windows

To create a Windows executable (.exe):

```bash
dotnet publish HRChatbot/HRChatbot.csproj -f net10.0-windows10.0.19041.0 -c Release -r win-x64 --self-contained
```

The executable will be in: `HRChatbot/bin/Release/net10.0-windows10.0.19041.0/win-x64/publish/`

## Adding Custom Functions

To add your own callable functions:

1. Create a new class implementing `ICallableFunction`:

```csharp
using HRChatbot.Data;
using HRChatbot.Functions;

public class MyCustomFunction : ICallableFunction
{
    public string Name => "my_custom_function";
    
    public string Description => "Does something custom with data";

    public object Parameters => new
    {
        type = "object",
        properties = new
        {
            param1 = new
            {
                type = "string",
                description = "Description of parameter"
            }
        },
        required = new[] { "param1" }
    };

    public Task<object> ExecuteAsync(dynamic arguments, DataContext dataContext)
    {
        // Your custom logic here
        // Access dataContext.Items or add your own data structures
        var result = new { success = true, message = "Done!" };
        return Task.FromResult<object>(result);
    }
}
```

2. Register the function in `MauiProgram.cs`:

```csharp
functionRegistry.RegisterFunction(new MyCustomFunction());
```

3. Add any additional data structures to `DataContext.cs` as needed

## Project Structure

```
HRChatbot/
├── Models/              # Data models (ChatMessage, FunctionDefinition)
├── Data/                # DataContext for global data structures
├── Functions/           # Callable functions (ICallableFunction implementations)
├── Services/            # Business logic (OpenAI service, FunctionRegistry)
├── ViewModels/          # MVVM view models
├── Converters/          # XAML value converters
└── Platforms/           # Platform-specific code
```

## Architecture

The app uses:
- **MVVM Pattern**: ViewModels manage UI state and business logic
- **Dependency Injection**: Services registered in `MauiProgram.cs`
- **Function Calling**: OpenAI API function calling for extensible AI capabilities
- **Secure Storage**: API keys stored securely using MAUI SecureStorage

## License

MIT License - See LICENSE file for details
