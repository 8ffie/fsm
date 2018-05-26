using System;
using System.Collections.Generic;
using System.Text;

namespace FrequentSubtreeMining.Algorithm.Models
{
    public class SearchParameters : ICloneable
    {
        /// <summary>
        /// Минимальная поддержка (доля частых поддеревьев)
        /// </summary>
        public static double minimumSupport;

        /// <summary>
        /// Количество анализируемых деревьев
        /// </summary>
        internal static int treeNumber;

        /// <summary>
        /// Максимальное время поиска
        /// </summary>
        internal static long maxTime;

        public static List<TextTreeEncoding> initialTrees;

        /// <summary>
        /// Поддержка (число деревьев, в которых должны встречаться частые поддеревья)
        /// </summary>
        internal int Support
        {
            get
            {
                return (int)(minimumSupport * treeNumber);
            }
        }

        /// <summary>
        /// Минимальное число узлов
        /// </summary>
        internal int MinimumNodeNumber { get; private set; }

        /// <summary>
        /// Максимальное число узлов
        /// </summary>
        internal int MaximumNodeNumber { get; private set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="minimumSupport">Минимальная поддержка</param>
        /// <param name="minNodeNumber">Минимальное число узлов</param>
        /// <param name="maxNodeNumber">Максимальное число узлов</param>
        internal SearchParameters(double minimumSupport, int minNodeNumber, int maxNodeNumber)
        {
            SearchParameters.minimumSupport = minimumSupport;
            MinimumNodeNumber = minNodeNumber;
            MaximumNodeNumber = maxNodeNumber;
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="another">Параметры поиска</param>
        internal SearchParameters(SearchParameters another)
        {
            MinimumNodeNumber = another.MinimumNodeNumber;
            MaximumNodeNumber = another.MaximumNodeNumber;
        }

        /// <summary>
        /// Строковое представление параметров поиска
        /// </summary>
        /// <returns></returns>
        public string[] ToStrings()
        {
            List<string> sb = new List<string>();
            sb.Add("Условия поиска:");
            sb.Add(string.Format("Поддержка: {0}", minimumSupport));
            sb.Add(string.Format("Минимальное число узлов: {0}", MinimumNodeNumber));
            sb.Add(string.Format("Максимальное число узлов: {0}", MaximumNodeNumber));
            sb.Add(string.Format("Общее число деревьев: {0}", treeNumber));
            return sb.ToArray();
        }

        /// <summary>
        /// Клонирование параметров поиска
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return new SearchParameters(this);
        }
    }
}
