FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["BlazorIdentityTutorial/BlazorIdentityTutorial.csproj", "BlazorIdentityTutorial/"]
RUN dotnet restore "BlazorIdentityTutorial/BlazorIdentityTutorial.csproj"
COPY . .
WORKDIR "/src/BlazorIdentityTutorial"
RUN dotnet build "BlazorIdentityTutorial.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BlazorIdentityTutorial.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BlazorIdentityTutorial.dll"]
