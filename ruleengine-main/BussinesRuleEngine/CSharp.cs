using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Security.Cryptography;
using System.Text;

namespace BussinesRuleEngine
{
    public static class CSharp
    {

        static string _assemblyPath = Path.GetDirectoryName(typeof(object).Assembly.Location);
        public static uint CacheLimit = 1 << 12;

        public static T Execute<T>(string code, object args, IEnumerable<Assembly> refs = null)
                  => Execute<T>(code, _getArgs(args), refs);

        public static void Execute(string code, object args, IEnumerable<Assembly> refs = null)
            => Execute(code, _getArgs(args), refs);

        public static T Execute<T>(string code, Dictionary<string, object> args, IEnumerable<Assembly> refs = null)
            => Execute<T>(code, _getArgs(args), refs);

        public static void Execute(string code, Dictionary<string, object> args, IEnumerable<Assembly> refs = null)
            => Execute(code, _getArgs(args), refs);

        public static T Execute<T>(string code, Dictionary<string, Tuple<Type, object>> args = null, IEnumerable<Assembly> refs = null)
        {
            if (code.IndexOf(_return) < 0)
                code = string.Concat(_return, code);

            return (T)_execute(code, args, refs);
        }

        public static void Execute(string code, Dictionary<string, Tuple<Type, object>> args = null, IEnumerable<Assembly> refs = null)
        {
            _execute(string.Concat(code, "; return null"), args, refs);
        }

        public static void ClearCache()
        {
            lock (_compiled) _compiled.Clear();
        }


        static object _execute(string code, Dictionary<string, Tuple<Type, object>> args, IEnumerable<Assembly> refs)
        {
            refs = (refs ?? new[] { Assembly.GetEntryAssembly(), Assembly.GetCallingAssembly() })
                .Concat(args?.Select(x => x.Value?.Item1?.Assembly) ?? new Assembly[0])
                .Where(x => x != null);

            var codeToCompile = _getCompileCode(code, args);


            MethodInfo compiled = null;

            if (CacheLimit == 0)
            {
                compiled = _compile(codeToCompile, refs);
            }
            else
            {
                var hash = _getHash(codeToCompile);

                if (_compiled.ContainsKey(hash))
                    compiled = _compiled[hash];
                else
                    lock (_compiled)
                        if (!_compiled.ContainsKey(hash))
                        {
                            compiled = _compile(codeToCompile, refs);

                            if (_compiled.Count > CacheLimit)
                                _compiled.Clear();

                            _compiled.Add(hash, compiled);
                        }
            }

            return compiled.Invoke(null, args?.Values.Select(x => x.Item2).ToArray());
        }

        static string _getCompileCode(string code, Dictionary<string, Tuple<Type, object>> args)
        {
            var codeBuilder = new StringBuilder(@"
            using System;
            using System.Text;
            using System.Linq;
            using System.Collections.Generic;

            namespace Eval
            {
                public class Code
                {
                    public static object Run(");
            codeBuilder.Append(string.Join(", ", args?.Select(x => $"{_getType(x.Value.Item1)} {x.Key}") ?? new string[0]));
            codeBuilder.Append(@") {
                        ");
            codeBuilder.Append(code);
            codeBuilder.AppendLine(@";
                    }
                }
            }");

            return codeBuilder.ToString();
        }

        static readonly IEnumerable<Assembly> _commonRefs = new[] {
            typeof(Object).Assembly,
            typeof(Console).Assembly,
            typeof(Enumerable).Assembly,
            typeof(IEnumerable).Assembly,
            typeof(IEnumerable<>).Assembly,
            Assembly.GetExecutingAssembly(),
        };

        static string _getType(Type type)
        {
            if (type.IsGenericType == true)
            {
                var gTypes = type.GenericTypeArguments.Select(x => _getType(x));
                return $"{type.FullName.Substring(0, type.FullName.IndexOf('`'))}<{string.Join(",", gTypes)}>";
            }

            return type.FullName;
        }

        static string _getHash(string data)
        {
            var sha = new SHA256Managed();
            byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(data));
            var base64 = Convert.ToBase64String(hash);
            return base64;
        }

        static Dictionary<string, Tuple<Type, object>> _getArgs(object args)
        {
            return args.GetType().GetProperties().Where(x => x.CanRead)
                .ToDictionary(x => x.Name, x =>
                {
                    var val = x.GetValue(args);
                    return new Tuple<Type, object>(val?.GetType() ?? x.PropertyType, val);
                });
        }

        static Dictionary<string, Tuple<Type, object>> _getArgs(Dictionary<string, object> args)
        {
            return args?.ToDictionary(x => x.Key, x => new Tuple<Type, object>(x.Value?.GetType() ?? typeof(object), x.Value));
        }

        static readonly Dictionary<string, MethodInfo> _compiled = new Dictionary<string, MethodInfo>();

        const string _return = "return ";

        static MethodInfo _compile(string code, IEnumerable<Assembly> refs)
        {
            var assemblyPath = Path.GetDirectoryName(typeof(object).Assembly.Location);

            var references = _references.Concat(refs.Select(x => x.Location)).Distinct();

            var compilation = CSharpCompilation.Create(
                assemblyName: Path.GetRandomFileName(),
                syntaxTrees: new[] { CSharpSyntaxTree.ParseText(code) },
                references: references.Select(x => MetadataReference.CreateFromFile(x)),
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using (var ms = new MemoryStream())
            {
                var result = compilation.Emit(ms);

                if (!result.Success)
                {
                    throw new Exception(string.Join(" \n", result.Diagnostics
                        .Where(diagnostic => diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error)
                        .Select(x => x.GetMessage())));
                }
                else
                {
                    ms.Seek(0, SeekOrigin.Begin);
                    var assembly = AssemblyLoadContext.Default.LoadFromStream(ms);
                    return assembly.GetType("Eval.Code").GetMember("Run").First() as MethodInfo;
                }
            }
        }


        static IEnumerable<string> _references = _commonRefs.Select(x => x.Location)
            .Concat(new[] {
                Path.Combine(_assemblyPath, "netstandard.dll"),
                Path.Combine(_assemblyPath, "System.Runtime.dll"),
            }.Where(x => File.Exists(x))).Distinct();
    }

}

