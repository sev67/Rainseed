using Planter.Factories.Abstract;

namespace RainSeedCli;

sealed internal class Arguments
{
    public string DumpFile
    { get; }

    public DbEngineAbstractFactory.SupportedDatabaseEngines DatabaseEngine
    { get; }
    
    public string ConnectionString
    { get; }

    public bool IgnoreCache
    { get; }

    internal Arguments(ParameterDefinitions definition)
    {
        static T ThrowInvalid<T>(char c) 
            => throw new ApplicationException($"Value for parameter -{c} is invalid.");
        
        DumpFile = definition.DumpFile ?? ThrowInvalid<string>('d');
        DatabaseEngine = EvaluateDatabaseEngine(definition.DatabaseEngine);
        ConnectionString = definition.ConnectionString ?? ThrowInvalid<string>('c');
        IgnoreCache = definition.IgnoreCache;
    }
    
    private DbEngineAbstractFactory.SupportedDatabaseEngines EvaluateDatabaseEngine(string? databaseEngine)
    {
        string input = databaseEngine?.ToLower().Trim() ?? string.Empty;

        if (input == "tsql" | input == "mssql" | input == "sqlserver") 
            return DbEngineAbstractFactory.SupportedDatabaseEngines.MicrosoftSQL;
        
        if (input == "odb" | input == "oracle") 
            return DbEngineAbstractFactory.SupportedDatabaseEngines.Oracle;
        
        if (input == "mysql" | input == "maria" | input == "mariadb") 
            return DbEngineAbstractFactory.SupportedDatabaseEngines.MariaDB;
        
        if (input == "sqlite") 
            return DbEngineAbstractFactory.SupportedDatabaseEngines.SQLite;
        
        if (input == "psql" | input == "postgre" | input == "postgresql") 
            return DbEngineAbstractFactory.SupportedDatabaseEngines.PostgreSQL;

        throw new ApplicationException("Invalid option for database engine.");
    }
}

sealed internal class ParameterDefinitions
{
    [Option('d', "dump", Required = true, HelpText = "Input dump file to be processed.")]
    public string? DumpFile
    { get; set; }

    [Option('e', "engine", Required = true, HelpText = "Database Engine.")]
    public string? DatabaseEngine
    { get; set; }
    
    [Option('c', "connection", Required = true, HelpText = "Connection string to seeded database.")]
    public string? ConnectionString
    { get; set; }

    [Option('r', "rebuild", Required = false)]
    public bool IgnoreCache
    { get; set; } = false;
    
    public static implicit operator Arguments(ParameterDefinitions self)
    => new (self);
}