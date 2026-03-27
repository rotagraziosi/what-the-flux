namespace WhatTheFlux.Api.Models;

public class CriterionCount
{
    public int Id { get; set; }
    public int DayEntryId { get; set; }
    public int CriterionId { get; set; }
    public int Count { get; set; }

    public DayEntry DayEntry { get; set; } = null!;
}
