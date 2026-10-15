using System.ComponentModel.DataAnnotations;

namespace DeviceManagement.Models;

public enum DeviceStatus
{
    [Display(Name = "In use")]
    InUse = 1,

    [Display(Name = "Broken")]
    Broken = 2,

    [Display(Name = "Under maintenance")]
    UnderMaintenance = 3
}
