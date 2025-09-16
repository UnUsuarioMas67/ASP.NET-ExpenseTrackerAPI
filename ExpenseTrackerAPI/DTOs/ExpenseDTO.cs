using System.ComponentModel.DataAnnotations;

namespace ExpenseTrackerAPI.DTOs;

public class ExpenseDTO
{
    [Required, StringLength(200)] public string Description { get; set; }
    [Required, DataType(DataType.Currency), Range(0.01, double.MaxValue)] public decimal Amount { get; set; }
    [Required, DataType(DataType.Date)] public DateTime Date { get; set; }
    [Required, StringLength(25)] public string Category { get; set; }
}