using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccordNET9
{
    public class AprioriReducer
    {
        public Dictionary<string, int> Reduce(List<Dictionary<string, int>> intermediateResults, int minSupport)
        {
            var finalCounts = new Dictionary<string, int>();
            foreach (var result in intermediateResults)
            {
                foreach (var kvp in result)
                {
                    if (finalCounts.ContainsKey(kvp.Key))
                    {
                        finalCounts[kvp.Key] += kvp.Value;
                    }
                    else
                    {
                        finalCounts[kvp.Key] = kvp.Value;
                    }
                }
            }
            // Filter out items with support < minSupport
            return finalCounts.Where(kvp => kvp.Value >= minSupport).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }
        public Dictionary<int, int> Reduce1(List<Dictionary<int, int>> intermediateResults, int minSupport)
        {
            var finalCounts = new Dictionary<int, int>();
            foreach (var result in intermediateResults)
            {
                foreach (var kvp in result)
                {
                    if (finalCounts.ContainsKey(kvp.Key))
                    {
                        finalCounts[kvp.Key] += kvp.Value;
                    }
                    else
                    {
                        finalCounts[kvp.Key] = kvp.Value;
                    }
                }
            }
            // Filter out items with support < minSupport
            return finalCounts.Where(kvp => kvp.Value >= minSupport).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }
    }
}

