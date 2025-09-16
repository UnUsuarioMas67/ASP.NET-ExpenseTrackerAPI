using System.ComponentModel.DataAnnotations;

namespace ExpenseTrackerAPI.DTOs;

public class ExpenseListResult
{
    public List<Expense> Expenses { get; set; }
    [EmailAddress] public string Email { get; set; }
    public decimal TotalSpent { get; set; }
}