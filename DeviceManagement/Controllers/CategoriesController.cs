using DeviceManagement.Data;
using DeviceManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DeviceManagement.Controllers;

public class CategoriesController : Controller
{
    private readonly AppDbContext _context;

    public CategoriesController(AppDbContext context)
    {
        _context = context;
    }

    // GET: Categories
    public async Task<IActionResult> Index()
    {
        var categories = await _context.Categories
            .Include(c => c.Devices)
            .OrderBy(c => c.Name)
            .ToListAsync();

        return View(categories);
    }

    // GET: Categories/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var category = await _context.Categories
            .Include(c => c.Devices)
                .ThenInclude(d => d.User)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (category == null)
        {
            return NotFound();
        }

        return View(category);
    }

    // GET: Categories/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Categories/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Name,Description")] DeviceCategory category)
    {
        // Validation: Unique Name
        if (await _context.Categories.AnyAsync(c => c.Name.ToLower() == category.Name.Trim().ToLower()))
        {
            ModelState.AddModelError("Name", "A category with this name already exists.");
        }

        if (ModelState.IsValid)
        {
            category.Name = category.Name.Trim();
            _context.Add(category);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Category '{category.Name}' created successfully!";
            return RedirectToAction(nameof(Index));
        }

        return View(category);
    }

    // GET: Categories/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var category = await _context.Categories.FindAsync(id);
        if (category == null)
        {
            return NotFound();
        }

        return View(category);
    }

    // POST: Categories/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description")] DeviceCategory category)
    {
        if (id != category.Id)
        {
            return NotFound();
        }

        // Validation: Unique Name (excluding current)
        if (await _context.Categories.AnyAsync(c => c.Id != id && c.Name.ToLower() == category.Name.Trim().ToLower()))
        {
            ModelState.AddModelError("Name", "Another category with this name already exists.");
        }

        if (ModelState.IsValid)
        {
            try
            {
                category.Name = category.Name.Trim();
                _context.Update(category);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Category '{category.Name}' updated successfully!";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await CategoryExists(category.Id))
                {
                    return NotFound();
                }
                throw;
            }
            return RedirectToAction(nameof(Index));
        }

        return View(category);
    }

    // GET: Categories/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var category = await _context.Categories
            .Include(c => c.Devices)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (category == null)
        {
            return NotFound();
        }

        return View(category);
    }

    // POST: Categories/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var category = await _context.Categories
            .Include(c => c.Devices)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category != null)
        {
            // Business rule: Prevent deletion if category still has devices
            if (category.Devices.Any())
            {
                TempData["ErrorMessage"] = $"Cannot delete category '{category.Name}' because it contains {category.Devices.Count} device(s). Please reassign or delete these devices first.";
                return RedirectToAction(nameof(Index));
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Category '{category.Name}' deleted successfully!";
        }

        return RedirectToAction(nameof(Index));
    }

    // GET: Categories/Devices/5
    // Requirement: Display a list of devices by category
    public async Task<IActionResult> Devices(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var category = await _context.Categories
            .Include(c => c.Devices)
                .ThenInclude(d => d.User)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category == null)
        {
            return NotFound();
        }

        ViewBag.CategoryName = category.Name;
        ViewBag.CategoryId = category.Id;
        return View(category.Devices.OrderBy(d => d.Name).ToList());
    }

    private async Task<bool> CategoryExists(int id)
    {
        return await _context.Categories.AnyAsync(e => e.Id == id);
    }
}
