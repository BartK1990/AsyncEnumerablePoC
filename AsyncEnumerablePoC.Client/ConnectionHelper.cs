using System.Net;
using AsyncEnumerablePoC.Server;
using Flurl;

namespace AsyncEnumerablePoC.Client;
public static class ConnectionHelper
{
    public const string MongoDbLocalhost = "mongodb://localhost:27017";
    public const string MongoDbName = "AsyncEnumerablePoC_DB";

    public static async Task CheckServerConnection(HttpClient httpClient)
    {
        bool success;
        int i = 0;
        do
        {
            Console.WriteLine($"Attempt to connect ... {++i}");

            using HttpResponseMessage response = await httpClient.GetAsync(
                Url.Combine(IpAddress.Localhost, "HistoricalData", "Connect")).ConfigureAwait(false);

            success = response.StatusCode == HttpStatusCode.OK;

            await Task.Delay(2000);
        } while (!success);

        Console.WriteLine("Connected!");
    }

    public static async Task FeedData(HttpClient httpClient, int samples)
    {
        using HttpResponseMessage response = await httpClient.PutAsync(
            Url.Combine(IpAddress.Localhost, "HistoricalData", "InsertData", samples.ToString()), null).ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode(); // It will throw if not success
    }
}
