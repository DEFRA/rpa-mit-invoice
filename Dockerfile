ARG PARENT_VERSION=1.5.0-dotnet6.0

# Development
FROM defradigital/dotnetcore-development:$PARENT_VERSION AS development

ARG PARENT_VERSION

LABEL uk.gov.defra.parent-image=defra-dotnetcore-development:${PARENT_VERSION}

RUN mkdir -p /home/dotnet/EST.MIT.Invoice.Api/ /home/dotnet/EST.MIT.Invoice.Api.Test/

COPY --chown=dotnet:dotnet ./EST.MIT.Invoice.Api/*.csproj ./EST.MIT.Invoice.Api/
RUN dotnet restore ./EST.MIT.Invoice.Api/EST.MIT.Invoice.Api.csproj

COPY --chown=dotnet:dotnet ./EST.MIT.Invoice.Api.Test/*.csproj ./EST.MIT.Invoice.Api.Test/
RUN dotnet restore ./EST.MIT.Invoice.Api.Test/EST.MIT.Invoice.Api.Test.csproj

COPY --chown=dotnet:dotnet ./EST.MIT.Invoice.Api/ ./EST.MIT.Invoice.Api/
COPY --chown=dotnet:dotnet ./EST.MIT.Invoice.Api.Test/ ./EST.MIT.Invoice.Api.Test/

RUN dotnet publish ./EST.MIT.Invoice.Api/ -c Release -o /home/dotnet/out
 
ARG PORT=3000
ENV PORT ${PORT}
EXPOSE ${PORT}

CMD dotnet watch --project ./EST.MIT.Invoice.Api run --urls "http://*:${PORT}"

# # Production
FROM defradigital/dotnetcore:$PARENT_VERSION AS production

ARG PARENT_VERSION
ARG PARENT_REGISTRY

LABEL uk.gov.defra.parent-image=defra-dotnetcore-development:${PARENT_VERSION}

ARG PORT=3000
ENV ASPNETCORE_URLS=http://*:${PORT}
EXPOSE ${PORT}

COPY --from=development /home/dotnet/out/ ./
 
CMD dotnet EST.MIT.Invoice.Api.dll