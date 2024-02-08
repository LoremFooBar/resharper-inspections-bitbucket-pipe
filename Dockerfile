FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine as build

ARG ProjectName=Resharper.CodeInspections.BitbucketPipe

WORKDIR /source

COPY Directory.Build.props .
COPY src/Resharper.CodeInspections.BitbucketPipe/Resharper.CodeInspections.BitbucketPipe.csproj .

RUN dotnet restore

COPY src/$ProjectName/. ./

RUN dotnet publish -c Release -o /app


FROM mcr.microsoft.com/dotnet/runtime:8.0-alpine as runtime

WORKDIR /app

COPY --from=build /app .

ENTRYPOINT ["dotnet", "/app/Resharper.CodeInspections.BitbucketPipe.dll"]
