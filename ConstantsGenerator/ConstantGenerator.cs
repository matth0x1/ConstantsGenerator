using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Csv;

namespace ConstantGenerator
{
    public static class ConstantGenerator
    {
        public static List<ConstantClass> Generate(IConfiguration configuration, out Dictionary<string, List<ConstantEntry>> errors)
        {
            errors = new Dictionary<string, List<ConstantEntry>>();

            List<ConstantEntry> entries = new List<ConstantEntry>();
            List<ConstantEntry> duplicateEntries;

            // Read and parse CSV file to ConstantEntries.
            using (FileStream fileStream = new FileStream(configuration.InputFilePath, FileMode.Open, FileAccess.Read))
                entries.AddRangeOfConstants(CsvReader.ReadFromStream(fileStream), out duplicateEntries);

            if (entries.Count == 0)
                throw new Exception("No lines to parse in the input file; " + configuration.InputFilePath);

            if (duplicateEntries.Count > 0)
                errors.Add("Duplicates", duplicateEntries);

            // DEBUG
            if (configuration.DebugLevel > 0)
            {
                Console.WriteLine("DEBUG Read & validated Entries");
                
                foreach (ConstantEntry entry in entries)
                    Console.WriteLine("  "+entry);

                Console.WriteLine();
            }

            List<ConstantClass> rootClasses = BuildConstantClasses(entries);
            StringBuilder output = new StringBuilder();

            // Build Output File containing all the root classes
            ConstantClass.BuildOutputFile(configuration.RootNamespace, output, rootClasses);

            try
            {
                File.WriteAllText(configuration.OutputFilePath, output.ToString(), Encoding.UTF8);
            }
            catch (Exception e)
            {
                throw new Exception("Erro writing to output file; " + configuration.OutputFilePath, e);
            }

            return rootClasses;
        }

        /// <summary>
        /// Builds and returns a list of Root ConstantClasses.  These root 
        // classes will contain all the other constant classes.
        /// </summary>
        /// <param name="entries">Entries read from the input file.</param>
        /// <returns></returns>
        private static List<ConstantClass> BuildConstantClasses(List<ConstantEntry> entries)
        {
            List<string> classNames = GetClassNamesFromConstantEntries(entries);
            Dictionary<string, ConstantClass> classDirectory = new Dictionary<string, ConstantClass>();

            foreach (string className in classNames)
                classDirectory.Add(className, new ConstantClass(className));

            // Add entries to the ConstantClasses
            foreach (ConstantEntry entry in entries)
                classDirectory[entry.NameSpace].Constants.Add(entry);

            List<ConstantClass> rootClasses = new List<ConstantClass>();

            foreach (ConstantClass constantClass in classDirectory.Values)
            {
                if (string.IsNullOrEmpty(constantClass.NameSpace))
                    rootClasses.Add(constantClass);
                else
                {
                    classDirectory[constantClass.NameSpace].ChildClasses.Add(constantClass.Name, constantClass);
                    constantClass.ParentClass = classDirectory[constantClass.NameSpace];
                }
            }

            return rootClasses;
        }

        // TODO: Probably should be optimised
        private static List<string> GetClassNamesFromConstantEntries(List<ConstantEntry> entries)
        {
            List<string> classNames = new List<string>();

            foreach (ConstantEntry entry in entries)
            {
                // Ignore if we already have this name
                if (classNames.Contains(entry.NameSpace))
                    continue;

                classNames.Add(entry.NameSpace);

                // Are there sub-parts
                if (entry.NameSpace.Contains("."))
                {
                    string[] parts = entry.NameSpace.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                    string name = string.Empty;

                    // Check each sub-part, there are better ways to do this.
                    for (int i = 0; i < parts.Length; i++)
                    {
                        if (i > 0) // Add seperator when needed.
                            name += '.';

                        name += parts[i];

                        // Ignore if we alreayd have this name.
                        if (classNames.Contains(name))
                            continue;

                        classNames.Add(name);
                    }
                }
            }

            classNames.Sort();
            return classNames;
        }

        private static void AddRangeOfConstants(this List<ConstantEntry> list, IEnumerable<ICsvLine> csvLines, out List<ConstantEntry> duplicateEntries)
        {
            // Really lazy duplicate checking.
            HashSet<string> names = new HashSet<string>();
            duplicateEntries = new List<ConstantEntry>();

            foreach (ICsvLine line in csvLines)
            {
                // Parse the ICsvLine, into a ConstantEntry
                ConstantEntry entry = new ConstantEntry(line);

                // Does this name already exist
                if (names.Contains(entry.FullName))
                    duplicateEntries.Add(entry);
                else
                {
                    names.Add(entry.FullName);
                    list.Add(entry);
                }
            }
        }
    }
}