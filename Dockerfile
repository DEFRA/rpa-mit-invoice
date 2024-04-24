# Development
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS development

RUN mkdir -p /home/dotnet/EST.MIT.Invoice.Api/ /home/dotnet/EST.MIT.Invoice.Api.Test/

COPY --chown=dotnet:dotnet ./EST.MIT.Invoice.Api/*.csproj ./EST.MIT.Invoice.Api/
RUN dotnet restore ./EST.MIT.Invoice.Api/EST.MIT.Invoice.Api.csproj

COPY --chown=dotnet:dotnet ./EST.MIT.Invoice.Api.Test/*.csproj ./EST.MIT.Invoice.Api.Test/
RUN dotnet restore ./EST.MIT.Invoice.Api.Test/EST.MIT.Invoice.Api.Test.csproj

COPY --chown=dotnet:dotnet ./EST.MIT.Invoice.Api/ ./EST.MIT.Invoice.Api/
COPY --chown=dotnet:dotnet ./EST.MIT.Invoice.Api.Test/ ./EST.MIT.Invoice.Api.Test/

RUN dotnet publish ./EST.MIT.Invoice.Api/ -c Release -o /home/dotnet/out
 
ARG PORT=5000
ENV PORT ${PORT}
EXPOSE ${PORT}

CMD dotnet watch --project ./EST.MIT.Invoice.Api run --urls "http://*:${PORT}"

# # Production
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS production

ARG PARENT_VERSION
ARG PARENT_REGISTRY

ARG PORT=5000
ENV ASPNETCORE_URLS=http://*:${PORT}
EXPOSE ${PORT}

COPY --from=development /home/dotnet/out/ ./
 
CMD dotnet EST.MIT.Invoice.Api.dll