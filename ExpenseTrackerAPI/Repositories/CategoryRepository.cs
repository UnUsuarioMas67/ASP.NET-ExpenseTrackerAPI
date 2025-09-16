using Dapper;
using Microsoft.Data.SqlClient;

namespace ExpenseTrackerAPI.Repositories;

public interface ICategoryRepository
{
    Task<Category?> GetCategoryByIdAsync(string id);
    Task<List<Category>> GetAllCategoriesAsync();
}

public class CategoryRepository(IConfiguration configuration) : ICategoryRepository
{
    /*
     * -- VALID CATEGORIES IDs --
     * clothing
     * electronics
     * groceries
     * health
     * leisure
     * other
     * utilities
     */
    
    public async Task<Category?> GetCategoryByIdAsync(string id)
    {
        var sql = "SELECT CategoryId, CategoryName FROM ExpenseCategories Where CategoryId = @id";

        await using var conn = new SqlConnection(configuration.GetConnectionString("ExpenseTracker"));
        await conn.OpenAsync();

        var category = await conn.QuerySingleOrDefaultAsync<Category>(sql, new { id });
        return category;
    }

    public async Task<List<Category>> GetAllCategoriesAsync()
    {
        var sql = "SELECT CategoryId, CategoryName FROM ExpenseCategories";

        await using var conn = new SqlConnection(configuration.GetConnectionString("ExpenseTracker"));
        await conn.OpenAsync();

        var categories = await conn.QueryAsync<Category>(sql);
        return categories.ToList();
    }
}