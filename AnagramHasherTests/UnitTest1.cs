using System.Collections.Generic;
using System.IO;
using System.Linq;
using AnagramHasher.Core;
using NUnit.Framework;

namespace AnagramHasherTests
{
    public class Tests
    {
        private Processor Processor { get; set; }

        private const string pathToDictionary = @"../../../../wordlist.txt";

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
        public void Test1()
        {
            Assert.Pass();
        }

        [Test]
        public void ReadDictionary()
        {
           Assert.IsNotEmpty( Processor.ReadDictionary(pathToDictionary));
        }

        [Test]
        public void FilterBySymbolsContained()
        {
            var dictionary = Processor.ReadDictionary(pathToDictionary);
            Assert.False(Processor.FilterBySymbolsContained(dictionary).Any(o => o.Contains("z")));
        }

        //[Test]
        //public void GetAnagrams()
        //{
        //    var filteredDictionary = Processor.GetFilteredWords(Processor.ReadDictionary(pathToDictionary));
        //    Processor.GetAnagrams(filteredDictionary);
        //}

        [Test]
        public void MatchedHashes()
        {
            var filteredDictionary = Processor.GetFilteredWords(Processor.ReadDictionary(pathToDictionary));
            var matches = Processor.SearchIncreasingDepth(filteredDictionary);
            Assert.True(matches.Count > 0);
        }
    }
}