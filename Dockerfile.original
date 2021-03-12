
FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Servize.csproj", ""]
RUN dotnet restore "./Servize.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "Servize.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Servize.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Servize.dll"]

ENV GoogleClientId	= "767916686704-fql4bubmbka31ftnadb70t656pa5kvab.apps.googleusercontent.com",
ENV  GoogleSecret	= "_IASP8rZypXBJdYi3TMO8xyb",
ENV  AppId			=	"527735858190215",
ENV  AppSecret		= "35c7e9fbe416b54c5ce9f45ada3d725b",
ENV  Server			=	 "servizetest.database.windows.net",
ENV  DatabaseName	= "serviceTestDb",
ENV  Password		= "@Lfred1205",
ENV  User Id		= "servizeAdmin"