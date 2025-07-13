using System.ComponentModel.DataAnnotations;

namespace Reserve.Core.Features.Queue;

public class QueueTicketView
{
    public Guid Id { get; set; }

    [Required]
    public string? CustomerName { get; set; }

    [Required]
    public string? CustomerPhoneNumber { get; set; }

    [Required]
    public int QueueNumber { get; set; }

    [Required]
    public QueueEvent? QueueEvent { get; set; }

    public string? Status { get; set; }
}