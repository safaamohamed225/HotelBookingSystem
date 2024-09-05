using System.Collections.Generic;
namespace HotelBookingSystem.Models
{
public class RoomType
{
    public int RoomTypeID { get; set; }
    public string TypeName { get; set; }

    public string Description { get; set; }


    public ICollection<Room> Rooms { get; set; }
}

}
