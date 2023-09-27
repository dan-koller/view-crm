using System.ComponentModel.DataAnnotations;

namespace View.WebApi.Data;

public class UpdateAccountRequest
{
    [Required(ErrorMessage = "Email is required.")]
    public string Email { get; set; } = null!;
    [Required(ErrorMessage = "Your name cannot be empty.")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Current password is required.")]
    public string CurrentPassword { get; set; } = null!;
    public string? NewPassword { get; set; }
}
