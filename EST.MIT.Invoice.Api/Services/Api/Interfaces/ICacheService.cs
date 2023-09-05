namespace EST.MIT.Invoice.Api.Services.Api.Interfaces;

public interface ICacheService
{
    /// <summary>
    /// Get Data using key
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    T GetData<T>(object key);

    /// <summary>
    /// Set Data with Value and Expiration Time of Key
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="value"></param>
    void SetData<T>(object key, T value);

    /// <summary>
    /// Remove Data
    /// </summary>
    /// <param name="key"></param>
    void RemoveData(object key);
}