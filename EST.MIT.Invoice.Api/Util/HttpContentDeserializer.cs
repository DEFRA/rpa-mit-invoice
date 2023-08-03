
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
            return await content.ReadFromJsonAsync<IEnumerable<T>>() ?? new List<T>();
        }
    }
}
