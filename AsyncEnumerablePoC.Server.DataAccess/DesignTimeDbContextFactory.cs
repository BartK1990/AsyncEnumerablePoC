using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace AsyncEnumerablePoC.Server.DataAccess;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ReadDataDbContext> 
{ 
    public ReadDataDbContext CreateDbContext(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("connectionstrings.json", optional: false, reloadOnChange: false)
            .Build();

        string connectionString = configuration.GetConnectionString(ConnectionStrings.TheOnlyDatabase)!;
        return new ReadDataDbContext(connectionString);
    } 
}