
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
            var result = await content.ReadFromJsonAsync<IEnumerable<T>>();
            return result?.ToList() ?? new List<T>();
        }
    }
}
