namespace WhatTheFlux.Api.Models;

public record CriterionMeta(int Id, string Label, int Multiplier);

public static class CriterionDefinitions
{
    public static readonly IReadOnlyList<CriterionMeta> All = new[]
    {
        new CriterionMeta(1, "Slightly soaked protection",   1),
        new CriterionMeta(2, "Moderately soaked protection", 5),
        new CriterionMeta(3, "Saturated protection",         20),
        new CriterionMeta(4, "Small blood clot (<1cm)",      1),
        new CriterionMeta(5, "Large blood clot (>1cm)",      5),
    };
}
