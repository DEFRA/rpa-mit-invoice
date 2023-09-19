using System.Collections;

namespace EST.MIT.Invoice.Api.Test;

public class PaymentRequestsBatchValidationTestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] { "", "bps", "awaiting", "Id" };
        yield return new object[] { "1", "", "awaiting", "SchemeType" };
        yield return new object[] { "1", "bps", "", "Status" };
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
