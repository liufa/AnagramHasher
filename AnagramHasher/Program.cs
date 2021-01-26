using System;
using AnagramHasher.Core;

namespace AnagramHasher
{
    class Program
    {
        static void Main(string[] args)
        {
            var inputAnagram = string.Empty;
            string[] hashes = null;
            string pathToDictionary = null;

            if (args.Length == 3)
            {
                inputAnagram = args[0];
                hashes = args[1].Split(",");
                pathToDictionary = args[2];
            }
            else
            {
                inputAnagram = "poultry outwits ants";
                hashes = new[]
                {
                    "e4820b45d2277f3844eac66c903e84be",
                    "23170acc097c24edb98fc5488ab033fe",
                    "665e5bcb0c20062fe8abaaf4628bb154"
                };
                pathToDictionary = "wordlist.txt";
            }

            var processor = new Processor(inputAnagram, hashes);
            var filteredDictionary = processor.FilterBySymbolsContained($"../../../../{pathToDictionary}");
            var matches = processor.SearchIncreasingDepth(filteredDictionary);
            foreach (var match in matches)
            {
                Console.WriteLine("{match.Key}, {match.Value}");
            }
            
            Console.ReadKey();
        }
    }
}
