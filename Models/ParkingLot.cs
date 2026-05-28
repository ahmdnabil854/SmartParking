using System.ComponentModel.DataAnnotations;

namespace SmartParking.Models;

public class ParkingLot
{
    [Key]
    public int LotId { get; set; }
    public string Name { get; set; } = "";
    public string? Location { get; set; }
    public int TotalSlots { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public ICollection<Slot> Slots { get; set; } = new List<Slot>();
}