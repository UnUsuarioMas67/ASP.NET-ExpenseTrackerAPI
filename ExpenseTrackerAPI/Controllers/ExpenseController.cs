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
    public async Task<IActionResult> Create(ExpenseDTO dto)
    {
        var email = HttpContext.User.FindFirstValue(ClaimTypes.Email)!;
        
        var category = await categoryRepository.GetCategoryByIdAsync(dto.Category);
        if (category == null)
            return BadRequest(new { Message = "Invalid category" });
        
        var expense = new Expense
        {
            Description = dto.Description,
            Amount = dto.Amount,
            Date = dto.Date,
            Category = category
        };

        var addedExpense = await expenseRepository.AddExpenseAsync(expense, email);
        return Ok(addedExpense);
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ExpenseListResult))]
    public async Task<IActionResult> Get()
    {
        var email = HttpContext.User.FindFirstValue(ClaimTypes.Email)!;
        var expenses = await expenseRepository.GetUserExpensesAsync(email);

        var result = new ExpenseListResult
        {
            Email = email,
            Expenses = expenses,
            TotalSpent = expenses.Sum(e => e.Amount),
        };
        
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
    public async Task<IActionResult> Update(int id, ExpenseDTO dto)
    {
        var email = HttpContext.User.FindFirstValue(ClaimTypes.Email)!;
        var expenses = await expenseRepository.GetUserExpensesAsync(email);
        var expense = expenses.FirstOrDefault(e => e.ExpenseId == id);
        
        if (expense == null)
            return NotFound();

        if (expense.Category.CategoryId != dto.Category)
        {
            var newCategory = await categoryRepository.GetCategoryByIdAsync(dto.Category);
            if (newCategory == null)
                return BadRequest(new { Message = "Invalid category" });
            
            expense.Category = newCategory;
        }
        expense.Description = dto.Description;
        expense.Amount = dto.Amount;
        expense.Date = dto.Date;

        var updatedExpense = await expenseRepository.UpdateExpenseAsync(expense);
        return Ok(updatedExpense);
    }
}