FROM mcr.microsoft.com/dotnet/core/sdk:3.1 as build-env
WORKDIR /app

COPY . ./

FROM build-env as build-env-sender
COPY Messages/* ./Messages/
COPY Sender/*.csproj ./Sender/
RUN dotnet restore Sender
COPY Sender/* ./Sender/
RUN dotnet publish --no-restore Sender -c Release -o out

FROM build-env as build-env-receiver
COPY Messages/* ./Messages/
COPY Receiver/*.csproj ./Receiver/
RUN dotnet restore Receiver
COPY Receiver/* ./Receiver/
RUN dotnet publish --no-restore Receiver -c Release -o out

FROM mcr.microsoft.com/dotnet/core/runtime:3.1 as runtime-sender
WORKDIR /app
COPY --from=build-env-sender /app/out .
ENTRYPOINT ["dotnet", "Sender.dll"]

FROM mcr.microsoft.com/dotnet/core/runtime:3.1 as runtime-receiver
WORKDIR /app
COPY --from=build-env-receiver /app/out .
ENTRYPOINT ["dotnet", "Receiver.dll"]
