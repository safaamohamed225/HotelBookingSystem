using System.Collections.Generic;
namespace HotelBookingSystem.Models

{
    public class Branch
{
    public int BranchID { get; set; }
    public string BranchName { get; set; }
    public string Location { get; set; }
    public string PhoneNumber { get; set; }

    public ICollection<Room> Rooms { get; set; }

}

}
