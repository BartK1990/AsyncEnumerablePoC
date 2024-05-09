namespace AsyncEnumerablePoC.Server.DataAccess.Model;

public record HistoricalComplexData : DatabaseEntity
{
    public DateTime Timestamp { get; set; }

    public double Value1 { get; set; }

    public double Value2 { get; set; }

    public double Value3 { get; set; }

    public double Value4 { get; set; }

    public double Value5 { get; set; }
}