﻿using System.ComponentModel.DataAnnotations;

namespace Reserve.Core.Features.Queue;

public class QueueEventView
{
    public Guid Id { get; set; }

    [Required]
    public string? Title { get; set; }

    [Required]
    public string? OrganizerEmail { get; set; }

    [Required]
    public string? Description { get; set; }

    [Required]
    public int CurrentNumberServed { get; set; }

    [Required]
    public int TicketCounter { get; set; }

    public DateTime LastReset { get; set; }
}