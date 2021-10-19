//using Microsoft.CodeAnalysis.CSharp.Scripting;
//using Microsoft.CodeAnalysis.Scripting;
 
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace BussinesRuleEngine
{
    public class Program2
    { 
        static void Main(string[] args)
        {
            string rule = "new[] { 'Maserati', 'Mercury', 'Oldsmobile', 'Polestar', 'Pontiac', 'Porsche', 'Saab', 'Saturn', 'Smart', 'Smart', 'Suzuki', 'Tesla' }.Contains(Make)";
            string rule2 = "Price > 80000 || isEligible.Equals(true)";
            string rule3 = "Year < DateTime.Now.Year - 10 && Price > 10000";

            

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
             
            for (int i = 0; i < 1000000; i++)
            {
                var eval1 = Eval.Execute<bool>(rule, vehicle, result);
                var eval2 = Eval.Execute<bool>(rule2, vehicle, result);
                var eval3 = Eval.Execute<bool>(rule3, vehicle, result);
            }

            var m = $"Console.WriteLine";
        } 
    } 
}
