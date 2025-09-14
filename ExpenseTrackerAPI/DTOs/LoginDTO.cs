using System.ComponentModel.DataAnnotations;

namespace ExpenseTrackerAPI.DTOs;

public class LoginDTO
{
    [EmailAddress, Required] public string Email { get; set; }
    [MinLength(8), Required] public string Password { get; set; }
}