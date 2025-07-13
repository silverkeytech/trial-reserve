namespace Reserve.Core.Features.Appointment;

public class Availability
{
    public Guid Id { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool Available { get; set; }
    public  AppointmentCalendar? AppointmentCalendar { get; set; }
}