FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

COPY . ./
RUN dotnet publish ./src/TMTProductizer/TMTProductizer.csproj -c Release -o out 

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app/out .

EXPOSE 5286

ENTRYPOINT ["dotnet", "TMTProductizer.dll"]