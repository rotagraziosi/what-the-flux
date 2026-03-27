using Microsoft.EntityFrameworkCore;
using WhatTheFlux.Api.Data;
using WhatTheFlux.Api.DTOs;
using WhatTheFlux.Api.Models;

namespace WhatTheFlux.Api.Services;

public class PeriodService(AppDbContext db)
{
    public async Task<IReadOnlyList<PeriodSummaryDto>> GetAllAsync()
    {
        var periods = await db.Periods
            .Include(p => p.Days)
                .ThenInclude(d => d.Counts)
            .OrderByDescending(p => p.StartDate)
            .ToListAsync();

        return periods.Select(MapToSummary).ToList();
    }

    public async Task<PeriodDetailDto?> GetByIdAsync(int id)
    {
        var period = await db.Periods
            .Include(p => p.Days)
                .ThenInclude(d => d.Counts)
            .FirstOrDefaultAsync(p => p.Id == id);

        return period is null ? null : MapToDetail(period);
    }

    public async Task<PeriodDetailDto> CreateAsync(CreatePeriodRequest request)
    {
        var period = new Period
        {
            StartDate = request.StartDate,
            Notes = request.Notes,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        db.Periods.Add(period);
        await db.SaveChangesAsync();

        var days = Enumerable.Range(1, Math.Max(1, request.InitialDays))
            .Select(n => new DayEntry { PeriodId = period.Id, DayNumber = n })
            .ToList();

        db.DayEntries.AddRange(days);
        await db.SaveChangesAsync();

        return MapToDetail(await LoadPeriodAsync(period.Id));
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var period = await db.Periods.FindAsync(id);
        if (period is null) return false;

        db.Periods.Remove(period);
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<DayRowDto?> AddDayAsync(int periodId, AddDayRequest request)
    {
        var period = await LoadPeriodAsync(periodId);
        if (period is null) return null;

        int nextDay = request.DayNumber
            ?? (period.Days.Any() ? period.Days.Max(d => d.DayNumber) + 1 : 1);

        if (period.Days.Any(d => d.DayNumber == nextDay))
            return null;

        var entry = new DayEntry { PeriodId = periodId, DayNumber = nextDay };
        db.DayEntries.Add(entry);
        await db.SaveChangesAsync();

        period.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();

        return null; // caller will reload
    }

    public async Task<bool> RemoveDayAsync(int periodId, int dayNumber)
    {
        var entry = await db.DayEntries
            .FirstOrDefaultAsync(d => d.PeriodId == periodId && d.DayNumber == dayNumber);

        if (entry is null) return false;

        db.DayEntries.Remove(entry);
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpsertCountAsync(int periodId, int dayNumber, int criterionId, UpsertCountRequest request)
    {
        if (!CriterionDefinitions.All.Any(c => c.Id == criterionId))
            return false;

        var dayEntry = await db.DayEntries
            .FirstOrDefaultAsync(d => d.PeriodId == periodId && d.DayNumber == dayNumber);

        if (dayEntry is null) return false;

        var existing = await db.CriterionCounts
            .FirstOrDefaultAsync(c => c.DayEntryId == dayEntry.Id && c.CriterionId == criterionId);

        if (existing is null)
        {
            db.CriterionCounts.Add(new CriterionCount
            {
                DayEntryId = dayEntry.Id,
                CriterionId = criterionId,
                Count = request.Count
            });
        }
        else
        {
            existing.Count = request.Count;
        }

        var period = await db.Periods.FindAsync(periodId);
        if (period is not null)
        {
            period.UpdatedAt = DateTime.UtcNow;
        }

        await db.SaveChangesAsync();
        return true;
    }

    private async Task<Period> LoadPeriodAsync(int id) =>
        await db.Periods
            .Include(p => p.Days)
                .ThenInclude(d => d.Counts)
            .FirstAsync(p => p.Id == id);

    private static PeriodSummaryDto MapToSummary(Period p)
    {
        int total = ComputeTotal(p.Days);
        return new PeriodSummaryDto(p.Id, p.StartDate, p.Notes, p.Days.Count, total);
    }

    private static PeriodDetailDto MapToDetail(Period p)
    {
        var dayNumbers = p.Days.OrderBy(d => d.DayNumber).Select(d => d.DayNumber).ToList();
        var rows = CriterionDefinitions.All.Select(criterion =>
        {
            var countsByDay = new Dictionary<int, int>();
            foreach (var day in p.Days)
            {
                var count = day.Counts.FirstOrDefault(c => c.CriterionId == criterion.Id)?.Count ?? 0;
                countsByDay[day.DayNumber] = count;
            }
            int rowTotal = countsByDay.Values.Sum() * criterion.Multiplier;
            return new DayRowDto(criterion.Id, criterion.Label, criterion.Multiplier, countsByDay, rowTotal);
        }).ToList();

        return new PeriodDetailDto(p.Id, p.StartDate, p.Notes, dayNumbers, rows);
    }

    private static int ComputeTotal(IEnumerable<DayEntry> days)
    {
        int total = 0;
        foreach (var day in days)
        {
            foreach (var count in day.Counts)
            {
                var criterion = CriterionDefinitions.All.FirstOrDefault(c => c.Id == count.CriterionId);
                if (criterion is not null)
                    total += count.Count * criterion.Multiplier;
            }
        }
        return total;
    }
}
