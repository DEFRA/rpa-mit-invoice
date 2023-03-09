# Introduction 
This repository contains the code for the Manual Invoice Template Invoice Api

# Getting Started

## Azurite

Follow the following guide to setup Azurite:

- [Azurite emulator for local Azure Storage development](https://dev.azure.com/defragovuk/DEFRA-EST/_wiki/wikis/DEFRA-EST/7722/Azurite-emulator-for-local-Azure-Storage-development)

- [Docker](https://dev.azure.com/defragovuk/DEFRA-EST/_wiki/wikis/DEFRA-EST/9601/Azurite-with-Docker)

## Storage

The function app uses Azure Storage for Table and Queue.

The function app requires:

- Table name: `invoices`
- Queue name: `invoice-generation`

## local.settings

```
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "Storage": {
    "ConnectionString": "UseDevelopmentStorage=true",
    "ContainerName": "invoices",
    "TableName": "invoices"
  },
  "AllowedHosts": "*"
}
```

## Endpoints

`GET /invoice/{scheme}/{invoiceId}`

### Response 200

```
{
  "id": "123456789",
  "scheme": "bps",
  "status": "approved",
  "createdBy": "me",
  "updatedBy": null,
  "header": {
    "id": "123456789",
    "claimReference": "123456789",
    "claimReferenceNumber": "MIT123456",
    "frn": "123456789",
    "agreementNumber": "MIT987654321",
    "currency": "gdp",
    "description": "Test payload"
  }
}
```

`POST /invoice`

### Payload Example

```
{
  "id": "123456789",
  "scheme": "bps",
  "status": "awaiting",
  "createdBy": "me",
  "header": {
    "id": "123456789",
    "claimReference": "123456789",
    "claimReferenceNumber": "MIT123456",
    "frn": "123456789",
    "agreementNumber": "MIT987654321",
    "currency": "gdp",
    "description": "Test payload"
  }
}
```

`PUT /invoice`

### Payload Example

```
{
  "id": "123456789",
  "scheme": "bps",
  "status": "approved",
  "createdBy": "me",
  "header": {
    "id": "123456789",
    "claimReference": "123456789",
    "claimReferenceNumber": "MIT123456",
    "frn": "123456789",
    "agreementNumber": "MIT987654321",
    "currency": "gdp",
    "description": "Test payload"
  }
}
```

## Queue

### Message Example

```
{
  "id": "123456789",
  "scheme": "bps"
}
```

# Build and Test
To run the function:

`cd TEST.Function`

`func start`

## Useful links

- [gov Notify](https://www.notifications.service.gov.uk/using-notify/api-documentation)

- [Use dependency injection in .NET Azure Functions](https://learn.microsoft.com/en-us/azure/azure-functions/functions-dotnet-dependency-injection)