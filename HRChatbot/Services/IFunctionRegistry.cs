using HRChatbot.Functions;
using HRChatbot.Models;

namespace HRChatbot.Services;

public interface IFunctionRegistry
{
    void RegisterFunction(ICallableFunction function);
    ICallableFunction? GetFunction(string name);
    List<FunctionDefinition> GetAllFunctionSchemas();
    Task<object> ExecuteFunctionAsync(string functionName, dynamic arguments);
}
