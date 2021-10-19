using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace BussinesRuleEngine
{
    public static class Expression1
    {
        internal static  ScriptOptions Options
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

        private static List<object> ParseInputParameters(params object[] parameters)
        {
            List<object> dict = new List<object>();

            for (int i = 0; i < parameters.Length; i++)
            {
                dict.Add(parameters[i]);
            }
            return dict;
        }

        public static void Parse(params object[] parameters)
        {  
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            Dictionary<string, Type> dictionary1 = new Dictionary<string, Type>();
            for (int i = 0; i < (int)parameters.Length; i++)
            {
                var str = String.Concat("{", i, "}");
                dictionary.Add(str, parameters[i]);
                dictionary1.Add(str, parameters[i].GetType());
            }
            Console.WriteLine("Runtime");
            //return .< Func < IDictionary, TResult >> (this, code, dictionary1, typeof(TResult), EvalCompilerParameterKind.Dictionary, false, false, false).Invoke(dictionary);
        }
        //public static async  Task<TResult> EvalAsync<TResult>()
        //{

        //    //new List<string>() { 'Maserati', 'Mercury', 'Oldsmobile', 'Polestar', 'Pontiac', 'Porsche', 'Saab', 'Saturn', 'Smart', 'Smart', 'Suzuki', 'Tesla' }.Contains(Make)
        //    //ScriptState state = await CSharpScript.RunAsync<TResult>("int x = 5");  //a.RunAsync("int x = 5;");

        //    //new List<string>() { "Maserati","Tesla"}.Contains("Tesla");
        //    var options = ScriptOptions.Default
        //                    .AddImports("System",
        //                                "System.IO",
        //                                "System.Collections.Generic",
        //                                "System.Linq", 
        //                                "System.Threading.Tasks")
        //                    .AddReferences("System", "System.Core", "Microsoft.CSharp"); 

        //    new List<string>() { "a", "b" }.Contains("a");

        //    ScriptState state = await CSharpScript.RunAsync<TResult>("new List<string>() {\"Maserati\",\"Tesla\"}.Contains(\"Tesla\");",options);
        //    ScriptState state = await CSharpScript.RunAsync<TResult>("new List<string>() {\"a\",\"b\"}.Contains(\"a\");",options);
        //    state = await state.ContinueWithAsync<TResult>("x + 1"); 
        //    return (TResult)state.ReturnValue;
        //}
        //  Func<int, int, int> square = (num, num2) => num * num;


        public static TResult Execute<TResult>(string code, params object[] parameters)
        {
             
            var dict =  ParseInputParameters(parameters);
            buildVariables(dict);
            //BuildExpression(dict, code);
            //var script = CSharpScript.Create("(Brand) => new List<string>() {\"Maserati\",\"Tesla\"}.Contains(Brand)", Options);
            //script.Compile();
            //var exec = script.RunAsync();
            //return (TResult)exec.Result.ReturnValue; 

            //var script = CSharpScript.Create<Func<TResult>>("1==1", Options);
            //script.Compile();
            //BuildExpression(code, typeof(TResult),parameters);
             
            return default(TResult);
        }

        private static string buildVariables(List<object> parameters)
        {
            StringBuilder parms = new StringBuilder();
            parameters.ForEach(item => 
            {
                var props =  item.GetType().GetProperties();
                for (int i = 0; i < props.Length; i++)
                {
                    if(props[i].PropertyType.Name == "String")
                    {
                        parms.Append($"var {props[i].Name} = \"{item.GetType().GetProperty(props[i].Name).GetValue(item, null)}\"; ");
                    }
                    else
                    {
                        parms.Append($"var {props[i].Name} = {item.GetType().GetProperty(props[i].Name).GetValue(item, null)}; ");
                    } 
                }
            });
            return parms.ToString();
        }

        private static string BuildExpression(IList<object> parameters, string code)
        {
            var types =  string.Join(",", parameters.Select(item => "dynamic"));

            var func = $"Func<{types},dynamic> expression = ()";
            return null;
        }



        //  Func<int, int, int> square = (num, num2) => num * num;

        private static string BuildExpressionZ(string code, Type typeResult, params object[] parameters) 
        { 
            //string types = string.Empty;
            //string namedParams = string.Empty;
            //int index = 1;

            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            for (int i = 0; i < (int)parameters.Length; i++)
            {
                //if (index == parameters.Length)
                //{
                //    types += $"{parameters[i].GetType().Name}";
                //    types += $"{parameters[i].GetType().Name}";
                //}
                //else
                //{ 
                //    types += $"{parameters[i].GetType().Name},"; 
                //}
                //index++; 
            }
            //return $"Func<{types},{typeOutput.Name}>eval=() => {code}";
            return null;
        } 
    }
}
