using DeviceManagement.Data;
using DeviceManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DeviceManagement.Controllers;

public class UsersController : Controller
{
    private readonly AppDbContext _context;

    public UsersController(AppDbContext context)
    {
        _context = context;
    }

    // GET: Users
    public async Task<IActionResult> Index()
    {
        var users = await _context.Users
            .Include(u => u.Devices)
            .OrderBy(u => u.FullName)
            .ToListAsync();

        return View(users);
    }

    // GET: Users/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var user = await _context.Users
            .Include(u => u.Devices)
                .ThenInclude(d => d.Category)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }

    // GET: Users/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Users/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,FullName,Email,PhoneNumber,Department")] User user)
    {
        // Validation: Unique Email
        if (await _context.Users.AnyAsync(u => u.Email.ToLower() == user.Email.Trim().ToLower()))
        {
            ModelState.AddModelError("Email", "A user with this email address already exists.");
        }

        if (ModelState.IsValid)
        {
            user.FullName = user.FullName.Trim();
            user.Email = user.Email.Trim().ToLowerInvariant();
            user.PhoneNumber = user.PhoneNumber.Trim();
            _context.Add(user);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"User '{user.FullName}' created successfully!";
            return RedirectToAction(nameof(Index));
        }

        return View(user);
    }

    // GET: Users/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }

    // POST: Users/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,FullName,Email,PhoneNumber,Department")] User user)
    {
        if (id != user.Id)
        {
            return NotFound();
        }

        // Validation: Unique Email (excluding current user)
        if (await _context.Users.AnyAsync(u => u.Id != id && u.Email.ToLower() == user.Email.Trim().ToLower()))
        {
            ModelState.AddModelError("Email", "Another user with this email address already exists.");
        }

        if (ModelState.IsValid)
        {
            try
            {
                user.FullName = user.FullName.Trim();
                user.Email = user.Email.Trim().ToLowerInvariant();
                user.PhoneNumber = user.PhoneNumber.Trim();
                _context.Update(user);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"User '{user.FullName}' updated successfully!";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await UserExists(user.Id))
                {
                    return NotFound();
                }
                throw;
            }
            return RedirectToAction(nameof(Index));
        }

        return View(user);
    }

    // GET: Users/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var user = await _context.Users
            .Include(u => u.Devices)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }

    // POST: Users/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var user = await _context.Users
            .Include(u => u.Devices)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user != null)
        {
            // Note: FK configured with OnDelete(SetNull), so any devices assigned to this user will have UserId set to null.
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"User '{user.FullName}' deleted successfully!";
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task<bool> UserExists(int id)
    {
        return await _context.Users.AnyAsync(e => e.Id == id);
    }
}
