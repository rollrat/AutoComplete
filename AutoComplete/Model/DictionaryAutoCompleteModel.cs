/***

   Copyright (C) 2019. rollrat. All Rights Reserved.

   Author: Jeong HyunJun

***/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoComplete.Model
{
    public class DictionaryAutoCompleteAlgorithm : IAutoCompleteAlgorithm
    {
        List<string> functions = new List<string>();

        public DictionaryAutoCompleteAlgorithm()
        {
            foreach (var func in File.ReadAllLines("ntoskrnl.txt"))
            {
                functions.Add(func);
            }
        }
        
        public List<AutoCompleteTagData> GetResults(ref string word, ref int position, bool using_fuzzy)
        {
            var result = new List<AutoCompleteTagData>();

            if (!using_fuzzy)
            {
                var target = word.ToLower();
                // 먼저 앞부분이 일치하는 목록 탐색
                functions.Where(x => x.ToLower().StartsWith(target)).ToList().ForEach(x => result.Add(new AutoCompleteTagData { Tag = x }));

                // 부분적으로 일치하는 경우 탐색
                var ff = new Dictionary<int, List<string>>();
                functions.Where(x => !x.ToLower().StartsWith(target) && x.ToLower().Contains(target)).ToList().ForEach(x =>
                {
                    var pos = x.ToLower().IndexOf(target);
                    if (!ff.ContainsKey(pos))
                        ff.Add(pos, new List<string>());
                    ff[pos].Add(x);
                });
                var ll = ff.ToList();
                ll.Sort((x, y) => x.Key.CompareTo(y.Key));
                result.AddRange(ll.Select(x => x.Value.Select(y => new AutoCompleteTagData { Tag = y })).SelectMany(x => x));
            }
            else
            {
                var target = word.ToLower();
                foreach (var func in functions)
                {
                    result.Add(new AutoCompleteTagData { Tag = func, Count = -Strings.ComputeLevenshteinDistance(target, func.ToLower()) });
                }
                result.Sort((a, b) => b.Count.CompareTo(a.Count));
            }

            return result;
        }
    }
}
