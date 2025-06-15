# Stage 1: The Build Environment
# We use the full .NET 8 SDK to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Step 1: Copy only the project files to leverage Docker's layer caching.
# This layer is only rebuilt if you change your project references.
COPY Bookstore/Bookstore.csproj ./Bookstore/
COPY Presentation/Presentation.csproj ./Presentation/
COPY Application/Application.csproj ./Application/
COPY Domain/Domain.csproj ./Domain/
COPY Infrastructure/Infrastructure.csproj ./Infrastructure/

# Step 2: Restore all NuGet packages for all projects.
# If you have a .sln file, running restore on it is often easier.
# If not, restoring the main project will resolve the dependency tree.
RUN dotnet restore "Bookstore/Bookstore.csproj"

# Step 3: Copy the rest of the source code.
# This is done *after* restore so code changes don't invalidate the restore layer.
COPY . .

# Step 4: Publish the main application.
# We specify the main project to publish. The output will be placed in the /app folder.
RUN dotnet publish "Bookstore/Bookstore.csproj" -c Release -o /app --no-restore

# Stage 2: The Final Production Environment
# We use the much smaller ASP.NET runtime for the final image.
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Step 5: Copy the published output from the build stage.
# This ensures the final image is small and contains no source code.
COPY --from=build /app .

# Step 6: Define the entry point to run your API.
ENTRYPOINT ["dotnet", "Bookstore.dll"]