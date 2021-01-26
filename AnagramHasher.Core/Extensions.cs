using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace AnagramHasher.Core
{
    public static class Extensions
    {
        public static string WithoutSpaces(this string str)
        {
            return str.Replace(" ", string.Empty);
        }

        public static List<char> Supersect(this string str1, string str2)
        {
            List<char> a = str1.ToCharArray().ToList();
            List<char> b = str2.ToCharArray().ToList();
            var result = a.Where(b.Remove).ToList();
            return result;
        }

        public static IEnumerable<T[]> Permutations<T>(this T[] values, int fromInd = 0)
        {
            if (fromInd + 1 == values.Length)
                yield return values;
            else
            {
                foreach (var v in Permutations(values, fromInd + 1))
                    yield return v;

                for (var i = fromInd + 1; i < values.Length; i++)
                {
                    SwapValues(values, fromInd, i);
                    foreach (var v in Permutations(values, fromInd + 1))
                        yield return v;
                    SwapValues(values, fromInd, i);
                }
            }
        }

        private static void SwapValues<T>(T[] values, int pos1, int pos2)
        {
            if (pos1 != pos2)
            {
                T tmp = values[pos1];
                values[pos1] = values[pos2];
                values[pos2] = tmp;
            }
        }
        public static string CreateMD5HashString(this MD5 md5, string input)
        {

                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }

                return sb.ToString();
        }
        public static IEnumerable<TSource> ExceptAll<TSource>(
            this IEnumerable<TSource> first,
            IEnumerable<TSource> second)
        {
            return ExceptAll(first, second, null);
        }
        public static IEnumerable<TSource> ExceptAll<TSource>(
            this IEnumerable<TSource> first,
            IEnumerable<TSource> second,
            IEqualityComparer<TSource> comparer)
        {
            if (first == null) { throw new ArgumentNullException("first"); }
            if (second == null) { throw new ArgumentNullException("second"); }


            var secondCounts = new Dictionary<TSource, int>(comparer ?? EqualityComparer<TSource>.Default);
            int count;
            int nullCount = 0;

            // Count the values from second
            foreach (var item in second)
            {
                if (item == null)
                {
                    nullCount++;
                }
                else
                {
                    if (secondCounts.TryGetValue(item, out count))
                    {
                        secondCounts[item] = count + 1;
                    }
                    else
                    {
                        secondCounts.Add(item, 1);
                    }
                }
            }

            // Yield the values from first
            foreach (var item in first)
            {
                if (item == null)
                {
                    nullCount--;
                    if (nullCount < 0)
                    {
                        yield return item;
                    }
                }
                else
                {
                    if (secondCounts.TryGetValue(item, out count))
                    {
                        if (count == 0)
                        {
                            secondCounts.Remove(item);
                            yield return item;
                        }
                        else
                        {
                            secondCounts[item] = count - 1;
                        }
                    }
                    else
                    {
                        yield return item;
                    }
                }
            }
        }
    }
}
