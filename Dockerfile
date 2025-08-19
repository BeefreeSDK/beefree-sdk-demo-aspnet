FROM ubuntu:20.04

# Prevent interactive prompts during package installation
ENV DEBIAN_FRONTEND=noninteractive

# Install Mono and required packages
RUN apt-get update && apt-get install -y \
    gnupg \
    ca-certificates \
    && apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys 3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF \
    && echo "deb https://download.mono-project.com/repo/ubuntu stable-focal main" > /etc/apt/sources.list.d/mono-official-stable.list \
    && apt-get update \
    && apt-get install -y \
    mono-complete \
    nuget \
    && rm -rf /var/lib/apt/lists/*

# Set working directory
WORKDIR /app

# Copy project files
COPY . .

# Restore NuGet packages and build the project
RUN nuget restore BeePluginIntegration.sln && \
    msbuild BeePluginIntegration.sln /p:Configuration=Debug /p:Platform="Any CPU"

# Expose port
EXPOSE 54067

# Start the application
WORKDIR /app/BeePluginIntegration
CMD ["mono", "/usr/lib/mono/4.5/xsp4.exe", "--port", "54067", "--root", "/app/BeePluginIntegration", "--nonstop"]
