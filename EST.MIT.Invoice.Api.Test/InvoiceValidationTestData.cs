using System.Collections;

namespace Invoices.Api.Test;

public class InvoiceValidationTestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] { "", "bps", "awaiting", "Id" };
        yield return new object[] { "1", "", "awaiting", "Scheme" };
        yield return new object[] { "1", "bps", "", "Status" };
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
