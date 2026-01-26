using HRChatbot.Data;

namespace HRChatbot.Functions;

public class SortListFunction : ICallableFunction
{
    public string Name => "sort_list";
    
    public string Description => "Sorts the list of items. Can sort ascending or descending.";

    public object Parameters => new
    {
        type = "object",
        properties = new
        {
            ascending = new
            {
                type = "boolean",
                description = "Whether to sort in ascending order. Default is true.",
                @default = true
            }
        }
    };

    public Task<object> ExecuteAsync(dynamic arguments, DataContext dataContext)
    {
        try
        {
            bool ascending = arguments.ascending?.ToString().ToLower() != "false" && arguments.ascending != false;

            if (ascending)
            {
                dataContext.Items.Sort();
            }
            else
            {
                dataContext.Items.Sort((a, b) => string.Compare(b, a, StringComparison.Ordinal));
            }

            return Task.FromResult<object>(new
            {
                success = true,
                message = $"List sorted {(ascending ? "ascending" : "descending")}",
                itemCount = dataContext.Items.Count
            });
        }
        catch (Exception ex)
        {
            return Task.FromResult<object>(new
            {
                error = $"Error sorting list: {ex.Message}"
            });
        }
    }
}
