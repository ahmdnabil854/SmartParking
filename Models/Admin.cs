namespace SmartParking.Models;
using System.ComponentModel.DataAnnotations;

public class Admin
{
    [Key]
    public int AdminId { get; set; }
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
}