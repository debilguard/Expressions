using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BussinesRuleEngine
{
    public static class Expression
    {
        internal static ScriptOptions Options
        {
            get
            {
                return ScriptOptions.Default
                             .AddImports("System",
                                         "System.IO",
                                         "System.Collections.Generic",
                                         "System.Linq",
                                         "System.Threading.Tasks")
                             .AddReferences("System", "System.Core", "Microsoft.CSharp");

            }
        }

        public static TResult Eval<TResult>(string code, params object[] parameters)
        { 
            var param = ListParameters(parameters);
            var variables = BuildVariables(param);
            return Execute<TResult>(variables, code).Result;          
        }
         
        private static async Task<TResult> Execute<TResult>(string variables, string code)
        {
            string codeParsedQuotes = code.Replace("'", "\"");
            ScriptState state = await CSharpScript.RunAsync(variables,Options);
            state = await state.ContinueWithAsync<TResult>(codeParsedQuotes);
            return (TResult)state.ReturnValue;
        }

        private static string BuildVariables(List<object> parameters)
        {
            StringBuilder parms = new StringBuilder();
            parameters.ForEach(item =>
            {
                var props = item.GetType().GetProperties();
                for (int i = 0; i < props.Length; i++)
                {
                    string parseString = string.Empty;
                    if (props[i].PropertyType.Name == "String")
                    {
                       parseString =  $"var {props[i].Name} = \"{item.GetType().GetProperty(props[i].Name).GetValue(item, null)}\"; ";
                    }
                    else if(props[i].PropertyType.Name == "Boolean")
                    {
                       parseString = $"var {props[i].Name} = {item.GetType().GetProperty(props[i].Name).GetValue(item, null).ToString().ToLower()}; ";
                    }
                    else
                    {
                        parseString = $"var {props[i].Name} = {item.GetType().GetProperty(props[i].Name).GetValue(item, null)}; ";
                    }
                    parms.Append(parseString);
                }
            });
            return parms.ToString();
        }

        private static List<object> ListParameters(params object[] parameters)
        {
            List<object> dict = new List<object>();

            for (int i = 0; i < parameters.Length; i++)
            {
                dict.Add(parameters[i]);
            }
            return dict;
        } 
    }
}
