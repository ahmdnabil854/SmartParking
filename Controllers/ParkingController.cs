using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartParking.Data;
using SmartParking.Models;

namespace SmartParking.Controllers;

public class ParkingController : Controller
{
    private readonly AppDbContext _context;

    public ParkingController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        if (HttpContext.Session.GetInt32("UserId") == null)
            return RedirectToAction("Login", "Account");

        var slots = await _context.Slots
            .Include(s => s.ParkingLot)
            .ToListAsync();
        return View(slots);
    }

    [HttpPost]
    public async Task<IActionResult> Book(int slotId, DateTime startTime, DateTime endTime)
    {
        if (HttpContext.Session.GetInt32("UserId") == null)
            return RedirectToAction("Login", "Account");

        var slot = await _context.Slots.FindAsync(slotId);
        if (slot == null || slot.Status != "Available")
            return BadRequest("المكان مش متاح");

        slot.Status = "Reserved";

        var booking = new Booking
        {
            SlotId = slotId,
            UserId = HttpContext.Session.GetInt32("UserId") ?? 1,
            StartTime = startTime,
            EndTime = endTime,
            Status = "Active"
        };

        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }
}