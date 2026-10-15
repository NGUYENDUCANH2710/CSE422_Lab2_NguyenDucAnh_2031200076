using System.ComponentModel.DataAnnotations;

namespace DeviceManagement.Models;

public class Device
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Device name is required.")]
    [StringLength(100, ErrorMessage = "Device name cannot exceed 100 characters.")]
    [Display(Name = "Device Name")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Device code is required.")]
    [StringLength(50, ErrorMessage = "Device code cannot exceed 50 characters.")]
    [RegularExpression(@"^[A-Za-z0-9\-_]+$", ErrorMessage = "Device code can only contain letters, numbers, hyphens, and underscores.")]
    [Display(Name = "Device Code")]
    public string Code { get; set; } = string.Empty;

    [Required(ErrorMessage = "Device category is required.")]
    [Display(Name = "Category")]
    public int CategoryId { get; set; }

    [Display(Name = "Category")]
    public virtual DeviceCategory? Category { get; set; }

    [Required(ErrorMessage = "Status is required.")]
    [Display(Name = "Status")]
    public DeviceStatus Status { get; set; } = DeviceStatus.InUse;

    [Required(ErrorMessage = "Date of entry is required.")]
    [DataType(DataType.Date)]
    [Display(Name = "Date of Entry")]
    public DateTime EntryDate { get; set; } = DateTime.Today;

    [Display(Name = "Assigned User")]
    public int? UserId { get; set; }

    [Display(Name = "Assigned User")]
    public virtual User? User { get; set; }

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
    [Display(Name = "Description / Notes")]
    public string? Description { get; set; }
}
