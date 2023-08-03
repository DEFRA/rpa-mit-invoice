using System.Text.Json;

namespace EST.MIT.Invoice.Api.Util
{
    public interface IHttpContentDeserializer
    {
        Task<IEnumerable<T>> DeserializeList<T>(HttpContent content);
    }

    public class HttpContentDeserializer : IHttpContentDeserializer
    {
        public async Task<IEnumerable<T>> DeserializeList<T>(HttpContent content)
        {
            try
            {
                var result = await content.ReadFromJsonAsync<IEnumerable<T>>();
                return result?.ToList() ?? new List<T>();
            }
            catch (JsonException)
            {
                throw new Exception("An error occurred while processing the response.");
            }
        }
    }
}
