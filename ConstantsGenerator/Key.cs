using System;

namespace ConstantGenerator
{
    public class Key
    {
        protected Key() { }

        public Key(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName.Trim(new[] { '.' })))
                throw new ArgumentException("A Key's FullName must not be null, empty, whitespace or just '.'s.");

            ParseFullName(fullName);
        }

        public string FullName { get; protected set; }
        public string Name { get; protected set; }
        public string NameSpace { get; protected set; }
        
        protected void ParseFullName(string fullName)
        {
            FullName = fullName;
            string[] parts = FullName.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 1)
            {
                Name = FullName;
                NameSpace = string.Empty;
            }
            else
            {
                Name = parts[parts.Length - 1];

                for (int i = 0; i < parts.Length - 1; i++)
                    NameSpace += parts[i] + '.';

                NameSpace = NameSpace.Trim(new[] { '.' });
            }
        }

        public override string ToString()
        {
            return NameSpace + " :: " + Name;
        }
    }
}