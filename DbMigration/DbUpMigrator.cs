using DbUp;
using DbUp.Engine;
using DbUp.Helpers;
using DbUp.Support;
using System;
using System.Linq;

namespace DbMigration
{
    public class DbUpMigrator : IMigrator
    {
        public void Execute(string connectionString)
        {
            EnsureDatabase.For.SqlDatabase(connectionString);

            EnsureJournalSchema(connectionString);

            var result = DeployChanges.To
                .SqlDatabase(connectionString)
                .JournalToSqlTable("DbUp", "SchemaVersions")
                .WithScriptsEmbeddedInAssembly(typeof(DbUpMigrator).Assembly)
                .LogToConsole()
                .WithTransactionPerScript()
                .WithExecutionTimeout(TimeSpan.FromMinutes(10))
                .Build()
                .PerformUpgrade();

            HandleResult(result);
        }

        private void EnsureJournalSchema(string connectionString) =>
            DeployChanges.To
                .SqlDatabase(connectionString)
                .JournalTo(new NullJournal())
                .WithScript(new SqlScript("EnsureDataBaseExists.sql"
                    , @"IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = 'Store') CREATE DATABASE [Store]"
                    , new SqlScriptOptions { ScriptType = ScriptType.RunOnce }))
                .WithScript(new SqlScript("EnsureJournalSchemaExists.sql"
                    , @"IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = N'DbUp') EXEC('CREATE SCHEMA [DbUp]')"
                    , new SqlScriptOptions { ScriptType = ScriptType.RunOnce }))
                .LogToConsole()
                .WithTransactionPerScript()
                .Build()
                .PerformUpgrade();

        private void HandleResult(DatabaseUpgradeResult result)
        {
            if (!result.Successful)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(result.Error);
                Console.ResetColor();
#if DEBUG
                Console.ReadLine();
#endif
                throw result.Error;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Success!");
            Console.WriteLine($"Executed scripts: ({result.Scripts.Count()}) {string.Join(", ", result.Scripts)}");
            Console.ResetColor();
        }
    }
}
