FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /source


COPY *.sln .
COPY LinkPulseDefinitions/*.csproj ./LinkPulseDefinitions/
COPY LinkPulseImplementations/*.csproj ./LinkPulseImplementations/
COPY LinkPulseTests/*.csproj ./LinkPulseTests/
COPY WebInterface.Tests/*.csproj ./WebInterface.Tests/
RUN dotnet restore


COPY LinkPulseDefinitions/. ./LinkPulseDefinitions/
COPY LinkPulseImplementations/. ./LinkPulseImplementations/
COPY LinkPulseTests/. ./LinkPulseTests/
COPY WebInterface/. ./WebInterface/
WORKDIR /source/WebInterface
RUN dotnet publish -c release -o /app --no-restore



FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /app ./
EXPOSE 5000
ENV ASPNETCORE_ENVIRONMENT Production
ENTRYPOINT ["dotnet", "WebInterface.dll", "--urls=http://0.0.0.0:5000"]