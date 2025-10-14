# Stage 1 — Build and publish
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy project file(s) and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy all source files
COPY . .

# Publish Release build
RUN dotnet publish -c Release -o /app/publish

# Stage 2 — Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Copy published output from build stage
COPY --from=build /app/publish .

# Expose API port
EXPOSE 8080

# Run the app
ENTRYPOINT ["dotnet", "api.dll"]


