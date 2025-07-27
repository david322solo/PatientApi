FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80


FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY PatientApi/ ./PatientApi/
COPY PatientApi.sln ./
RUN dotnet restore PatientApi/PatientApi.csproj
RUN dotnet publish PatientApi/PatientApi.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PatientApi.dll"]