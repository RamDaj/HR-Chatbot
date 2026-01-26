using HRChatbot.Data;
using HRChatbot.Functions;
using HRChatbot.Models;
using System.Text.Json;

namespace HRChatbot.Services;

public class FunctionRegistry : IFunctionRegistry
{
    private readonly Dictionary<string, ICallableFunction> _functions = new();
    private readonly DataContext _dataContext;

    public FunctionRegistry(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public void RegisterFunction(ICallableFunction function)
    {
        _functions[function.Name] = function;
    }

    public ICallableFunction? GetFunction(string name)
    {
        return _functions.TryGetValue(name, out var function) ? function : null;
    }

    public List<FunctionDefinition> GetAllFunctionSchemas()
    {
        return _functions.Values.Select(f => new FunctionDefinition
        {
            Name = f.Name,
            Description = f.Description,
            Parameters = f.Parameters
        }).ToList();
    }

    public async Task<object> ExecuteFunctionAsync(string functionName, dynamic arguments)
    {
        var function = GetFunction(functionName);
        if (function == null)
        {
            return new { error = $"Function '{functionName}' not found" };
        }

        try
        {
            return await function.ExecuteAsync(arguments, _dataContext);
        }
        catch (Exception ex)
        {
            return new { error = $"Error executing function '{functionName}': {ex.Message}" };
        }
    }
}
