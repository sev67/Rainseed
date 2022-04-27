namespace Planter.Factories.Abstract;

sealed public class DbEngineAbstractFactory : AbstractFactory<KataCompilers::Compiler>
{
    private readonly KataCompilers::Compiler _compiler;
    
    public enum SupportedDatabaseEngines
    {
        MicrosoftSQL,
        SQLite,
        PostgreSQL,
        MariaDB,
        Oracle
    }

    public DbEngineAbstractFactory(SupportedDatabaseEngines engine)
    {
        _compiler = engine switch
        {
            SupportedDatabaseEngines.MicrosoftSQL => new KataCompilers::SqlServerCompiler(),
            SupportedDatabaseEngines.SQLite => new KataCompilers::SqliteCompiler(),
            SupportedDatabaseEngines.PostgreSQL => new KataCompilers::PostgresCompiler(),
            SupportedDatabaseEngines.MariaDB => new KataCompilers::MySqlCompiler(),
            SupportedDatabaseEngines.Oracle => new KataCompilers::OracleCompiler(),
            _ => throw new ArgumentOutOfRangeException(nameof(engine), engine, null)
        };
    }

    override public KataCompilers::Compiler Result() 
    => _compiler;
}