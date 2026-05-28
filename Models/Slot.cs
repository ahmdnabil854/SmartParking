namespace SmartParking.Models;
using System.ComponentModel.DataAnnotations;

public class Slot
{
    [Key]
    public int SlotId { get; set; }
    public int LotId { get; set; }
    public string SlotNumber { get; set; } = "";
    public string Status { get; set; } = "Available";
    public ParkingLot? ParkingLot { get; set; }
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}