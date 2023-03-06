using System.Collections;

namespace Invoices.Api.Test;

public class InvoiceValidationTestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] { "", "bps", "awaiting", "Bob Test", "Id" };
        yield return new object[] { "1", "", "awaiting", "Bob Test", "Scheme" };
        yield return new object[] { "1", "bps", "", "Bob Test", "Status" };
        yield return new object[] { "1", "bps", "awaiting", "", "CreatedBy" };
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
