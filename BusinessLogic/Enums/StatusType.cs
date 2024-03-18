namespace BusinessLogic.Enums
{
    public enum StatusType : short
    {
        Draft = 0,
        Active = 1,
    }
    public enum StatusTicket : short
    {
        New = 1, 
        Sold = 2, // Ticket sales
        Return = 3, // Return the ticket
        Cancel = 4 //ticket cancellation
    }
}
