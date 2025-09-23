using System.Security.Claims;
using ExpenseTrackerAPI.DTOs;
using ExpenseTrackerAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTrackerAPI.Controllers;

[ApiController, Route("api/expenses"), Authorize]
public class ExpenseController(ICategoryRepository categoryRepository, IExpenseRepository expenseRepository)
    : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Expense))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(ExpenseRequest request)
    {
        var email = HttpContext.User.FindFirstValue(ClaimTypes.Email)!;

        var category = await categoryRepository.GetCategoryByIdAsync(request.Category);
        if (category == null)
            return BadRequest(new { Message = "Invalid category" });

        var expense = new Expense
        {
            Description = request.Description,
            Amount = request.Amount,
            Date = request.Date,
            Category = category
        };

        var addedExpense = await expenseRepository.AddExpenseAsync(expense, email);
        return Ok(ExpenseResult.FromExpense(addedExpense));
    }

    [HttpGet]
    [HttpGet("{date1}")]
    [HttpGet("{date1}/{date2}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ExpenseListResult))]
    public async Task<IActionResult> Get([FromQuery] GetEndpointParams param, DateOnly? date1 = null,
        DateOnly? date2 = null)
    {
        var email = HttpContext.User.FindFirstValue(ClaimTypes.Email)!;
        var expenses = await expenseRepository.GetUserExpensesAsync(email);

        var filteredExpenses = expenses.Where(e =>
        {
            var dateOnly = DateOnly.FromDateTime(e.Date);
            var dateOnlyNow = DateOnly.FromDateTime(DateTime.Now);

            if (date1.HasValue && date2.HasValue)
                return dateOnly >= date1 && dateOnly <= date2;
            if (date1.HasValue)
                return dateOnly >= date1;

            return param.DateFilter switch
            {
                DateFilter.PastWeek => dateOnly >= dateOnlyNow.AddDays(-7),
                DateFilter.PastMonth => dateOnly >= dateOnlyNow.AddMonths(-1),
                DateFilter.Last3Months => dateOnly >= dateOnlyNow.AddMonths(-3),
                _ => true
            };
        }).Where(e => e.Description.Contains(param.SearchString, StringComparison.CurrentCultureIgnoreCase));

        var orderedExpenses = filteredExpenses.OrderByDescending(object (e) =>
        {
            return param.SortBy switch
            {
                SortOption.Date => e.Date,
                SortOption.Amount => e.Amount,
                _ => throw new ArgumentOutOfRangeException()
            };
        });

        var result = new ExpenseListResult(orderedExpenses, email);

        return Ok(result);
    }

    [HttpDelete, Route("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var email = HttpContext.User.FindFirstValue(ClaimTypes.Email)!;
        var expenses = await expenseRepository.GetUserExpensesAsync(email);

        if (!expenses.Exists(e => e.ExpenseId == id))
            return NotFound();

        await expenseRepository.DeleteExpenseAsync(id);
        return NoContent();
    }

    [HttpPut, Route("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Expense))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, ExpenseRequest request)
    {
        var email = HttpContext.User.FindFirstValue(ClaimTypes.Email)!;
        var expenses = await expenseRepository.GetUserExpensesAsync(email);
        var expense = expenses.FirstOrDefault(e => e.ExpenseId == id);

        if (expense == null)
            return NotFound();

        if (expense.Category.CategoryId != request.Category)
        {
            var newCategory = await categoryRepository.GetCategoryByIdAsync(request.Category);
            if (newCategory == null)
                return BadRequest(new { Message = "Invalid category" });

            expense.Category = newCategory;
        }

        expense.Description = request.Description;
        expense.Amount = request.Amount;
        expense.Date = request.Date;

        var updatedExpense = await expenseRepository.UpdateExpenseAsync(expense);
        return Ok(ExpenseResult.FromExpense(updatedExpense));
    }
}