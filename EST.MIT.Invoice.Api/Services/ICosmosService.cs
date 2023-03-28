using Invoices.Api.Models;

namespace Invoices.Api.Services;

public interface ICosmosService
{
    Task<List<Invoice>> Get(string sqlCosmosQuery);
    Task<Invoice> Create(Invoice invoice);
    Task<Invoice> Update(Invoice invoice);
    Task Delete(string id, string scheme);
}