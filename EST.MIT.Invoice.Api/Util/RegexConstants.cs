using EST.MIT.Invoice.Api.Models;
using EST.MIT.Invoice.Api.Services.Models;
using Newtonsoft.Json;

namespace EST.MIT.Invoice.Api.Util;

public static class RegexConstants
{
    public const string TwoDecimalPlaces = @"^-?(\d+)(\.\d{1,2})?$";
}