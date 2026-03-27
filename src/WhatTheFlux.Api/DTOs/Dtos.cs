using System.ComponentModel.DataAnnotations;

namespace WhatTheFlux.Api.DTOs;

public record PeriodSummaryDto(
    int Id,
    DateOnly StartDate,
    string? Notes,
    int DayCount,
    int TotalScore
);

public record PeriodDetailDto(
    int Id,
    DateOnly StartDate,
    string? Notes,
    IReadOnlyList<int> DayNumbers,
    IReadOnlyList<DayRowDto> Rows
);

public record DayRowDto(
    int CriterionId,
    string Label,
    int Multiplier,
    Dictionary<int, int> CountsByDay,
    int RowTotal
);

public record CriterionDto(int Id, string Label, int Multiplier);

public record CreatePeriodRequest(
    DateOnly StartDate,
    string? Notes,
    int InitialDays = 7
);

public record UpsertCountRequest([Range(0, 9999)] int Count);

public record AddDayRequest(int? DayNumber);
