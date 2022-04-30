using RainCompiler.Lexer.Streams;
using StandardLib.Friends;
using StandardLib.Middleware.Disk;
using RainCompiler = RainCompiler.RainCompiler;

const int SIGFNSH = 0X000000; //POSIX-compatible process completed successfully.
const int SIGABRT = 0x000006; //POSIX-compatible process ended unexpectedly.

Parser.Default.ParseArguments<RS::ParameterDefinitions>(args)
    .WithParsed(options =>
    {
#if DEBUG
        Bootstrap(options);
#else
        try
        {
            Bootstrap(options);
        }
        catch (Exception gex)
        {
            CoreDump(gex);
            Halt();
        }
#endif
        Environment.Exit(SIGFNSH);
    })
    .WithNotParsed(Halt);

static void Bootstrap(RS::Arguments args)
{
    using FileStream fStream = new FileStream(args.DumpFile, FileMode.Open, FileAccess.Read);
    using DumpStreamReader dReader = new DumpStreamReader(fStream);
    AppData appData = new (".rs");
    
    RC::RainCompiler compiler = new (dReader, appData);
    if (compiler.HasBuilt())
    {
        compiler.Build();
    }
    else
    {
        compiler.Rebuild();
    }
    
    
}

static void Halt(IEnumerable<Error>? errs = null)
{
    "A critical failure has occured, the program has stopped to avoid memory corruption.".Endl().Cout();
    Environment.Exit(SIGABRT);
}

static void CoreDump(Exception gex)
{
    using FileStream fStream = new FileStream("CORE_DUMP", FileMode.Create, FileAccess.Write);
    using StreamWriter writer = new StreamWriter(fStream);

    writer.Write(gex);
}