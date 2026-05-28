using Microsoft.AspNetCore.Mvc;
using SmartParking.Data;
using SmartParking.Models;

namespace SmartParking.Controllers;

public class AccountController : Controller
{
    private readonly AppDbContext _context;

    public AccountController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Register() => View();

    [HttpPost]
    public async Task<IActionResult> Register(string fullName, string email, string password, string phone)
    {
        if (_context.Users.Any(u => u.Email == email))
        {
            ViewBag.Error = "Email already exists";
            return View();
        }

        var user = new User
        {
            FullName = fullName,
            Email = email,
            Password = password,
            Phone = phone
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        HttpContext.Session.SetInt32("UserId", user.UserId);
        HttpContext.Session.SetString("UserName", user.FullName);

        return RedirectToAction("Index", "Parking");
    }

    public IActionResult Login() => View();

    [HttpPost]
    public IActionResult Login(string email, string password)
    {
        var user = _context.Users.FirstOrDefault(u => u.Email == email && u.Password == password);

        if (user == null)
        {
            ViewBag.Error = "Invalid email or password";
            return View();
        }

        HttpContext.Session.SetInt32("UserId", user.UserId);
        HttpContext.Session.SetString("UserName", user.FullName);

        return RedirectToAction("Index", "Parking");
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }


    public async Task<IActionResult> Settings()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
            return RedirectToAction("Login");

        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return RedirectToAction("Login");

        return View(user);
    }

    [HttpPost]
    public async Task<IActionResult> Settings(string fullName, string phone, string? newPassword)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
            return RedirectToAction("Login");

        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return RedirectToAction("Login");

        user.FullName = fullName;
        user.Phone = phone;

        if (!string.IsNullOrEmpty(newPassword))
            user.Password = newPassword;

        await _context.SaveChangesAsync();

        HttpContext.Session.SetString("UserName", user.FullName);
        ViewBag.Success = "Profile updated successfully!";
        return View(user);
    }
}