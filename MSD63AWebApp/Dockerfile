FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build-env
WORKDIR /app

COPY MSD63AWebApp/MSD63AWebApp.csproj MSD63AWebApp/
COPY Common/Common.csproj Common/
RUN dotnet restore MSD63AWebApp/MSD63AWebApp.csproj

COPY . ./
RUN dotnet publish MSD63AWebApp -c Release -o out
 
FROM mcr.microsoft.com/dotnet/aspnet:3.1
WORKDIR /app
EXPOSE 80
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "MSD63AWebApp.dll"]