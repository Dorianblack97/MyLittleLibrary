FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["MyLittleLibrary.csproj", "./"]
RUN dotnet restore "MyLittleLibrary.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "MyLittleLibrary.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MyLittleLibrary.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MyLittleLibrary.dll"]