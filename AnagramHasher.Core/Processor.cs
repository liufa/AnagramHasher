using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AnagramHasher.Core
{
    public class Processor
    {
        private readonly string _anagram;
        private readonly string[] _hashesToLookFor;

        private string pathToDictionary = @"../../../../wordlist.txt";

        public string AnagramToCompare => _anagram.WithoutSpaces();

        public Processor(string anagram, string[] hashesToLookFor)
        {
            _anagram = anagram;
            _hashesToLookFor = hashesToLookFor;
        }

        public IEnumerable<string> ReadDictionary(string path)
        {
            return File.ReadLines(path).ToList();
        }

        public IEnumerable<string> FilterBySymbolsContained(IEnumerable<string> dictionary)
        {
            var result = dictionary.ToList().Where(o => o.Supersect(AnagramToCompare).Count() == o.Length)
                .ToList();
            return result;
        }

        public IEnumerable<string> FilterBySymbolsContained(string path)
        {
            return FilterBySymbolsContained(ReadDictionary(path));
        }

        public IEnumerable<string> GetFilteredWords(IEnumerable<string> dictionary)
        {
            var filteredByLetters = FilterBySymbolsContained(dictionary);
            var distinctFiltered = filteredByLetters.Distinct();
            return distinctFiltered;
        }

        public List<string> GetAnagrams(IEnumerable<string> dictionary, int depth)
        {
            var container = new List<string>();
            var listedDictionary = dictionary.ToList();
            Parallel.ForEach(listedDictionary, word =>
            {
                var innerContainer = new List<string>();
                    GetAnagram(word, listedDictionary, innerContainer, depth);
                    if (innerContainer.Any())
                    {
                        container.AddRange(innerContainer);
                    }
            });

            return container;
        }

        public void GetAnagram(string anagram, List<string> dictionary, List<string> container, int depth, int i = 0)
        {
            
            int dictionaryLength = dictionary.Count();
            while (i < dictionaryLength)
            {
                var word = dictionary.ElementAt(i);
                if (anagram.Split(" ").Length <= depth 
                    && !anagram.Split(" ").Contains(word) && word.Supersect(new string(AnagramToCompare.ExceptAll(anagram.WithoutSpaces()).ToArray()))
                        .Count() == word.Length)
                {
                    anagram += $" {word}";
                    if (!AnagramToCompare.ExceptAll(anagram).Any())
                    {
                        if (!container.Contains(anagram))
                        {
                            container.Add(anagram);
                        }
                    }
                    else
                    {
                        GetAnagram(anagram, dictionary, container, depth, ++i);
                    }

                    anagram = anagram.Remove(anagram.LastIndexOf(' ') + 1).TrimEnd();
                }

                i++;
            }
        }

        public  Dictionary<string, string> SearchIncreasingDepth(IEnumerable<string> filteredDictionary)
        {
            var matches = new Dictionary<string, string>();
            var i = 2;
            while (matches.Count < 3 && i < 3) //This should be _anagram.WithoutSpaces().Length but to actually do 18 "words" and permutations one needs to parallelise in a cloud.
            {
                var anagrams = GetAnagrams(filteredDictionary, i);

                foreach (var hash in GetPermutationsAndHashes(anagrams))
                {
                    if (!matches.ContainsKey(hash.Key))
                    {
                        matches.Add(hash.Key, hash.Value);
                        Console.WriteLine($"{hash.Key}, {hash.Value}");
                    }
                }

                i++;
            }

            return matches;
        }

        public Dictionary<string,string> GetPermutationsAndHashes(IEnumerable<string> anagrams)
        {
            var md5 = MD5.Create();
            var container = new Dictionary<string, string>();
            Parallel.ForEach(anagrams, (anagram) =>
            {
                var splitAnagram = anagram.Split(new[] {' '}, StringSplitOptions.None);
                Parallel.ForEach(splitAnagram.Permutations(), (permutation) =>
                {
                    var spacedPermutation = string.Join(" ", permutation);
                    var permutationHash = md5.CreateMD5HashString(spacedPermutation).ToLower();
                    if (_hashesToLookFor.Contains(permutationHash))
                    {
                        if(!container.ContainsKey(spacedPermutation))
                            container.Add(spacedPermutation, permutationHash);
                    }
                });
            });

            return container;
        }
    }
}
