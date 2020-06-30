FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY *.sln .
COPY ObjectStorageWeb/*.csproj ./ObjectStorageWeb/
COPY ObjectStorage/*.csproj ./ObjectStorage/
RUN dotnet restore

# copy everything else and build app
COPY ObjectStorageWeb/. ./ObjectStorageWeb/
COPY ObjectStorage/. ./ObjectStorage/
WORKDIR /app/ObjectStorageWeb
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS runtime
WORKDIR /app
COPY --from=build /app/ObjectStorageWeb/out ./
ENTRYPOINT ["dotnet", "ObjectStorageWeb.dll"]
