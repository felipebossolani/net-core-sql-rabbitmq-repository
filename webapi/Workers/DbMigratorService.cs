using DbMigration;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace WebApi.Workers
{
    public class DbMigratorService : BackgroundService
    {
        private readonly IMigrator _migrator;
        private readonly string _connectionString = "";

        public DbMigratorService(IMigrator migrator, string connectionString)
        {
            _migrator = migrator;
            _connectionString = connectionString;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _migrator.Execute(_connectionString);
            return Task.CompletedTask;
        }
    }
}
