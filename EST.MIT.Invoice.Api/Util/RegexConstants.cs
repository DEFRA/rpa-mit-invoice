using Invoices.Api.Models;
using Invoices.Api.Services.Models;
using Newtonsoft.Json;

namespace Invoices.Api.Util;

public static class RegexConstants
{
    public const string TwoDecimalPlaces = @"^-?(\d+)(\.\d{1,2})?$";
}