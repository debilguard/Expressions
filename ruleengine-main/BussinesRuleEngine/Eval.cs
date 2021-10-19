using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace BussinesRuleEngine
{
    public static class Eval
    {
        public static T Execute<T>(string code, params object[] parameters)
        {
            string codeParsedQuotes = code.Replace("'", "\"");
            var context = new Dictionary<string, object>();
            ExpandoObject newClass = new ExpandoObject();

            for (int i = 0; i < parameters.Length; i++)
            {
                var props = parameters[i].GetType().GetProperties();
                for (int y = 0; y < props.Length; y++)
                {
                    context[props[y].Name] = parameters[i].GetType().GetProperty(props[y].Name).GetValue(parameters[i], null);
                }
            }

            var param = Expression2.ListParameters(parameters);
            var variables = Expression2.BuildVariables(param);

            T result = CSharp.Execute<T>(codeParsedQuotes, context);
            return result;
        }
    }
}
