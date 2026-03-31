using System.Text;
using Microsoft.EntityFrameworkCore;
using WhatTheFlux.Api.Data;
using WhatTheFlux.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<PeriodService>();
builder.Services.AddControllers();

builder.Services.AddCors(options =>
    options.AddPolicy("Angular", policy =>
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()));

var app = builder.Build();

// Basic Auth — disabled in Development so local work is frictionless
if (!app.Environment.IsDevelopment())
{
    var authUser = app.Configuration["Auth:Username"] ?? "admin";
    var authPass = app.Configuration["Auth:Password"]
        ?? throw new InvalidOperationException("Auth:Password must be set in production.");

    app.Use(async (context, next) =>
    {
        var authHeader = context.Request.Headers.Authorization.ToString();
        if (authHeader.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
        {
            var encoded = authHeader["Basic ".Length..].Trim();
            var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(encoded));
            var colon = decoded.IndexOf(':');
            if (colon > 0
                && decoded[..colon] == authUser
                && decoded[(colon + 1)..] == authPass)
            {
                await next();
                return;
            }
        }

        context.Response.Headers.WWWAuthenticate = "Basic realm=\"WhatTheFlux\", charset=\"UTF-8\"";
        context.Response.StatusCode = 401;
    });
}

// Auto-migrate on startup
using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.Migrate();
}

app.UseCors("Angular");

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// In production, serve the Angular app from wwwroot
if (!app.Environment.IsDevelopment())
{
    app.UseDefaultFiles();
    app.UseStaticFiles();
}

app.MapControllers();

// Fallback to index.html for Angular routing in production
if (!app.Environment.IsDevelopment())
{
    app.MapFallbackToFile("index.html");
}

app.Run();
