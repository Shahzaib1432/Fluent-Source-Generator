﻿using FluentSourceGenerator.CSharp.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentSourceGenerator.CSharp
{
    internal sealed class SourceBuilder
    {
        private readonly List<IToken> m_Tokens = new List<IToken>();

        public SourceBuilderOptions ChildOptions
        {
            get
            {
                if (m_ChildOptions == null)
                    ChildOptions = SourceBuilderOptions.Default;
                return m_ChildOptions;
            }
            set => m_ChildOptions = value.Clone();
        }
        private SourceBuilderOptions m_ChildOptions;

        public void Token(IToken token)
        {
            m_Tokens.Add(token);
        }

        public void Line(string line = "")
        {
            m_Tokens.Add(new LineToken(ChildOptions, line));
        }

        public void Lines(params string[] lines)
        {
            m_Tokens.Add(new LinesToken(ChildOptions, lines));
        }

        public void Using(string statement)
        {
            m_Tokens.Add(new LineToken(ChildOptions, $"using {statement};"));
        }

        public void Namespace(string @namespace, Action<SourceBuilder> source_builder_act)
        {
            var source_builder = new SourceBuilder();
            source_builder.ChildOptions = ChildOptions;
            source_builder_act.Invoke(source_builder);
            m_Tokens.Add(new BlockToken(ChildOptions, $"namespace {@namespace}", source_builder));
        }

        public void Class(string modifiers, string class_name, Action<SourceBuilder> source_builder_act)
        {
            var source_builder = new SourceBuilder();
            source_builder.ChildOptions = ChildOptions;
            source_builder_act.Invoke(source_builder);
            m_Tokens.Add(new BlockToken(ChildOptions, $"{modifiers} class {class_name}", source_builder));
        }

        public void Method(string modifiers, string return_type, string method_name, string parameters, Action<SourceBuilder> source_builder_act)
        {
            var source_builder = new SourceBuilder();
            source_builder.ChildOptions = ChildOptions;
            source_builder_act.Invoke(source_builder);
            m_Tokens.Add(new BlockToken(ChildOptions, $"{modifiers} {return_type} {method_name}({parameters})", source_builder));
        }

        public override string ToString()
        {
            var string_builder = new StringBuilder();
            for (int i = 0; i < m_Tokens.Count; i++)
            {
                var token = m_Tokens[i];

                if (token is BlockToken || token is LinesToken)
                {
                    if (i != 0)
                        string_builder.AppendLine();
                    string_builder.Append(token.ToString());
                    if (i != m_Tokens.Count - 1)
                        string_builder.AppendLine();
                }
                else
                {
                    if (i != 0)
                        string_builder.AppendLine();
                    string_builder.Append(token.ToString());
                    if (i != m_Tokens.Count - 1 && !(m_Tokens.ElementAtOrDefault(i + 1) is LineToken))
                        string_builder.AppendLine();
                }
            }

            return string_builder.ToString();
        }
    }
}
