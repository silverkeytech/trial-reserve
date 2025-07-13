namespace Reserve.Core.Features.Appointment;

public class AppointerNotifications
{
    public Guid Id { get; set; }
    public required string? ReserverName { get; set; }
    public required string? ReserverEmail { get; set; }
    public required string? ReserverPhoneNumber { get; set; }
    public required string? NotificationType { get; set; }
    public DateTime? NewSlot { get; set; }
    public required Availability? Slot { get; set; }
}