using AsyncEnumerablePoC.Server;
using AsyncEnumerablePoC.Server.DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls(IpAddress.Localhost);

builder.Configuration
    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
    .AddJsonFile("connectionstrings.json", optional: false, reloadOnChange: false);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton(s => new ReadDataDbContext(
    builder.Configuration.GetConnectionString(ConnectionStrings.TheOnlyDatabase)!));

builder.Services.AddSingleton<HistoricalDataProvider>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.MapControllers();

// Apply Db Migrations automatically
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ReadDataDbContext>();
    context.Database.Migrate();
}

app.Use(async (context, next) =>
{
    await next.Invoke();
    app.Logger.LogInformation("Request status {Status} on endpoint: {Endpoint}",
        (HttpStatusCode)context.Response.StatusCode, context.Request.Path.Value);
});

app.Run();