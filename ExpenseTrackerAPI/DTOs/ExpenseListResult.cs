using System.ComponentModel.DataAnnotations;

namespace ExpenseTrackerAPI.DTOs;

public class ExpenseListResult
{
    public List<ExpenseResult> Expenses { get; set; }
    [EmailAddress] public UserResult User { get; set; }
    public decimal TotalSpent { get; set; }

    public ExpenseListResult()
    {
    }

    public ExpenseListResult(IEnumerable<Expense> expenses, string email, string username)
    {
        User = new UserResult { Email = email, Username = username };
        Expenses = expenses.Select(ExpenseResult.FromExpense).ToList();
        TotalSpent = Expenses.Sum(e => e.Amount);
    }
}

public class UserResult
{
    [EmailAddress] public string Email { get; set; }
    public string Username { get; set; }
}