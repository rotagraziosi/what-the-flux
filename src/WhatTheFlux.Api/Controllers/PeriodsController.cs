using Microsoft.AspNetCore.Mvc;
using WhatTheFlux.Api.DTOs;
using WhatTheFlux.Api.Models;
using WhatTheFlux.Api.Services;

namespace WhatTheFlux.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PeriodsController(PeriodService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await service.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var period = await service.GetByIdAsync(id);
        return period is null ? NotFound() : Ok(period);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePeriodRequest request)
    {
        var result = await service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await service.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }

    [HttpPost("{id:int}/days")]
    public async Task<IActionResult> AddDay(int id, [FromBody] AddDayRequest request)
    {
        await service.AddDayAsync(id, request);
        var updated = await service.GetByIdAsync(id);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id:int}/days/{dayNumber:int}")]
    public async Task<IActionResult> RemoveDay(int id, int dayNumber)
    {
        var removed = await service.RemoveDayAsync(id, dayNumber);
        if (!removed) return NotFound();
        var updated = await service.GetByIdAsync(id);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpPut("{id:int}/days/{dayNumber:int}/counts/{criterionId:int}")]
    public async Task<IActionResult> UpsertCount(
        int id, int dayNumber, int criterionId,
        [FromBody] UpsertCountRequest request)
    {
        var success = await service.UpsertCountAsync(id, dayNumber, criterionId, request);
        return success ? NoContent() : NotFound();
    }
}
