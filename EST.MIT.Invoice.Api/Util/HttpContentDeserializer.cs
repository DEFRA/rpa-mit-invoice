
namespace EST.MIT.Invoice.Api.Util
{
    public interface IHttpContentDeserializer
    {
        Task<IEnumerable<T>> DeserializeListAsync<T>(HttpContent content);
    }

    public class HttpContentDeserializer : IHttpContentDeserializer
    {
        public async Task<IEnumerable<T>> DeserializeListAsync<T>(HttpContent content)
        {
            return await content.ReadFromJsonAsync<IEnumerable<T>>() ?? new List<T>();
        }
    }
}
