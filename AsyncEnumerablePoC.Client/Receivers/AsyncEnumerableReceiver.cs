using System.Net.Http.Json;
using AsyncEnumerablePoC.Server;
using Flurl;
using System.Text.Json;

namespace AsyncEnumerablePoC.Client.Receivers;
public static class AsyncEnumerableReceiver
{
    public static int DefaultBufferSize = 128;

    public static async IAsyncEnumerable<T> RequestData<T>(HttpClient httpClient, string subUrl)
    {
        using HttpResponseMessage response = await httpClient.GetAsync(
            Url.Combine(IpAddress.Localhost, subUrl),
            HttpCompletionOption.ResponseHeadersRead
        ).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        Stream responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

        var dataRows = JsonSerializer.DeserializeAsyncEnumerable<T>(
            responseStream,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                DefaultBufferSize = DefaultBufferSize
            });

        await foreach (T dataRow in dataRows)
        {
            yield return dataRow;
        }
    }

    public static async IAsyncEnumerable<T> PostData<T>(HttpClient httpClient, string subUrl, object body)
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

        var dataRows = JsonSerializer.DeserializeAsyncEnumerable<T>(
            responseStream,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                DefaultBufferSize = DefaultBufferSize
            });

        await foreach (T dataRow in dataRows)
        {
            yield return dataRow;
        }
    }
}
