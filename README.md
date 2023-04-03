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
   "Id":"e9e128c3-fd53-40f5-a2f5-5c3b1046eacb",
   "InvoiceType":"AP",
   "AccountType":"",
   "Organisation":"",
   "SchemeType":"bps",
   "PaymentRequests":[
      {
         "PaymentRequestId":"9ce7e2a6-04d6-4b6e-8b4d-baf944f7d0f1",
         "FRN":1234567890,
         "SourceSystem":"Manual",
         "MarketingYear":0,
         "DeliveryBody":"RP00",
         "PaymentRequestNumber":0,
         "AgreementNumber":"",
         "ContractNumber":"",
         "Value":0.0,
         "DueDate":"",
         "InvoiceLines":[
            {
               "Value":0,
               "Currency":"GBP",
               "SchemeCode":"",
               "Description":"",
               "FundCode":""
            }
         ],
         "AppendixReferences":{
            "ClaimReference":""
         }
      }
   ],
   "status":"awaiting",
   "created":"2023-03-28T00:00:00+00:00",
   "updated":"2023-03-28T00:00:00+00:00",
   "createdBy":"",
   "updatedBy":""
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