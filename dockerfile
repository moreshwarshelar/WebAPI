FROM mcr.microsoft.com/dotnet/core/aspnet AS base
WORKDIR /app
#EXPOSE 44351

FROM mcr.microsoft.com/dotnet/core/sdk AS build
WORKDIR /src

COPY Web-API-Calls-Graph.sln ./
COPY Microsoft.Identity.Web/*.csproj ./Microsoft.Identity.Web/
COPY TodoListService/*.csproj ./TodoListService/


RUN dotnet restore
COPY . .
WORKDIR /src/Microsoft.Identity.Web
RUN dotnet build -c Release -o /app

WORKDIR /src/TodoListService
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "GREATRoomAPI.dll"]
