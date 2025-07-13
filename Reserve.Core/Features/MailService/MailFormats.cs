using System.Globalization;
using Microsoft.AspNetCore.Http;
namespace Reserve.Core.Features.MailService;

public static class MailFormats
{
    public static string EventCreationNotification(string eventId,string baseUrl)
    {
        string response = CultureInfo.CurrentCulture.Name=="en"? $"""
            <h1 class="notification-title mb-lg-5">Event Created Successfully!</h1>
            <div class="d-flex flex-column justify-content-center align-items-center">
            <div class="notification-border mb-lg-5">
            <div>
            <p class="notification-text d-flex justify-content-center mb-1">You can now share your Event on the following link:</p>
            <a class="d-flex justify-content-center mb-md-2" href="{baseUrl}/event-details/{eventId}">{baseUrl}/event/{eventId}</a>
            </div>
            <div>
            <p class="notification-text d-flex justify-content-center mb-1">If you don’t have an account, you can still edit your event at the following link:</p>
            <a class="d-flex justify-content-center mb-md-2" href="{baseUrl}/attendees/{eventId}">{baseUrl}/event/edit-event/{eventId}</a>
            </div>
            </div>
            </div>
            """: $"""
                <h1 class="notification-title mb-lg-5">تم إنشاء الحدث بنجاح!</h1>
                <div class="d-flex flex-column justify-content-center align-items-center">
                <div class="notification-border mb-lg-5">
                <div>
                <p class="notification-text d-flex justify-content-center mb-1">يمكنك الآن مشاركة الحدث على الرابط التالي:</p>
                <a class="d-flex justify-content-center mb-md-2" href="{baseUrl}/event-details/{eventId}">{baseUrl}/event/{eventId}</a>
                </div>
                <div>
                <p class="notification-text d-flex justify-content-center mb-1">إذا لم يكن لديك حساب، يمكنك تعديل الحدث من خلال الرابط التالي:</p>
                <a class="d-flex justify-content-center mb-md-2" href="{baseUrl}/attendees/{eventId}">{baseUrl}/event/edit-event/{eventId}</a>
                </div>
                </div>
                </div>
                """;
        return response;
    }

    public static string QueueCreationNotification(string queueId, string baseUrl)
    {
        string response = CultureInfo.CurrentCulture.Name=="en"?$"""
            <h1 class="notification-title mb-lg-5">Queue Created Successfully!</h1>
            <div class="d-flex flex-column justify-content-center align-items-center">
            <div class="notification-border mb-lg-5">
            <div>
            <p class="notification-text d-flex justify-content-center mb-1">You can now share your Queue on the following link:</p>
            <a class="d-flex justify-content-center mb-md-2" href="{baseUrl}/q-reg/{queueId}">{baseUrl}/QueueRegistration/{queueId}</a>
            </div>
            <div>
            <p class="notification-text d-flex justify-content-center mb-1">If you don’t have an account, you can still edit your queue at the following link:</p>
            <a class="d-flex justify-content-center mb-md-2" href="{baseUrl}/q-admin/{queueId}">{baseUrl}/QueueAdmin/{queueId}</a>
            </div>
            </div>
            </div>
            """:$"""
                <h1 class="notification-title mb-lg-5">تم إنشاء الطابور بنجاح!</h1>
                <div class="d-flex flex-column justify-content-center align-items-center">
                <div class="notification-border mb-lg-5">
                <div>
                <p class="notification-text d-flex justify-content-center mb-1">يمكنك الآن مشاركة الطابور على الرابط التالي:</p>
                <a class="d-flex justify-content-center mb-md-2" href="{baseUrl}/q-reg/{queueId}">{baseUrl}/QueueRegistration/{queueId}</a>
                </div>
                <div>
                <p class="notification-text d-flex justify-content-center mb-1">إذا لم يكن لديك حساب، يمكنك تعديل الطابور من خلال الرابط التالي:</p>
                <a class="d-flex justify-content-center mb-md-2" href="{baseUrl}/q-admin/{queueId}">{baseUrl}/QueueAdmin/{queueId}</a>
                </div>
                </div>
                </div>
                """;
        return response;
    }

    public static string ReservationSuccessfulNotificationEvent(string ticketId, string baseUrl)
    {
        string response = CultureInfo.CurrentCulture.Name=="en" ? $"""
            <h1 class="notification-title mb-lg-5">Reservation Successful!</h1>
            <div class="d-flex flex-column justify-content-center align-items-center">
            <div class="notification-border mb-lg-5">
            <div>
            <p class="notification-text d-flex justify-content-center mb-1">You can view Event details or cancel reservation at the below link.</p>
            <a class="d-flex justify-content-center mb-md-2" href="{baseUrl}/reserver-details/{ticketId}"> {baseUrl} /Event/ReserverDetails/{ticketId}</a>
            </div>
            <div>
            <p class="notification-text d-flex justify-content-center align-content-center">An email has been sent with the link.</p>
            </div>
            </div>
            </div>
            """ : $"""
                <h1 class="notification-title mb-lg-5">تم الحجز بنجاح!</h1>
                <div class="d-flex flex-column justify-content-center align-items-center">
                <div class="notification-border mb-lg-5">
                <div>
                <p class="notification-text d-flex justify-content-center mb-1">يمكنك عرض تفاصيل الحدث أو إلغاء الحجز على الرابط أدناه.</p>
                <a class="d-flex justify-content-center mb-md-2" href="{baseUrl}/reserver-details/{ticketId}">{baseUrl}/Event/ReserverDetails/{ticketId}</a>
                </div>
                <div>
                <p class="notification-text d-flex justify-content-center align-content-center">تم إرسال بريد إلكتروني بالرابط.</p>
                </div>
                </div>
                </div>
                """;
        return response;
    }

    public static string ReservationSuccessfulNotificationQueue(string ticketId, string baseUrl)
    {
        string response = CultureInfo.CurrentCulture.Name=="en"? $"""
            <h1 class="notification-title mb-lg-5">Reservation Successful!</h1>
            <div class="d-flex flex-column justify-content-center align-items-center">
            <div class="notification-border mb-lg-5">
            <div>
            <p class="notification-text d-flex justify-content-center mb-1">You can view Event details or cancel reservation at the below link.</p>
            <a class="d-flex justify-content-center mb-md-2" href="{baseUrl}/Event/ReserverDetails/{ticketId}">{baseUrl}/Event/ReserverDetails/{ticketId}</a>
            </div>
            <div>
            <p class="notification-text d-flex justify-content-center align-content-center">An email has been sent with the link.</p>
            </div>
            </div>
            </div>
            """: $"""
                <h1 class="notification-title mb-lg-5">تم الحجز بنجاح!</h1>
                <div class="d-flex flex-column justify-content-center align-items-center">
                <div class="notification-border mb-lg-5">
                <div>
                <p class="notification-text d-flex justify-content-center mb-1">يمكنك عرض تفاصيل الحدث أو إلغاء الحجز على الرابط أدناه.</p>
                <a class="d-flex justify-content-center mb-md-2" href="{baseUrl}/Event/ReserverDetails/{ticketId}">{baseUrl}/Event/ReserverDetails/{ticketId}</a>
                </div>
                <div>
                <p class="notification-text d-flex justify-content-center align-content-center">تم إرسال بريد إلكتروني بالرابط.</p>
                </div>
                </div>
                </div>
                """;    
        return response;
    }

    public static string AppointmentCreationNotification(string appointmentId, string baseUrl)
    {
        string response = CultureInfo.CurrentCulture.Name=="en"? $"""
            <h1 class="notification-title mb-lg-5">Appointment Created Successfully!</h1>
            <div class="d-flex flex-column justify-content-center align-items-center">
            <div class="notification-border mb-lg-5">
            <div>
            <p class="notification-text d-flex justify-content-center mb-1">You can now share your Calendar on the following link:</p>
            <a class="d-flex justify-content-center mb-md-2" href="{baseUrl}/reserve-appointment/{appointmentId}">{baseUrl}/reserve-appointment/{appointmentId}</a>
            </div>
            <div>
            <p class="notification-text d-flex justify-content-center mb-1">If you don’t have an account, you can still edit your calendar at the following link:</p>
            <a class="d-flex justify-content-center mb-md-2" href="{baseUrl}/upcoming-appointments/{appointmentId}">{baseUrl}/event/upcoming-appointments/{appointmentId}</a>
            </div>
            </div>
            </div>
            """ : $"""
                <h1 class="notification-title mb-lg-5">تم إنشاء الموعد بنجاح!</h1>
                <div class="d-flex flex-column justify-content-center align-items-center">
                <div class="notification-border mb-lg-5">
                <div>
                <p class="notification-text d-flex justify-content-center mb-1">يمكنك الآن مشاركة التقويم الخاص بك على الرابط التالي:</p>
                <a class="d-flex justify-content-center mb-md-2" href="{baseUrl}/reserve-appointment/{appointmentId}">{baseUrl}/reserve-appointment/{appointmentId}</a>
                </div>
                <div>
                <p class="notification-text d-flex justify-content-center mb-1">إذا لم يكن لديك حساب، يمكنك تعديل التقويم على الرابط التالي:</p>
                <a class="d-flex justify-content-center mb-md-2" href="{baseUrl}/upcoming-appointments/{appointmentId}">{baseUrl}/event/upcoming-appointments/{appointmentId}</a>
                </div>
                </div>
                </div>
                """;
        return response;
    }

    public static string ReschedulingRequestAcceptedFromClietToOwner(string appointmentId, string baseUrl)
    {
        string response = CultureInfo.CurrentCulture.Name=="en"? $"""
            <h1 class="notification-title mb-lg-5">Rescheduling Request Accepted!</h1>
            <div class="d-flex flex-column justify-content-center align-items-center">
            <div class="notification-border mb-lg-5">
            <div>
            <p class="notification-text d-flex justify-content-center mb-1">You can now share your Calendar on the following link:</p>
            <a class="d-flex justify-content-center mb-md-2" href="{baseUrl}/reserve-appointment/{appointmentId}">{baseUrl}/reserve-appointment/{appointmentId}</a>
            </div>
            <div>
            <p class="notification-text d-flex justify-content-center mb-1">If you don’t have an account, you can still edit your calendar at the following link:</p>
            <a class="d-flex justify-content-center mb-md-2" href="{baseUrl}/upcoming-appointments/{appointmentId}">{baseUrl}/event/upcoming-appointments/{appointmentId}</a>
            </div>
            </div>
            </div>
            """: $"""
                <h1 class="notification-title mb-lg-5">تم قبول طلب إعادة الجدولة!</h1>
                <div class="d-flex flex-column justify-content-center align-items-center">
                <div class="notification-border mb-lg-5">
                <div>
                <p class="notification-text d-flex justify-content-center mb-1">يمكنك الآن مشاركة التقويم الخاص بك على الرابط التالي:</p>
                <a class="d-flex justify-content-center mb-md-2" href="{baseUrl}/reserve-appointment/{appointmentId}">{baseUrl}/reserve-appointment/{appointmentId}</a>
                </div>
                <div>
                <p class="notification-text d-flex justify-content-center mb-1">إذا لم يكن لديك حساب، يمكنك تعديل التقويم على الرابط التالي:</p>
                <a class="d-flex justify-content-center mb-md-2" href="{baseUrl}/upcoming-appointments/{appointmentId}">{baseUrl}/event/upcoming-appointments/{appointmentId}</a>
                </div>
                </div>
                </div>
                """;
        return response;
    }

    public static string ReschedulingRequestFromOwnerToClient(string appointmentId, string baseUrl)
    {
        string response = CultureInfo.CurrentCulture.Name=="en"? $"""
            <h1 class="notification-title mb-lg-5">Rescheduling request for your appointment</h1>
            <div class="d-flex flex-column justify-content-center align-items-center">
            <div class="notification-border mb-lg-5">
            <div>
            <p class="notification-text d-flex justify-content-center mb-1">You can now share details on the following link:</p>
            <a class="d-flex justify-content-center mb-md-2" href="{baseUrl}/user-dashboard/{appointmentId}">{baseUrl}/user-dashboard/{appointmentId}</a>
            </div>
            </div>
            </div>
            """: $"""
                <h1 class="notification-title mb-lg-5">طلب إعادة جدولة لموعدك</h1>
                <div class="d-flex flex-column justify-content-center align-items-center">
                <div class="notification-border mb-lg-5">
                <div>
                <p class="notification-text d-flex justify-content-center mb-1">يمكنك الآن مشاركة التفاصيل على الرابط التالي:</p>
                <a class="d-flex justify-content-center mb-md-2" href="{baseUrl}/user-dashboard/{appointmentId}">{baseUrl}/user-dashboard/{appointmentId}</a>
                </div>
                </div>
                </div>
                """;
        return response;
    }

    public static string AppointmentCreationNotificationClient(string appointmentId, string baseUrl)
    {
        string response = CultureInfo.CurrentCulture.Name=="en"? $"""
            <h1 class="notification-title mb-lg-5">You have reserved an appointment!</h1>
            <div class="d-flex flex-column justify-content-center align-items-center">
            <div class="notification-border mb-lg-5">
            <div>
            <p class="notification-text d-flex justify-content-center mb-1">View your details at this Link:</p>
            <a class="d-flex justify-content-center mb-md-2" href="{baseUrl}/user-dashboard/{appointmentId}">{baseUrl}/user-dashboard/{appointmentId}</a>
            </div>
            </div>
            </div>
            """ : $"""
                <h1 class="notification-title mb-lg-5">لقد حجزت موعدًا!</h1>
                <div class="d-flex flex-column justify-content-center align-items-center">
                <div class="notification-border mb-lg-5">
                <div>
                <p class="notification-text d-flex justify-content-center mb-1">عرض تفاصيلك على هذا الرابط:</p>
                <a class="d-flex justify-content-center mb-md-2" href="{baseUrl}/user-dashboard/{appointmentId}">{baseUrl}/user-dashboard/{appointmentId}</a>
                </div>
                </div>
                </div>
                """;
        return response;
    }

    public static string AppointmentCreationNotificationOwner(string appointmentId, string baseUrl)
    {
        string response = CultureInfo.CurrentCulture.Name=="en"? $"""
            <h1 class="notification-title mb-lg-5">You have a new appointmrnt reserved!</h1>
            <div class="d-flex flex-column justify-content-center align-items-center">
            <div class="notification-border mb-lg-5">
            <div>
            <p class="notification-text d-flex justify-content-center mb-1">View your appointments details at this Link:</p>
            <a class="d-flex justify-content-center mb-md-2" href="{baseUrl}/upcoming-appointments/{appointmentId}">{baseUrl}/upcoming-appointments/{appointmentId}</a>
            </div>
            </div>
            </div>
            """: $"""
                <h1 class="notification-title mb-lg-5">لديك موعد جديد محجوز!</h1>
                <div class="d-flex flex-column justify-content-center align-items-center">
                <div class="notification-border mb-lg-5">
                <div>
                <p class="notification-text d-flex justify-content-center mb-1">عرض تفاصيل مواعيدك على هذا الرابط:</p>
                <a class="d-flex justify-content-center mb-md-2" href="{baseUrl}/upcoming-appointments/{appointmentId}">{baseUrl}/upcoming-appointments/{appointmentId}</a>
                </div>
                </div>
                </div>
                """;
        return response;
    }

    public static string AppointmentCancellationNotificationOwner(string appointmentId, string baseUrl)
    {
        string response = CultureInfo.CurrentCulture.Name=="en"? $"""
            <h1 class="notification-title mb-lg-5">You have an appointment Cancelled!</h1>
            <div class="d-flex flex-column justify-content-center align-items-center">
            <div class="notification-border mb-lg-5">
            <div>
            <p class="notification-text d-flex justify-content-center mb-1">View your appointments details at this Link:</p>
            <a class="d-flex justify-content-center mb-md-2" href="{baseUrl}/upcoming-appointments/{appointmentId}">{baseUrl}/upcoming-appointments/{appointmentId}</a>
            </div>
            </div>
            </div>
            """: $"""
                <h1 class="notification-title mb-lg-5">لديك موعد تم إلغاؤه!</h1>
                <div class="d-flex flex-column justify-content-center align-items-center">
                <div class="notification-border mb-lg-5">
                <div>
                <p class="notification-text d-flex justify-content-center mb-1">عرض تفاصيل مواعيدك على هذا الرابط:</p>
                <a class="d-flex justify-content-center mb-md-2" href="{baseUrl}/upcoming-appointments/{appointmentId}">{baseUrl}/upcoming-appointments/{appointmentId}</a>
                </div>
                </div>
                </div>
                """;
        return response;
    }

    public static string EventReservationNotificationClient(string eventId, string baseUrl)
    {
        string response = CultureInfo.CurrentCulture.Name == "en" ? $"""
            <h1 class="notification-title mb-lg-5">You have reserved slot in the event!</h1>
            <div class="d-flex flex-column justify-content-center align-items-center">
            <div class="notification-border mb-lg-5">
            <div>
            <p class="notification-text d-flex justify-content-center mb-1">View your details at this Link:</p>
            <a class="d-flex justify-content-center mb-md-2" href="{baseUrl}/reserver-details/{eventId}">{baseUrl}/reserver-details/{eventId}</a>
            </div>
            </div>
            </div>
            """: $"""
                <h1 class="notification-title mb-lg-5">لقد حجزت فتحة في الحدث!</h1>
                <div class="d-flex flex-column justify-content-center align-items-center">
                <div class="notification-border mb-lg-5">
                <div>
                <p class="notification-text d-flex justify-content-center mb-1">عرض تفاصيلك على هذا الرابط:</p>
                <a class="d-flex justify-content-center mb-md-2" href="{baseUrl}/reserver-details/{eventId}">{baseUrl}/reserver-details/{eventId}</a>
                </div>
                </div>
                </div>
                """;
        return response;
    }

    public static string EventReservationNotificationOwner(string eventId, string baseUrl)
    {
        string response = CultureInfo.CurrentCulture.Name == "en" ? $"""
            <h1 class="notification-title mb-lg-5">You have a new slot reserved in your event!</h1>
            <div class="d-flex flex-column justify-content-center align-items-center">
            <div class="notification-border mb-lg-5">
            <div>
            <p class="notification-text d-flex justify-content-center mb-1">View your event details at this Link:</p>
            <a class="d-flex justify-content-center mb-md-2" href="{baseUrl}/attendees/{eventId}">{baseUrl}/attendees/{eventId}</a>
            </div>
            </div>
            </div>
            """: $"""
                <h1 class="notification-title mb-lg-5">لديك فتحة جديدة محجوزة في حدثك!</h1>
                <div class="d-flex flex-column justify-content-center align-items-center">
                <div class="notification-border mb-lg-5">
                <div>
                <p class="notification-text d-flex justify-content-center mb-1">عرض تفاصيل الحدث على هذا الرابط:</p>
                <a class="d-flex justify-content-center mb-md-2" href="{baseUrl}/attendees/{eventId}">{baseUrl}/attendees/{eventId}</a>
                </div>
                </div>
                </div>
                """;
        return response;
    }

    public static string EventCancellationNotificationOwner(string eventId, string baseUrl)
    {
        string response = CultureInfo.CurrentCulture.Name == "en" ? $"""
            <h1 class="notification-title mb-lg-5">Someone cancelled their reservation in your event!</h1>
            <div class="d-flex flex-column justify-content-center align-items-center">
            <div class="notification-border mb-lg-5">
            <div>
            <p class="notification-text d-flex justify-content-center mb-1">View your event details at this Link:</p>
            <a class="d-flex justify-content-center mb-md-2" href="{baseUrl}/attendees/{eventId}">{baseUrl}/attendees/{eventId}</a>
            </div>
            </div>
            </div>
            """: $"""
                <h1 class="notification-title mb-lg-5">ألغى شخص حجزه في حدثك!</h1>
                <div class="d-flex flex-column justify-content-center align-items-center">
                <div class="notification-border mb-lg-5">
                <div>
                <p class="notification-text d-flex justify-content-center mb-1">عرض تفاصيل الحدث على هذا الرابط:</p>
                <a class="d-flex justify-content-center mb-md-2" href="{baseUrl}/attendees/{eventId}">{baseUrl}/attendees/{eventId}</a>
                </div>
                </div>
                </div>
                """;
        return response;
    }

    public static string EmailServiceValidation()
    {
        string response = $"""
            <h1 class="notification-title mb-lg-5">Validation email from Reserve!</h1>
            <div class="d-flex flex-column justify-content-center align-items-center">
            <div class="notification-border mb-lg-5">
            <div>
            <p class="d-flex justify-content-center mb-md-2">This is a test message from Reserve to make sure that the email configuration works. Please ignore this message.</p>
            </div>
            </div>
            </div>
            """;
        return response;
    }
}