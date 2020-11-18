FROM mcr.microsoft.com/dotnet/sdk:3.1 as build

LABEL maintainer="amite15@gmail.com"

ARG ProjectName=Resharper.CodeInspections.BitbucketPipe

WORKDIR /source

COPY src/Resharper.CodeInspections.BitbucketPipe/Resharper.CodeInspections.BitbucketPipe.csproj .

RUN dotnet restore

COPY src/$ProjectName/. ./

RUN dotnet publish -c release -o /app


FROM mcr.microsoft.com/dotnet/runtime:3.1 as runtime

WORKDIR /app

COPY --from=build /app .

ENTRYPOINT ["dotnet", "/app/Resharper.CodeInspections.BitbucketPipe.dll"]
