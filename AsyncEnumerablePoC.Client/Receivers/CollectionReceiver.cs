using Flurl;
using System.Text.Json;
using AsyncEnumerablePoC.Server;
using System.Net.Http.Json;

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

        var dataRows = await JsonSerializer.DeserializeAsync<IReadOnlyCollection<T>>(
            responseStream,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                DefaultBufferSize = 128
            });

        return dataRows;
    }

    public static async Task<IReadOnlyCollection<T>> PostData<T>(HttpClient httpClient, string subUrl, object body)
    {
        using HttpResponseMessage response = await httpClient.SendAsync(
            new HttpRequestMessage(HttpMethod.Post, Url.Combine(IpAddress.Localhost, subUrl))
            {
                Content = JsonContent.Create(body)
            },
            HttpCompletionOption.ResponseHeadersRead
        ).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        Stream responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

        var dataRows = await JsonSerializer.DeserializeAsync<IReadOnlyCollection<T>>(
            responseStream,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                DefaultBufferSize = 128
            });

        return dataRows;
    }
}
