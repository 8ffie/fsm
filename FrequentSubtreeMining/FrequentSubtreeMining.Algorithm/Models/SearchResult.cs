using System;
using System.Collections.Generic;

namespace FrequentSubtreeMining.Algorithm.Models
{
    public class SearchResult
    {
        /// <summary>
        /// Время поиска
        /// </summary>
        internal long TotalTimeElapsed { get; set; }

        /// <summary>
        /// Параметры поиска
        /// </summary>
        internal SearchParameters SearchParams { get; set; }

        /// <summary>
        /// Частые подддеревья
        /// </summary>
        internal List<Tree> FrequentSubtrees { get; set; }

        /// <summary>
        /// Число найденных частых поддеревьев
        /// </summary>
        internal int FrequentSubtreesCount
        {
            get
            {
                return FrequentSubtrees == null ? 0 : FrequentSubtrees.Count;
            }
        }

        /// <summary>
        /// Строковое представление результатов поиска
        /// </summary>
        /// <returns></returns>
        public string[] ToStrings()
        {
            List<string> results = new List<string>();
            results.AddRange(SearchParams.ToStrings());
            results.Add(Environment.NewLine);
            results.Add(string.Format("Прошло времени: {0} мс", TotalTimeElapsed));
            results.Add("Число найденных частых подграфов: " + FrequentSubtreesCount);
            foreach (Tree tree in FrequentSubtrees)
            {
                results.Add(tree.ToString());
                results.AddRange(tree.ToTagEntry());
            }
            return results.ToArray();
        }
    }
}
