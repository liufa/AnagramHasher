using System.Collections.Generic;
using System.IO;
using System.Linq;
using AnagramHasher.Core;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace AnagramHasherTests
{
    public class Tests
    {
        private Processor Processor { get; set; }

        private const string pathToDictionary = @"../../../../wordlist.txt";

        private List<string> _dictionary = null;

        private List<string> _anagrams = null;

        private Dictionary<string,string> _permutations = null;

        [SetUp]
        public void Setup()
        {
            Processor = new Processor("poultry outwits ants",
                new[]
                {
                    "e4820b45d2277f3844eac66c903e84be", 
                    "23170acc097c24edb98fc5488ab033fe",
                    "665e5bcb0c20062fe8abaaf4628bb154"
                });
        }
        
        [Test]
        public void ReadDictionary()
        {
           Assert.IsNotEmpty( Processor.ReadDictionary(pathToDictionary));
        }

        [Test,Order(1)]
        public void FilterBySymbolsContained()
        {
            var dictionary = Processor.ReadDictionary(pathToDictionary);
            var filtered = Processor.FilterBySymbolsContained(dictionary);
            Assert.False(filtered.Any(o => o.Contains("z")));
            _dictionary = filtered.ToList();
        }

        [Test, Order(2)]
        public void GetAnagrams()
        {
           //Normally every test should be run in isolation and order shouldn't matter, however since anagram search is quite expensive pragmatism takes over and I am reusing operations.
           //var filteredDictionary = Processor.GetFilteredWords(Processor.ReadDictionary(pathToDictionary));
           var anagrams = Processor.GetAnagrams(_dictionary,2);
           var orderedInput = new string(Processor.AnagramToCompare.ToCharArray().OrderBy(o => o).ToArray());
            Assert.True(anagrams.TrueForAll(o=>new string(o.ToCharArray().OrderBy(oo => oo).ToArray()).TrimStart() == orderedInput));
           _anagrams = anagrams;
        }

        [Test]
        public void MatchedHashes()
        {
            var filteredDictionary = Processor.GetFilteredWords(Processor.ReadDictionary(pathToDictionary));
            var matches = Processor.SearchIncreasingDepth(filteredDictionary);
            Assert.True(matches.Count > 0);
        }
    }
}