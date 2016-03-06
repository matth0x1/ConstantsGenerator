using System;
using System.Collections.Generic;
using System.Text;
using NDesk.Options;

namespace ConstantGenerator
{
    class Arguments : IConfiguration
    {
        public static readonly string HelpString ="Options: i|input  o|output  n|namespace\n" +
                                                  "d|debug  s|silent  h|?|Help";

        public Arguments(string[] args)
        {
            try
            {
                var p = new OptionSet()
                {
                    {"i|input=", delegate(string v) { InputFilePath = v; }},
                    {"o|output=", delegate(string v) { OutputFilePath = v; }},
                    {"n|namespace=", delegate(string v) { RootNamespace = v; }},
                    {"d|debug", delegate(string v) { DebugLevel ++; }},
                    {"h|?|help", delegate(string v) { Help = v != null; }},
                    {"s|silent", delegate(string v) { Silent = v != null; }},
                };

                ExtraArgs = p.Parse(args);
            }
            catch (Exception e)
            {
                throw new ArgumentException("Invalid command line arguments, unable to parse.", e);
            }

            try
            {
                // Check manditory/requried options are present.
                if (string.IsNullOrWhiteSpace(InputFilePath))
                    throw new Exception("Manditory option i|input is missing.");

                if (string.IsNullOrWhiteSpace(OutputFilePath))
                    throw new Exception("Manditory option o|output is missing.");

                if (string.IsNullOrWhiteSpace(RootNamespace))
                    throw new Exception("Manditory option n|namespace is missing.");
            }
            catch
            {
                if (DebugLevel > 0)
                    Console.WriteLine(PrintArguments());

                throw;
            }
        }

        public string InputFilePath { get; private set; }
        public string OutputFilePath { get; private set; }
        public string RootNamespace { get; set; }

        public int DebugLevel { get; private set; }
        public bool Help { get; private set; }
        public bool Silent { get; private set; }
        public List<string> ExtraArgs { get; private set; }

        public override string ToString()
        {
            return PrintArguments();
        }

        private string PrintArguments()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Arguments");
            sb.AppendLine("  InputFilePath: " + InputFilePath);
            sb.AppendLine("  OutputFilePath: " + OutputFilePath);
            sb.AppendLine("  Namespace: " + RootNamespace);
            sb.AppendLine("  Silent: " + Silent);
            sb.AppendLine("  DebugLevel: " + DebugLevel);
            sb.AppendLine("  Help: " + Help);
            sb.AppendLine("  ExtraArgs:");

            foreach (string extraArg in ExtraArgs)
                sb.AppendLine("    " + extraArg);

            return sb.ToString();
        }
    }
}