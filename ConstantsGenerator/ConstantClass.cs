using System;
using System.Collections.Generic;
using System.Text;

namespace ConstantGenerator
{
    public class ConstantClass : Key
    {
        public ConstantClass(string fullName) : this(fullName, null, null) { }

        public ConstantClass(string fullName, List<ConstantEntry> constants, Dictionary<string, ConstantClass> childClasses, ConstantClass parentClass = null): base(fullName)
        {
            if(constants == null)
                constants = new List<ConstantEntry>();

            if (childClasses == null)
                childClasses = new Dictionary<string, ConstantClass>();

            Constants = constants;
            ChildClasses = childClasses;

            // If null, we assume no parent
            ParentClass = parentClass;
        }

        public List<ConstantEntry> Constants {get; private set;}
        public Dictionary<string, ConstantClass> ChildClasses { get; private set; }
        public ConstantClass ParentClass { get; set; }

        public static void BuildOutputFile(string rootNamespace, StringBuilder sb, List<ConstantClass> constantClasses)
        {
            if (sb == null)
                throw new ArgumentNullException("sb");

            Indent indent = new Indent();

            // FileHeader
            sb.AppendLine("namespace " + rootNamespace);
            sb.AppendLine("{");

            indent.Increment();
            sb.AppendLine(indent + "// ReSharper disable MemberHidesStaticFromOuterClass");

            for (int i = 0; i < constantClasses.Count; i++)
            {
                constantClasses[i].BuildOutputClass(sb, indent);

                if (i < constantClasses.Count - 1)
                    sb.AppendLine();
            }

            // File Footer
            sb.AppendLine("}");
        }

        internal void BuildOutputClass(StringBuilder sb, Indent indent)
        {
            if (sb == null)
                throw new ArgumentNullException("sb");

            sb.AppendLine(indent + "public static class " + Name);
            sb.AppendLine(indent + "{");
            indent.Increment();

            BuildOutputConstants(sb, indent);

            // Check if a new line is needed.
            if (Constants.Count > 0 && ChildClasses.Count > 0)
                sb.AppendLine();
            
            BuildOutputChildClasses(sb, indent);

            indent.Decrement();
            sb.AppendLine(indent + "}");
        }

        private void BuildOutputConstants(StringBuilder sb, Indent indent)
        {
            if (sb == null)
                throw new ArgumentNullException("sb");

            foreach (ConstantEntry constant in Constants)
                sb.AppendLine(indent + "public const string " + constant.Name + " = \"" + constant.FullName + "\";");
        }

        private void BuildOutputChildClasses(StringBuilder sb, Indent indent)
        {
            if (sb == null)
                throw new ArgumentNullException("sb");

            List<ConstantClass> children = new List<ConstantClass>(ChildClasses.Values);

            // Get output for each child
            for (int i = 0; i < children.Count; i++)
            {
                children[i].BuildOutputClass(sb, indent);

                if (i < children.Count - 1)
                    sb.AppendLine();
            }
        }
        
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("  " + FullName);

            if (Constants.Count > 0)
            {
                sb.AppendLine("    Constants");

                foreach (ConstantEntry constant in Constants)
                    sb.AppendLine("      " + constant);
            }

            if (ChildClasses.Count > 0)
            {
                sb.AppendLine("    Child Classes");

                foreach (KeyValuePair<string, ConstantClass> childClass in ChildClasses)
                    sb.AppendLine("      " + childClass.Value.FullName);

                // Print the child classes in full
                foreach (KeyValuePair<string, ConstantClass> childClass in ChildClasses)
                    sb.Append("\n" + childClass.Value);
            }

            return sb.ToString();
        }

        internal class Indent
        {
            public Indent(string indentString = "    ", int indentIndex = 0)
            {
                _indentString = indentString;
                _indentIndex = indentIndex;

                SetIndent(indentIndex);
            }

            private readonly string _indentString;
            private readonly object _locker = new object();
            
            private int _indentIndex;
            private string _currentIndent = string.Empty;

            public void Increment()
            {
                lock (_locker)
                {
                    _indentIndex ++;
                    SetIndent(_indentIndex);
                }
            }

            public void SetIndent(int indentIndex)
            {
                lock (_locker)
                {
                    // reset _currentIndent
                    _currentIndent = string.Empty;

                    for (int i = 0; i < indentIndex; i++)
                        _currentIndent += _indentString;
                }
            }

            public void Decrement()
            {
                lock (_locker)
                {
                    _indentIndex--;
                    SetIndent(_indentIndex);
                }
            }

            public override string ToString()
            {
                lock (_locker)
                    return _currentIndent;
            }
        }
    }
}