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

- Table name: `invoices`
- Queue name: `payment`

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
    "QueueName": "payment"
  },
  ,
  "AzureCosmosDbSettings": {
    "URL": "https://localhost:8081/",
    "PrimaryKey": "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==",
    "DatabaseName": "Manual-Invoice-Template",
    "ContainerName": "Invoice"
  },
  "AllowedHosts": "*"
}
```

## CosmosDb

Database Name: `Manual-Invoice-Template`
Container Id: `Invoice`
PartitionKey: `/schemeType`

## Endpoints

`GET /invoice/{scheme}/{invoiceId}`

### Response 200

```
{
    {
  "id": "string",
  "invoiceType": "string",
  "accountType": "string",
  "organisation": "string",
  "schemeType": "string",
  "headers": [
    {
      "paymentRequestId": "string",
      "frn": 0,
      "sourceSystem": "string",
      "marketingYear": 0,
      "ledger": "string",
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
  "created": "2023-03-24T12:23:35.949Z",
  "updated": "2023-03-24T12:23:35.949Z",
  "createdBy": "string",
  "updatedBy": "string"
}
```

`POST /invoice`

### Payload Example

```
{
    "Id": "00000000-0000-0000-0000-000000000000",
    "InvoiceType": "",
    "AccountType": "",
    "Organisation": "",
    "SchemeType": "",
    "Headers": [{
        "PaymentRequestId": "",
        "FRN": 0,
        "SourceSystem": "",
        "MarketingYear": 0,
        "Ledger": "",
        "DeliveryBody": "RP00",
        "PaymentRequestNumber": 0,
        "AgreementNumber": "",
        "ContractNumber": "",
        "Value": 0,
        "DueDate": "",
        "InvoiceLines": [{
            "Value": 0,
            "Currency": "GBP",
            "SchemeCode": "",
            "Description": "",
            "FundCode": ""
        }],
        "AppendixReferences": {
            "ClaimReferenceNumber": ""
        }
    }]
}
```

`PUT /invoice/{invoiceId}`

### Payload Example

```
{
    "Id": "00000000-0000-0000-0000-000000000000",
    "InvoiceType": "",
    "AccountType": "",
    "Organisation": "",
    "SchemeType": "",
    "Headers": [{
        "PaymentRequestId": "",
        "FRN": 0,
        "SourceSystem": "",
        "MarketingYear": 0,
        "Ledger": "",
        "DeliveryBody": "RP00",
        "PaymentRequestNumber": 0,
        "AgreementNumber": "",
        "ContractNumber": "",
        "Value": 0,
        "DueDate": "",
        "InvoiceLines": [{
            "Value": 0,
            "Currency": "GBP",
            "SchemeCode": "",
            "Description": "",
            "FundCode": ""
        }],
        "AppendixReferences": {
            "ClaimReferenceNumber": ""
        }
    }]
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