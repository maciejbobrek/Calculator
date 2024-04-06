using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Calculator
{
    public class Program
    {
        public class Operation
        {
            // Użyj '?' po typie, aby oznaczyć, że dopuszczasz wartość null
            public string? Operator { get; set; }
            public double? Value1 { get; set; }
            public double? Value2 { get; set; }
        }

        public class ResultOperation
        {
            public string OperationId { get; set; } = string.Empty; // Inicjalizacja właściwości, aby uniknąć null
            public double Result { get; set; }
        }

        static void Main(string[] args)
        {
            string inputJson = File.ReadAllText("./input.json");

            Dictionary<string, Operation>? operations = JsonConvert.DeserializeObject<Dictionary<string, Operation>>(inputJson); 
            if (operations == null)
            {
                Console.WriteLine("Nie udało się odczytać operacji z pliku JSON.");
                return;
            }

            List<ResultOperation> results = new List<ResultOperation>();
            foreach (var operation in operations)
            {
                if (operation.Value == null)
                {
                    Console.WriteLine($"Operacja o kluczu '{operation.Key}' jest pusta.");
                    continue;
                }

                if (string.IsNullOrEmpty(operation.Value.Operator))
                {
                    Console.WriteLine($"Nieprawidłowy operator dla operacji o kluczu '{operation.Key}'.");
                    continue;
                }

                string operatorName = operation.Value.Operator.ToLower();
                var value1 = operation.Value.Value1;
                var value2 = operation.Value.Value2;
                double result;
                switch (operatorName)
                {
                    case "add":
                        EnsureValuesProvided(operation.Value);
                        result = value1!.Value + value2!.Value;
                        break;
                    case "sub":
                        EnsureValuesProvided(operation.Value);
                        result = value1!.Value - value2!.Value;
                        break;
                    case "mul":
                        EnsureValuesProvided(operation.Value);
                        result = value1!.Value * value2!.Value;
                        break;
                    case "sqrt":
                        EnsureValuesProvided(operation.Value, value2IsRequired: false);
                        result = Math.Sqrt(value1!.Value);
                        break;
                    default:
                        throw new ArgumentException($"Nieznany operator: {operatorName}");
                }
                results.Add(new ResultOperation
                {
                    OperationId = operation.Key,
                    Result = result
                });
            }

            results.Sort((r1, r2) => r1.Result.CompareTo(r2.Result));

            using (StreamWriter sw = new StreamWriter("./output.txt"))
            {
                foreach (var result in results)
                {
                    sw.WriteLine($"{result.OperationId}: {result.Result}");
                }
            }
        }

        private static void EnsureValuesProvided(Operation operation, bool value2IsRequired = true)
        {
            if (operation.Value1 == null || (value2IsRequired && operation.Value2 == null))
            {
                throw new ArgumentException($"Brakujące wartości dla operacji {operation.Operator}.");
            }

            if (operation.Operator!.ToLower() == "sqrt" && operation.Value2 != null)
            {
                Console.WriteLine($"Ostrzeżenie: Wartość Value2 jest dostarczona dla operacji sqrt, ale zostanie zignorowana.");
            }
        }
    }
}