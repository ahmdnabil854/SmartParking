using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartParking.Data;
using SmartParking.Models;

namespace SmartParking.Controllers;

public class AdminController : Controller
{
    private readonly AppDbContext _context;

    public AdminController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        if (HttpContext.Session.GetString("IsAdmin") != "true")
            return RedirectToAction("Login");

        ViewBag.TotalSlots = await _context.Slots.CountAsync();
        ViewBag.Available = await _context.Slots.CountAsync(s => s.Status == "Available");
        ViewBag.Occupied = await _context.Slots.CountAsync(s => s.Status == "Occupied");
        ViewBag.Reserved = await _context.Slots.CountAsync(s => s.Status == "Reserved");
        ViewBag.TotalBookings = await _context.Bookings.CountAsync();
        ViewBag.ActiveBookings = await _context.Bookings.CountAsync(b => b.Status == "Active");
        ViewBag.TotalUsers = await _context.Users.CountAsync();
        ViewBag.Slots = await _context.Slots.Include(s => s.ParkingLot).ToListAsync();

        var bookings = await _context.Bookings
            .Include(b => b.User)
            .Include(b => b.Slot)
            .ThenInclude(s => s!.ParkingLot)
            .OrderByDescending(b => b.BookedAt)
            .ToListAsync();

        return View(bookings);
    }

    public IActionResult Login() => View();

    [HttpPost]
    public IActionResult Login(string username, string password)
    {
        var admin = _context.Admins.FirstOrDefault(a => a.Username == username && a.Password == password);
        if (admin == null)
        {
            ViewBag.Error = "Invalid credentials";
            return View();
        }
        HttpContext.Session.SetString("IsAdmin", "true");
        HttpContext.Session.SetString("AdminName", admin.Username);
        return RedirectToAction("Index");
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Remove("IsAdmin");
        HttpContext.Session.Remove("AdminName");
        return RedirectToAction("Login");
    }

    [HttpPost]
    public async Task<IActionResult> CancelBooking(int bookingId)
    {
        if (HttpContext.Session.GetString("IsAdmin") != "true")
            return RedirectToAction("Login");

        var booking = await _context.Bookings
            .Include(b => b.Slot)
            .FirstOrDefaultAsync(b => b.BookingId == bookingId);

        if (booking != null)
        {
            booking.Status = "Cancelled";
            if (booking.Slot != null)
                booking.Slot.Status = "Available";
            await _context.SaveChangesAsync();
        }
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> UpdateSlotStatus(int slotId, string status)
    {
        if (HttpContext.Session.GetString("IsAdmin") != "true")
            return RedirectToAction("Login");

        var slot = await _context.Slots.FindAsync(slotId);
        if (slot != null)
        {
            slot.Status = status;
            await _context.SaveChangesAsync();
        }
        return RedirectToAction("Index");
    }
}