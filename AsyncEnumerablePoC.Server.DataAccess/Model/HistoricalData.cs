namespace AsyncEnumerablePoC.Server.DataAccess.Model;

public class HistoricalData : DatabaseEntity
{
    public DateTime Timestamp { get; set; }

    public double Value { get; set; }
}