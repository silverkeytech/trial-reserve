namespace Reserve.Core.Features.Event;

public class CasualTicketView
{
    public Guid Id { get; set; }
    public string? ReserverName { get; set; }
    public string? ReserverEmail { get; set; }
    public string? ReserverPhoneNumber { get; set; }
    public CasualEvent? CasualEvent { get; set; }
}