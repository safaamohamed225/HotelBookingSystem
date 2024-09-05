namespace HotelBookingSystem.Models
{
    public class Room
    {
        public int RoomID { get; set; }
        public string RoomNumber { get; set; }
        public int BranchID { get; set; }
        public int RoomTypeID { get; set; }
        public decimal PricePerNight { get; set; }
        public string Status { get; set; }
        public string Note { get; set; }
        public string Imagepath { get; set; }

        public bool isBooked { get; set; }


        public Branch Branch { get; set; }
        public RoomType RoomType { get; set; }
    }
    public class RoomDTO
    {
        public string RoomNumber { get; set; }
        public int BranchID { get; set; }
        public int RoomTypeID { get; set; }
        public decimal PricePerNight { get; set; }
        public string Note { get; set; }
        public string Status { get; set; }
        public bool isBooked { get; set; }

        public IFormFile file { get; set; }
    }
}
