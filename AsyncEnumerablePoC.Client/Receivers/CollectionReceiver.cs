using Flurl;
using System.Text.Json;
using AsyncEnumerablePoC.Server;

namespace AsyncEnumerablePoC.Client.Receivers;
public static class CollectionReceiver
{
    public static async Task<IReadOnlyCollection<T>> RequestData<T>(HttpClient httpClient, string subUrl)
    {
        using HttpResponseMessage response = await httpClient.GetAsync(
            Url.Combine(IpAddress.Localhost, subUrl),
            HttpCompletionOption.ResponseHeadersRead
        ).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        Stream responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

        IReadOnlyCollection<T>? dataRows = await JsonSerializer.DeserializeAsync<IReadOnlyCollection<T>>(
            responseStream,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                DefaultBufferSize = 128
            });

        return dataRows;
    }
}
