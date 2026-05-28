using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartParking.Data;

namespace SmartParking.Controllers;

public class BookingController : Controller
{
    private readonly AppDbContext _context;

    public BookingController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> MyBookings()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
            return RedirectToAction("Login", "Account");

        var bookings = await _context.Bookings
            .Include(b => b.Slot)
            .ThenInclude(s => s!.ParkingLot)
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.BookedAt)
            .ToListAsync();

        return View(bookings);
    }

    [HttpPost]
    public async Task<IActionResult> Cancel(int bookingId)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
            return RedirectToAction("Login", "Account");

        var booking = await _context.Bookings
            .Include(b => b.Slot)
            .FirstOrDefaultAsync(b => b.BookingId == bookingId && b.UserId == userId);

        if (booking == null)
            return NotFound();

        booking.Status = "Cancelled";
        if (booking.Slot != null)
            booking.Slot.Status = "Available";

        await _context.SaveChangesAsync();
        return RedirectToAction("MyBookings");
    }
}