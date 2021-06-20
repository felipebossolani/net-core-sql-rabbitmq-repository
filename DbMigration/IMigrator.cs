namespace DbMigration
{
    public interface IMigrator
    {
        void Execute(string connectionString);
    }
}
