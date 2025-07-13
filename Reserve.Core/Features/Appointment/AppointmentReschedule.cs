namespace Reserve.Core.Features.Appointment;

public class AppointmentReschedule
{
    public Guid Id { get; set; }
    public required AppointmentDetails OriginalAppointment { get; set; }
    public required Availability RequestedTime { get; set; }
    public RescheduleState RescheduleStatus { get; set; }
}

public enum RescheduleState
{
    Pending,
    Accepted,
    Declined
}