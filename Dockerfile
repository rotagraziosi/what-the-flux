# Stage 1: Build Angular frontend
FROM node:22-alpine AS frontend-build
WORKDIR /app

COPY src/WhatTheFlux.Client/package*.json ./
RUN npm ci

COPY src/WhatTheFlux.Client/ ./
RUN npm run build

# Stage 2: Build .NET backend
FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS backend-build
WORKDIR /app

COPY src/WhatTheFlux.Api/*.csproj ./
RUN dotnet restore

COPY src/WhatTheFlux.Api/ ./
RUN dotnet publish -c Release -o /app/publish

# Stage 3: Production runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS runtime
WORKDIR /app

# Create non-root user for security
RUN addgroup -S appgroup && adduser -S appuser -G appgroup

# Copy published .NET app
COPY --from=backend-build /app/publish ./

# Copy Angular build to wwwroot
COPY --from=frontend-build /app/dist/WhatTheFlux.Client/browser ./wwwroot

# Create directory for SQLite database with proper permissions
RUN mkdir -p /app/data && chown -R appuser:appgroup /app/data

# Switch to non-root user
USER appuser

# Expose port
EXPOSE 8080

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "WhatTheFlux.Api.dll"]
