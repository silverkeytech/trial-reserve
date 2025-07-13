﻿using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Reserve.Core.Features.Appointment;
using Reserve.Core.Features.MailService;
using static Reserve.Core.Features.MailService.MailFormats;
using System.Text.Json;
using System.Text;

namespace Reserve.Endpoints;

public static class AppointmentEndpoints
{
    public static RouteGroupBuilder MapAppointmentsApi(this RouteGroupBuilder group)
    {
        group.MapPost("create-calendar", async ([FromBody] AppointmentCalendar newCalendar, HttpContext context, IAntiforgery antiforgery, IAppointmentRepository _appointmentRepository) =>
        {
            try
            {
                if (newCalendar == null)
                {
                    Console.WriteLine("newCalendar is null");
                    return Results.BadRequest("newCalendar is null");
                }

                string slots = JsonSerializer.Serialize(newCalendar.AvailabilitySlots);
                await _appointmentRepository.CreateAppointmentCalendarAsync(newCalendar, slots);

                context.Response.Headers["X-Success-Redirect"] = $"/calendar-creation-notification/{newCalendar.Id}";
                return Results.Ok();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
                return Results.BadRequest(e.Message);
            }
        });


        group.MapDelete("delete-slot/{id}", async ([FromRoute] string id, HttpContext context, IAppointmentRepository _appointmentRepository, IAntiforgery _antiforgery) =>
        {
            try
            {
                Availability? deletedSlot = await _appointmentRepository.GetSlotByIdAsync(id);
                if(deletedSlot?.Available == false)
                {
                    context.Response.Headers["HX-Redirect"] = $"/upcoming-appointments/{deletedSlot.AppointmentCalendar?.Id}";
                    context.Response.Cookies.Append("error", "Can't delete a reserved slot");
                    return Results.BadRequest("Can't delete a reserved slot");
                }
                await _appointmentRepository.DeleteAppointmentSlotAsync(id);
                context.Response.Headers["HX-Redirect"] = $"/upcoming-appointments/{deletedSlot?.AppointmentCalendar?.Id}";
                context.Response.Cookies.Append("success", "Slot deleted successfully");
                return Results.Ok();
            }
            catch (Exception e)
            {
                context.Response.Cookies.Append("error", "error in deleting slot");
                return Results.BadRequest(e.Message);
            }
        });
        group.MapPost("add-slot/{id}", async ([FromBody] Availability? availabilitySlot, string id, HttpContext context, IAppointmentRepository _appointmentRepository, IAntiforgery _antiforgery) =>
        {
            try
            {
                availabilitySlot = await _appointmentRepository.AddAppointmentSlotAsync(id, availabilitySlot);
                if (availabilitySlot is null)
                {
                    return Results.BadRequest("Enter values in start date and end date");
                }
                if(availabilitySlot.EndTime < availabilitySlot.StartTime)
                {
                    return Results.BadRequest("End time must be after start time");
                }
                return Results.Ok(availabilitySlot);
            }
            catch (Exception e)
            {
                return Results.BadRequest(e.Message);
            }
        });
        group.MapDelete("cancel-appointment/{id}", async (string id, HttpContext context, IAntiforgery _antiforgery, IAppointmentRepository _appointmentRepository, IEmailService _emailService) =>
        {
            try
            {
                AppointmentReschedule? reschedule = await _appointmentRepository.GetRescheduleByIdAsync(id);
                if(reschedule is not null)
                {
                    context.Response.Headers["HX-Redirect"] = $"/user-dashboard/{id}";
                    context.Response.Cookies.Append("check", "Check notifications before taking this action");
                    return Results.BadRequest("Check notifications before taking this action");
                }
                AppointmentDetails? cancelledAppointment = await _appointmentRepository.GetAppointmentDetailsByIdAsync(id);
                if(cancelledAppointment?.AppointmentStatus == AppointmentState.Done)
                {
                    context.Response.Headers["HX-Redirect"] = $"/user-dashboard/{id}";
                    context.Response.Cookies.Append("error", "appointment already finished");
                    return Results.BadRequest("appointment already finished");
                }

                string? email = cancelledAppointment?.Slot?.AppointmentCalendar?.Email;
                if (email == null)
                {
                    context.Response.Headers["HX-Redirect"] = "/user-dashboard/{id}";
                    context.Response.Cookies.Append("error", "Calendar email is missing");
                    return Results.BadRequest("Calendar email is missing");
                }

                AppointmentCalendar? calendar = await _appointmentRepository.GetCalendarFromEmail(email);

                if (calendar != null)
                {
                    string baseUrl = $"{context.Request.Scheme}://{context.Request.Host}";
                    var ownerEmail = new MailRequest
                    {
                        ToEmail = cancelledAppointment?.Slot?.AppointmentCalendar?.Email,
                        Subject = "Appointment Cancellation Notice",
                        Body = AppointmentCancellationNotificationOwner(calendar.Id.ToString(),baseUrl)
                    };

                    await _emailService.SendEmailAsync(ownerEmail);
                }
                

                await _appointmentRepository.CancelAppointmentAsync(cancelledAppointment);
                context.Response.Headers["HX-Redirect"] = "/event-cancellation";
                return Results.Ok();
            }
            catch (Exception e)
            {
                return Results.BadRequest(e.Message);
            }
        });
        group.MapDelete("delete-notification/{id}", async (string id, HttpContext context, IAntiforgery _antiforgery, IAppointmentRepository _appointmentRepository) =>
        {
            try
            {
                await _appointmentRepository.DeleteNotification(id);
                return Results.Ok();
            }
            catch (Exception e)
            {
                return Results.BadRequest(e.Message);
            }
        });
        group.MapPut("finish-appointment/{id}", async (string id, HttpContext context, IAntiforgery _antiforgery, IAppointmentRepository _appointmentRepository) =>
        {
            try
            {
                AppointmentDetails? appointmentToCheck = await _appointmentRepository.GetAppointmentDetailsByIdAsync(id);
                if(appointmentToCheck is null)
                {
                    context.Response.Cookies.Append("error", "appointment not found");
                    context.Response.Headers["HX-Redirect"] = $"/user-details/{id}";
                    return Results.BadRequest("appointment not found");
                }
                if(appointmentToCheck.AppointmentStatus == AppointmentState.Done)
                {
                    context.Response.Cookies.Append("error", "appointment already finished");
                    context.Response.Headers["HX-Redirect"] = $"/user-details/{id}";
                    return Results.BadRequest("appointment already finished");
                }
                var result = await _appointmentRepository.GetRescheduleByIdAsync(id);
                if(result is not null)
                {
                    context.Response.Cookies.Append("error", "please wait for rescheduled requests to be resolved or delete them");
                    context.Response.Headers["HX-Redirect"] = $"/user-details/{id}";
                    return Results.BadRequest("please wait for rescheduled requests to be resolved or delete them");
                }
                await _appointmentRepository.FinishAppointment(id);
                AppointmentDetails? finishedAppointment = await _appointmentRepository.GetAppointmentDetailsByIdAsync(id);
                context.Response.Headers["HX-Redirect"] = $"/upcoming-appointments/{finishedAppointment?.Slot?.AppointmentCalendar?.Id}";
                context.Response.Cookies.Append("success", "appointment finished successfully");
                return Results.Ok();
            }
            catch (Exception e)
            {
                context.Response.Cookies.Append("check", "error in finishing appointment, try again");
                context.Response.Headers["HX-Redirect"] = $"/user-details/{id}";
                return Results.BadRequest(e.Message);
            }
        });
        group.MapGet("get-appointments/{id}", async (string id, IAppointmentRepository _appointmentRepository) =>
        {
            try
            {
                List<AppointmentDetails?> appointments = await _appointmentRepository.GetAppointmentDetailsForCalendarAsync(id);
                return Results.Ok(appointments);
            }
            catch (Exception e)
            {
                return Results.BadRequest(e.Message);
            }
        });
        group.MapGet("free-slots/{id}", async (string id, IAppointmentRepository _appointmentRepository) =>
        {
            try
            {
                List<Availability?> availableSlots = await _appointmentRepository.GetFreeSlotsForCalendarView(id);
                return Results.Ok(availableSlots);
            }
            catch (Exception e)
            {
                return Results.BadRequest(e.Message);
            }
            
        });
        group.MapDelete("delete-request/{id}", async (string id, HttpContext context, IAntiforgery _antiforgery, IAppointmentRepository _appointmentRepository) =>
        {
            try
            {
                AppointmentReschedule? reschedule = await _appointmentRepository.GetRequestByIdAsync(id);
                if(reschedule is null)
                {
                    return Results.NotFound();
                }
                if(reschedule.RescheduleStatus == RescheduleState.Pending)
                {
                    await _appointmentRepository.DeclineRescheduling(id);
                    await _appointmentRepository.DeleteRequest(id);
                    return Results.Ok();
                }
                await _appointmentRepository.DeleteRequest(id);
                return Results.Ok();
            }
            catch (Exception e)
            {
                return Results.BadRequest(e.Message);
            }
        });
        group.MapGet("get-pending-slots/{id}", async (string id, IAppointmentRepository _appointmentRepository) =>
        {
            try
            {
                List<Availability?> pendingSlots = await _appointmentRepository.GetPendingSlots(id);
                return Results.Ok(pendingSlots);
            }
            catch (Exception e)
            {
                return Results.BadRequest(e.Message);
            }
        });
        group.MapGet("get-done-appointments/{id}", async (string id, IAppointmentRepository _appointmentRepository) =>
        {
            try
            {
                List<AppointmentDetails?> doneAppointments = await _appointmentRepository.GetDoneAppointmentsByCalendarId(id);
                return Results.Ok(doneAppointments);
            }
            catch (Exception e)
            {
                return Results.BadRequest(e.Message);
            }
        });
        return group;
    }
}
