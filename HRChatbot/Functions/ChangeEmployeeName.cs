using HRChatbot.Data;

namespace HRChatbot.Functions;

public class ChangeEmployeeName : ICallableFunction
{
    public string Name => "change_employee_name";

    public string Description => "Changes the first and last name of an employee in the employeeData dictionary.";

    public object Parameters => new
    {
        type = "object",
        properties = new
        {
            employeeId = new
            {
                type = "integer",
                description = "The unique ID of the employee whose name will be changed."
            },
            firstName = new
            {
                type = "string",
                description = "The new first name for the employee."
            },
            lastName = new
            {
                type = "string",
                description = "The new last name for the employee."
            }
        },
        required = new[] { "employeeId", "firstName", "lastName" }
    };

    public Task<object> ExecuteAsync(dynamic arguments, DataContext dataContext)
    {
        try
        {
            int employeeId = (int)arguments.employeeId;
            string firstName = arguments.firstName?.ToString() ?? string.Empty;
            string lastName = arguments.lastName?.ToString() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
            {
                return Task.FromResult<object>(new
                {
                    error = "First name and last name cannot be empty."
                });
            }

            // Use strongly-typed EmployeeData dictionary on DataContext
            if (!dataContext.EmployeeData.TryGetValue(employeeId, out var employee))
            {
                return Task.FromResult<object>(new
                {
                    error = $"Employee with ID {employeeId} was not found in EmployeeData."
                });
            }

            employee.FirstName = firstName;
            employee.LastName = lastName;
            dataContext.EmployeeData[employeeId] = employee;

            return Task.FromResult<object>(new
            {
                success = true,
                employeeId,
                firstName,
                lastName
            });
        }
        catch (Exception ex)
        {
            return Task.FromResult<object>(new
            {
                error = $"Error changing employee name: {ex.Message}"
            });
        }
    }
}

