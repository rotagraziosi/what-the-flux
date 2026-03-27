namespace WhatTheFlux.Api.Models;

public class DayEntry
{
    public int Id { get; set; }
    public int PeriodId { get; set; }
    public int DayNumber { get; set; }

    public Period Period { get; set; } = null!;
    public ICollection<CriterionCount> Counts { get; set; } = new List<CriterionCount>();
}
