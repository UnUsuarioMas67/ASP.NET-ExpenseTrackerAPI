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
}