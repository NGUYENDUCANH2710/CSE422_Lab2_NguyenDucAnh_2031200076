using Microsoft.AspNetCore.Mvc.Rendering;

namespace DeviceManagement.Models.ViewModels;

public class DeviceFilterViewModel
{
    // Search keyword (matches device name or code)
    public string? SearchTerm { get; set; }

    // Filter by Category
    public int? CategoryId { get; set; }

    // Filter by Status
    public DeviceStatus? Status { get; set; }

    // Filter by User (optional bonus)
    public int? UserId { get; set; }

    // Filtered result list
    public IEnumerable<Device> Devices { get; set; } = new List<Device>();

    // Select lists for dropdown filters
    public SelectList? CategoryList { get; set; }
    public SelectList? StatusList { get; set; }
    public SelectList? UserList { get; set; }

    // Statistics counts for the current filter view
    public int TotalCount { get; set; }
}
