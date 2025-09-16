namespace ExpenseTrackerAPI;

public class Expense
{
    public int ExpenseId { get; set; }
    public string Description { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public Category Category { get; set; }
}