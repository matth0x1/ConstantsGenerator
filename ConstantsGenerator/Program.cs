using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace ConstantGenerator
{
    class Program
    {
        // TODO Configuration: Silent/quiet switch; no console output.

        public static string Version { get { return Assembly.GetExecutingAssembly().GetName().Version.ToString(); } }
        public static string Help { get { return Title + "\n  " + Arguments.HelpString; } }
        public static string Title { get { return "ConstantGenerator, version " + Version; } }
        
        static void Main(string[] args)
        {
            //StringBuilder sb = new StringBuilder();

            //for (int i = 0; i < 10; i++)
            //    for (int j = 0; j < 100; j++)
            //        for (int k = 0; k < 100; k++)
            //            sb.AppendLine("First_" + i + ".Second_" + j + ".Third_" + k + ", sometype, some comment here");

            //string output = sb.ToString();

            try
            {
                // Parse arguments
                Arguments arguments = new Arguments(args);

                // Print help and exit
                if (arguments.Help)
                {
                    Console.WriteLine(Help);
                    return;
                }

                // If silent (no output), redirect all console output.
                if (arguments.Silent)
                    Console.SetOut(new StringWriter());

                Console.WriteLine(Title+"\n");

                if (arguments.DebugLevel >= 2)
                    Console.WriteLine(arguments + "\n");

                Dictionary<string, List<ConstantEntry>> errors;
                List<ConstantClass> rootClasses = ConstantGenerator.Generate(arguments, out errors);

                if (arguments.DebugLevel > 0)
                {
                    Console.WriteLine("Constant Classes Generated");

                    foreach (ConstantClass rootClass in rootClasses)
                        Console.WriteLine(rootClass);
                }

                PrintErrors(errors);

                Console.WriteLine("Constants written to output file\n  " + Path.GetFullPath(arguments.OutputFilePath));
            }
            catch (Exception e)
            {
                Environment.ExitCode = 1;
                Console.WriteLine(Help);
                Console.WriteLine("\nAn exception was thrown: \n\n" + e);
            }
            finally
            {
#if DEBUG
                Console.ReadKey(false);
#endif
            }
        }

        private static void PrintErrors(Dictionary<string, List<ConstantEntry>> errors)
        {
            Console.WriteLine("Errors");

            foreach (KeyValuePair<string, List<ConstantEntry>> error in errors)
            {
                Console.WriteLine("  " + error.Key);

                foreach (ConstantEntry entry in error.Value)
                    Console.WriteLine("    " + entry);
            }

            Console.WriteLine();
        }
    }
}
