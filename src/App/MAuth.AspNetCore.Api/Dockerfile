#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["App/MAuth.AspNetCore.Api/MAuth.AspNetCore.Api.csproj", "App/MAuth.AspNetCore.Api/"]
COPY ["Database/MAuth.AspNetCore.Database/MAuth.AspNetCore.Database.csproj", "Database/MAuth.AspNetCore.Database/"]
COPY ["Models/MAuth.AspNetCore.Enum/MAuth.AspNetCore.Enum.csproj", "Models/MAuth.AspNetCore.Enum/"]
COPY ["Database/MAuth.AspNetCore.Mongo/MAuth.AspNetCore.Mongo.csproj", "Database/MAuth.AspNetCore.Mongo/"]
COPY ["Models/MAuth.AspNetCore.Models/MAuth.AspNetCore.Models.csproj", "Models/MAuth.AspNetCore.Models/"]
COPY ["Database/MAuth.ASpNetCore.MySql/MAuth.AspNetCore.MySql.csproj", "Database/MAuth.ASpNetCore.MySql/"]
COPY ["Infrastructure/MAuth.AspNetCore.Infrastructure/MAuth.AspNetCore.Infrastructure.csproj", "Infrastructure/MAuth.AspNetCore.Infrastructure/"]
COPY ["Service/MAuth.AspNetCore.Services/MAuth.AspNetCore.Services.csproj", "Service/MAuth.AspNetCore.Services/"]
RUN dotnet restore "App/MAuth.AspNetCore.Api/MAuth.AspNetCore.Api.csproj"
COPY . .
WORKDIR "/src/App/MAuth.AspNetCore.Api"
RUN dotnet build "MAuth.AspNetCore.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MAuth.AspNetCore.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MAuth.AspNetCore.Api.dll"]