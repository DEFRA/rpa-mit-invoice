# Introduction 
This repository contains the code for the Manual Invoice Template Invoice Api

# Getting Started

## CosmosDb

- [Install and use the Azure Cosmos DB Emulator for local development and testing](https://learn.microsoft.com/en-us/azure/cosmos-db/local-emulator?tabs=ssl-netstd21)

## Azurite

Follow the following guide to setup Azurite:

- [Azurite emulator for local Azure Storage development](https://dev.azure.com/defragovuk/DEFRA-EST/_wiki/wikis/DEFRA-EST/7722/Azurite-emulator-for-local-Azure-Storage-development)

- [Docker](https://dev.azure.com/defragovuk/DEFRA-EST/_wiki/wikis/DEFRA-EST/9601/Azurite-with-Docker)

## Storage

The function app uses Azure Storage for Table and Queue.

The function app requires:

- Table name: `rpamitinvoices`
- Queue name: `rpa-mit-payment`
- Queue name: `rpa-mit-events`

## local.settings

```
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "QueueConnectionString": "UseDevelopmentStorage=true",
  "EventQueueName": "rpa-mit-events"
  "PaymentQueueName": "rpa-mit-payment"
  "POSTGRES_HOST": "",
  "POSTGRES_PORT": "",
  "POSTGRES_DB": "",
  "POSTGRES_USER": "",
  "POSTGRES_PASSWORD": "",
  "AzureADPostgreSQLResourceID": "https://ossrdbms-aad.database.windows.net/.default",
  "ReferenceDataApiBaseUri": "https://localhost:7012"
}
```

## Endpoints

`GET /invoice/{scheme}/{invoiceId}` - Get an individual invoice

### Response 200

```
{
  "id": "e9e128c3-fd53-40f5-a2f5-5c3b1046eacb",
  "invoiceType": "AP",
  "accountType": "",
  "organisation": "",
  "schemeType": "bps",
  "paymentRequests": [
    {
      "paymentRequestId": "9ce7e2a6-04d6-4b6e-8b4d-baf944f7d0f1",
      "frn": 1234567890,
      "sourceSystem": "Manual",
      "marketingYear": 0,
      "deliveryBody": "RP00",
      "paymentRequestNumber": 0,
      "agreementNumber": "",
      "contractNumber": "",
      "value": 0,
      "dueDate": "",
      "invoiceLines": [
        {
          "value": 0,
          "currency": "GBP",
          "schemeCode": "",
          "description": "",
          "fundCode": ""
        }
      ],
      "appendixReferences": {
        "claimReferenceNumber": null
      }
    }
  ],
  "status": "awaiting",
  "reference": null,
  "created": "2023-03-28T01:00:00+01:00",
  "updated": "2023-03-28T01:00:00+01:00",
  "createdBy": "",
  "updatedBy": ""
}
```

`POST /invoice` - Create an individual invoice

### Payload Example

```
{
  "id": "e9e128c3-fd53-40f5-a2f5-5c3b1046eacb",
  "accountType": "AP",
  "organisation": "RPA",
  "paymentType": "DOM",
  "schemeType": "DA",
  "paymentRequests": [
    {
      "paymentRequestId": "a123",
      "sourceSystem": "Manual",
      "value": 100,
      "currency": "GBP",
      "description": "test invoice",
      "originalInvoiceNumber": "12345",
      "originalSettlementDate": "2023-09-26T05:57:30.358Z",
      "recoveryDate": "2023-09-26T05:57:30.358Z",
      "invoiceCorrectionReference": "string",
      "invoiceLines": [
        {
          "value": 100,
          "fundCode": "EXQ00",
          "mainAccount": "SOS210",
          "schemeCode": "10501",
          "marketingYear": 2099,
          "deliveryBody": "RP00",
          "description": "test invoice line",
          "currency": "GBP"
        }
      ],
      "marketingYear": 2023,
      "paymentRequestNumber": 1,
      "agreementNumber": "12345",
      "dueDate": "2023-11-01",
      "appendixReferences": {
        "claimReferenceNumber": "string"
      },
      "sbi": 0,
      "vendor": "string"
    }
  ],
  "status": "string",
  "reference": "string",
  "created": "2023-09-26T05:57:30.358Z",
  "updated": "2023-09-26T05:57:30.358Z",
  "createdBy": "string",
  "updatedBy": "string"
}
```

`POST /invoices` - Create multiple invoices

### Payload Example

```
{
  "invoices": [
    {
      "id": "string",
      "invoiceType": "string",
      "accountType": "string",
      "organisation": "string",
      "schemeType": "string",
      "paymentRequests": [
        {
          "paymentRequestId": "string",
          "frn": 0,
          "sourceSystem": "string",
          "marketingYear": 0,
          "deliveryBody": "string",
          "paymentRequestNumber": 0,
          "agreementNumber": "string",
          "contractNumber": "string",
          "value": 0,
          "dueDate": "string",
          "invoiceLines": [
            {
              "value": 0,
              "currency": "string",
              "schemeCode": "string",
              "description": "string",
              "fundCode": "string"
            }
          ],
          "appendixReferences": {
            "claimReferenceNumber": "string"
          }
        }
      ],
      "status": "string",
      "reference": "string",
      "created": "2023-04-03T07:18:19.457Z",
      "updated": "2023-04-03T07:18:19.457Z",
      "createdBy": "string",
      "updatedBy": "string"
    }
  ],
  "reference": "string",
  "schemeType": "string"
}
```

`PUT /invoice/{invoiceId}` - Update invoice

### Payload Example

```
{
  "id": "e9e128c3-fd53-40f5-a2f5-5c3b1046eacb",
  "invoiceType": "AP",
  "accountType": "",
  "organisation": "",
  "schemeType": "bps",
  "paymentRequests": [
    {
      "paymentRequestId": "9ce7e2a6-04d6-4b6e-8b4d-baf944f7d0f1",
      "frn": 1234567890,
      "sourceSystem": "Manual",
      "marketingYear": 0,
      "deliveryBody": "RP00",
      "paymentRequestNumber": 0,
      "agreementNumber": "",
      "contractNumber": "",
      "value": 0,
      "dueDate": "",
      "invoiceLines": [
        {
          "value": 0,
          "currency": "GBP",
          "schemeCode": "",
          "description": "",
          "fundCode": ""
        }
      ],
      "appendixReferences": {
        "claimReferenceNumber": null
      }
    }
  ],
  "status": "awaiting",
  "reference": null,
  "created": "2023-03-28T01:00:00+01:00",
  "updated": "2023-03-28T01:00:00+01:00",
  "createdBy": "",
  "updatedBy": ""
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