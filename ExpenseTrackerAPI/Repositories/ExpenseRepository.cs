using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ExpenseTrackerAPI.Repositories;

public interface IExpenseRepository
{
    Task<Expense> AddExpenseAsync(Expense expense, string userEmail);
    Task<List<Expense>> GetUserExpensesAsync(string email);
    Task<Expense> UpdateExpenseAsync(Expense expense);
    Task<bool> DeleteExpenseAsync(Expense expense);
}

public class ExpenseRepository(IConfiguration configuration) : IExpenseRepository
{
    public async Task<Expense> AddExpenseAsync(Expense expense, string userEmail)
    {
        var param =
            new
            {
                expense.Description,
                expense.Amount,
                ExpenseDate = expense.Date,
                expense.Category.CategoryId,
                UserEmail = userEmail
            };

        await using var conn = new SqlConnection(configuration.GetConnectionString("ExpenseTracker"));
        await conn.OpenAsync();

        var query =
            await conn.QueryAsync<Expense, Category, Expense>(
                "sp_AddExpense",
                (expense, category) =>
                {
                    expense.Category = category;
                    return expense;
                },
                param,
                splitOn: "CategoryId",
                commandType: CommandType.StoredProcedure);

        var newExpense = query.Single();

        return newExpense;
    }

    public async Task<List<Expense>> GetUserExpensesAsync(string email)
    {
        await using var conn = new SqlConnection(configuration.GetConnectionString("ExpenseTracker"));
        await conn.OpenAsync();
        
        var expenses = await conn.QueryAsync<Expense, Category, Expense>(
            "sp_GetExpensesFromUser",
            (expense, category) =>
            {
                expense.Category = category;
                return expense;
            },
            new { UserEmail = email },
            splitOn: "CategoryId",
            commandType: CommandType.StoredProcedure);

        return expenses.ToList();
    }

    public async Task<Expense> UpdateExpenseAsync(Expense expense)
    {
        await using var conn = new SqlConnection(configuration.GetConnectionString("ExpenseTracker"));
        await conn.OpenAsync();

        throw new NotImplementedException();
    }

    public async Task<bool> DeleteExpenseAsync(Expense expense)
    {
        await using var conn = new SqlConnection(configuration.GetConnectionString("ExpenseTracker"));
        await conn.OpenAsync();

        throw new NotImplementedException();
    }
}