using System.ComponentModel.DataAnnotations;

namespace ExpenseTrackerAPI.DTOs;

public class ExpenseListResult
{
    public List<ExpenseResult> Expenses { get; set; }
    [EmailAddress] public string Email { get; set; }
    public decimal TotalSpent { get; set; }

    public ExpenseListResult() {}

    public ExpenseListResult(IEnumerable<Expense> expenses, string email)
    {
        Email = email;
        Expenses = expenses.Select(e => ExpenseResult.FromExpense(e)).ToList();
        TotalSpent = Expenses.Sum(e => e.Amount);
    }
}