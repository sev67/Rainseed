namespace Planter.Factories;

sealed public class QueryFactory : KataExec::QueryFactory
{
    private const int SQL_TIMEOUT = 30;
    
    /// <summary>
    /// Default constructor.
    /// </summary>
    public QueryFactory()
        : base()
    {/* ... */}

    /// <summary>
    /// Parameterized constructor.
    /// </summary>
    public QueryFactory(SysDat::IDbConnection connection, KataCompilers::Compiler compiler, int timeout = SQL_TIMEOUT)
        : base (connection, compiler, timeout)
    {/* ... */}

    /// <summary>
    /// Copy constructor.
    /// </summary>
    public QueryFactory(QueryFactory self)
        : base(self.Connection, self.Compiler, self.QueryTimeout)
    {/* ... */}
}