
# Use the official Microsoft .NET SDK image as a base image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 

# Set the working directory in the container
WORKDIR /app

# Copy the .csproj file and restore dependencies
COPY Reserve.Core/*.csproj ./Reserve/Reserve.Core/
COPY Reserve/*.csproj ./Reserve/Reserve/

RUN cd /app/Reserve/Reserve.Core && dotnet restore
RUN cd /app/Reserve/Reserve && dotnet restore

# Copy the rest of the source code
COPY . ./Reserve

# Build the application
RUN cd /app/Reserve/Reserve && dotnet publish -c Release -o out


# Build the runtime image
WORKDIR /app/Reserve/Reserve/out


ENV ASPNETCORE_URLS="http://+:80"
ENV ASPNETCORE_ENVIRONMENT="deployment_prod"

# Set the entry point for the application
ENTRYPOINT ["dotnet", "Reserve.dll"]