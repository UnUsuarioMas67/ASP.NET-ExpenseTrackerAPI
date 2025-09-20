namespace ExpenseTrackerAPI.DTOs;

public class ExpenseResult
{
    public int Id { get; set; }
    public string Description { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string Category { get; set; }

    public static ExpenseResult FromExpense(Expense expense)
    {
        var result = new ExpenseResult
        {
            Id = expense.ExpenseId,
            Description = expense.Description,
            Amount = expense.Amount,
            Date = expense.Date,
            Category = expense.Category.CategoryName,
        };

        return result;
    }
}