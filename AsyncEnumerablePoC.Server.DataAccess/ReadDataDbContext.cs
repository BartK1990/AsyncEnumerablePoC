using AsyncEnumerablePoC.Server.DataAccess.Model;
using Microsoft.EntityFrameworkCore;

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
    }

    #nullable disable
    public DbSet<HistoricalData> HistoricalData { get; set; }

    public DbSet<HistoricalComplexData> HistoricalComplexData { get; set; }

    #nullable enable
}