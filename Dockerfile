﻿FROM mcr.microsoft.com/dotnet/sdk:5.0 as build

LABEL maintainer="@lazyboy1"

ARG ProjectName=Resharper.CodeInspections.BitbucketPipe

WORKDIR /source

COPY src/Resharper.CodeInspections.BitbucketPipe/Resharper.CodeInspections.BitbucketPipe.csproj .

RUN dotnet restore

COPY src/$ProjectName/. ./

RUN dotnet publish -c release -o /app


FROM mcr.microsoft.com/dotnet/runtime:5.0 as runtime

WORKDIR /app

COPY --from=build /app .

ENTRYPOINT ["dotnet", "/app/Resharper.CodeInspections.BitbucketPipe.dll"]
