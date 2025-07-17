FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY DATNBACK_END.csproj ./
COPY NuGet.config ./
RUN dotnet restore DATNBACK_END.csproj --configfile NuGet.config

COPY . ./
RUN dotnet publish DATNBACK_END.csproj -c Release -o /app/out /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

# Bạn có thể giữ hoặc bỏ ENV này
ENV ASPNETCORE_URLS=http://0.0.0.0:7102

EXPOSE 7102

ENTRYPOINT ["dotnet", "DATNBACK_END.dll"]
