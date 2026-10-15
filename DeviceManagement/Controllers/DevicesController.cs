using DeviceManagement.Data;
using DeviceManagement.Models;
using DeviceManagement.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DeviceManagement.Controllers;

public class DevicesController : Controller
{
    private readonly AppDbContext _context;

    public DevicesController(AppDbContext context)
    {
        _context = context;
    }

    // GET: Devices (with Search and Filter)
    public async Task<IActionResult> Index(string? searchTerm, int? categoryId, DeviceStatus? status, int? userId)
    {
        // Start with all devices and eager load relations
        var query = _context.Devices
            .Include(d => d.Category)
            .Include(d => d.User)
            .AsQueryable();

        // 1. Search by device name or device code
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var search = searchTerm.Trim().ToLower();
            query = query.Where(d => d.Name.ToLower().Contains(search) || d.Code.ToLower().Contains(search));
        }

        // 2. Filter by category
        if (categoryId.HasValue && categoryId.Value > 0)
        {
            query = query.Where(d => d.CategoryId == categoryId.Value);
        }

        // 3. Filter by status
        if (status.HasValue)
        {
            query = query.Where(d => d.Status == status.Value);
        }

        // 4. Filter by user (optional)
        if (userId.HasValue && userId.Value > 0)
        {
            query = query.Where(d => d.UserId == userId.Value);
        }

        var devices = await query.OrderByDescending(d => d.EntryDate).ToListAsync();

        // Build View Model
        var categories = await _context.Categories.OrderBy(c => c.Name).ToListAsync();
        var users = await _context.Users.OrderBy(u => u.FullName).ToListAsync();

        var viewModel = new DeviceFilterViewModel
        {
            SearchTerm = searchTerm,
            CategoryId = categoryId,
            Status = status,
            UserId = userId,
            Devices = devices,
            TotalCount = devices.Count,
            CategoryList = new SelectList(categories, "Id", "Name", categoryId),
            StatusList = new SelectList(Enum.GetValues<DeviceStatus>().Select(e => new
            {
                Id = (int)e,
                Name = e switch
                {
                    DeviceStatus.InUse => "In use",
                    DeviceStatus.Broken => "Broken",
                    DeviceStatus.UnderMaintenance => "Under maintenance",
                    _ => e.ToString()
                }
            }), "Id", "Name", status.HasValue ? (int)status.Value : null),
            UserList = new SelectList(users, "Id", "FullName", userId)
        };

        return View(viewModel);
    }

    // GET: Devices/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var device = await _context.Devices
            .Include(d => d.Category)
            .Include(d => d.User)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (device == null)
        {
            return NotFound();
        }

        return View(device);
    }

    // GET: Devices/Create
    public async Task<IActionResult> Create()
    {
        await PopulateDropDownsAsync();
        return View(new Device { EntryDate = DateTime.Today });
    }

    // POST: Devices/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Name,Code,CategoryId,Status,EntryDate,UserId,Description")] Device device)
    {
        // Validation: Unique Device Code
        if (await _context.Devices.AnyAsync(d => d.Code.ToLower() == device.Code.Trim().ToLower()))
        {
            ModelState.AddModelError("Code", "This device code already exists. Please use a unique code.");
        }

        if (ModelState.IsValid)
        {
            device.Code = device.Code.Trim().ToUpperInvariant();
            device.Name = device.Name.Trim();
            _context.Add(device);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Device '{device.Name}' ({device.Code}) created successfully!";
            return RedirectToAction(nameof(Index));
        }

        await PopulateDropDownsAsync(device.CategoryId, device.UserId);
        return View(device);
    }

    // GET: Devices/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var device = await _context.Devices.FindAsync(id);
        if (device == null)
        {
            return NotFound();
        }

        await PopulateDropDownsAsync(device.CategoryId, device.UserId);
        return View(device);
    }

    // POST: Devices/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Code,CategoryId,Status,EntryDate,UserId,Description")] Device device)
    {
        if (id != device.Id)
        {
            return NotFound();
        }

        // Validation: Unique Device Code (excluding current device)
        if (await _context.Devices.AnyAsync(d => d.Id != id && d.Code.ToLower() == device.Code.Trim().ToLower()))
        {
            ModelState.AddModelError("Code", "Another device already uses this code. Please choose another one.");
        }

        if (ModelState.IsValid)
        {
            try
            {
                device.Code = device.Code.Trim().ToUpperInvariant();
                device.Name = device.Name.Trim();
                _context.Update(device);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Device '{device.Name}' ({device.Code}) updated successfully!";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await DeviceExists(device.Id))
                {
                    return NotFound();
                }
                throw;
            }
            return RedirectToAction(nameof(Index));
        }

        await PopulateDropDownsAsync(device.CategoryId, device.UserId);
        return View(device);
    }

    // GET: Devices/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var device = await _context.Devices
            .Include(d => d.Category)
            .Include(d => d.User)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (device == null)
        {
            return NotFound();
        }

        return View(device);
    }

    // POST: Devices/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var device = await _context.Devices.FindAsync(id);
        if (device != null)
        {
            _context.Devices.Remove(device);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Device '{device.Name}' ({device.Code}) deleted successfully!";
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropDownsAsync(int? selectedCategory = null, int? selectedUser = null)
    {
        var categories = await _context.Categories.OrderBy(c => c.Name).ToListAsync();
        var users = await _context.Users.OrderBy(u => u.FullName).ToListAsync();

        ViewBag.CategoryId = new SelectList(categories, "Id", "Name", selectedCategory);
        ViewBag.UserId = new SelectList(users, "Id", "FullName", selectedUser);
    }

    private async Task<bool> DeviceExists(int id)
    {
        return await _context.Devices.AnyAsync(e => e.Id == id);
    }
}
