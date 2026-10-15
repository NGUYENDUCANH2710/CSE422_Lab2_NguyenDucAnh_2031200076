using DeviceManagement.Models;

namespace DeviceManagement.Data;

public static class DbInitializer
{
    public static void Initialize(AppDbContext context)
    {
        // Ensure database is created
        context.Database.EnsureCreated();

        // Check if DB already seeded
        if (context.Categories.Any())
        {
            return; // DB has been seeded
        }

        // Seed Categories
        var categories = new List<DeviceCategory>
        {
            new DeviceCategory { Name = "Computer", Description = "Laptops, Desktops, and Workstations" },
            new DeviceCategory { Name = "Printer", Description = "Laser, Inkjet, and Multifunction printers" },
            new DeviceCategory { Name = "Phone", Description = "Smartphones and Office IP Phones" },
            new DeviceCategory { Name = "Monitor", Description = "Desktop displays and presentation monitors" },
            new DeviceCategory { Name = "Network Equipment", Description = "Routers, Switches, and Access Points" }
        };

        context.Categories.AddRange(categories);
        context.SaveChanges();

        // Seed Users (Employees)
        var users = new List<User>
        {
            new User
            {
                FullName = "Nguyen Duc Anh",
                Email = "ducanh.nguyen@company.com",
                PhoneNumber = "0987654321",
                Department = "IT Engineering"
            },
            new User
            {
                FullName = "Tran Thi Mai",
                Email = "mai.tran@company.com",
                PhoneNumber = "0912345678",
                Department = "Human Resources"
            },
            new User
            {
                FullName = "Le Hoang Nam",
                Email = "nam.le@company.com",
                PhoneNumber = "0908123456",
                Department = "Marketing"
            },
            new User
            {
                FullName = "Pham Minh Tuan",
                Email = "tuan.pham@company.com",
                PhoneNumber = "0934567890",
                Department = "Finance & Accounting"
            }
        };

        context.Users.AddRange(users);
        context.SaveChanges();

        // Seed Devices
        var devices = new List<Device>
        {
            new Device
            {
                Name = "Dell XPS 15 9530",
                Code = "DEV-001",
                CategoryId = categories[0].Id, // Computer
                Status = DeviceStatus.InUse,
                EntryDate = DateTime.Today.AddMonths(-6),
                UserId = users[0].Id,
                Description = "High-performance laptop for senior developer"
            },
            new Device
            {
                Name = "HP LaserJet Pro MFP 4103fdw",
                Code = "DEV-002",
                CategoryId = categories[1].Id, // Printer
                Status = DeviceStatus.InUse,
                EntryDate = DateTime.Today.AddMonths(-4),
                UserId = users[1].Id,
                Description = "Office multi-function monochrome laser printer"
            },
            new Device
            {
                Name = "iPhone 15 Pro Max 256GB",
                Code = "DEV-003",
                CategoryId = categories[2].Id, // Phone
                Status = DeviceStatus.InUse,
                EntryDate = DateTime.Today.AddMonths(-2),
                UserId = users[2].Id,
                Description = "Testing and media production phone"
            },
            new Device
            {
                Name = "LG UltraWide 34WN80C-B",
                Code = "DEV-004",
                CategoryId = categories[3].Id, // Monitor
                Status = DeviceStatus.UnderMaintenance,
                EntryDate = DateTime.Today.AddMonths(-8),
                UserId = null,
                Description = "Display flickers intermittently, sent for power board repair"
            },
            new Device
            {
                Name = "Cisco Catalyst 1000 Switch",
                Code = "DEV-005",
                CategoryId = categories[4].Id, // Network Equipment
                Status = DeviceStatus.InUse,
                EntryDate = DateTime.Today.AddYears(-1),
                UserId = users[0].Id,
                Description = "Main rack server room switch"
            },
            new Device
            {
                Name = "Canon Pixma G3010",
                Code = "DEV-006",
                CategoryId = categories[1].Id, // Printer
                Status = DeviceStatus.Broken,
                EntryDate = DateTime.Today.AddYears(-2),
                UserId = null,
                Description = "Print head blocked, waiting for replacement parts"
            },
            new Device
            {
                Name = "MacBook Pro 14 M3 Pro",
                Code = "DEV-007",
                CategoryId = categories[0].Id, // Computer
                Status = DeviceStatus.InUse,
                EntryDate = DateTime.Today.AddMonths(-1),
                UserId = users[3].Id,
                Description = "Assigned for financial data modeling and reports"
            }
        };

        context.Devices.AddRange(devices);
        context.SaveChanges();
    }
}
