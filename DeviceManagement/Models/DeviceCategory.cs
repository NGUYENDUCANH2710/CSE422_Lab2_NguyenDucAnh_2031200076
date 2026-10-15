using System.ComponentModel.DataAnnotations;

namespace DeviceManagement.Models;

public class DeviceCategory
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Category name is required.")]
    [StringLength(100, ErrorMessage = "Category name cannot exceed 100 characters.")]
    [Display(Name = "Category Name")]
    public string Name { get; set; } = string.Empty;

    [StringLength(255, ErrorMessage = "Description cannot exceed 255 characters.")]
    [Display(Name = "Description")]
    public string? Description { get; set; }

    // Navigation property
    public virtual ICollection<Device> Devices { get; set; } = new List<Device>();
}
