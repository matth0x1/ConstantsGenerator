namespace ConstantGenerator
{
    public interface IConfiguration
    {
        string InputFilePath { get; }
        string OutputFilePath { get; }
        string RootNamespace { get; }
        int DebugLevel { get; }
    }
}