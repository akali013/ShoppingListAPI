# syntax=docker/dockerfile:1

# Create a stage for building the application.
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build

COPY . /source

WORKDIR /source

# This is the architecture you’re building for, which is passed in by the builder.
# Placing it here allows the previous steps to be cached across architectures.
# ARG TARGETARCH
# ARG TARGETOS

# Build the application.
# Leverage a cache mount to /root/.nuget/packages so that subsequent builds don't have to re-download packages.
RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    dotnet publish -r linux-x64 --use-current-runtime --self-contained false -o /app

################################################################################
# Create a new stage for running the application that contains the minimal
# runtime dependencies for the application. This often uses a different base
# image from the build stage where the necessary files are copied from the build
# stage.
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS final
WORKDIR /app

# Copy everything needed to run the app from the "build" stage.
COPY --from=build /app .

RUN apk add icu-libs
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

# Switch to a non-privileged user (defined in the base image) that the app will run under.
USER $APP_UID

ENTRYPOINT ["dotnet", "ShoppingListAPI.dll"]
