//using Microsoft.CodeAnalysis.CSharp.Scripting;
//using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.Generic;
using System.Linq.Expressions; 

namespace BussinesRuleEngine
{
    public class Program
    { 
        public static void Main(string[] args)
        {
            string rule = "var Make=\"ford\"; new List<string>() { \"Maserati\", \"Suzuki\", \"Tesla\"}.Contains(Make)";
            string rule1 = "new List<string>() { 'Maserati', 'Mercury', 'Oldsmobile', 'Polestar', 'Pontiac', 'Porsche', 'Saab', 'Saturn', 'Smart', 'Smart', 'Suzuki', 'Tesla' }.Contains(Make)";
            string rule2 = "Price > 80000 || isEligible.Equals(true)";
            string rule3 = "Year < DateTime.Now.Year - 10 && Price > 10000";

            //CSharp(rule1);

            Func<int, int, int> square = (num, num2) => num * num;

            Func<Vehicle, bool> vhe = (x) => x.Mileage > 1;

            Func<dynamic, bool> dynVehicle = (x) => x.Mileage > 1;

            var vehicle = new Vehicle()
            {
                Subsegment = "High Performance",
                Year = 1999,
                Make = "MaseratiX",
                Mileage = 5000,
                Price = 100.99
            };

            var result = new Result()
            {
                isEligible = true,
                Message = "Vehicle is not eligible"
            };

            //var resultR1 = Expression.Eval<bool>(rule1, vehicle);
            //var resultR2 = Expression.Eval<bool>(rule2, vehicle, result);
            //var resultR3 = Expression.Eval<bool>(rule3,vehicle);

            for (int i = 0; i < 100; i++)
            {
                Expression2.Eval<bool>(rule1, vehicle);
                 Expression2.Eval<bool>(rule2, vehicle, result);
                  Expression2.Eval<bool>(rule3, vehicle);
            }


            var r1 = dynVehicle(vehicle);

            //var r = Expression.Execute<bool>("", vehicle,result);

            try
            {
                //Execute
                var r = vhe(vehicle);
                var resu = Expression1.Execute<bool>(rule, vehicle, result);


                //var options = ScriptOptions.Default
                //            .AddImports("System",
                //                        "System.IO",
                //                        "System.Collections.Generic",
                //                        "System.Linq",
                //                        "System.Threading.Tasks")
                //            .AddReferences("System", "System.Core", "Microsoft.CSharp");

                //var script = CSharpScript.Create("new List<string>() {\"Maserati\",\"Tesla\"}.Contains(\"Ford\")", options);
                //script.Compile();
                //var exec =  script.RunAsync();
                //var res = exec.Result.ReturnValue;

                //var r =  Expression.EvalAsync<bool>();
                //Console.WriteLine(r);
            }
            catch (Exception ex)
            {

                throw;
            }

            //var x = CSharpScript.EvaluateAsync("System.DateTime.Now.Year - 10 > 2010");
            //var yearRule = Eval.Execute<bool>("DateTime.Now.Year - Year <= 10", vehicle); 
            //var subRule = Eval.Execute<bool>("new List<string>() { \'Maserati\', \'Mercury\', \'Oldsmobile\', \'Polestar\', \'Pontiac\'}.Contains(Make)", vehicle); 
        }

       //public static async void CSharp(string code)
       // {
       //     ScriptState state = await CSharpScript.RunAsync(code, Options);
       //     //state = await state.ContinueWithAsync<int>("");
       //     var variables = state.Variables;
       // }  
    }
     
}
