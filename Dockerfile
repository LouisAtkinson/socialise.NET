FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

COPY socialApi.csproj ./
RUN dotnet restore socialApi.csproj

COPY . ./
RUN dotnet publish socialApi.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

COPY --from=build /app/out ./

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "socialApi.dll"]
