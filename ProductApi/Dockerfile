# =========================
# Build Stage
# =========================

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Copy project files first
COPY ProductApi/ProductApi.csproj ProductApi/
COPY Invoice.Authentication/Invoice.Authentication.csproj Invoice.Authentication/
COPY Invoice.Models/Invoice.Models.csproj Invoice.Models/

# Restore dependencies
RUN dotnet restore ProductApi/ProductApi.csproj

# Copy source code
COPY ProductApi/ ProductApi/
COPY Invoice.Authentication/ Invoice.Authentication/
COPY Invoice.Models/ Invoice.Models/

# Publish
RUN dotnet publish ProductApi/ProductApi.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# =========================
# Runtime Stage
# =========================

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

ENTRYPOINT ["dotnet", "ProductApi.dll"]