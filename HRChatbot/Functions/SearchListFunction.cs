using HRChatbot.Data;
using System.Text.Json;

namespace HRChatbot.Functions;

public class SearchListFunction : ICallableFunction
{
    public string Name => "search_list";
    
    public string Description => "Searches for a string in the list of items. Returns matching items.";

    public object Parameters => new
    {
        type = "object",
        properties = new
        {
            searchTerm = new
            {
                type = "string",
                description = "The string to search for in the list"
            },
            caseSensitive = new
            {
                type = "boolean",
                description = "Whether the search should be case sensitive. Default is false.",
                @default = false
            }
        },
        required = new[] { "searchTerm" }
    };

    public Task<object> ExecuteAsync(dynamic arguments, DataContext dataContext)
    {
        try
        {
            string searchTerm = arguments.searchTerm?.ToString() ?? string.Empty;
            bool caseSensitive = arguments.caseSensitive?.ToString().ToLower() == "true" || arguments.caseSensitive == true;

            var matchingItems = dataContext.Items
                .Where(item => caseSensitive 
                    ? item.Contains(searchTerm) 
                    : item.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .ToList();

            return Task.FromResult<object>(new
            {
                found = matchingItems.Count,
                items = matchingItems
            });
        }
        catch (Exception ex)
        {
            return Task.FromResult<object>(new
            {
                error = $"Error searching list: {ex.Message}"
            });
        }
    }
}
