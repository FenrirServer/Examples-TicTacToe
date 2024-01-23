FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["TicTacToe.Server/TicTacToe.Server.csproj", "TicTacToe.Server/"]
COPY ["TicTacToe.Shared/TicTacToe.Shared.csproj", "TicTacToe.Shared/"]
RUN dotnet restore "TicTacToe.Server/TicTacToe.Server.csproj"
COPY . .
WORKDIR "/src/TicTacToe.Server"
RUN dotnet build "TicTacToe.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TicTacToe.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TicTacToe.Server.dll"]