namespace DeviceManagement.Models.ViewModels;

public class DashboardViewModel
{
    public int TotalDevices { get; set; }
    public int TotalCategories { get; set; }
    public int TotalUsers { get; set; }

    public int DevicesInUse { get; set; }
    public int DevicesBroken { get; set; }
    public int DevicesUnderMaintenance { get; set; }

    public IEnumerable<Device> RecentDevices { get; set; } = new List<Device>();
    public IEnumerable<DeviceCategorySummary> CategorySummaries { get; set; } = new List<DeviceCategorySummary>();
}

public class DeviceCategorySummary
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public int DeviceCount { get; set; }
}
