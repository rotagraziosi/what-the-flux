using Microsoft.AspNetCore.Mvc;
using WhatTheFlux.Api.DTOs;
using WhatTheFlux.Api.Models;

namespace WhatTheFlux.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CriteriaController : ControllerBase
{
    [HttpGet]
    public IActionResult GetAll() =>
        Ok(CriterionDefinitions.All.Select(c => new CriterionDto(c.Id, c.Label, c.Multiplier)));
}
