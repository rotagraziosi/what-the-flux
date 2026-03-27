namespace WhatTheFlux.Api.Models;

public class Period
{
    public int Id { get; set; }
    public DateOnly StartDate { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ICollection<DayEntry> Days { get; set; } = new List<DayEntry>();
}
