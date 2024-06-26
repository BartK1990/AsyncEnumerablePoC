﻿using AsyncEnumerablePoC.Server;
using Flurl;
using System.Text.Json;

namespace AsyncEnumerablePoC.Client.Receivers;
public static class AsyncEnumerableReceiver
{
    public static async IAsyncEnumerable<T> RequestData<T>(HttpClient httpClient, string subUrl)
    {
        using HttpResponseMessage response = await httpClient.GetAsync(
            Url.Combine(IpAddress.Localhost, subUrl),
            HttpCompletionOption.ResponseHeadersRead
        ).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        Stream responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

        IAsyncEnumerable<T?> dataRows = JsonSerializer.DeserializeAsyncEnumerable<T>(
            responseStream,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                DefaultBufferSize = 128
            });

        await foreach (T dataRow in dataRows)
        {
            yield return dataRow;
        }
    }
}
