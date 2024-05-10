namespace BusinessLogic.Dtos.CheckInOut
{
    public class CheckInOutResponse
    {
        public int Id { get; set; }

        public string? RoomTypes { get; set; }

        public string? Name { get; set; }

        public string? Location { get; set; }

        public short? StatusRoom { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
