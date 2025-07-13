namespace Reserve.Core.Features.Queue;

public class QueueTicketInput
{
    public Guid Id { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerPhoneNumber { get; set; }
    public int QueueNumber { get; set; }
    public QueueEvent? QueueEvent { get; set; }
}