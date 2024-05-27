using AsyncEnumerablePoC.Server.DataAccess.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AsyncEnumerablePoC.Server.DataAccess;
public class ReadDataDbContext : DbContext
{
    private readonly string _connectionString;

    public ReadDataDbContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(_connectionString);
        //optionsBuilder.UseLoggerFactory(_loggerFactory)
        //    .LogTo(Console.WriteLine, LogLevel.Information)//.EnableSensitiveDataLogging()
        //    .UseSqlServer(_connectionString);
        base.OnConfiguring(optionsBuilder);
    }

    private readonly ILoggerFactory _loggerFactory = new LoggerFactory();

    #nullable disable
    public DbSet<HistoricalData> HistoricalData { get; set; }

    public DbSet<HistoricalComplexData> HistoricalComplexData { get; set; }

    #nullable enable
}