using System.Diagnostics;
using DeviceManagement.Data;
using DeviceManagement.Models;
using DeviceManagement.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DeviceManagement.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _context;

    public HomeController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var totalDevices = await _context.Devices.CountAsync();
        var totalCategories = await _context.Categories.CountAsync();
        var totalUsers = await _context.Users.CountAsync();

        var inUse = await _context.Devices.CountAsync(d => d.Status == DeviceStatus.InUse);
        var broken = await _context.Devices.CountAsync(d => d.Status == DeviceStatus.Broken);
        var underMaintenance = await _context.Devices.CountAsync(d => d.Status == DeviceStatus.UnderMaintenance);

        var recentDevices = await _context.Devices
            .Include(d => d.Category)
            .Include(d => d.User)
            .OrderByDescending(d => d.EntryDate)
            .Take(5)
            .ToListAsync();

        var categorySummaries = await _context.Categories
            .Select(c => new DeviceCategorySummary
            {
                CategoryId = c.Id,
                CategoryName = c.Name,
                DeviceCount = c.Devices.Count
            })
            .OrderByDescending(s => s.DeviceCount)
            .ToListAsync();

        var viewModel = new DashboardViewModel
        {
            TotalDevices = totalDevices,
            TotalCategories = totalCategories,
            TotalUsers = totalUsers,
            DevicesInUse = inUse,
            DevicesBroken = broken,
            DevicesUnderMaintenance = underMaintenance,
            RecentDevices = recentDevices,
            CategorySummaries = categorySummaries
        };

        return View(viewModel);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
