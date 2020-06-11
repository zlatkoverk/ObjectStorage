FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY *.sln .
COPY ObjectStorageWeb/*.csproj ./ObjectStorageWeb/
COPY ObjectStorage/*.csproj ./ObjectStorage/
COPY ObjectStorageTest/*.csproj ./ObjectStorageTest/
COPY ObjectStorageConsole/*.csproj ./ObjectStorageConsole/
RUN dotnet restore

# copy everything else and build app
COPY ObjectStorageWeb/. ./ObjectStorageWeb/
COPY ObjectStorage/. ./ObjectStorage/
COPY ObjectStorageTest/. ./ObjectStorageTest/
COPY ObjectStorageConsole/. ./ObjectStorageConsole/
WORKDIR /app/ObjectStorageWeb
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS runtime
WORKDIR /app
COPY --from=build /app/ObjectStorageWeb/out ./
ENTRYPOINT ["dotnet", "ObjectStorageWeb.dll"]
