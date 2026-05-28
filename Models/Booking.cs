using System.ComponentModel.DataAnnotations;

namespace SmartParking.Models;

public class Booking
{
    [Key]
    public int BookingId { get; set; }
    public int UserId { get; set; }
    public int SlotId { get; set; }
    public DateTime BookedAt { get; set; } = DateTime.Now;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Status { get; set; } = "Active";
    public User? User { get; set; }
    public Slot? Slot { get; set; }
}