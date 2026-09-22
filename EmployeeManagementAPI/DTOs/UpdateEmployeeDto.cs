using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementAPI.DTOs;

public class UpdateEmployeeDto
{
    [Required]
    [StringLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Department { get; set; } = string.Empty;

    [Range(0, 100000000)]
    public decimal Salary { get; set; }
}