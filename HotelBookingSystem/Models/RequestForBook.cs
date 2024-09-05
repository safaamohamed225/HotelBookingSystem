namespace HotelBookingSystem.Models
{
    public class RequestForBook
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public string userId { get; set; }
        public string Note {  get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }   
        public string Name{ get; set; }
        public bool isAccpted { get; set; }
    }
}
