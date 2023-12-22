using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Consumer.Entities;

namespace Consumer.Data;
public class DataContext: DbContext
{
    protected readonly IConfiguration _configuration;

    public DataContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        // connect to postgres with connection string from app settings

        var serviceConnection = _configuration.GetSection("serviceConnection");
        var connection = serviceConnection.GetValue<string>("PostgresConnection");
        options.UseNpgsql(connection);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

    }

    public DbSet<MessageEntity> Messages {get; set;}
}