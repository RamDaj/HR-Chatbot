using System;
using System.Collections.Generic;

namespace HRChatbot.Models;

public class EmployeeDayOff
{
    /// <summary>
    /// The date of the day off.
    /// </summary>
    public DateOnly Date { get; set; }

    /// <summary>
    /// Whether this day off has been approved.
    /// </summary>
    public bool IsApproved { get; set; }
}

public class Employee
{
    public int Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;
    public DateOnly BirthDate { get; set; }
    public List<EmployeeDayOff> DaysOff { get; set; } = new();
    public string CountryCode { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public int? ManagerId { get; set; }
    public DateOnly HireDate { get; set; }
    public bool IsActive { get; set; } = true;
}

