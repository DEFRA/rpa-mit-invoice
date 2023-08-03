using System.Diagnostics.CodeAnalysis;
using EST.MIT.Invoice.Api.Util;


namespace EST.MIT.Invoice.Api.Test.Services.Api.ReferenceDataApiService
{
    [ExcludeFromCodeCoverage]
    public class FaultedHttpContentDeserializer : IHttpContentDeserializer
    {
        public Task<IEnumerable<T>> DeserializeList<T>(HttpContent content)
        {
            var taskCompletionSource = new TaskCompletionSource<IEnumerable<T>>(TaskCreationOptions.RunContinuationsAsynchronously);
            taskCompletionSource.SetException(new Exception("An error occurred while processing the response."));
            return taskCompletionSource.Task;
        }
    }
}
