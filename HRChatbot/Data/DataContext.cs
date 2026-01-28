using System.Collections.Generic;
using HRChatbot.Models;

namespace HRChatbot.Data;

public class DataContext
{
    // Employee data keyed by employee Id
    public Dictionary<int, Employee> EmployeeData { get; set; } = new();
}
