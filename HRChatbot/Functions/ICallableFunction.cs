using HRChatbot.Data;

namespace HRChatbot.Functions;

public interface ICallableFunction
{
    string Name { get; }
    string Description { get; }
    object Parameters { get; }
    
    Task<object> ExecuteAsync(dynamic arguments, DataContext dataContext);
}
