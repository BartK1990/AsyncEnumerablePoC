namespace AsyncEnumerablePoC.Server.DataAccess.Model;

public record HistoricalData : DatabaseEntity
{
    public DateTime Timestamp { get; set; }

    public double Value { get; set; }
}