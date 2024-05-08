using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;

namespace AsyncEnumerablePoC.Server.DataAccess;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<DataDbContext> 
{ 
    public DataDbContext CreateDbContext(string[] args)
    {
        string curDir = Directory.GetCurrentDirectory();

        var test = new ConfigurationBuilder();

        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
            .Build(); 

        string connectionString = configuration.GetConnectionString("TheOnlyDatabase");

        Debugger.Launch();

        return new DataDbContext(connectionString);
    } 
}