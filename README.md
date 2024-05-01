# Invoice API

This repository hosts a minimal API that exposes multiple endpoints, these endpoints are primarily called by other services, its use is as a data store for persisting data on in flight manual invoice templates.

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=rpa-mit-invoice&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=rpa-mit-invoice) [![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=rpa-mit-invoice&metric=code_smells)](https://sonarcloud.io/summary/new_code?id=rpa-mit-invoice) [![Coverage](https://sonarcloud.io/api/project_badges/measure?project=rpa-mit-invoice&metric=coverage)](https://sonarcloud.io/summary/new_code?id=rpa-mit-invoice) [![Lines of Code](https://sonarcloud.io/api/project_badges/measure?project=rpa-mit-invoice&metric=ncloc)](https://sonarcloud.io/summary/new_code?id=rpa-mit-invoice)
## Requirements

Amend as needed for your distribution, this assumes you are using windows with WSL.
- <details>
    <summary> .NET 8 SDK </summary>
    

    #### Basic instructions for installing the .NET 8 SDK on a debian based system.
  
    Amend as needed for your distribution.

    ```bash
    wget https://packages.microsoft.com/config/debian/12/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
    sudo dpkg -i packages-microsoft-prod.deb
    sudo apt-get update && sudo apt-get install -y dotnet-sdk-8.0
    ```
</details>

- [Docker](https://docs.docker.com/desktop/install/linux-install/)
- Service Bus Queue

---
## Create the database

Create the postgres database in docker

```bash
docker pull postgres
```
```bash
docker run --name MY_POSTGRES_DB -e POSTGRES_PASSWORD=password -p 5432:5432 -d postgres
```

---
## Local Setup

To run this service locally complete the following steps.
### Set up user secrets

Use the secrets-template to create a secrets.json in the same folder location. 

**Example** values that work in local environments for these 2 keys.

```json
{
    "AzureADPostgreSQLResourceID": "https://ossrdbms-aad.database.windows.net/.default",
    "DbConnectionTemplate": "Server={0};Port={1};Database={2};User Id={3};Password={4};"
}
```

Once this is done run the following command to add the projects user secrets

```bash
cat secrets.json | dotnet user-secrets set
```

These values can also be created as environment variables or as a development app settings file, but the preferred method is via user secrets.

### Start the Api

**NOTE** - You will need to create the database in postgres before starting for the first time.

```bash
cd EST.MIT.Invoice.Api
```

```bash
dotnet run
```

---
## Endpoints

### HTTP

#### Invoices

Retrieves invoice details for a specific scheme and invoice ID.
```http
GET /invoice/{scheme}/{invoiceId} 
``` 

Retrieves invoice details for a specific invoice ID.
```http
GET /invoice/{invoiceId} 
``` 

Retrieves invoice details associated with a specific payment request ID.
```http
GET /invoice/paymentrequest/{paymentRequestId} 
``` 

Retrieves approval details for a specific invoice.
```http
GET /invoice/approvals/{invoiceId} 
``` 

Retrieves all invoice approvals.
```http
GET /invoice/approvals 
``` 

Retrieves all invoices for a specific user ID.
```http
GET /invoices/user/{userId} 
``` 

Creates a new invoice.
```http
POST /invoice 
``` 

Creates multiple invoices in bulk.
```http
POST /invoices 
``` 

Updates an existing invoice by its ID.
```http
PUT /invoice/{invoiceId} 
``` 

Deletes a specific invoice by its scheme and ID.
```http
DELETE /invoice/{scheme}/{invoiceId} 
``` 

#### Swagger

Swagger is also available in development environments with more detailed information on the endpoints and their expected payloads.
```http
/swagger
```

