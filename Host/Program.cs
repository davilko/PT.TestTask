using System;
using System.Threading.Tasks;
using Analyzer;

namespace PT.Analyzer
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var filePath = args[0];
            var result = await CsharpAnalyzer.AnalyzeAsync(filePath);

            foreach (var value in result)
            {
                Console.WriteLine(value);
            }
        }
    }
}